using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimiSoft.InformationCollection;
using TimiSoft.InformationCollection.Models;
using TimiSoft.InformationCollectionWeb.Controllers;

namespace TimiSoft.InformationCollectionWeb.Areas.Admin.Controllers
{
    [Authorize]
    public class ManagementController : BaseController
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return RedirectToAction("Index", "User");
        }

        public ActionResult Source()
        {
            var model = SourceManager.GetSourceList();
            ViewBag.Title = "源管理";
            ViewBag.Selected = "Index";
            return View(model);
        }

        public ActionResult Collect(int id)
        {
            var source = SourceManager.GetSource(id);
            if( source == null)
            {
                return HttpNotFound();
            }

            SourceContentManager.ReloadSourceRegexes();
            SourceContentManager.Collect(source, DateTime.Now, SourceContentType.System);

            ViewBag.Title = "源管理";
            ViewBag.Selected = "Index";
            return RedirectToAction("Content", "Management", new { id });
        }

        public ActionResult Clear(int id)
        {
            var source = SourceManager.GetSource(id);
            if (source == null)
            {
                return HttpNotFound();
            }

            SourceContentManager.ReloadSourceRegexes();
            SourceContentManager.ClearSourceContents(id);

            ViewBag.Title = "源管理";
            ViewBag.Selected = "Index";
            return RedirectToAction("Content", "Management", new { id });
        }

        public ActionResult Content(int id)
        {
            var model = SourceContentManager.GetSourceContents(id);

            ViewBag.Title = "源管理";
            ViewBag.Selected = "Index";
            return View(model);
        }

    }
}
