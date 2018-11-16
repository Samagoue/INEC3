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
    public class CirconscriptionController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: Circonscription
        public ActionResult Index()
        {
            var circonscriptions = db.Circonscriptions.Include(t => t.Province).Include(t => t.Territoire);
            return View(circonscriptions.ToList());
        }

        // GET: Circonscription/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Circonscription tbl_Circonscription = db.Circonscriptions.Find(id);
            if (tbl_Circonscription == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Circonscription);
        }

        // GET: Circonscription/Create
        public ActionResult Create()
        {
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom");
            ViewBag.ID_Territoire = new SelectList(db.Territoires, "ID_Territoire", "Nom");
            return View();
        }

        // POST: Circonscription/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Circonscription,Nom,Enroles,SiegesProv,SiegesNat,ID_Territoire,ID_Province")] tbl_Circonscription tbl_Circonscription)
        {
            if (ModelState.IsValid)
            {
                db.Circonscriptions.Add(tbl_Circonscription);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Circonscription.ID_Province);
            ViewBag.ID_Territoire = new SelectList(db.Territoires, "ID_Territoire", "Nom", tbl_Circonscription.ID_Territoire);
            return View(tbl_Circonscription);
        }

        // GET: Circonscription/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Circonscription tbl_Circonscription = db.Circonscriptions.Find(id);
            if (tbl_Circonscription == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Circonscription.ID_Province);
            ViewBag.ID_Territoire = new SelectList(db.Territoires, "ID_Territoire", "Nom", tbl_Circonscription.ID_Territoire);
            return View(tbl_Circonscription);
        }

        // POST: Circonscription/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Circonscription,Nom,Enroles,SiegesProv,SiegesNat,ID_Territoire,ID_Province")] tbl_Circonscription tbl_Circonscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Circonscription).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Province = new SelectList(db.Provinces, "ID_Province", "Nom", tbl_Circonscription.ID_Province);
            ViewBag.ID_Territoire = new SelectList(db.Territoires, "ID_Territoire", "Nom", tbl_Circonscription.ID_Territoire);
            return View(tbl_Circonscription);
        }

        // GET: Circonscription/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Circonscription tbl_Circonscription = db.Circonscriptions.Find(id);
            if (tbl_Circonscription == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Circonscription);
        }

        // POST: Circonscription/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Circonscription tbl_Circonscription = db.Circonscriptions.Find(id);
            db.Circonscriptions.Remove(tbl_Circonscription);
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
