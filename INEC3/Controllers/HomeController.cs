using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INEC3.Models;
using Microsoft.AspNet.SignalR;
using INEC3.DbConn;
using System.Data;
using Newtonsoft.Json;

namespace INEC3.Controllers
{
    public class HomeController : Controller
    {
        private inecDBContext db = new inecDBContext();
        private Sqldbconn _db = new Sqldbconn();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {


            return View();
        }

        public ActionResult Chart()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Carte()
        {
            DataSet dt = new DataSet();
            dt = _db.GetDatatable("proc_GetProvinceResult", "");
            ViewBag.Province = JsonConvert.SerializeObject(dt);
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Artech Consulting";

            return View();
        }


        public ActionResult ResultList()
        {
            var results = db.Results.Include(t => t.BureauVote).Include(t => t.Candidat).Include(t => t.Party);
            return View(results.ToList());
        }

        public ActionResult Result(int? id)
        {
            ViewBag.Message = "Artech Consulting";
            ViewBag.ID_Bureauvote = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom");
            ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom");
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle");
            ViewBag.Message = "Your application description page.";

            if (id == null)
            {
                return View();
            }
            else
            {
                tbl_Results tbl_Results = db.Results.Find(id);
                if (tbl_Results == null)
                {
                    return HttpNotFound();
                }
                return View(tbl_Results);
            }

        }

        public JsonResult GetParty(int candidateid)
        {
            var party = db.Candidats.Where(w => w.ID_Candidat == candidateid).Select(s => new { s.ID_Party, s.Party.Color }).FirstOrDefault();
            return Json(party, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVoters(int polingstationid)
        {
            var voters = db.BureauVotes.Where(w => w.ID_Bureauvote == polingstationid).Select(s => s.Commune.Enroles).FirstOrDefault();
            return Json(voters, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRecord(tbl_Results tbl_Results)
        {
            try
            {


                if (tbl_Results.ID_Result == 0)
                {
                    db.Results.Add(tbl_Results);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(tbl_Results).State = EntityState.Modified;
                    db.SaveChanges();
                }

                DataSet dt = new DataSet();
                dt = _db.GetDatatable("proc_GetProvinceResult", "");
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
                hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));


                return Json("success", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json("error " + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}