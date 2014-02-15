using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimiSoft.InformationCollection;
using TimiSoft.InformationCollection.Models;

namespace TimiSoft.InformationCollectionWeb.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            ViewData["Message"] = "源管理";
            //UserProfile userProfile = this.GetUser(User.Identity.Name);
            var model = SourceManager.GetSourceList(1);
            return View(model);
        }

        public ActionResult Collect(int id)
        {
            var source = SourceManager.GetSource(id);
            if( source == null)
            {
                return HttpNotFound();
            }

            SourceContentManager.Collect(source, DateTime.Now, SourceContentType.System);
            return View();
        }

        public ActionResult Content(int id)
        {
            var model = SourceContentManager.GetContents(id);
            return View(model);
        }

    }
}
