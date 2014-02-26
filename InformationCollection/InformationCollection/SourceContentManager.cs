using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Policy;

namespace TimiSoft.InformationCollection
{
    public static class SourceContentManager
    {
        //private static Regex linkRegex = new Regex("<a(.*?)href(.*?)[\"'](?<url>.*?)[\"']>(.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        //private static Regex urlRegex = new Regex("href(.*?)[\"'][^\"']*[\"']", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static Regex dateRegex = new Regex(@"[（(\[]*(?<year>2\d(1[4-9]|[2-9]\d))[-.年](?<month>\d{1,2})[-.月](?<day>\d{1,2})[日）)\]]*", RegexOptions.IgnoreCase);
        private static Regex htmlRegex = new Regex(@"<(.[^>]*)>", RegexOptions.IgnoreCase);
        private static Regex slashRegex = new Regex("([\"'][^\"']*[\"'])");
        private static Regex crlfRegex = new Regex(@"[\r\n]");
        private static List<SourceContentRegexGroup> allGroupRegexes = new List<SourceContentRegexGroup>();

        private class SourceContentRegexGroup
        {
            public bool decode;
            public Regex groupRegex;
            public Regex linkRegex; // = new Regex("<a(.*?)href(.*?)>(.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            public Regex dateRegex; // = new Regex(@"[（(\[]*20(1[4-9]|[2-9]\d)[-.年]\d{1,2}[-.月]\d{1,2}[日）)\]]*", RegexOptions.IgnoreCase);
            //public Regex urlRegex; // = new Regex("href(.*?)[\"'][^\"']*[\"']", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //public List<SourceContentRegex> hrefRegexes = new List<SourceContentRegex>();
            public List<SourceContentRegex> containerRegexes = new List<SourceContentRegex>();
            public List<SourceContentRegex> contentRegexes = new List<SourceContentRegex>();

            public SourceContentRegexGroup(string groupName, string groupRegex, bool decode, IList<Models.SourceRegex> regexes)
            {
                this.GroupName = groupName;
                this.decode = decode;
                this.groupRegex = new Regex(groupRegex, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                foreach (var sourceRegex in regexes)
                {
                    switch (sourceRegex.RegexType)
                    {
                        //case "href":
                        //    hrefRegexes.Add(new SourceContentRegex(sourceRegex.Regex, sourceRegex.IsMatched));
                        //    break;
                        case "content":
                            contentRegexes.Add(new SourceContentRegex(sourceRegex.Regex, sourceRegex.IsMatched));
                            break;
                        case "container":
                            containerRegexes.Add(new SourceContentRegex(sourceRegex.Regex, sourceRegex.IsMatched));
                            break;
                        case "link":
                            linkRegex = new Regex(sourceRegex.Regex, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                            break;
                        case "date":
                            dateRegex = new Regex(sourceRegex.Regex, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                            break;
                        default:
                            break;
                    }
                }
            }

            public bool IsNotMatchSourceRegexes(string value, IList<SourceContentRegex> regexes)
            {
                foreach (var regex in regexes)
                {
                    if (regex.IsMatch(value) != regex.IsMatched)
                    {
                        return true;
                    }
                }

                return false;
            }

            public string GroupName { get; set; }
        }

        /// <summary>
        /// Source content regex class
        /// </summary>
        private class SourceContentRegex : Regex
        {
            public SourceContentRegex(string pattern, bool isMatched)
                : base(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase)
            {
                this.IsMatched = isMatched;
            }

            public bool IsMatched { get; set; }
        }

        /// <summary>
        /// Reload Source regex list
        /// </summary>
        public static void ReloadSourceRegexes()
        {
            allGroupRegexes.Clear();

            using (Models.ICContext context = new Models.ICContext())
            {
                foreach (var regexGroup in context.SourceRegexGroups.Where(p => p.Domain == "All"))
                {
                    var regexes = regexGroup.SourceRegexes.ToList();
                    if (regexes.Count > 0)
                    {
                        allGroupRegexes.Add(new SourceContentRegexGroup(regexGroup.Name, regexGroup.Regex, regexGroup.Decode, regexes));
                    }
                }
            }
        }

        /// <summary>
        /// Get source content
        /// </summary>
        /// <param name="source">watched source</param>
        /// <param name="collectTime">collect time</param>
        public static void Collect(Models.Source source, DateTime collectTime, SourceContentType sourceContentType)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                string response = Utility.RequestHelper.GetResponse(source.Url);

                var regexGroups = context.SourceRegexGroups.Where(p => p.Domain == source.Domain).ToList();
                if (regexGroups != null && regexGroups.Count > 0)
                {
                    List<SourceContentRegexGroup> groupRegexes = new List<SourceContentRegexGroup>();
                    foreach (var regexGroup in regexGroups)
                    {
                        var regexes = regexGroup.SourceRegexes.ToList();
                        if (regexes.Count > 0)
                        {
                            groupRegexes.Add(new SourceContentRegexGroup(regexGroup.Name, regexGroup.Regex, regexGroup.Decode, regexes));
                        }
                    }

                    Collect(source, collectTime, sourceContentType, context, response, groupRegexes);
                }
                else
                {
                    Collect(source, collectTime, sourceContentType, context, response, allGroupRegexes);
                }
            }
        }

        private static void Collect(Models.Source source, DateTime collectTime, SourceContentType sourceContentType, Models.ICContext context, string response, List<SourceContentRegexGroup> groupRegexes)
        {
            var domain = source.Domain;
            var hrefhead = GetUrlHead(source.Url);

            foreach (var regexGroup in groupRegexes)
            {
                if(regexGroup.linkRegex == null)
                {
                    continue;
                }

                var sourceDate = collectTime;
                var match = regexGroup.groupRegex.Match(response, 0);
                while (match.Success)
                {
                    var matchValue = regexGroup.decode ? System.Net.WebUtility.HtmlDecode(match.Value) : match.Value;
                    if (!regexGroup.IsNotMatchSourceRegexes(matchValue, regexGroup.containerRegexes))
                    {
                        if(regexGroup.dateRegex != null)
                        {
                            var dateMatch = regexGroup.dateRegex.Match(matchValue);
                            if(!dateMatch.Success)
                            {
                                match = match.NextMatch();
                                continue;
                            }

                            var year = Convert.ToInt32(dateMatch.Groups["year"].Value);
                            var month = Convert.ToInt32(dateMatch.Groups["month"].Value);
                            var day = Convert.ToInt32(dateMatch.Groups["day"].Value);
                            sourceDate = new DateTime(year, month, day);
                        }

                        var linkMatch = regexGroup.linkRegex.Match(matchValue);
                        if (linkMatch.Success)
                        {
                            if (linkMatch.Groups["url"].Success && linkMatch.Groups["content"].Success)// && !regexGroup.IsNotMatchSourceRegexes(linkValue, regexGroup.hrefRegexes))
                            {
                                var url = linkMatch.Groups["url"].Value;
                                url = url.Substring(1, url.Length - 2);
                                if (!url.StartsWith("http"))
                                {
                                    if (url.StartsWith("./"))
                                    {
                                        url = hrefhead.TrimEnd('/') + url.TrimStart('.');
                                    }
                                    else if(url.StartsWith("/")){
                                        url = source.Domain + url.TrimStart('/');
                                    }
                                    else
                                    {
                                        url = hrefhead + url;
                                    }
                                }

                                var content = crlfRegex.Replace(linkMatch.Groups["content"].Value, "");
                                if (!regexGroup.IsNotMatchSourceRegexes(content, regexGroup.contentRegexes))
                                {
                                    content = System.Net.WebUtility.HtmlDecode(htmlRegex.Replace(dateRegex.Replace(content, ""), "")).Trim('·', ' ', '\t');   // remove date string
                                    var sourceContent = context.SourceContents.Where(p => p.SourceId == source.SourceId && p.Url == url).FirstOrDefault();
                                    if (sourceContent == null)
                                    {
                                        context.SourceContents.Add(new Models.SourceContent
                                        {
                                            AddTime = collectTime, 
                                            AddDate = collectTime.Date,
                                            AddHour = collectTime.Hour,
                                            SourceDate = sourceDate,
                                            Content = content,
                                            ContentType = (int)sourceContentType,
                                            SourceId = source.SourceId,
                                            Url = url,
                                        });
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    match = match.NextMatch();
                }
            }
        }

        private static string GetUrlHead(string url)
        {
           System.Uri r = new Uri(url);
           string reVal = "http://" + r.Authority;
           for (int i = 0; i < r.Segments.Length; i++ )
           {
               var segment =  r.Segments[i];
               if( !segment.EndsWith("/")){
                   break;
               }

               reVal += segment;
           }

           return reVal;
        }

        public static void Clear(int sourceId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var sourceContents = context.SourceContents.Where(p => p.SourceId == sourceId);
                foreach (var sourceContent in sourceContents)
                {
                    context.SourceContents.Remove(sourceContent);
                }

                context.SaveChanges();
            }
        }

        public static IList<Models.SourceContent> GetContents(int sourceId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                return context.SourceContents.Where(p => p.SourceId == sourceId).ToList();
            }
        }

        public static IList<Models.UserSourceContent> GetUserFavorContents(int userId, string keywords, DateTime beginDate, DateTime endDate, int page, int pageSize, out int count)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                             join q in context.SourceContents
                                 on p.SourceId equals q.SourceId
                             join s in context.UserSourceLinks
                                on p.SourceId equals s.SourceId
                             join r in context.UserSourceContentLinks
                                 on new { s.UserId, q.SourceContentId } equals new { r.UserId, r.SourceContentId }
                             where s.UserId == userId && (string.IsNullOrEmpty(keywords) || q.Content.Contains(keywords) || s.SourceName.Contains(keywords))
                                && q.AddDate >= beginDate && q.AddDate <= endDate
                             orderby q.SourceId
                             orderby q.AddHour - (q.AddHour % s.Interval) descending
                             orderby q.AddDate descending
                             select new Models.UserSourceContent
                             {
                                 IsFavor = true,
                                 SourceContentId = q.SourceContentId,
                                 SourceDate = q.SourceDate,
                                 AddTime = q.AddTime,
                                 AddDate = q.AddDate,
                                 AddHours = q.AddHour - (q.AddHour % s.Interval),
                                 Content = q.Content,
                                 ContentType = q.ContentType,
                                 Source = p.SourceName,
                                 Url = q.Url
                             });
                count = query.Count();
                return query.Skip(page > 0 ? page * pageSize - pageSize : 0).Take(pageSize).ToList();
            }
        }

        public static IList<Models.UserSourceContent> GetUserContents(int userId, string keywords, DateTime beginDate, DateTime endDate, int page, int pageSize, out int count)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                             join q in context.SourceContents
                                 on p.SourceId equals q.SourceId
                             join s in context.UserSourceLinks
                                on p.SourceId equals s.SourceId
                             join r in context.UserSourceContentLinks
                                 on new { s.UserId, q.SourceContentId } equals new { r.UserId, r.SourceContentId }
                             into rDefault
                             from rD in rDefault.DefaultIfEmpty()
                             where s.UserId == userId
                                && (string.IsNullOrEmpty(keywords) || q.Content.Contains(keywords) || s.SourceName.Contains(keywords))
                                && q.AddDate >= beginDate && q.AddDate <= endDate
                             orderby q.SourceId
                             orderby q.AddHour - (q.AddHour % s.Interval) descending
                             orderby q.AddDate descending
                             select new Models.UserSourceContent
                             {
                                 IsFavor = rD != null,
                                 SourceContentId = q.SourceContentId,
                                 SourceDate = q.SourceDate,
                                 AddTime = q.AddTime,
                                 AddDate = q.AddDate,
                                 AddHours = q.AddHour - (q.AddHour % s.Interval),
                                 Content = q.Content,
                                 ContentType = q.ContentType,
                                 Source = p.SourceName,
                                 Url = q.Url
                             });
                count = query.Count();
                return query.Skip(page > 0 ? page * pageSize - pageSize : 0).Take(pageSize).ToList();
            }
        }

        public static IList<Models.UserSourceContent> GetAllContents(string keywords, DateTime beginDate, DateTime endDate, int page, int pageSize, out int count)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                             join q in context.SourceContents
                                 on p.SourceId equals q.SourceId
                             where (string.IsNullOrEmpty(keywords) || q.Content.Contains(keywords))
                                && q.AddDate >= beginDate && q.AddDate <= endDate
                             orderby q.SourceId
                             orderby q.AddDate descending
                             select new Models.UserSourceContent
                             {
                                 IsFavor = false,
                                 SourceContentId = q.SourceContentId,
                                 SourceDate = q.SourceDate,
                                 AddTime = q.AddTime,
                                 AddDate = q.AddDate,
                                 AddHours = q.AddHour,
                                 Content = q.Content,
                                 ContentType = q.ContentType,
                                 Source = p.SourceName,
                                 Url = q.Url
                             });
                count = query.Count();
                return query.Skip(page > 0 ? page * pageSize - pageSize : 0).Take(pageSize).ToList();
            }
        }

        public static IList<Models.UserSourceContent> GetUserContents(int userId, int sourceId, string keywords, DateTime beginDate, DateTime endDate, int page, int pageSize, out int count)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                             join q in context.SourceContents
                                 on p.SourceId equals q.SourceId
                             join s in context.UserSourceLinks
                                 on p.SourceId equals s.SourceId
                             join r in context.UserSourceContentLinks
                                 on new { s.UserId, q.SourceContentId } equals new { r.UserId, r.SourceContentId }
                             into rDefault
                             from rD in rDefault.DefaultIfEmpty()
                             where s.UserId == userId && p.SourceId == sourceId
                                  && (string.IsNullOrEmpty(keywords) || q.Content.Contains(keywords))
                                && q.AddDate >= beginDate && q.AddDate <= endDate
                             orderby q.SourceId
                             orderby q.AddHour - (q.AddHour % s.Interval) descending
                             orderby q.AddDate descending
                             select new Models.UserSourceContent
                             {
                                 IsFavor = rD != null,
                                 SourceContentId = q.SourceContentId,
                                 SourceDate = q.SourceDate,
                                 AddTime = q.AddTime,
                                 AddDate = q.AddDate,
                                 AddHours = q.AddHour - (q.AddHour % s.Interval),
                                 Content = q.Content,
                                 ContentType = q.ContentType,
                                 Source = p.SourceName,
                                 Url = q.Url
                             });
                count = query.Count();
                return query.Skip(page > 0 ? page * pageSize - pageSize : 0).Take(pageSize).ToList();
            }
        }

        public static void ClearSourceContents(int interval)
        {
            using(Models.ICContext context = new Models.ICContext())
            {
                string endTime = DateTime.Now.AddDays(-interval).ToString("yyyy-MM-dd HH:mm:ss");
                string cmdText = @"
Delete P
From UserSourceContentLink P
    Join SourceContent Q on P.SourceContentId=Q.SourceContentId
Where Q.AddTime<'" + endTime + "'";
                context.Database.ExecuteSqlCommand(cmdText);

                cmdText = @"
Delete P
From SourceContent P
Where P.AddTime<'" + endTime + "'";
                context.Database.ExecuteSqlCommand(cmdText);
            }
        }
    }

    /// <summary>
    /// Source content type
    /// </summary>
    public enum SourceContentType
    {
        /// <summary>
        /// System content
        /// </summary>
        System,

        /// <summary>
        /// 
        /// </summary>
        Content
    }

}