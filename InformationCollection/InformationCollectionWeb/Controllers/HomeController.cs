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
                this.userProfile = this.GetUser(User.Identity.Name);
                ViewBag.Sources = this.GetSources();
            }
        }

        private class QueryModel
        {
           internal int page;
           internal int pageSize;
           internal string search;
           internal DateTime beginDate;
           internal DateTime endDate;
        }

        public ActionResult Index()
        {
            QueryModel query = this.GetQueryModel();

            int count = 0;
            ViewBag.Selected = "Index";
            ViewBag.SourceContents = InformationCollection.SourceContentManager.GetUserContents(this.userProfile.UserId, query.search, query.beginDate, query.endDate, query.page, query.pageSize, out count);
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

        public ActionResult All()
        {
            QueryModel query = this.GetQueryModel();
            int count = 0;

            ViewBag.Selected = "All";
            ViewBag.SourceContents = InformationCollection.SourceContentManager.GetAllContents(query.search, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult Favor()
        {
            QueryModel query = this.GetQueryModel();

            int count = 0;

            ViewBag.Title = "关注信息";
            ViewBag.Selected = "Favor";

            ViewBag.SourceContents = InformationCollection.SourceContentManager.GetUserFavorContents(this.userProfile.UserId, query.search, query.beginDate, query.endDate, query.page, query.pageSize, out count);
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

            ViewBag.Title = "所有信息";
            ViewBag.Selected = source.SourceName;

            ViewBag.SourceContents = InformationCollection.SourceContentManager.GetUserContents(this.userProfile.UserId, id, query.search, query.beginDate, query.endDate, query.page, query.pageSize, out count);
            ViewBag.PageCount = 1 + (count - 1) / query.pageSize;
            ViewBag.PageIndex = query.page;
            return View();
        }

        public ActionResult Source()
        {
            ViewBag.Message = "源管理";
            ViewBag.Selected = "Source";
            var model = SourceManager.GetSourceList(this.userProfile.UserId);
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
        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            TimiSoft.InformationCollectionWeb.Controllers.AccountController.ManageMessageId? message = null;

            // 只有在当前登录用户是所有者时才取消关联帐户
            if (ownerAccount == User.Identity.Name)
            {
                // 使用事务来防止用户删除其上次使用的登录凭据
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = TimiSoft.InformationCollectionWeb.Controllers.AccountController.ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(TimiSoft.InformationCollectionWeb.Controllers.AccountController.ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == TimiSoft.InformationCollectionWeb.Controllers.AccountController.ManageMessageId.ChangePasswordSuccess ? "已更改你的密码。"
                : message == TimiSoft.InformationCollectionWeb.Controllers.AccountController.ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == TimiSoft.InformationCollectionWeb.Controllers.AccountController.ManageMessageId.RemoveLoginSuccess ? "已删除外部登录。"
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }
        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // 在某些失败方案中，ChangePassword 将引发异常，而不是返回 false。
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = TimiSoft.InformationCollectionWeb.Controllers.AccountController.ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "当前密码不正确或新密码无效。");
                    }
                }
            }
            else
            {
                // 用户没有本地密码，因此将删除由于缺少
                // OldPassword 字段而导致的所有验证错误
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = TimiSoft.InformationCollectionWeb.Controllers.AccountController.ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
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
