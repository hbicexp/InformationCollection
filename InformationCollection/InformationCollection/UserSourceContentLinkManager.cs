// -----------------------------------------------------------------------
// <copyright file="UserSourceContentLinkManager.cs" company="TimiSoft">
// Copyright TimiSoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace TimiSoft.InformationCollection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// User source content link manager class
    /// </summary>
    public class UserSourceContentLinkManager
    {
        public static void AddFavorLink(int userId, int sourceContentId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var UserSourceContentFavorLink = context.UserSourceContentFavorLinks.Where(p => p.UserId == userId && p.SourceContentId == sourceContentId).FirstOrDefault();
                if (UserSourceContentFavorLink == null)
                {
                    context.UserSourceContentFavorLinks.Add(new Models.UserSourceContentFavorLink
                    {
                        UserId = userId,
                        SourceContentId = sourceContentId
                    });
                    context.SaveChanges();
                }
            }
        }

        public static void RemoveFavorLink(int userId, int sourceContentId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var UserSourceContentFavorLink = context.UserSourceContentFavorLinks.Where(p => p.UserId == userId && p.SourceContentId == sourceContentId).FirstOrDefault();
                if (UserSourceContentFavorLink != null)
                {
                    context.UserSourceContentFavorLinks.Remove(UserSourceContentFavorLink);
                    context.SaveChanges();
                }
            }
        }

        public static void AddLink(int userId, int sourceContentId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var UserSourceContentLink = context.UserSourceContentLinks.Where(p => p.UserId == userId && p.SourceContentId == sourceContentId).FirstOrDefault();
                if (UserSourceContentLink == null)
                {
                    context.UserSourceContentLinks.Add(new Models.UserSourceContentLink
                    {
                        UserId = userId,
                        SourceContentId = sourceContentId
                    });
                    context.SaveChanges();
                }
            }
        }

        public static void RemoveLink(int userId, int sourceContentId)
        {
            using (Models.ICContext context = new Models.ICContext())
            {
                var UserSourceContentLink = context.UserSourceContentLinks.Where(p => p.UserId == userId && p.SourceContentId == sourceContentId).FirstOrDefault();
                if (UserSourceContentLink != null)
                {
                    context.UserSourceContentLinks.Remove(UserSourceContentLink);
                    context.SaveChanges();
                }
            }
        }
    }
}
