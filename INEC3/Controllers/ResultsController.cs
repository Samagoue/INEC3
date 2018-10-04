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
    public class ResultsController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: Results
        public ActionResult Index()
        {
            var results = db.Results.Include(t => t.BureauVote).Include(t => t.Candidat).Include(t => t.Party);
            return View(results.ToList());
        }

        // GET: Results/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Results tbl_Results = db.Results.Find(id);
            if (tbl_Results == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Results);
        }

        // GET: Results/Create
        public ActionResult Create()
        {
            ViewBag.ID_Bureauvote = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom");
            ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom");
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle");
            return View();
        }

        // POST: Results/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Result,ID_Candidat,ID_Party,ID_Bureauvote,Voix,Pourcentage,Votants,Abstentions,Nuls,Exprimes")] tbl_Results tbl_Results)
        {
            if (ModelState.IsValid)
            {
                db.Results.Add(tbl_Results);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Bureauvote = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom", tbl_Results.ID_Bureauvote);
            ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom", tbl_Results.ID_Candidat);
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle", tbl_Results.ID_Party);
            return View(tbl_Results);
        }

        // GET: Results/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Results tbl_Results = db.Results.Find(id);
            if (tbl_Results == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Bureauvote = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom", tbl_Results.ID_Bureauvote);
            ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom", tbl_Results.ID_Candidat);
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle", tbl_Results.ID_Party);
            return View(tbl_Results);
        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Result,ID_Candidat,ID_Party,ID_Bureauvote,Voix,Pourcentage,Votants,Abstentions,Nuls,Exprimes")] tbl_Results tbl_Results)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Results).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Bureauvote = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom", tbl_Results.ID_Bureauvote);
            ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom", tbl_Results.ID_Candidat);
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle", tbl_Results.ID_Party);
            return View(tbl_Results);
        }

        // GET: Results/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Results tbl_Results = db.Results.Find(id);
            if (tbl_Results == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Results);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Results tbl_Results = db.Results.Find(id);
            db.Results.Remove(tbl_Results);
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
