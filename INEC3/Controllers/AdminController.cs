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
using INEC3.Helper;
using INEC3.Models.Service;
namespace INEC3.Controllers
{
    public class AdminController : Controller
    {
        private inecDBContext db = new inecDBContext();
        private Sqldbconn _db = new Sqldbconn();
        private AccountService accountService = new AccountService();
        private ResultsService resultsService = new ResultsService();
        [AuthenticatUser]
        public ActionResult Index()
        {
            var results = db.Results.Include(t => t.BureauVote).Include(t => t.Candidat).Include(t => t.Party);
            return View(results.ToList());
        }
        [AuthenticatUser]
        public ActionResult Result(int? id)
        {
            ViewBag.Message = "Artech Consulting";
            ViewBag.ID_Bureauvote = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom");
            ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom");
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle");
            ViewBag.Message = "Your application description page.";
            ViewBag.Province = new SelectList(db.Provinces.Select(s => new { ID_Province = s.ID_Province, Nom = s.Nom }).ToList(), "ID_Province", "Nom", 0);
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

        public ActionResult Users()
        {

            List<UserDisplay> model = new List<UserDisplay>();
            return View(accountService.GetUserList());
        }
        [HttpPost]
        public ActionResult ManageUser(string userid)
        {
            ViewBag.Roles = new SelectList(resultsService.GetRoleList(), "Id", "Role");
            ViewBag.Province = new SelectList(db.Provinces.Select(s => new { s.ID_Province, s.Nom }).ToList(), "ID_Province", "Nom", 0);
            ViewBag.Territoire = new SelectList(db.Territoires.Select(s => new { s.ID_Territoire, s.Nom }).ToList(), "ID_Territoire", "Nom", 0);
            ViewBag.Commune = new SelectList(db.Communes.Select(s => new { s.ID_Commune, s.Nom }).ToList(), "ID_Commune", "Nom", 0);
            ViewBag.Polingstation = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom");
            UserDisplay model = new UserDisplay();
            model.UserId = userid;

            return View(model);
        }

        public JsonResult GetParty(int candidateid)
        {
            try
            {
                var party = db.Candidats.Where(w => w.ID_Candidat == candidateid).Select(s => new { s.ID_Party, s.Party.Color }).FirstOrDefault();
                return Json(party, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult GetVoters(int polingstationid)
        {
            try
            {


                var res = new Dictionary<string, string>();
                var voters = db.BureauVotes.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.Commune.Enroles }).FirstOrDefault();
                var exprims = db.Results.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.Abstentions, s.Exprimes, s.Nuls, s.Total_Votes }).FirstOrDefault();
                var res1 = db.Results.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.ID_Result, s.ID_Candidat, s.Candidat.Nom, s.ID_Party, Party = s.Party.Sigle, s.Pourcentage, s.Voix }).ToList();
                res.Add("voters", JsonConvert.SerializeObject(voters));
                res.Add("list", JsonConvert.SerializeObject(res1));
                if (exprims != null)
                {
                    res.Add("exprims", JsonConvert.SerializeObject(exprims));
                }
                //return Json(voters, JsonRequestBehavior.AllowGet);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult SaveListRecord(tbl_Results obj)
        {
            try
            {


                if (obj.ID_Result == 0)
                {
                    List<tbl_Results> isExist = db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).ToList();//&& w.ID_Candidat == obj.ID_Candidat
                    if (isExist.Count == 0)
                    {
                        obj.Pourcentage = 100;
                        obj.Exprimes = (obj.Total_Votes + obj.Abstentions + obj.Nuls);
                        db.Results.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {

                        tbl_Results isdublicate = isExist.Where(w => w.ID_Candidat == obj.ID_Candidat).FirstOrDefault();
                        if (isdublicate != null)
                        {
                            db.Results.RemoveRange(db.Results.Where(w => w.ID_Result == isdublicate.ID_Result).ToList());
                            db.SaveChanges();
                            //Total_Votes = db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).Sum(s => s.Voix);
                        }

                        int Total_Votes = 0;
                        if (db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).Count() > 0)
                        {
                            var tem_total = db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).Sum(s => s.Voix);
                            Total_Votes = string.IsNullOrEmpty(Convert.ToString(tem_total)) ? 0 : tem_total;
                        }


                        double Perc = Double.Parse(((obj.Voix * 100.00) / (Total_Votes + obj.Voix)).ToString("0.00"));
                        obj.Pourcentage = Perc;
                        obj.Total_Votes = Total_Votes + obj.Voix;
                        db.Results.Add(obj);
                        db.SaveChanges();
                        Total_Votes = Total_Votes + obj.Voix;

                        foreach (var it in isExist)
                        {
                            tbl_Results re = new tbl_Results();
                            re = isExist.Where(w => w.ID_Result == it.ID_Result).FirstOrDefault();
                            double Percc = Double.Parse(((re.Voix * 100.00) / Total_Votes).ToString("0.00"));
                            re.Pourcentage = Percc;
                            re.Total_Votes = Total_Votes;
                            re.Abstentions = obj.Abstentions;
                            re.Nuls = obj.Nuls;
                            re.Exprimes = (re.Total_Votes + obj.Abstentions + obj.Nuls);

                            db.SaveChanges();
                        }
                        //foreach()
                    }


                }
                var res = db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).Select(s => new { s.ID_Result, s.ID_Candidat, s.Candidat.Nom, s.ID_Party, Party = s.Party.Sigle, s.Pourcentage, s.Voix, s.Exprimes, s.Nuls, s.Abstentions, s.Total_Votes }).ToList();
                DataSet dt = new DataSet();
                dt = _db.GetDatatable("proc_GetProvinceResult", "");
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
                hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));

                SqlNotification objRepo = new SqlNotification();
                objRepo.GetAllMessages();

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult RemoveResult(int ResultId, int ID_Bureauvote)
        {
            try
            {
                db.Results.RemoveRange(db.Results.Where(w => w.ID_Result == ResultId).ToList());
                db.SaveChanges();
                List<tbl_Results> isExist = db.Results.Where(w => w.ID_Bureauvote == ID_Bureauvote).ToList();

                if (isExist.Count > 0)
                {
                    int Total_Votes = isExist.Where(w => w.ID_Bureauvote == ID_Bureauvote).Sum(s => s.Voix);
                    foreach (var it in isExist)
                    {
                        tbl_Results re = new tbl_Results();
                        re = isExist.Where(w => w.ID_Result == it.ID_Result).FirstOrDefault();
                        double Percc = Double.Parse(((re.Voix * 100.00) / Total_Votes).ToString("0.00"));
                        re.Pourcentage = Percc;
                        re.Total_Votes = Total_Votes;
                        re.Exprimes = (Total_Votes + re.Abstentions + re.Nuls);
                        db.SaveChanges();
                    }
                }
                var res = db.Results.Where(w => w.ID_Bureauvote == ID_Bureauvote).Select(s => new { s.ID_Result, s.ID_Candidat, s.Candidat.Nom, s.ID_Party, Party = s.Party.Sigle, s.Pourcentage, s.Voix, s.Exprimes, s.Nuls, s.Abstentions, s.Total_Votes }).ToList();
                DataSet dt = new DataSet();
                dt = _db.GetDatatable("proc_GetProvinceResult", "");
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
                hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));

                SqlNotification objRepo = new SqlNotification();
                objRepo.GetAllMessages();

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult GetTerritoireList(int ProvinceId)
        {
            try
            {
                var Ter = db.Territoires.Where(w => w.ID_Province == ProvinceId).Select(s => new { s.ID_Territoire, s.Nom }).ToList();
                return Json(Ter, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult GetPoolingStationList(int CommuneId)
        {
            try
            {
                var res = db.BureauVotes.Where(w => w.ID_Commune == CommuneId).Select(s => new { s.ID_Bureauvote, s.Nom }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult GetCommune(int ProvinceId, int TerritoireId)
        {
            try
            {
                var res = db.Communes.Where(w => w.ID_Province == ProvinceId && w.ID_Territoire == TerritoireId).Select(s => new { s.ID_Commune, s.Nom }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet); }
        }
    }
}