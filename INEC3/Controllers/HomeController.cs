
using System.Web.Mvc;
using INEC3.Models;
using INEC3.DbConn;
using System.Data;
using Newtonsoft.Json;
namespace INEC3.Controllers
{
    [System.Web.Http.Authorize]
    public class HomeController : Controller
    {
        private inecDBContext db = new inecDBContext();
        private Sqldbconn _db = new Sqldbconn();
        public ActionResult Index()
        {
            DataSet dt = new DataSet();
            dt = _db.GetDatatable("proc_GetProvinceResult", "");
            ViewBag.Province = JsonConvert.SerializeObject(dt);
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