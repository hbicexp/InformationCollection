using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimiSoft.InformationCollection.Models;

namespace TimiSoft.InformationCollection
{
    public class UserManager
    {
        public static UserProfile GetUser(string userName)
        {
            using (ICContext db = new ICContext())
            {
                return db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            }
        }
    }
}
