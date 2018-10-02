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
    public class PaysController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: Pays
        public ActionResult Index()
        {
            return View(db.Pays.ToList());
        }

        // GET: Pays/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Pays tbl_Pays = db.Pays.Find(id);
            if (tbl_Pays == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Pays);
        }

        // GET: Pays/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Pays,Nom,Enroles")] tbl_Pays tbl_Pays)
        {
            if (ModelState.IsValid)
            {
                db.Pays.Add(tbl_Pays);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_Pays);
        }

        // GET: Pays/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Pays tbl_Pays = db.Pays.Find(id);
            if (tbl_Pays == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Pays);
        }

        // POST: Pays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Pays,Nom,Enroles")] tbl_Pays tbl_Pays)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Pays).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_Pays);
        }

        // GET: Pays/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Pays tbl_Pays = db.Pays.Find(id);
            if (tbl_Pays == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Pays);
        }

        // POST: Pays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Pays tbl_Pays = db.Pays.Find(id);
            db.Pays.Remove(tbl_Pays);
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
