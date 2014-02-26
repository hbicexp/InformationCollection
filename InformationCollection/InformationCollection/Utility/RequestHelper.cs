// -----------------------------------------------------------------------
// <copyright file="RequestHelper.cs" company="TimiSoft">
// Copyright TimiSoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace TimiSoft.InformationCollection.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Request helper class
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// Get response from request of url
        /// </summary>
        /// <param name="url">request url</param>
        /// <returns>response of request</returns>
        internal static string GetResponse(string url)
        {
            System.Net.HttpWebRequest webRequest = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            webRequest.Method = "Get";
            webRequest.KeepAlive = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            var webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();
            var r = webResponse.GetResponseStream();
            if (webResponse.CharacterSet != "ISO-8859-1")
            {
                return GetS(Encoding.GetEncoding(webResponse.CharacterSet), r);
            }
            else
            {
                var streamReader = new System.IO.StreamReader(r, Encoding.UTF8);
                StringBuilder builder = new StringBuilder(1000);
                var line = streamReader.ReadLine();
                while (line != null)
                {
                    builder.Append(line);
                    if (line.IndexOf("gb2312") > 0)
                    {
                        streamReader = new System.IO.StreamReader(r, Encoding.GetEncoding("GB2312"));
                        string s = streamReader.ReadToEnd();
                        streamReader.Close();
                        return s;
                    }

                    line = streamReader.ReadLine();
                }

                streamReader.Close();
                return builder.ToString();
            }
        }



        static Regex utfRegex = new Regex("[\u4e00-\u9fa5]{1}?");
        //static Regex gbkRegex = new Regex("(([\xB0-\xF7][\xA1-\xFE])|([\x81-\xA0][\x40-\xFE])|([\xAA-\xFE][\x40-\xA0])|(\w))?");
        
        private static string GetS(Encoding encoding, System.IO.Stream r)
        {
            var streamReader = new System.IO.StreamReader(r, encoding);
            var s = streamReader.ReadToEnd();
            streamReader.Close();
            return s;
        }

        private bool isLuan(string txt)
        {
            var bytes = Encoding.UTF8.GetBytes(txt);
            //239 191 189
            for (var i = 0; i < bytes.Length; i++)
            {
                if (i < bytes.Length - 3)
                    if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                    {
                        return true;
                    }
            }

            return false;
        }

        //public static string GetEncoding(string url)
        //{
        //    HttpWebRequest request = null;
        //    HttpWebResponse response = null;
        //    StreamReader reader = null;
        //    try
        //    {
        //        request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Timeout = 30000;
        //        request.AllowAutoRedirect = false;
        //        string html = "";
        //        response = (HttpWebResponse)request.GetResponse();
        //        if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 1024)
        //        {
        //            if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
        //                reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
        //            else
        //                reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII);

        //            //此处不用ReadToEnd 方法，采用ReadLine 当读到charset时跳出。
        //            //string html = reader.ReadToEnd();
        //            StringBuilder htmlBuilder = new StringBuilder();
        //            string temp;
        //            while ((temp = reader.ReadLine()) != null)
        //            {
        //                htmlBuilder.Append(temp);
        //                html = htmlBuilder.ToString();
        //                if (html.IndexOf("charset", StringComparison.InvariantCultureIgnoreCase) > 0)
        //                {
        //                    break;
        //                }
        //            }

        //            Regex regCharset = new Regex(@"charset\b\s*=\s*(?<charset>[^""]*)");//http://www.8kmm.com
        //            if (regCharset.IsMatch(html))
        //            {
        //                return regCharset.Match(html).Groups["charset"].Value;
        //            }
        //            else if (response.CharacterSet != string.Empty)
        //            {
        //                return response.CharacterSet;
        //            }
        //            else
        //                return Encoding.Default.BodyName;
        //        }
        //        else if (response.StatusCode == HttpStatusCode.MovedPermanently || response.StatusCode == HttpStatusCode.Found)
        //        {
        //            //页面跳转返回301，如：www.sina.com
        //            //重新读取跳转地址
        //            if (response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
        //                reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
        //            else
        //                reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII);
        //            html = reader.ReadToEnd();
        //            Regex regHref = new Regex("<a[\\s]+href[\\s]*=[\\s]*\"([^<\"]+)\"");
        //            if (regHref.IsMatch(html))
        //            {
        //                string targetUrl = regHref.Match(html).Groups[1].Value;
        //                if (!IsUrl(targetUrl))
        //                {
        //                    url = url + targetUrl;
        //                }
        //                else
        //                {
        //                    url = targetUrl;
        //                }
        //                return GetEncoding(url);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    finally
        //    {
        //        if (response != null)
        //        {
        //            response.Close();
        //            response = null;
        //        }
        //        if (reader != null)
        //            reader.Close();

        //        if (request != null)
        //            request = null;
        //    }
        //    return Encoding.Default.BodyName;
        //}
    }
}
