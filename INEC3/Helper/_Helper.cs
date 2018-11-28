using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Data;
using Newtonsoft.Json;
using INEC3.DbConn;
namespace INEC3.Helper
{
    public class _Helper
    {
        private Sqldbconn _db = new Sqldbconn();
        public bool SendNotification()
        {
            try
            {
                DataSet dt = new DataSet();
                dt = _db.GetDatatable("proc_GetProvinceResult", "");
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
                hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ActiveSqlNotification()
        {
            try
            {
                SqlNotification objRepo = new SqlNotification();
                objRepo.GetAllMessages();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}