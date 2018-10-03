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
    public class CandidatController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: Candidat
        public ActionResult Index()
        {
            var candidats = db.Candidats.Include(t => t.Party);
            return View(candidats.ToList());
        }

        // GET: Candidat/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Candidat tbl_Candidat = db.Candidats.Find(id);
            if (tbl_Candidat == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Candidat);
        }

        // GET: Candidat/Create
        public ActionResult Create()
        {
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle");
            return View();
        }

        // POST: Candidat/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Candidat,Nom,ID_Party")] tbl_Candidat tbl_Candidat)
        {
            if (ModelState.IsValid)
            {
                db.Candidats.Add(tbl_Candidat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle", tbl_Candidat.ID_Party);
            return View(tbl_Candidat);
        }

        // GET: Candidat/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Candidat tbl_Candidat = db.Candidats.Find(id);
            if (tbl_Candidat == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle", tbl_Candidat.ID_Party);
            return View(tbl_Candidat);
        }

        // POST: Candidat/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Candidat,Nom,ID_Party")] tbl_Candidat tbl_Candidat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Candidat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle", tbl_Candidat.ID_Party);
            return View(tbl_Candidat);
        }

        // GET: Candidat/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Candidat tbl_Candidat = db.Candidats.Find(id);
            if (tbl_Candidat == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Candidat);
        }

        // POST: Candidat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Candidat tbl_Candidat = db.Candidats.Find(id);
            db.Candidats.Remove(tbl_Candidat);
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
