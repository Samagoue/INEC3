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
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
namespace INEC3.Controllers
{
    [System.Web.Http.RoutePrefix("api/Results")]
    [System.Web.Http.Authorize]
    public class ResultsApiController : ApiController
    {
        private inecDBContext db;
        private Sqldbconn _db;
        private _Helper _Helper;
        private ApplicationDbContext _context;
        private ResultsService resultsService;

        public ResultsApiController()
        {
            _context = new ApplicationDbContext();
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
                var name = User.Identity.Name;
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
        public JsonResult GetParty(int candidateid)
        {
            JsonResult res = new JsonResult();
            try
            {
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

        [System.Web.Http.Route("GetPartyList")]
        [System.Web.Http.HttpGet]
        public JsonResult GetPartyList(int candidateid)
        {
            JsonResult res = new JsonResult();
            try
            {
                var resp = resultsService.GetPartyList(candidateid);
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
        public JsonResult PolStationCahngeGet(int polingstationid)
        {
            JsonResult res = new JsonResult();
            try
            {
                res.Data = resultsService.PolStationCahngeGet(polingstationid);
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
        public JsonResult SaveListRecord(tbl_Results obj)
        {
            JsonResult res = new JsonResult();
            try
            {
                string UserId = "";
                //UserId = User.Identity.Name;
                UserId= System.Web.HttpContext.Current.User.Identity.GetUserId();

                if (obj.ID_Bureauvote == 0)
                {
                    res.ContentType = "fail";
                    res.Data = "ID_Bureauvote require";
                    return res;
                }

                if (string.IsNullOrEmpty(UserId))
                {
                    res.ContentType = "fail";
                    res.Data = "Invalid request";
                    return res;
                }
                else
                {
                    obj.UserId = UserId;
                    res.Data = (resultsService.SaveListRecord(obj));
                }

            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }

        [System.Web.Http.Route("RemoveResult")]
        [System.Web.Http.HttpGet]
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
                var res = db.Results.Where(w => w.ID_Bureauvote == ID_Bureauvote).Select(s => new { s.ID_Result, s.ID_Candidat, Candidate= s.Candidat.Nom, s.ID_Party, Party = s.Party.Sigle, s.Pourcentage, Votes= s.Voix, s.Exprimes, s.Nuls, s.Abstentions, s.Total_Votes,s.ID_Bureauvote }).ToList();
                _Helper.SendNotification();

                //_Helper.ActiveSqlNotification();

                SqlNotification objRepo = new SqlNotification();
                objRepo.GetAllMessages();

                return Json(res);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }); }
        }

        [System.Web.Http.Route("GetTerritoireList")]
        [System.Web.Http.HttpGet]
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
        public IHttpActionResult GetCommune(int ProvinceId, int TerritoireId)
        {
            try
            {
                var res = db.Communes.Where(w => w.ID_Province == ProvinceId && w.ID_Territoire == TerritoireId).Select(s => new { s.ID_Commune, s.Nom }).ToList();
                return Json(res);
            }
            catch (Exception ex) { return Json(new { Result = false, ErrorMessage = ex.Message }); }
        }

        [System.Web.Http.Route("GetAllRoll")]
        [System.Web.Http.HttpGet]
        public JsonResult GetAllRoll()
        {
            JsonResult res = new JsonResult();
            try
            {
                res.Data = (resultsService.GetRoleList());
            }
            catch (Exception ex)
            {

                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }


        [System.Web.Http.Route("GetUserDDL")]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize]
        public JsonResult GetUserDDL()
        {
            JsonResult res = new JsonResult();
            try

            {
                string UserId = "";
                UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                res.Data = (resultsService.GetUserDDL(UserId));
            }
            catch (Exception ex)
            {

                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }


        [System.Web.Http.Route("GetDashBoardTiles")]
        [System.Web.Http.HttpGet]
        public JsonResult GetDashBoardTiles()
        {
            JsonResult res = new JsonResult();
            try
            {
                res.Data = (resultsService.GetDashBoardTiles());
            }
            catch (Exception ex)
            {

                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }
        [System.Web.Http.Route("UserIndexList")]
        [System.Web.Http.HttpGet]
        public JsonResult UserIndexList()
        {
            JsonResult res = new JsonResult();
            try
            {
                string UserId = "";
                UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                res.Data = (resultsService.UserIndexList(UserId));
            }
            catch (Exception ex)
            {

                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
               // _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
