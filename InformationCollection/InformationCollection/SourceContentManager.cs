using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TimiSoft.InformationCollection
{
    public static class SourceContentManager
    {
        private static Regex urlRegex = new Regex("<a(.*?)href(.*?)>(.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase );
        private static Regex zhRegex = new Regex("[\\u4E00-\\u9FA5]{6,}");
        private static Regex hrefRegex = new Regex("href(.*?)[\"'][^\"']*[\"']");
        private static Regex slashRegex = new Regex("[\"'][^\"']*[\"']");

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
                    string content = Regex.Replace(matchedValue, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                    if (zhRegex.IsMatch(content))
                    {
                        var hrefMatch = hrefRegex.Match(matchedValue);
                        if( hrefMatch.Success)
                        {
                            string href = slashRegex.Match(hrefMatch.Value).Value;
                            href = href.Substring(1, href.Length - 2);
                            if( !href.StartsWith("http"))
                            {
                                href = source.Domain + href;
                            }

                            var sourceContent = context.SourceContents.Where(p => p.SourceId == source.SourceId && p.Url == href).FirstOrDefault();
                            if( sourceContent == null)
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
