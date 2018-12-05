using INEC3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data;
using Newtonsoft.Json;
using INEC3.DbConn;
using INEC3.Helper;
using INEC3.Models.Service;
using INEC3.IdentityClass;

using System.Web.Mvc;
using System.Security.Claims;

namespace INEC3.Controllers
{
    [System.Web.Http.RoutePrefix("api/Results")]
    public class ResultsApiController : ApiController
    {
        private inecDBContext db;
        private Sqldbconn _db;
        private _Helper _Helper;
        private AuthContext _context;
        private ResultsService resultsService;

        public ResultsApiController()
        {
            _context = new AuthContext();
            db = new inecDBContext();
            _db = new Sqldbconn();
            _Helper = new _Helper();
            resultsService = new ResultsService();

        }
        [System.Web.Http.Route("GetCandidateList")]
        [System.Web.Http.HttpGet]
        public JsonResult GetCandidateList()
        {
            JsonResult res = new JsonResult();
            try
            {
                res.Data = resultsService.getcandidate();
            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }

        [System.Web.Http.Route("GetParty")]
        [System.Web.Http.HttpGet]
        //[System.Web.Http.Authorize]
        public JsonResult GetParty(int candidateid)
        {
            JsonResult res = new JsonResult();
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    string UserName = claimsIdentity?.FindFirst(c => c.Type == "UserName")?.Value;
                }
                var resp = resultsService.GetParty(candidateid);
                if (resp != null)
                    res.Data = resp;
                else
                {
                    res.ContentType = "fail";
                    res.Data = "No record found";
                }
            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }
        [System.Web.Http.Route("GetProvince")]
        [System.Web.Http.HttpGet]
        //[System.Web.Http.Authorize]
        public JsonResult GetProvince()
        {
            JsonResult res = new JsonResult();
            try
            {
                res.Data = resultsService.getProvince();
            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }

        /////////////////////////////
        
        [System.Web.Http.Route("PolStationCahngeGet")]
        [System.Web.Http.HttpGet]
        //[System.Web.Http.Authorize]
        public JsonResult PolStationCahngeGet(int polingstationid)
        //public IHttpActionResult GetVoters(int polingstationid)
        {
            JsonResult res = new JsonResult();
            try
            {
                res.Data = JsonConvert.DeserializeObject(resultsService.PolStationCahngeGet(polingstationid));
            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }

        [System.Web.Http.Route("SaveListRecord")]
        [System.Web.Http.HttpPost]
        //[System.Web.Http.Authorize]
        public IHttpActionResult SaveListRecord(tbl_Results obj)
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
                _Helper.SendNotification();

                //var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
                //hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));

                SqlNotification objRepo = new SqlNotification();
                objRepo.GetAllMessages();

                return Json(res);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }); }
        }

        [System.Web.Http.Route("RemoveResult")]
        [System.Web.Http.HttpGet]
        //[System.Web.Http.Authorize]
        public IHttpActionResult RemoveResult(int ResultId, int ID_Bureauvote)
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
                _Helper.SendNotification();

                //_Helper.ActiveSqlNotification();
                //var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
                //hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));

                SqlNotification objRepo = new SqlNotification();
                objRepo.GetAllMessages();

                return Json(res);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }); }
        }

        [System.Web.Http.Route("GetTerritoireList")]
        [System.Web.Http.HttpGet]
        //[System.Web.Http.Authorize]
        public IHttpActionResult GetTerritoireList(int ProvinceId)
        {
            try
            {
                var Ter = db.Territoires.Where(w => w.ID_Province == ProvinceId).Select(s => new { s.ID_Territoire, s.Nom }).ToList();
                return Json(Ter);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }); }
        }

        //Use UserPoolingStationGet Instred This
        [System.Web.Http.Route("GetPoolingStationList")]
        [System.Web.Http.HttpGet]
        //[System.Web.Http.Authorize]
        public IHttpActionResult GetPoolingStationList(int CommuneId)
        {
            try
            {
                var res = db.BureauVotes.Where(w => w.ID_Commune == CommuneId).Select(s => new { s.ID_Bureauvote, s.Nom }).ToList();
                return Json(res);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }); }
        }

        [System.Web.Http.Route("UserPolingStationGet")]
        [System.Web.Http.HttpGet]
        //[System.Web.Http.Authorize]
        public JsonResult UserPolingStationGet()
        {
            JsonResult res = new JsonResult();
            try
            {
                res.Data = resultsService.UserPolingStationGet();
            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }

        [System.Web.Http.Route("GetCommune")]
        [System.Web.Http.HttpGet]
        //[System.Web.Http.Authorize]
        public IHttpActionResult GetCommune(int ProvinceId, int TerritoireId)
        {
            try
            {
                var res = db.Communes.Where(w => w.ID_Province == ProvinceId && w.ID_Territoire == TerritoireId).Select(s => new { s.ID_Commune, s.Nom }).ToList();
                return Json(res);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }); }
        }

    }
}
