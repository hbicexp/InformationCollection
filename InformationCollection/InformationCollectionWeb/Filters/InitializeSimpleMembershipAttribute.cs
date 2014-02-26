using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Linq;
using WebMatrix.WebData;
using TimiSoft.InformationCollectionWeb.Models;
using TimiSoft.InformationCollection.Models;
using System.Web.Security;

namespace TimiSoft.InformationCollectionWeb.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;
        private ActionExecutingContext filterContext;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
            this.filterContext = filterContext;
        }


        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<DatabaseContext>(null);

                try
                {
                    using (var context = new DatabaseContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                    
                    var roles = (SimpleRoleProvider)Roles.Provider;
                    var membership = (SimpleMembershipProvider)Membership.Provider;

                    if (membership.GetUser("Admin", false) == null)
                    {
                        WebSecurity.CreateUserAndAccount("Admin", "!Admin", new { UserName = "Admin" }, false);
                    }

                    if (!roles.RoleExists("Administrator"))
                    {
                        roles.CreateRole("Administrator");
                    }

                    if (!roles.GetRolesForUser("Admin").Contains("Administrator"))
                    {
                        roles.AddUsersToRoles(new[] { "Admin" }, new[] { "Administrator" });
                    }
                    
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
