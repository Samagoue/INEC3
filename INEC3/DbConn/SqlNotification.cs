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
        // singleton instance

        //private Sqldbconn _db = new Sqldbconn();
        ////public IEnumerable<Result> GetData()
        ////{

        ////    using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString))
        ////    {
        ////        connection.Open();
        ////        using (SqlCommand command = new SqlCommand(@"Select [ID_Result],[ID_Candidat],[ID_Party],[ID_Bureauvote],[Voix],[Votants],[Abstentions],[Nuls],[Exprimes],[Total_Votes] from [dbo].[tbl_Results]", connection))
        ////        {
        ////            // Make sure the command object does not already have
        ////            // a notification object associated with it.
        ////            command.Notification = null;

        ////            SqlDependency dependency = new SqlDependency(command);
        ////            dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

        ////            if (connection.State == ConnectionState.Closed)
        ////                connection.Open();

        ////            using (var reader = command.ExecuteReader())
        ////                return reader.Cast<IDataRecord>()
        ////                    .Select(x => new Result()
        ////                    {
        ////                        ID_Result = x.GetInt32(0),
        ////                        ID_Candidat = x.GetInt32(1),
        ////                        ID_Party = x.GetInt32(2),
        ////                        ID_Bureauvote = x.GetInt32(3),
        ////                        Voix = x.GetInt32(4),
        ////                        Votants = x.GetInt32(5),
        ////                        Abstentions = x.GetInt32(6),
        ////                        Nuls = x.GetInt32(7),
        ////                        Exprimes = x.GetInt32(8),
        ////                        Total_Votes = x.GetInt32(9),
        ////                    }).ToList();



        ////        }
        ////    }
        ////}

        //private static string connectionString = ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString;
        ////private static string _con = ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString;
        //public SqlNotification()
        //{
        //    // The mapper object is used to map model properties that do not have a corresponding table column name.
        //    // In case all properties of your model have same name of table columns, you can avoid to use the mapper.
        //    var mapper = new ModelToTableMapper<Result>();
        //    mapper.AddMapping(c => c.ID_Result, "ID_Result");
        //    mapper.AddMapping(c => c.ID_Candidat, "ID_Candidat");
        //    mapper.AddMapping(c => c.ID_Party, "ID_Party");
        //    mapper.AddMapping(c => c.ID_Bureauvote, "ID_Bureauvote");
        //    mapper.AddMapping(c => c.Voix, "Voix");
        //    mapper.AddMapping(c => c.Votants, "Votants");
        //    mapper.AddMapping(c => c.Abstentions, "Abstentions");
        //    mapper.AddMapping(c => c.Nuls, "Nuls");
        //    mapper.AddMapping(c => c.Exprimes, "Exprimes");
        //    mapper.AddMapping(c => c.Total_Votes, "Total_Votes");


        //    using (var dep = new SqlTableDependency<Result>(connectionString, tableName: "tbl_Results", mapper: mapper))
        //    {
        //        dep.OnChanged += this.dependency_OnChange;
        //        dep.OnError += SqlTableDependency_OnError;
        //        dep.Start();

        //    }
        //}

        //public void Notification()
        //{
        //    using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(@"Select [ID_Result],[ID_Candidat],[ID_Party],[ID_Bureauvote],[Voix],[Votants],[Abstentions],[Nuls],[Exprimes],[Total_Votes] from [dbo].[tbl_Results]", connection))
        //        {
        //            using (var reader = command.ExecuteReader())
        //                 reader.Cast<IDataRecord>()
        //                    .Select(x => new Result()
        //                    {
        //                        ID_Result = x.GetInt32(0),
        //                        ID_Candidat = x.GetInt32(1),
        //                        ID_Party = x.GetInt32(2),
        //                        ID_Bureauvote = x.GetInt32(3),
        //                        Voix = x.GetInt32(4),
        //                        Votants = x.GetInt32(5),
        //                        Abstentions = x.GetInt32(6),
        //                        Nuls = x.GetInt32(7),
        //                        Exprimes = x.GetInt32(8),
        //                        Total_Votes = x.GetInt32(9),
        //                    }).ToList();



        //        }
        //    }
        //}
        //public  void dependency_OnChange(object sender, RecordChangedEventArgs<Result> e)
        //{
        //    var changedEntity = e.Entity;
        //    //JobHub.Show();
        //    DataSet dt = new DataSet();
        //    dt = _db.GetDatatable("proc_GetProvinceResult", "");
        //    var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
        //    hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));
        //}
        //void SqlTableDependency_OnError(object sender, ErrorEventArgs e)
        //{
        //    throw e.Error;
        //}
        //public class Result
        //{
        //    public int ID_Result { get; set; }
        //    public int ID_Candidat { get; set; }
        //    public int ID_Party { get; set; }
        //    public int ID_Bureauvote { get; set; }
        //    public int Voix { get; set; }
        //    //public int Pourcentage { get; set; }
        //    public int Votants { get; set; }
        //    public int Abstentions { get; set; }
        //    public int Nuls { get; set; }
        //    public int Exprimes { get; set; }
        //    public int Total_Votes { get; set; }

        //}
        private Sqldbconn _db = new Sqldbconn();
        readonly string _connString = ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString;

        public IEnumerable<messages> GetAllMessages()
        {
            var messages = new List<messages>();
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"select [ID_Result],[ID_Candidat],[ID_Party],[ID_Bureauvote],[Voix] from [dbo].[tbl_Results]", connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                }

            }
            return messages;


        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                if (e.Info == SqlNotificationInfo.Delete)
                {
                    DataSet dt = new DataSet();
                    dt = _db.GetDatatable("proc_GetProvinceResult", "");
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
                    hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));
                }
            }
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