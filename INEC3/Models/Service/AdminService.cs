using INEC3.DbConn;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace INEC3.Models.Service
{
    
    public class AdminService
    {
        private inecDBContext db = new inecDBContext();
        private Sqldbconn _sqldb = new Sqldbconn();
        string constring = ConfigurationManager.ConnectionStrings["inecConn"].ToString();
        public List<UserDisplay> GetUserList()
        {
            List<UserDisplay> user = new List<UserDisplay>();
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("Select * from vw_UserProfile", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        UserDisplay u = new UserDisplay();
                        u.UserId = Convert.ToString(rdr["Id"]);
                        u.Email = Convert.ToString(rdr["Email"]);
                        u.Name = Convert.ToString(rdr["FirstName"]);
                        u.Role = Convert.ToString(rdr["RoleName"]);
                        u.EmailConfirmed = Convert.ToBoolean(rdr["EmailConfirmed"]);
                        u.Isactive = Convert.ToBoolean(rdr["Isactive"]);
                        user.Add(u);
                    }
                    con.Close();
                    
                }

            }
            return user;

        }
    }
}