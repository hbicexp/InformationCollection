using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimiSoft.InformationCollection;
using TimiSoft.InformationCollectionWeb.Controllers;

namespace TimiSoft.InformationCollectionWeb.Areas.Admin.Controllers
{
    public class DataController : BaseController
    {
        //
        // GET: /Admin/Data/

        public ActionResult Index()
        {
            ViewBag.Title = "数据清理";
            ViewBag.Selected = "Data";
            return View();
        }

        public ActionResult Clear()
        {
            var intervalString = Request["Interval"];
            int interval;
            if (!int.TryParse(intervalString, out interval))
            {
                return HttpNotFound();
            }

            SourceContentManager.ClearSourceContents(interval);
            ViewBag.Message = "清理成功！";
            return View();
        }
    }
}
