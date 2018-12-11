using System;
using INEC3.DbConn;
using INEC3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data;
using INEC3.Helper;
using INEC3.Models.Service;
using INEC3.Repository;

namespace INEC3.Controllers
{
    public class AdminController : Controller
    {
        private inecDBContext db = new inecDBContext();
        private Sqldbconn _db = new Sqldbconn();
        private AccountService accountService = new AccountService();
        private ResultsService resultsService = new ResultsService();
        private AuthRepository authRepository = new AuthRepository();
        private Base _base = new Base();
        //============User Area
        [AuthenticatUser]
        public ActionResult Result(int? id)
        {
            string username = "";
            username = _base.GetCookie("inceusername");
            UserDisplay user = accountService.FindUserDisplay("UserName", username);
            ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom");
            ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle");
            if (user == null)
            {
                return View();
            }
            else
            {
                if (user.Role == UserManageRoles.SuperAdmin)
                {
                    return RedirectToAction("AdminResult");
                }
                UserDropDown ddl = resultsService.GetUserDDL(user.UserId);
                if (ddl.Province != null)
                    ViewBag.Province = new SelectList(ddl.Province, "Id", "Value", 0);
                if (ddl.Territoire != null)
                    ViewBag.Territoire = new SelectList(ddl.Territoire, "Id", "Value", 0);
                if (ddl.Commune != null)
                    ViewBag.Commune = new SelectList(ddl.Commune, "Id", "Value", 0);
                if (ddl.PolStation != null)
                    ViewBag.ID_Bureauvote = new SelectList(ddl.PolStation, "Id", "Value");
                return View(user);
            }
        }
        //User Area End 

        [AuthenticatUser]
        public ActionResult Index()
        {
            var results = db.Results.Include(t => t.BureauVote).Include(t => t.Candidat).Include(t => t.Party);
            return View(results.ToList());
        }

        [AuthenticatUser]
        public ActionResult AdminIndex()
        {

            //var results = db.Results.Include(t => t.BureauVote).Include(t => t.Candidat).Include(t => t.Party);
            return View(resultsService.ResultViewList());
        }
        [AuthenticatUser]
        public ActionResult AdminResult(int? id)
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



        [HttpPost]
        public JsonResult UserPolStation(UserPolStation obj)
        {
            JsonResult res = new JsonResult();
            try
            {
                var role = authRepository.ChangeUserRole(obj.UserID, obj.AssignRole);
                if (role.Errors.Count() == 0)
                {
                    UserPolStation isexist = db.UserPolStations.Where(w => w.UserID == obj.UserID).FirstOrDefault();
                    if (isexist == null)
                    {
                        db.UserPolStations.Add(obj);
                        db.SaveChanges();
                        res.Data = ("User role add succesfully");
                    }
                    else
                    {
                        isexist.AssignID = obj.AssignID;
                        isexist.AssignRole = obj.AssignRole;
                        db.SaveChanges();
                        res.Data = ("User role updated succesfully");
                    }
                    res.ContentType = "success";
                }
                else
                {
                    res.ContentType = "fail";
                    res.Data = (role.Errors.ToString());
                }
            }
            catch (Exception ex)
            {

                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }

    }
}