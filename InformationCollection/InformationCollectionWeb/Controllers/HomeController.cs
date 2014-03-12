using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.WebPages.OAuth;
using TimiSoft.InformationCollection;
using TimiSoft.InformationCollection.Models;
using TimiSoft.InformationCollectionWeb.App_Start;
using TimiSoft.InformationCollectionWeb.Models;
using WebMatrix.WebData;

namespace TimiSoft.InformationCollectionWeb.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private static DateTime queryDate = new DateTime(2014, 1, 1);

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                this.ViewBag.Sources = this.GetSources();
            }
        }

        private class QueryModel
        {
           internal int page;
           internal int pageSize;
           internal string search;
           internal DateTime beginDate;
           internal DateTime endDate;
           internal string province;
        }

        public ActionResult Index()
        {
            var userProfile = this.GetUser();
            var query = this.GetQueryModel();
            var count = 0;

            ViewBag.Selected = "Index";
            ViewBag.SourceContents = SourceContentManager.GetUserUnReadContents(BaseConfig.Company, userProfile.UserId, query.search, query.province, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult All()
        {
            var query = this.GetQueryModel();
            var count = 0;

            ViewBag.Selected = "All";
            ViewBag.SourceContents = SourceContentManager.GetAllSourceContents(BaseConfig.Company, query.search, query.province, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult UserAll()
        {
            var userProfile = this.GetUser();
            var query = this.GetQueryModel();
            var count = 0;

            ViewBag.Selected = "UserAll";
            ViewBag.SourceContents = SourceContentManager.GetUserSourceContents(BaseConfig.Company, userProfile.UserId, query.search, query.province, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult Favor()
        {
            var userProfile = this.GetUser();
            var query = this.GetQueryModel();
            var count = 0;

            ViewBag.Selected = "Favor";
            ViewBag.SourceContents = SourceContentManager.GetUserFavorContents(BaseConfig.Company, userProfile.UserId, query.search, query.province, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult Content(int id)
        {
            var source = InformationCollection.SourceManager.GetSource(id);
            if (source == null)
            {
                return this.HttpNotFound();
            }

            var userProfile = this.GetUser();
            var query = this.GetQueryModel();
            var count = 0;

            ViewBag.Selected = source.SourceName;
            ViewBag.SourceContents = SourceContentManager.GetUserSourceContents(BaseConfig.Company, userProfile.UserId, id, query.search, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        private QueryModel GetQueryModel()
        {
            var query = new QueryModel();
            var p = Request["page"];
            var page = 0;

            if (p != null) { int.TryParse(p, out page); }
            if (page == 0) { page = 1; }
            query.page = page;
            query.pageSize = 20;
            query.search = Request["search"];
            query.province = Request["province"];

            var sBeginData = Request["begindate"];
            var sEndData = Request["enddate"];
            DateTime beginDate;
            DateTime endDate;
            if (DateTime.TryParse(sBeginData, out beginDate) && beginDate < DateTime.Now.Date)
            {
                query.beginDate = beginDate > queryDate ? beginDate : queryDate;
            }
            else
            {
                query.beginDate = queryDate;
            }

            if (DateTime.TryParse(sEndData, out endDate) && beginDate <= endDate)
            {
                query.endDate = endDate.AddDays(1);
            }
            else
            {
                query.endDate = DateTime.Now.Date.AddDays(1);
            }

            return query;
        }

        public ActionResult Source()
        {
            var userProfile = this.GetUser();
            var count = 0;
            var pageSize = 15; 
            var search = Request["search"];
            var p = Request["page"];
            int page = 0;
            if (p != null) { int.TryParse(p, out page); }
            if (page == 0) { page = 1; }

            ViewBag.Selected = "Source";
            var model = SourceManager.GetSourceList(userProfile.UserId, search, page, pageSize, out count); 
            ViewBag.PageCount = 1 + (count - 1) / pageSize;
            ViewBag.PageIndex = page;
            return View(model);
        }

        public ActionResult DelSource(int id)
        {
            var userProfile = this.GetUser();
            SourceManager.RemoveSource(userProfile.UserId, id);
            Session["UserSources"] = null;
                ViewBag.Sources = this.GetSources();
            return RedirectToAction("Source", "Home");
        }

        public ActionResult AddSource(SourceView model)
        {
            var userProfile = this.GetUser();
            SourceManager.AddSource(BaseConfig.Company, userProfile.UserId, model);
            Session["UserSources"] = null;
            ViewBag.Sources = this.GetSources();
            return RedirectToAction("Source", "Home");
        }

        [HttpGet]
        public JsonResult GetSource(int id)
        {
            var userSource = this.GetSources().Where(p => p.SourceId == id).FirstOrDefault();
            return Json(userSource, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AddFavor(int id)
        {
            var userProfile = this.GetUser();
            UserSourceContentLinkManager.AddFavorLink(userProfile.UserId, id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Read(int id)
        {
            var userProfile = this.GetUser();
            UserSourceContentLinkManager.RemoveLink(userProfile.UserId, id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RemoveFavor(int id)
        {
            var userProfile = this.GetUser();
            UserSourceContentLinkManager.RemoveFavorLink(userProfile.UserId, id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private UserProfile GetUser()
        {
            var userProfile = Session["UserProfile"] as UserProfile;
            if (userProfile == null)
            {
                userProfile = UserManager.GetUser(User.Identity.Name);
                if (userProfile == null)
                {
                    return null;
                }

                Session["UserProfile"] = userProfile;
            }

            return userProfile;
        }

        private List<SourceView> GetSources()
        {
            var userSources = Session["UserSources"] as List<SourceView>;
            if (userSources == null)
            {
                var userProfile = this.GetUser();
                userSources = SourceManager.GetSourceList(userProfile.UserId);
                if (userSources == null)
                {
                    userSources = new List<SourceView>();
                }

                Session["UserSources"] = userSources;
            }

            return userSources;
        }
    }
}
