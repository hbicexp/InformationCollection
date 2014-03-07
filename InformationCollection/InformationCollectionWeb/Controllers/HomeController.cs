using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.WebPages.OAuth;
using TimiSoft.InformationCollection;
using TimiSoft.InformationCollection.Models;
using TimiSoft.InformationCollectionWeb.Models;
using WebMatrix.WebData;

namespace TimiSoft.InformationCollectionWeb.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private UserProfile userProfile;

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                this.userProfile = this.GetUser();
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
            QueryModel query = this.GetQueryModel();
            int count = 0;

            ViewBag.Selected = "Index";
            ViewBag.SourceContents = SourceContentManager.GetUserUnReadContents(this.userProfile.UserId, query.search, query.province, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult All()
        {
            QueryModel query = this.GetQueryModel();
            int count = 0;

            ViewBag.Selected = "All";
            ViewBag.SourceContents = SourceContentManager.GetAllSourceContents(query.search, query.province, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult UserAll()
        {
            QueryModel query = this.GetQueryModel();
            int count = 0;

            ViewBag.Selected = "UserAll";
            ViewBag.SourceContents = SourceContentManager.GetUserSourceContents(this.userProfile.UserId, query.search, query.province, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult Favor()
        {
            QueryModel query = this.GetQueryModel();
            int count = 0;

            ViewBag.Selected = "Favor";
            ViewBag.SourceContents = SourceContentManager.GetUserFavorContents(this.userProfile.UserId, query.search, query.province, query.beginDate, query.endDate, query.page, query.pageSize, out count);
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

            QueryModel query = this.GetQueryModel();
            int count = 0;

            ViewBag.Selected = source.SourceName;
            ViewBag.SourceContents = SourceContentManager.GetUserSourceContents(this.userProfile.UserId, id, query.search, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        private QueryModel GetQueryModel()
        {
            QueryModel query = new QueryModel();

            var p = Request["page"];
            int page = 0;
            if (p != null) { int.TryParse(p, out page); }
            if (page == 0) { page = 1; }
            query.page = page;
            query.pageSize = 20;
            query.search = Request["search"];
            query.province = Request["province"];

            var sBeginData = Request["begindate"];
            var sEndData = Request["enddate"];
            DateTime beginDate;
            DateTime endDate = DateTime.Now.Date.AddDays(1);
            if (DateTime.TryParse(sBeginData, out beginDate) && beginDate < DateTime.Now.Date)
            {
                query.beginDate = beginDate;
            }
            else
            {
                query.beginDate = new DateTime(2014, 1, 1);
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
            string search = Request["search"];
            int count = 0, pageSize = 15; 
            var p = Request["page"];
            int page = 0;
            if (p != null) { int.TryParse(p, out page); }
            if (page == 0) { page = 1; }

            ViewBag.Selected = "Source";
            var model = SourceManager.GetSourceList(this.userProfile.UserId, search, page, pageSize, out count); 
            ViewBag.PageCount = 1 + (count - 1) / pageSize;
            ViewBag.PageIndex = page;
            return View(model);
        }

        public ActionResult DelSource(int id)
        {
            SourceManager.RemoveSource(this.userProfile.UserId, id);
            Session["UserSources"] = null;
                ViewBag.Sources = this.GetSources();
            return RedirectToAction("Source", "Home");
        }

        public ActionResult AddSource(UserSource model)
        {
            SourceManager.AddSource(this.userProfile.UserId, model);
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
            UserSourceContentLinkManager.AddFavorLink(this.userProfile.UserId, id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Read(int id)
        {
            UserSourceContentLinkManager.RemoveLink(this.userProfile.UserId, id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RemoveFavor(int id)
        {
            UserSourceContentLinkManager.RemoveFavorLink(this.userProfile.UserId, id);
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
