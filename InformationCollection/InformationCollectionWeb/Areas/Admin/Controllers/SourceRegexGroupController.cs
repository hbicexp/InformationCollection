using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimiSoft.InformationCollection.Models;
using TimiSoft.InformationCollectionWeb.Controllers;

namespace TimiSoft.InformationCollectionWeb.Areas.Admin.Controllers
{
    public class SourceRegexGroupController : BaseController
    {
        private ICContext db = new ICContext();

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            ViewBag.Title = "规则组管理";
            ViewBag.Selected = "SourceRegexGroup";
        }

        //
        // GET: /Admin/SourceRegexGroup/

        public ActionResult Index()
        {
            return View(db.SourceRegexGroups.OrderBy(p=>p.Name).ToList());
        }

        //
        // GET: /Admin/SourceRegexGroup/Details/5

        public ActionResult Details(int id = 0)
        {
            SourceRegexGroup sourceregexgroup = db.SourceRegexGroups.Find(id);
            if (sourceregexgroup == null)
            {
                return HttpNotFound();
            }
            return View(sourceregexgroup);
        }

        //
        // GET: /Admin/SourceRegexGroup/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/SourceRegexGroup/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(SourceRegexGroup sourceregexgroup)
        {
            if (ModelState.IsValid)
            {
                db.SourceRegexGroups.Add(sourceregexgroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sourceregexgroup);
        }

        //
        // GET: /Admin/SourceRegexGroup/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SourceRegexGroup sourceregexgroup = db.SourceRegexGroups.Find(id);
            if (sourceregexgroup == null)
            {
                return HttpNotFound();
            }
            return View(sourceregexgroup);
        }

        //
        // POST: /Admin/SourceRegexGroup/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(SourceRegexGroup sourceregexgroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sourceregexgroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sourceregexgroup);
        }

        //
        // GET: /Admin/SourceRegexGroup/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SourceRegexGroup sourceregexgroup = db.SourceRegexGroups.Find(id);
            if (sourceregexgroup == null)
            {
                return HttpNotFound();
            }
            return View(sourceregexgroup);
        }

        //
        // POST: /Admin/SourceRegexGroup/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DeleteConfirmed(int id)
        {
            SourceRegexGroup sourceregexgroup = db.SourceRegexGroups.Find(id);
            db.SourceRegexGroups.Remove(sourceregexgroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}