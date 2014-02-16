using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Text;
using System.Text.RegularExpressions;

namespace TimiSoft.InformationCollection
{
    public static class SourceContentManager
    {
        private static Regex urlRegex = new Regex("<a(.*?)href(.*?)>(.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static Regex hrefRegex = new Regex("href(.*?)[\"'][^\"']*[\"']");
        private static Regex slashRegex = new Regex("[\"'][^\"']*[\"']");
        private static Regex htmlRegex = new Regex(@"<(.[^>]*)>", RegexOptions.IgnoreCase);
        private static Regex dateRegex = new Regex(@"[(\[]*\d{4}-\d{2}-\d{2}[)\]]*", RegexOptions.IgnoreCase);
        private static List<SourceContentRegex> sourceRegexes = new List<SourceContentRegex>();

        /// <summary>
        /// Source content regex class
        /// </summary>
        private class SourceContentRegex : Regex
        {
            public SourceContentRegex(string pattern)
                : base(pattern)
            {

            }

            public bool NeedMatched { get; set; }
        }

        /// <summary>
        /// Reload Source regex list
        /// </summary>
        public static void ReloadSourceRegexes()
        {
            sourceRegexes.Clear();
            using (Models.ICContext context = new Models.ICContext())
            {
                var list = context.SourceRegexes.ToList();
                if (list.Count > 0)
                {
                    foreach (var sourceRegex in list)
                    {
                        sourceRegexes.Add(new SourceContentRegex(sourceRegex.Regex));
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
                System.IO.StreamReader streamReader = new System.IO.StreamReader(Utility.RequestHelper.GetResponse(source.Url));
                string response = streamReader.ReadToEnd();

                var match = urlRegex.Match(response);
                while (match.Success)
                {
                    string matchedValue = match.Value.Replace("\n", "");
                    if (!IsSourceRegexesMatch(matchedValue))
                    {
                        var hrefMatch = hrefRegex.Match(matchedValue);
                        if (hrefMatch.Success)
                        {
                            string href = slashRegex.Match(hrefMatch.Value).Value;
                            href = href.Substring(1, href.Length - 2);
                            if (!href.StartsWith("http"))
                            {
                                href = source.Domain + href.TrimStart('/');
                            }

                            string content = htmlRegex.Replace(matchedValue, ""); // remove html tag
                            content = dateRegex.Replace(content, "");   // remove date string

                            var sourceContent = context.SourceContents.Where(p => p.SourceId == source.SourceId && p.Url == href).FirstOrDefault();
                            if (sourceContent == null)
                            {
                                context.SourceContents.Add(new Models.SourceContent
                                {
                                    AddTime = collectTime,
                                    Content = content,
                                    ContentType = (int)sourceContentType,
                                    SourceId = source.SourceId,
                                    Url = href,
                                });
                                context.SaveChanges();
                            }
                        }
                    }

                    match = match.NextMatch();
                }
            }
        }

        private static bool IsSourceRegexesMatch(string value)
        {
            foreach (var regex in sourceRegexes)
            {
                if (regex.IsMatch(value) == regex.NeedMatched)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public static IList<Models.SourceContent> GetContents(int sourceId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                return context.SourceContents.Where(p => p.SourceId == sourceId).ToList();
            }
        }

        public static IList<Models.UserSourceContent> GetUserFavorContents(int userId, string keywords, int page, int pageSize, out int count)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                             join q in context.SourceContents
                                 on p.SourceId equals q.SourceId
                             join r in context.UserSourceContentLinks
                                 on new { UserId = userId, q.SourceContentId } equals new { r.UserId, r.SourceContentId }
                             where string.IsNullOrEmpty(keywords) || q.Content.Contains(keywords)
                             orderby q.SourceId
                             orderby q.AddTime descending
                             select new Models.UserSourceContent
                             {
                                 IsFavor = true,
                                 SourceContentId = q.SourceContentId,
                                 AddTime = q.AddTime,
                                 Content = q.Content,
                                 ContentType = q.ContentType,
                                 Source = p.SourceName,
                                 Url = q.Url
                             });
                count = query.Count();
                return query.Skip(page > 0 ? page * pageSize - pageSize : 0).Take(pageSize).ToList();
            }
        }

        public static IList<Models.UserSourceContent> GetUserContents(int userId, string keywords, int page, int pageSize, out int count)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                             join q in context.SourceContents
                                 on p.SourceId equals q.SourceId
                             join r in context.UserSourceContentLinks
                                 on new { UserId = userId, q.SourceContentId } equals new { r.UserId, r.SourceContentId }
                             into rDefault
                             from rD in rDefault.DefaultIfEmpty()
                             where string.IsNullOrEmpty(keywords) || q.Content.Contains(keywords)
                             orderby q.SourceId
                             orderby q.AddTime descending
                             select new Models.UserSourceContent
                             {
                                 IsFavor = rD != null,
                                 SourceContentId = q.SourceContentId,
                                 AddTime = q.AddTime,
                                 Content = q.Content,
                                 ContentType = q.ContentType,
                                 Source = p.SourceName,
                                 Url = q.Url
                             });
                count = query.Count();
                return query.Skip(page > 0 ? page * pageSize - pageSize : 0).Take(pageSize).ToList();
            }
        }

        public static IList<Models.UserSourceContent> GetUserContents(int userId, string keywords, int sourceId, int page, int pageSize, out int count)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                        join q in context.SourceContents
                            on p.SourceId equals q.SourceId
                        join r in context.UserSourceContentLinks
                            on new { UserId = userId, q.SourceContentId } equals new { r.UserId, r.SourceContentId }
                        into rDefault
                        from rD in rDefault.DefaultIfEmpty()
                        where p.SourceId == sourceId
                             &&( string.IsNullOrEmpty(keywords) || q.Content.Contains(keywords))
                             orderby q.SourceId
                        orderby q.AddTime descending
                        select new Models.UserSourceContent
                        {
                            IsFavor = rD != null,
                            SourceContentId = q.SourceContentId,
                            AddTime = q.AddTime,
                            Content = q.Content,
                            ContentType = q.ContentType,
                            Source = p.SourceName,
                            Url = q.Url
                        });
                count = query.Count();
                return query.Skip(page > 0 ? page * pageSize - pageSize : 0).Take(pageSize).ToList();
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
