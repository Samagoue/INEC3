using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.EventArgs;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace INEC3.DbConn
{
    public class SqlNotification
    {
        private Sqldbconn _db = new Sqldbconn();
        readonly string _connString = ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString;

        public IEnumerable<messages> GetAllMessages()
        {
            var messages = new List<messages>();
            using (var connection = new SqlConnection(_connString))
            {
                //connection.Open();
                //using (var command = new SqlCommand(@"select [ID_Result],[ID_Candidat],[ID_Party],[ID_Bureauvote],[Voix] from [dbo].[tbl_Results]", connection))
                //{
                //    command.Notification = null;

                //    var dependency = new SqlDependency(command);
                //    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                //    if (connection.State == ConnectionState.Closed)
                //        connection.Open();
                //    var reader = command.ExecuteReader();

                //}

            }
            return messages;


        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //if (e.Type == SqlNotificationType.Change)
            //{
            //    if (e.Info == SqlNotificationInfo.Delete)
            //    {
            //        DataSet dt = new DataSet();
            //        dt = _db.GetDatatable("proc_GetProvinceResult", "");
            //        var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
            //        hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));
            //        GetAllMessages();
            //    }
            //}
        }
    }

    public class messages
    {
        public int ID_Result { get; set; }
        public int ID_Candidat { get; set; }
        public int ID_Party { get; set; }
        public int ID_Bureauvote { get; set; }
        public int Voix { get; set; }


    }
}