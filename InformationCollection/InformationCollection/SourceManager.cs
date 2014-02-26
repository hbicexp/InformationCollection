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

        public static void AddSource(int userId, Models.UserSource userSource)
        {
            if (string.IsNullOrEmpty(userSource.SourceName) || string.IsNullOrEmpty(userSource.Url))
            {
                throw new ArgumentException("源名称和源地址不能为空！");
            }

            if (!(userSource.Url.StartsWith("http://") || userSource.Url.StartsWith("https://")))
            {
                userSource.Url = "http://" + userSource.Url;
            }

            using (Models.ICContext context = new Models.ICContext())
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
                    };

                    source.UserSourceLinks.Add(new Models.UserSourceLink()
                    {
                        Source = sourceInDB,
                        SourceName = userSource.SourceName,
                        Interval = userSource.Interval,
                        UserId = userId, 
                        CreateTime = DateTime.Now
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
                        sourceInDB.UserSourceLinks.Add(new Models.UserSourceLink()
                        {
                            SourceName = userSource.SourceName,
                            Interval = userSource.Interval,
                            Source = sourceInDB, 
                            UserId = userId,
                            CreateTime = DateTime.Now
                        });
                    }

                    context.SaveChanges();
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

        public static List<Models.UserSource> GetSourceList(int userId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                return (from p in context.Sources
                        join q in context.UserSourceLinks
                        on p.SourceId equals q.SourceId
                        where q.UserId == userId
                        select new Models.UserSource
                        {
                            SourceId = p.SourceId,
                            SourceName = p.SourceName,
                            Url = p.Url,
                            CreateTime = q.CreateTime, Interval = q.Interval
                        }).ToList();
            }
        }

        public static List<Models.Source> GetSourceList()
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                return context.Sources.ToList();
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
Delete From UserSourceLink
Where UserId = {0} and SourceId = {1}", userId, sourceId);;
            }
        }
    }
}
