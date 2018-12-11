using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INEC3.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        //public ActionResult Index(string ermsg="")
        public ActionResult Index(string id="")
        {
            ViewBag.error = id;
            //ViewBag.error = id;
            return View();
        }
    }
}