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
using System.IO;

namespace INEC3.Controllers
{
    public class HomeController : Controller
    {
        private inecDBContext db = new inecDBContext();
        private Sqldbconn _db = new Sqldbconn();
        public ActionResult Index()
        {
            
            DataSet dt = new DataSet();
            dt = _db.GetDatatable("proc_GetProvinceResult", "");
            ViewBag.Province = JsonConvert.SerializeObject(dt);
            
            //using (StreamReader r = new StreamReader("/Resources/COD_TOPO.json"))
            //{
            //    string json = r.ReadToEnd();
            //    ViewBag.CODJson = json;
            //}

            SqlNotification objRepo = new SqlNotification();
            var res = objRepo.GetAllMessages();

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


       
    }
}