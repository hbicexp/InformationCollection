// -----------------------------------------------------------------------
// <copyright file="RequestHelper.cs" company="TimiSoft">
// Copyright TimiSoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace TimiSoft.InformationCollection.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
        internal static System.IO.Stream GetResponse(string url)
        {
            System.Net.HttpWebRequest webRequest = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            webRequest.Method = "Get";
            webRequest.KeepAlive = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            var webResponse = webRequest.GetResponse();
            return webResponse.GetResponseStream();
        }
    }
}
