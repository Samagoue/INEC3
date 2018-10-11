using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace INEC3.DbConn
{
    public class Sqldbconn
    {

        public SqlConnection con;
        string constring;
        public void connection()
        {
            constring = ConfigurationManager.ConnectionStrings["inecConn"].ToString();
            con = new SqlConnection(constring);
        }

        public DataSet GetDatatable(string spname, string parameter)
        {
            connection();

            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = spname + " " + parameter;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            adp.Fill(ds);
            //adp.Fill(dt);
            return ds;
        }
    }
}