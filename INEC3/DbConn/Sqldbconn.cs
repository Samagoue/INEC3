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
            con.Close();
            return ds;
        }

        public SqlDataReader SqlDataReader(string spname, string parameter)
        {
            connection();
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = spname + " " + parameter;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    con.Close();
                    return rdr;
                }

            }
        }


        public void Get()
        {
            using (SqlConnection con = new SqlConnection(""))
            {
                SqlCommand cmd = new SqlCommand("");
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    //MasterTradesModel employee = new MasterTradesModel();
                    //employee.MasterID = Convert.ToInt32(rdr["MasterID"]);
                    //employee.Exchange = rdr["Exchange"].ToString();
                    //employee.TradeType = rdr["TradeType"].ToString();
                    //lstemployee.Add(employee);
                }
                con.Close();
            }
        }
    }
}