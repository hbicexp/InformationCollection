using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimiSoft.InformationCollection;
using TimiSoft.InformationCollection.Models;

namespace TimiSoft.InformationCollectionWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "修改此模板以快速启动你的 ASP.NET MVC 应用程序。";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "你的应用程序说明页。";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "你的联系方式页。";

            return View();
        }

        public ActionResult Source()
        {
            ViewBag.Message = "源管理";
            UserProfile userProfile = this.GetUser(User.Identity.Name);
            var model = SourceManager.GetSourceList(userProfile.UserId);
            return View(model);
        }

        public ActionResult AddSource(UserSource model)
        {
            UserProfile userProfile = this.GetUser(User.Identity.Name);
            SourceManager.AddSource(userProfile.UserId, model);
            return RedirectToAction("Source", "Home");
        }

        private UserProfile GetUser(string userName)
        {
            var userProfile = Session["UserProfile"] as UserProfile;
            if (userProfile == null)
            {
                userProfile = UserManager.GetUser(userName);
                if (userProfile == null)
                {
                    throw new Exception("该用户不存在！");
                }

                Session["UserProfile"] = userProfile;
            }

            return userProfile;
        }
    }
}
