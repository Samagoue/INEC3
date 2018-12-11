using INEC3.DbConn;
using INEC3.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using INEC3.Repository;
using Microsoft.AspNet.Identity.EntityFramework;

namespace INEC3.Models.Service
{
    public class ResultsService
    {
        private inecDBContext db;
        private AuthRepository _auth;
        private Sqldbconn _db;
        private _Helper _Helper;
        string constring = ConfigurationManager.ConnectionStrings["inecConn"].ToString();
        public SqlCommandText _smodel = new SqlCommandText();
        public ResultsService()
        {
            //_context = new AuthContext();
            db = new inecDBContext();
            _db = new Sqldbconn();
            _Helper = new _Helper();
            _auth = new AuthRepository();
        }

        public dynamic getcandidate()
        {
            return db.Candidats.Select(s => new { Id = s.ID_Candidat, Value = s.Nom }).ToList();
        }
        public dynamic GetParty(int candidateid)
        {
            return db.Candidats.Where(w => w.ID_Candidat == candidateid).Select(s => new { s.ID_Party, s.Party.Color, s.Party.Sigle }).FirstOrDefault();

        }
        public dynamic GetPartyList(int candidateid)
        {
            return db.Candidats.Where(w => w.ID_Candidat == candidateid).Select(s => new { s.ID_Party, s.Party.Color, s.Party.Sigle }).ToList();

        }
        public dynamic getProvince()
        {
            return db.Provinces.Select(s => new { Id = s.ID_Province, Value = s.Nom }).ToList();
        }
        public string PolStationCahngeGet(int polingstationid)
        {
            Responce resp = new Responce();

            var voters = db.BureauVotes.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.Commune.Enroles }).FirstOrDefault();//Total Voters
            if (voters != null)
            {
                resp.Total_Voters = voters.Enroles;
            }
            var exprims = db.Results.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.Abstentions, s.Exprimes, s.Nuls, s.Total_Votes }).FirstOrDefault();
            if (exprims != null)
            {
                resp.Abstentions = exprims.Abstentions;
                resp.Exprimes = exprims.Exprimes;
                resp.Nuls = exprims.Nuls;
                resp.Total_Votes = exprims.Total_Votes;

            }

            List<Resultt> res1 = db.Results.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new Resultt { ID_Result = s.ID_Result, ID_Candidat = s.ID_Candidat, Candidate = s.Candidat.Nom, ID_Party = s.ID_Party, Party = s.Party.Sigle, Pourcentage = s.Pourcentage, Votes = s.Voix }).ToList();
            if (res1 != null)
            {
                resp.ResultList = res1;
            }
            return JsonConvert.SerializeObject(resp);


        }

        public dynamic GetDashBoardTiles()
        {
            DataSet dt = new DataSet();
            dt = _db.GetDatatable("proc_DashbardTiles", "");
            return dt;
        }
        public dynamic UserPolingStationGet()
        {
            //return db.BureauVotes.Where(w => w.ID_Commune == CommuneId).Select(s => new { s.ID_Bureauvote, s.Nom }).ToList();
            return db.BureauVotes.Select(s => new { Id = s.ID_Bureauvote, Value = s.Nom }).ToList();

        }
        public List<RoleModel> GetRoleList()
        {
            List<RoleModel> role = new List<RoleModel>();
            using (SqlConnection con = new SqlConnection(constring))
            {
                //using (SqlCommand cmd = new SqlCommand("Select * from vw_UserProfile", con))
                using (SqlCommand cmd = new SqlCommand(_smodel.SelectRoleList, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            RoleModel u = new RoleModel();
                            u.Id = Convert.ToString(rdr["Id"]);
                            u.Role = Convert.ToString(rdr["Name"]);
                            role.Add(u);
                        }
                    }
                    con.Close();

                }

            }
            return role;
        }
        public UserDropDown GetUserDDL(string UserId)
        {
            UserDropDown ddl = new UserDropDown();
            var userpol = db.UserPolStations.Where(w => w.UserID == UserId).FirstOrDefault();
            if (userpol != null)
            {
                if (userpol.AssignRole == UserManageRoles.ProvinceUser)
                {
                    ddl.Province = db.Provinces.Where(w => w.ID_Province == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Province, Value = s.Nom }).ToList();
                    ddl.Territoire = db.Territoires.Where(w => w.ID_Province == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Territoire, Value = s.Nom }).ToList();
                }
                else if (userpol.AssignRole == UserManageRoles.TerritoireUser)
                {
                    ddl.Territoire = db.Territoires.Where(w => w.ID_Territoire == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Territoire, Value = s.Nom }).ToList();
                    ddl.Commune = db.Communes.Where(w => w.ID_Territoire == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Commune, Value = s.Nom }).ToList();
                }
                else if (userpol.AssignRole == UserManageRoles.CommuneUser)
                {
                    ddl.Commune = db.Communes.Where(w => w.ID_Commune == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Commune, Value = s.Nom }).ToList();
                    ddl.PolStation = db.BureauVotes.Where(w => w.ID_Commune == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Bureauvote, Value = s.Nom }).ToList();
                }
                else if (userpol.AssignRole == UserManageRoles.PollingUser)
                {
                    ddl.PolStation = db.BureauVotes.Where(w => w.ID_Bureauvote == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Bureauvote, Value = s.Nom }).ToList();
                }

            }
            return ddl;
        }
        public dynamic UserIndexList()
        {
            var results = (from res in db.Results
                           join ce in db.Communes on res.BureauVote.ID_Commune equals ce.ID_Commune
                           join te in db.Territoires on ce.ID_Territoire equals te.ID_Territoire
                           join pr in db.Provinces on te.ID_Province equals pr.ID_Province
                           join ca in db.Candidats on res.ID_Candidat equals ca.ID_Candidat
                           join pa in db.Parties on res.ID_Party equals pa.ID_Party
                           select new { Party=pa.Nom,Candidats=ca.Nom, Provinces = pr.Nom, Territoires=te.Nom,res.Voix,res.Votants,res.Nuls,res.Exprimes,res.Total_Votes });
            return results;
        }
    }
    public class Responce
    {
        public List<Resultt> ResultList { get; set; }

        public int Abstentions { get; set; }
        public int Exprimes { get; set; }
        public int Nuls { get; set; }
        public int Total_Votes { get; set; }
        public int Total_Voters { get; set; }
        public int ID_Province { get; set; }
        public int ID_Territoire { get; set; }

        //public int ID_Party { get; set; }
        //public int ID_Result { get; set; }
        //public int ID_Candidat { get; set; }
        //public int ID_Bureauvote { get; set; }
        //public int Voix { get; set; }
        //public double? Pourcentage { get; set; }
        //public int Votants { get; set; }
    }
    public class Resultt
    {
        public int ID_Result { get; set; }
        public int ID_Candidat { get; set; }
        public string Candidate { get; set; }//Nom
        public int ID_Party { get; set; }
        public string Party { get; set; }//Sigle
        public double? Pourcentage { get; set; }//
        public int Votes { get; set; }//Voix
    }
    public class UserDropDown
    {
        public List<DropDown> Province { get; set; }
        public List<DropDown> Territoire { get; set; }
        public List<DropDown> Commune { get; set; }
        public List<DropDown> PolStation { get; set; }
    }
    public class DropDown
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}