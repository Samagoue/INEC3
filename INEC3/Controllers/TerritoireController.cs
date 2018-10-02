using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using INEC3.Models;

namespace INEC3.Controllers
{
    public class TerritoireController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: Territoire
        public ActionResult Index()
        {
            var territoires = db.Territoires.Include(t => t.Province);
            return View(territoires.ToList());
        }

        // GET: Territoire/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Territoire tbl_Territoire = db.Territoires.Find(id);
            if (tbl_Territoire == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Territoire);
        }

        // GET: Territoire/Create
        public ActionResult Create()
        {
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom");
            return View();
        }

        // POST: Territoire/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Territoire,Nom,Enroles,GID_2,GID_1,Sieges,ID_Province")] tbl_Territoire tbl_Territoire)
        {
            if (ModelState.IsValid)
            {
                db.Territoires.Add(tbl_Territoire);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Territoire.ID_Province);
            return View(tbl_Territoire);
        }

        // GET: Territoire/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Territoire tbl_Territoire = db.Territoires.Find(id);
            if (tbl_Territoire == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Territoire.ID_Province);
            return View(tbl_Territoire);
        }

        // POST: Territoire/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Territoire,Nom,Enroles,GID_2,GID_1,Sieges,ID_Province")] tbl_Territoire tbl_Territoire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Territoire).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Territoire.ID_Province);
            return View(tbl_Territoire);
        }

        // GET: Territoire/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Territoire tbl_Territoire = db.Territoires.Find(id);
            if (tbl_Territoire == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Territoire);
        }

        // POST: Territoire/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Territoire tbl_Territoire = db.Territoires.Find(id);
            db.Territoires.Remove(tbl_Territoire);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
