using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace TimiSoft.InformationCollection
{
    public static class SourceManager
    {
        public static Regex DomainRegex = new Regex("http[s]*://[^/]*/");

        public static void AddSource(int company, int userId, Models.SourceView userSource)
        {
            if (string.IsNullOrEmpty(userSource.SourceName) || string.IsNullOrEmpty(userSource.Url))
            {
                throw new ArgumentException("任务名称和任务地址不能为空！");
            }

            if (!(userSource.Url.StartsWith("http://") || userSource.Url.StartsWith("https://")))
            {
                userSource.Url = "http://" + userSource.Url;
            }

            using (Models.ICContext context = new Models.ICContext())
            {
                if (userSource.SourceId > 0)
                {
                    var sourceInDB = context.UserSourceLinks.Where(p => p.UserId == userId && p.SourceId == userSource.SourceId).FirstOrDefault();
                    if (sourceInDB != null)
                    {
                        sourceInDB.SourceName = userSource.SourceName;
                        sourceInDB.Interval = userSource.Interval;
                        context.SaveChanges();
                    }
                }
                else
                {
                    var sourceInDB = context.Sources.Where(p => p.Url == userSource.Url).FirstOrDefault();
                    Models.UserSourceLink userSourceLinkInDB = null;
                    if (sourceInDB == null)
                    {
                        Models.Source source = new Models.Source()
                        {
                            SourceName = userSource.SourceName,
                            Url = userSource.Url,
                            Domain = GetDomain(userSource.Url),
                            Interval = userSource.Interval,
                            CreateTime = DateTime.Now,
                            Company = company,
                            SourceType = 1
                        };

                        source.SourceUrls.Add(new Models.SourceUrl { 
                             Url = userSource.Url, 
                             Enabled = true
                        });

                        source.UserSourceLinks.Add(new Models.UserSourceLink()
                        {
                            Source = sourceInDB,
                            SourceName = userSource.SourceName,
                            Interval = userSource.Interval,
                            UserId = userId,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now
                        });

                        context.Sources.Add(source);
                        context.SaveChanges();

                        // collect system info
                        SourceContentManager.ReloadSourceRegexes();
                        SourceContentManager.Collect(source, DateTime.Now, SourceContentType.System);
                    }
                    else
                    {
                        userSourceLinkInDB = sourceInDB.UserSourceLinks.Where(p => p.UserId == userId).FirstOrDefault();
                        if (userSourceLinkInDB == null)
                        {
                            userSourceLinkInDB = new Models.UserSourceLink()
                            {
                                SourceName = userSource.SourceName,
                                Interval = userSource.Interval,
                                Source = sourceInDB,
                                UserId = userId,
                                CreateTime = DateTime.Now,
                                UpdateTime = DateTime.Now
                            };
                            sourceInDB.UserSourceLinks.Add(userSourceLinkInDB);
                            context.SaveChanges();

                            var queryDate = DateTime.Now.Date.AddMonths(-1);
                            var sourceCotents = context.SourceContents.Where(p => p.SourceId == sourceInDB.SourceId && p.SourceDate >= queryDate).ToList();
                            if (sourceCotents.Count > 0)
                            {
                                foreach (var sourceContent in sourceCotents)
                                {
                                    sourceContent.UserSourceContentLinks.Add(new Models.UserSourceContentLink
                                    {
                                        UserId = userId
                                    });
                                }
                            }

                            context.SaveChanges();
                        }
                    }
                }
            }
        }

        private static string GetDomain(string url)
        {
            var match = DomainRegex.Match(url);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        public static List<Models.SourceView> GetSourceList(int userId, string search, int page, int pageSize, out int count)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                        join q in context.UserSourceLinks
                        on p.SourceId equals q.SourceId
                        where q.UserId == userId && 
                        (String.IsNullOrEmpty(search) || q.SourceName.Contains(search))
                        orderby q.SourceName
                        select new Models.SourceView
                        {
                            SourceId = p.SourceId,
                            SourceName = q.SourceName,
                            Url = p.Url,
                            CreateTime = q.CreateTime,
                            Interval = q.Interval
                        });
                count = query.Count();
                return query.Skip(page > 0 ? page * pageSize - pageSize : 0).Take(pageSize).ToList();
            }
        }

        public static List<Models.Source> GetSourceList()
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                return context.Sources.OrderBy(p => p.SourceName).ToList();
            }
        }

        public static List<Models.SourceView> GetSourceList(int userId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var query = (from p in context.Sources
                             join q in context.UserSourceLinks
                             on p.SourceId equals q.SourceId
                             where q.UserId == userId 
                             orderby q.SourceName
                             select new Models.SourceView
                             {
                                 SourceId = p.SourceId,
                                 SourceName = q.SourceName,
                                 Url = p.Url,
                                 CreateTime = q.CreateTime,
                                 Interval = q.Interval
                             });
                return query.ToList();
            }
        }

        public static Models.Source GetSource(int id)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                return context.Sources.Find(id);
            }
        }

        public static void RemoveSource(int userId, int sourceId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                context.Database.ExecuteSqlCommand(@"
Delete From P 
From UserSourceContentLink P
 join SourceContent Q on P.SourceContentId=Q.SourceContentId
 join UserSourceLink R on Q.SourceId=R.SourceId
Where P.UserId = {0} and R.SourceId = {1}", userId, sourceId);

                context.Database.ExecuteSqlCommand(@"
Delete From P 
From UserSourceContentFavorLink P
 join SourceContent Q on P.SourceContentId=Q.SourceContentId
 join UserSourceLink R on Q.SourceId=R.SourceId
Where P.UserId = {0} and R.SourceId = {1}", userId, sourceId);

                context.Database.ExecuteSqlCommand(@"
Delete From UserSourceLink
Where UserId = {0} and SourceId = {1}", userId, sourceId); ;
            }
        }
    }
}
