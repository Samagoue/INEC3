using INEC3.DbConn;
using INEC3.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace INEC3.Models.Service
{
    public class ResultsService
    {
        private inecDBContext db;
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
        }

        public dynamic getcandidate()
        {
            return db.Candidats.Select(s => new { Id = s.ID_Candidat, Value = s.Nom }).ToList();
        }
        public dynamic GetParty(int candidateid)
        {
            return db.Candidats.Where(w => w.ID_Candidat == candidateid).Select(s => new { s.ID_Party, s.Party.Color,s.Party.Sigle }).FirstOrDefault();

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

}