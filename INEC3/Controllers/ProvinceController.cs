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
    public class ProvinceController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: Province
        public ActionResult Index()
        {
            var provinces = db.Provinces.Include(t => t.Pays);
            return View(provinces.ToList());
        }

        // GET: Province/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Province tbl_Province = db.Provinces.Find(id);
            if (tbl_Province == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Province);
        }

        // GET: Province/Create
        public ActionResult Create()
        {
            ViewBag.ID_Pays = new SelectList(db.Pays, "ID_Pays", "Nom");
            return View();
        }

        // POST: Province/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Province,Nom,Enroles,GID_1,ID_Pays")] tbl_Province tbl_Province)
        {
            if (ModelState.IsValid)
            {
                db.Provinces.Add(tbl_Province);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Pays = new SelectList(db.Pays, "ID_Pays", "Nom", tbl_Province.ID_Pays);
            return View(tbl_Province);
        }

        // GET: Province/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Province tbl_Province = db.Provinces.Find(id);
            if (tbl_Province == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Pays = new SelectList(db.Pays, "ID_Pays", "Nom", tbl_Province.ID_Pays);
            return View(tbl_Province);
        }

        // POST: Province/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Province,Nom,Enroles,GID_1,ID_Pays")] tbl_Province tbl_Province)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Province).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Pays = new SelectList(db.Pays, "ID_Pays", "Nom", tbl_Province.ID_Pays);
            return View(tbl_Province);
        }

        // GET: Province/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Province tbl_Province = db.Provinces.Find(id);
            if (tbl_Province == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Province);
        }

        // POST: Province/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Province tbl_Province = db.Provinces.Find(id);
            db.Provinces.Remove(tbl_Province);
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
