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
    public class SourceRegexController : BaseController
    {
        private ICContext db = new ICContext();

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            ViewBag.Title = "规则管理";
            ViewBag.Selected = "SourceRegex";
        }

        //
        // GET: /UserResource/

        public ActionResult Index(int id = 0)
        {
            var list = from p in db.SourceRegexes
                       join q in db.SourceRegexGroups
                       on p.SourceRegexGroupId equals q.SourceRegexGroupId
                       where id == 0 || p.SourceRegexGroupId == id
                       orderby q.Name, p.RegexType, p.Name
                       select new InformationCollection.Models.SourceRegexView
                       {
                           IsMatched = p.IsMatched,
                           Name = p.Name,
                           Regex = p.Regex,
                           RegexType = p.RegexType,
                           SourceRegexGroupId = q.SourceRegexGroupId,
                           SourceRegexId = p.SourceRegexId,
                           SourceRegexGroup = q.Name
                       };
            return View(list.ToList());
        }

        //
        // GET: /UserResource/Details/5

        public ActionResult Details(int id = 0)
        {
            SourceRegex sourceregex = db.SourceRegexes.Find(id);
            if (sourceregex == null)
            {
                return HttpNotFound();
            }
            return View(sourceregex);
        }

        //
        // GET: /UserResource/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /UserResource/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(InformationCollection.Models.SourceRegexView sourceregex)
        {
            if (ModelState.IsValid)
            {
                var sourceRegexGroup = db.SourceRegexGroups.Where(p => p.Name == sourceregex.SourceRegexGroup).FirstOrDefault();
                if (sourceRegexGroup == null)
                {
                    sourceRegexGroup = new SourceRegexGroup()
                    {
                        Name = sourceregex.SourceRegexGroup
                    };
                    db.SourceRegexGroups.Add(sourceRegexGroup);
                    db.SaveChanges();
                }

                sourceRegexGroup.SourceRegexes.Add(new SourceRegex
                {
                    IsMatched = sourceregex.IsMatched,
                    RegexType = sourceregex.RegexType,
                    Name = sourceregex.Name,
                    Regex = sourceregex.Regex
                });
                db.SaveChanges();
                return RedirectToAction("Index", new { id = sourceRegexGroup.SourceRegexGroupId });
            }

            return View(sourceregex);
        }

        //
        // GET: /UserResource/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SourceRegex sourceregex = db.SourceRegexes.Find(id);
            if (sourceregex == null)
            {
                return HttpNotFound();
            }

            return View(sourceregex);
        }

        //
        // POST: /UserResource/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(SourceRegex sourceregex)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sourceregex).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = sourceregex.SourceRegexGroupId });
            }

            return View(sourceregex);
        }

        //
        // GET: /UserResource/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SourceRegex sourceregex = db.SourceRegexes.Find(id);
            if (sourceregex == null)
            {
                return HttpNotFound();
            }

            return View(sourceregex);
        }

        //
        // POST: /UserResource/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SourceRegex sourceregex = db.SourceRegexes.Find(id);
            db.SourceRegexes.Remove(sourceregex);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = sourceregex.SourceRegexGroupId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}