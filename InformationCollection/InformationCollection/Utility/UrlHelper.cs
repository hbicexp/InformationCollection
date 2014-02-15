// -----------------------------------------------------------------------
// <copyright file="UrlHelper.cs" company="TimiSoft">
// Copyright TimiSoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace TimiSoft.InformationCollection.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Url helper class
    /// </summary>
    public static class UrlHelper
    {
        /// <summary>
        /// Domain regex
        /// </summary>
        private static Regex domainRegex = new Regex("http[s]*://[^/]*/");

        /// <summary>
        /// Get domain
        /// </summary>
        /// <param name="url">the url to get domain</param>
        /// <returns>url of domain</returns>
        private static string GetDomain(string url)
        {
            var match = domainRegex.Match(url);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
