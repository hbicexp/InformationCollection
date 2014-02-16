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
        private UserProfile userProfile;

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                this.userProfile = this.GetUser(User.Identity.Name);
                ViewBag.Sources = this.GetSources();
            }
        }

        public ActionResult Index()
        {
            var p = Request["page"];
            int page = 0;
            int pageSize = 20;
            if (p != null) { int.TryParse(p, out page); }
            if (page == 0) { page = 1; }
            int count = 0;
            var search = Request["search"];

            ViewBag.Selected = "Index";
            ViewBag.SourceContents = InformationCollection.SourceContentManager.GetUserContents(this.userProfile.UserId, search, page, pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / pageSize;
            ViewBag.PageIndex = page;
            return View();
        }

        public ActionResult Favor()
        {
            var p = Request["page"];
            int page = 0;
            int pageSize = 20;
            if (p != null) { int.TryParse(p, out page); }
            if (page == 0) { page = 1; }
            int count = 0;
            var search = Request["search"];

            ViewBag.Title = "关注信息";
            ViewBag.Selected = "Favor";

            ViewBag.SourceContents = InformationCollection.SourceContentManager.GetUserFavorContents(this.userProfile.UserId, search, page, pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / pageSize;
            ViewBag.PageIndex = page;
            return View();
        }

        public ActionResult Content(int id)
        {
            var source = InformationCollection.SourceManager.GetSource(id);
            if (source == null)
            {
                return this.HttpNotFound();
            }

            var p = Request["page"];
            int page = 0;
            int pageSize = 20;
            if (p != null) { int.TryParse(p, out page); }
            if (page == 0) { page = 1; }
            int count = 0;
            var search = Request["search"];

            ViewBag.Title = "所有信息";
            ViewBag.Selected = source.SourceName;

            ViewBag.SourceContents = InformationCollection.SourceContentManager.GetUserContents(this.userProfile.UserId, search, id, page, pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / pageSize;
            ViewBag.PageIndex = page;
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
            ViewBag.Selected = "Source";
            var model = SourceManager.GetSourceList(this.userProfile.UserId);
            return View(model);
        }

        public ActionResult AddSource(UserSource model)
        {
            SourceManager.AddSource(this.userProfile.UserId, model);
            return RedirectToAction("Source", "Home");
        }

        [HttpGet]
        public JsonResult AddFavor(int id)
        {
            UserSourceContentLinkManager.AddLink(this.userProfile.UserId, id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RemoveFavor(int id)
        {
            UserSourceContentLinkManager.RemoveLink(this.userProfile.UserId, id);
            return Json(true, JsonRequestBehavior.AllowGet);
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

        private List<UserSource> GetSources()
        {
            var userSources = Session["UserSources"] as List<UserSource>;
            if (userSources == null)
            {
                userSources = SourceManager.GetSourceList(this.userProfile.UserId);
                if (userSources == null)
                {
                    userSources = new List<UserSource>();
                }

                Session["UserSources"] = userSources;
            }

            return userSources;
        }
    }
}
