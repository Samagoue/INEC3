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
    [Authorize]
    public class AdminController : Controller
    {
        private inecDBContext db = new inecDBContext();
        private Sqldbconn _db = new Sqldbconn();
        private AccountService accountService = new AccountService();
        private ResultsService resultsService = new ResultsService();
        private AuthRepository authRepository = new AuthRepository();
        private Base _base = new Base();
        //============User Area
        public ActionResult Result(int? id)
        {
            UserDisplay user = new UserDisplay();
            try
            {
                string username = "";
                username = User.Identity.Name;
                if (string.IsNullOrEmpty(username))
                    return RedirectToAction("Index", "Home");
                user = accountService.FindUserDisplay("Id", username);
                if (user.Role == UserManageRoles.User)
                {
                    return RedirectToAction("Index", "error", new { id = "401" });
                }
                ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom");
                ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle");
                if (user == null)
                    return View();
                else
                {
                    if (user.Role == UserManageRoles.SuperAdmin)
                        return RedirectToAction("AdminResult");
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return View(user);
            }
        }
        //User Area End 

        public ActionResult Index()
        {
            List<ResultViewModel> model = new List<ResultViewModel>();
            try
            {
                var userid = User.Identity.Name;
                if (string.IsNullOrEmpty(userid))
                    return View(model);
                UserDisplay display = accountService.FindUserDisplay("Id", userid);
                if (display.Role == UserManageRoles.User)
                {
                    return RedirectToAction("Index", "error", new { id = "401" });
                }
                //var results = db.Results.Include(t => t.BureauVote).Include(t => t.Candidat).Include(t => t.Party).Where(w => w.UserId == userid);
                return View(resultsService.ResultViewListBykey("UserId", userid));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return View(model);
            }
        }

        //[AuthFilter(Roles = "SuperAdmin,User")]
        public ActionResult AdminIndex()
        {
            List<ResultViewModel> model = new List<ResultViewModel>();
            try
            {
                UserDisplay display = accountService.FindUserDisplay("Id", User.Identity.Name);
                if (display.Role != UserManageRoles.SuperAdmin)
                {
                    return RedirectToAction("Index", "error", new { id = "401" });
                }

                return View(resultsService.ResultViewList());
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return View(model);
            }
        }
        public ActionResult AdminResult(int? id)
        {
            tbl_Results tbl_Results = new tbl_Results();
            try
            {
                UserDisplay display = accountService.FindUserDisplay("Id", User.Identity.Name);
                if (display.Role != UserManageRoles.SuperAdmin)
                {
                    return RedirectToAction("Index", "error", new { id = "401" });
                }
                ViewBag.Province = new SelectList(db.Provinces.Select(s => new { ID_Province = s.ID_Province, Nom = s.Nom }).ToList(), "ID_Province", "Nom", 0);
                ViewBag.ID_Bureauvote = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom");
                ViewBag.ID_Candidat = new SelectList(db.Candidats, "ID_Candidat", "Nom");
                ViewBag.ID_Party = new SelectList(db.Parties, "ID_Party", "Sigle");

                tbl_Results = db.Results.Find(id);
                return View(tbl_Results);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return View(tbl_Results);
            }

        }

        public ActionResult Users()
        {
            List<UserDisplay> model = new List<UserDisplay>();
            try
            {
                UserDisplay display = accountService.FindUserDisplay("Id", User.Identity.Name);
                if (display.Role != UserManageRoles.SuperAdmin)
                {
                    return RedirectToAction("Index", "error", new { id = "401" });
                }
                return View(accountService.GetUserList());
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return View(model);
            }

        }
        [HttpPost]
        public ActionResult ManageUser(string userid)
        {
            UserDisplay model = new UserDisplay();
            try
            {
                UserDisplay display = accountService.FindUserDisplay("Id", User.Identity.Name);
                if (display.Role != UserManageRoles.SuperAdmin)
                {
                    return RedirectToAction("Index", "error", new { id = "401" });
                }
                ViewBag.Roles = new SelectList(resultsService.GetRoleList(), "Role", "Role");

                UserPolStation userpol = db.UserPolStations.Where(w => w.UserID == userid).FirstOrDefault();
                if (userpol != null && userpol.AssignRole == UserManageRoles.ProvinceUser)
                {
                    ViewBag.Province = new SelectList(db.Provinces.Select(s => new { s.ID_Province, s.Nom }).ToList(), "ID_Province", "Nom", userpol.AssignID);
                    ViewBag.Territoire = new SelectList(db.Territoires.Select(s => new { s.ID_Territoire, s.Nom }).ToList(), "ID_Territoire", "Nom", 0);
                    ViewBag.Commune = new SelectList(db.Communes.Select(s => new { s.ID_Commune, s.Nom }).ToList(), "ID_Commune", "Nom", 0);
                    ViewBag.Polingstation = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom", 0);
                }
                else if (userpol != null && userpol.AssignRole == UserManageRoles.TerritoireUser)
                {
                    ViewBag.Province = new SelectList(db.Provinces.Select(s => new { s.ID_Province, s.Nom }).ToList(), "ID_Province", "Nom", db.Territoires.Where(w => w.ID_Territoire == userpol.AssignID).Select(s => s.ID_Province).FirstOrDefault());
                    ViewBag.Territoire = new SelectList(db.Territoires.Select(s => new { s.ID_Territoire, s.Nom }).ToList(), "ID_Territoire", "Nom", userpol.AssignID);
                    ViewBag.Commune = new SelectList(db.Communes.Select(s => new { s.ID_Commune, s.Nom }).ToList(), "ID_Commune", "Nom", 0);
                    ViewBag.Polingstation = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom", 0);
                }
                else if (userpol != null && userpol.AssignRole == UserManageRoles.CommuneUser)
                {
                    var selecter = db.Communes.Where(w => w.ID_Commune == userpol.AssignID).Select(s => new { s.ID_Territoire, s.ID_Province }).FirstOrDefault();
                    ViewBag.Province = new SelectList(db.Provinces.Select(s => new { s.ID_Province, s.Nom }).ToList(), "ID_Province", "Nom", selecter.ID_Province);
                    ViewBag.Territoire = new SelectList(db.Territoires.Select(s => new { s.ID_Territoire, s.Nom }).ToList(), "ID_Territoire", "Nom", selecter.ID_Territoire);
                    ViewBag.Commune = new SelectList(db.Communes.Select(s => new { s.ID_Commune, s.Nom }).ToList(), "ID_Commune", "Nom", userpol.AssignID);
                    ViewBag.Polingstation = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom", 0);
                }
                else if (userpol != null && userpol.AssignRole == UserManageRoles.PollingUser)
                {
                    var selecter = (from co in db.Communes join po in db.BureauVotes on co.ID_Commune equals po.ID_Commune where po.ID_Bureauvote == userpol.AssignID select new {co.ID_Province,co.ID_Territoire,co.ID_Commune }).FirstOrDefault();
                    ViewBag.Province = new SelectList(db.Provinces.Select(s => new { s.ID_Province, s.Nom }).ToList(), "ID_Province", "Nom", selecter.ID_Province);
                    ViewBag.Territoire = new SelectList(db.Territoires.Select(s => new { s.ID_Territoire, s.Nom }).ToList(), "ID_Territoire", "Nom", selecter.ID_Territoire);
                    ViewBag.Commune = new SelectList(db.Communes.Select(s => new { s.ID_Commune, s.Nom }).ToList(), "ID_Commune", "Nom", selecter.ID_Commune);
                    ViewBag.Polingstation = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom", userpol.AssignID);
                }
                else
                {
                    ViewBag.Province = new SelectList(db.Provinces.Select(s => new { s.ID_Province, s.Nom }).ToList(), "ID_Province", "Nom", 0);
                    ViewBag.Territoire = new SelectList(db.Territoires.Select(s => new { s.ID_Territoire, s.Nom }).ToList(), "ID_Territoire", "Nom", 0);
                    ViewBag.Commune = new SelectList(db.Communes.Select(s => new { s.ID_Commune, s.Nom }).ToList(), "ID_Commune", "Nom", 0);
                    ViewBag.Polingstation = new SelectList(db.BureauVotes, "ID_Bureauvote", "Nom", 0);
                }

                model = accountService.FindUserDisplay("Id", userid);
                if (model == null)
                {
                    return View();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return View(model);
            }
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