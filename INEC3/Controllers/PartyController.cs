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
    public class PartyController : Controller
    {
        private inecDBContext db = new inecDBContext();

        // GET: Party
        public ActionResult Index()
        {
            return View(db.Parties.ToList());
        }

        // GET: Party/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Party tbl_Party = db.Parties.Find(id);
            if (tbl_Party == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Party);
        }

        // GET: Party/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Party/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Party,Sigle,Nom,Addresse,Dirigeant")] tbl_Party tbl_Party)
        {
            if (ModelState.IsValid)
            {
                db.Parties.Add(tbl_Party);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_Party);
        }

        // GET: Party/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Party tbl_Party = db.Parties.Find(id);
            if (tbl_Party == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Party);
        }

        // POST: Party/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Party,Sigle,Nom,Addresse,Dirigeant")] tbl_Party tbl_Party)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Party).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_Party);
        }

        // GET: Party/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Party tbl_Party = db.Parties.Find(id);
            if (tbl_Party == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Party);
        }

        // POST: Party/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Party tbl_Party = db.Parties.Find(id);
            db.Parties.Remove(tbl_Party);
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
