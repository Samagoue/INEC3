using System;
using INEC3.DbConn;
using INEC3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.SignalR;
using System.Data;
using Newtonsoft.Json;

namespace INEC3.Controllers
{
    public class AdminController : Controller
    {
        private inecDBContext db = new inecDBContext();
        private Sqldbconn _db = new Sqldbconn();

        public ActionResult Index()
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
            ViewBag.Province = new SelectList(db.Provinces.Select(s => new { ID_Province = s.ID_Province, Nom = s.Nom }).ToList(), "ID_Province", "Nom", 0);
            //ViewBag.Territoire = new SelectList(db.Provinces.Select(s=>s.).ToList(), "", "");
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

        public JsonResult SaveRecord(Results tbl_Results)
        {
            try
            {

                foreach (var it in tbl_Results.ResultList)
                {
                    tbl_Results re = new tbl_Results();

                    re.ID_Result = tbl_Results.ID_Result;
                    re.ID_Bureauvote = tbl_Results.ID_Bureauvote;
                    re.Votants = tbl_Results.Votants;
                    re.Abstentions = tbl_Results.Abstentions;
                    re.Nuls = tbl_Results.Nuls;
                    re.Exprimes = tbl_Results.Exprimes;

                    re.ID_Candidat = it.ID_Candidat;
                    re.ID_Party = it.ID_Party;
                    re.Pourcentage = it.Pourcentage;
                    re.Voix = it.Voix;
                    re.Total_Votes = tbl_Results.Total_Votes;

                    if (tbl_Results.ID_Result == 0)
                    {
                        db.Results.Add(re);
                        db.SaveChanges();
                    }
                    //else
                    //{
                    //    db.Entry(re).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
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

        public JsonResult GetTerritoireList(int ProvinceId)
        {
            var Ter = db.Territoires.Where(w => w.ID_Province == ProvinceId).Select(s => new { s.ID_Province, s.Nom }).ToList();
            return Json(Ter, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPoolingStationList(int CommuneId)
        {
            var res = db.BureauVotes.Where(w => w.ID_Commune == CommuneId).Select(s => new { s.ID_Bureauvote, s.Nom }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommune(int ProvinceId, int TerritoireId)
        {
            var res = db.Communes.Where(w => w.ID_Province == ProvinceId && w.ID_Territoire == TerritoireId).Select(s => new { s.ID_Commune, s.Nom }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}