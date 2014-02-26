using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TimiSoft.InformationCollection.Models;
using TimiSoft.InformationCollectionWeb.Filters;
using WebMatrix.WebData;

namespace TimiSoft.InformationCollectionWeb.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private ICContext context;

        public UserController ()
        {
            this.context = new ICContext();
        }

        //
        // GET: /Admin/Role/
        public ActionResult Index()
        {
            return View(context.UserProfiles.ToList());
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int Id)
        {
            var user = context.UserProfiles.Where(p => p.UserId == Id).FirstOrDefault();
            if( user != null)
            {
                context.UserProfiles.Remove(user);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult SetAdmin(string uName)
        {
            if( User.Identity.Name == uName)
            {
                return RedirectToAction("Index");
            }   

            var roles = (SimpleRoleProvider)Roles.Provider;
            roles.AddUsersToRoles(new[] { uName }, new[] { "Administrator" });
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AddAdmin(string uName)
        {
            var roles = (SimpleRoleProvider)Roles.Provider;
            roles.RemoveUsersFromRoles(new[] { uName }, new[] { "Administrator" });
            return RedirectToAction("Index");
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            ViewBag.Selected = "User";
            ViewBag.Title = "用户管理";
            context.Dispose();
        }
    }
}
