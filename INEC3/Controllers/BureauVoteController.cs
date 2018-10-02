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
    public class BureauVoteController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: BureauVote
        public ActionResult Index()
        {
            var bureauVotes = db.BureauVotes.Include(t => t.Commune);
            return View(bureauVotes.ToList());
        }

        // GET: BureauVote/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_BureauVote tbl_BureauVote = db.BureauVotes.Find(id);
            if (tbl_BureauVote == null)
            {
                return HttpNotFound();
            }
            return View(tbl_BureauVote);
        }

        // GET: BureauVote/Create
        public ActionResult Create()
        {
            ViewBag.ID_Commune = new SelectList(db.Communes, "ID_Commune", "Nom");
            return View();
        }

        // POST: BureauVote/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Bureauvote,Nom,Addresse,Enroles,ID_Commune,Code_SV")] tbl_BureauVote tbl_BureauVote)
        {
            if (ModelState.IsValid)
            {
                db.BureauVotes.Add(tbl_BureauVote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Commune = new SelectList(db.Communes, "ID_Commune", "Nom", tbl_BureauVote.ID_Commune);
            return View(tbl_BureauVote);
        }

        // GET: BureauVote/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_BureauVote tbl_BureauVote = db.BureauVotes.Find(id);
            if (tbl_BureauVote == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Commune = new SelectList(db.Communes, "ID_Commune", "Nom", tbl_BureauVote.ID_Commune);
            return View(tbl_BureauVote);
        }

        // POST: BureauVote/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Bureauvote,Nom,Addresse,Enroles,ID_Commune,Code_SV")] tbl_BureauVote tbl_BureauVote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_BureauVote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Commune = new SelectList(db.Communes, "ID_Commune", "Nom", tbl_BureauVote.ID_Commune);
            return View(tbl_BureauVote);
        }

        // GET: BureauVote/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_BureauVote tbl_BureauVote = db.BureauVotes.Find(id);
            if (tbl_BureauVote == null)
            {
                return HttpNotFound();
            }
            return View(tbl_BureauVote);
        }

        // POST: BureauVote/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_BureauVote tbl_BureauVote = db.BureauVotes.Find(id);
            db.BureauVotes.Remove(tbl_BureauVote);
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
