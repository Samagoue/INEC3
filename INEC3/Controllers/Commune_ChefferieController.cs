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
    public class Commune_ChefferieController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: Commune_Chefferie
        public ActionResult Index()
        {
            var communes = db.Communes.Include(t => t.Province).Include(t => t.Territoire);
            return View(communes.ToList());
        }

        // GET: Commune_Chefferie/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Commune_Chefferie tbl_Commune_Chefferie = db.Communes.Find(id);
            if (tbl_Commune_Chefferie == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Commune_Chefferie);
        }

        // GET: Commune_Chefferie/Create
        public ActionResult Create()
        {
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom");
            ViewBag.ID_Territoire = new SelectList(db.Territoires, "ID_Territoire", "Nom");
            return View();
        }

        // POST: Commune_Chefferie/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Commune,Nom,Enroles,Sieges,ID_Territoire,ID_Province")] tbl_Commune_Chefferie tbl_Commune_Chefferie)
        {
            if (ModelState.IsValid)
            {
                db.Communes.Add(tbl_Commune_Chefferie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Commune_Chefferie.ID_Province);
            ViewBag.ID_Territoire = new SelectList(db.Territoires, "ID_Territoire", "Nom", tbl_Commune_Chefferie.ID_Territoire);
            return View(tbl_Commune_Chefferie);
        }

        // GET: Commune_Chefferie/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Commune_Chefferie tbl_Commune_Chefferie = db.Communes.Find(id);
            if (tbl_Commune_Chefferie == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Commune_Chefferie.ID_Province);
            ViewBag.ID_Territoire = new SelectList(db.Territoires, "ID_Territoire", "Nom", tbl_Commune_Chefferie.ID_Territoire);
            return View(tbl_Commune_Chefferie);
        }

        // POST: Commune_Chefferie/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Commune,Nom,Enroles,Sieges,ID_Territoire,ID_Province")] tbl_Commune_Chefferie tbl_Commune_Chefferie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Commune_Chefferie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Commune_Chefferie.ID_Province);
            ViewBag.ID_Territoire = new SelectList(db.Territoires, "ID_Territoire", "Nom", tbl_Commune_Chefferie.ID_Territoire);
            return View(tbl_Commune_Chefferie);
        }

        // GET: Commune_Chefferie/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Commune_Chefferie tbl_Commune_Chefferie = db.Communes.Find(id);
            if (tbl_Commune_Chefferie == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Commune_Chefferie);
        }

        // POST: Commune_Chefferie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Commune_Chefferie tbl_Commune_Chefferie = db.Communes.Find(id);
            db.Communes.Remove(tbl_Commune_Chefferie);
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
