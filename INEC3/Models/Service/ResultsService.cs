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
        public AccountService accountService;
        public ResultsService()
        {
            //_context = new AuthContext();
            db = new inecDBContext();
            _db = new Sqldbconn();
            _Helper = new _Helper();
            _auth = new AuthRepository();
            accountService = new AccountService();
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
        public dynamic PolStationCahngeGet(int polingstationid)
        {
            Responce resp = new Responce();

            var voters = db.BureauVotes.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.Commune.Enroles, s.Code_SV }).FirstOrDefault();//Total Voters
            if (voters != null)
            {
                resp.Total_Voters = voters.Enroles;
                resp.Code_SV = voters.Code_SV;
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
            //return JsonConvert.SerializeObject(resp);
            return resp;


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
                if (userpol.AssignRole == UserManageRoles.SuperAdmin)
                {
                    ddl.Province = db.Provinces.Where(w => w.ID_Province == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Province, Value = s.Nom }).ToList();
                    ddl.Territoire = db.Territoires.Where(w => w.ID_Province == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Territoire, Value = s.Nom }).ToList();
                    ddl.Commune = new List<DropDown>() { new DropDown { Id = 0, Value = "Select Commune" } };
                    ddl.PolStation = new List<DropDown>() { new DropDown { Id = 0, Value = "Select Poll Station" } };
                    //ddl.Territoire.Add(new DropDown { Value = "Select Territoire" });
                }
                else if (userpol.AssignRole == UserManageRoles.ProvinceUser)
                {
                    ddl.Province = db.Provinces.Where(w => w.ID_Province == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Province, Value = s.Nom }).ToList();
                    ddl.Territoire = db.Territoires.Where(w => w.ID_Province == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Territoire, Value = s.Nom }).ToList();
                    ddl.Commune = new List<DropDown>() { new DropDown { Id = 0, Value = "Select Commune" } };
                    ddl.PolStation = new List<DropDown>() { new DropDown { Id = 0, Value = "Select Poll Station" } };
                    //ddl.Territoire.Insert(0,new DropDown {  Value = "Select Territoire" });
                }
                else if (userpol.AssignRole == UserManageRoles.TerritoireUser)
                {

                    ddl.Territoire = db.Territoires.Where(w => w.ID_Territoire == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Territoire, Value = s.Nom }).ToList();
                    ddl.Commune = db.Communes.Where(w => w.ID_Territoire == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Commune, Value = s.Nom }).ToList();
                    ddl.PolStation = new List<DropDown>() { new DropDown { Id = 0, Value = "Select Poll Station" } };
                    ddl.Province = db.Provinces.Where(w => w.ID_Province == db.Territoires.Where(z => z.ID_Territoire == userpol.AssignID).Select(s => s.ID_Province).FirstOrDefault()).Select(s => new DropDown { Id = s.ID_Province, Value = s.Nom }).ToList();
                }
                else if (userpol.AssignRole == UserManageRoles.CommuneUser)
                {
                    ddl.Commune = db.Communes.Where(w => w.ID_Commune == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Commune, Value = s.Nom }).ToList();
                    ddl.PolStation = db.BureauVotes.Where(w => w.ID_Commune == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Bureauvote, Value = s.Nom }).ToList();
                    ddl.Territoire = db.Territoires.Where(w => w.ID_Territoire == db.Communes.Where(z => z.ID_Commune == userpol.AssignID).Select(s => s.ID_Territoire).FirstOrDefault()).Select(s => new DropDown { Id = s.ID_Territoire, Value = s.Nom }).ToList();
                    ddl.Province = db.Provinces.Where(w => w.ID_Province == db.Communes.Where(z => z.ID_Commune == userpol.AssignID).Select(s => s.ID_Province).FirstOrDefault()).Select(s => new DropDown { Id = s.ID_Province, Value = s.Nom }).ToList();
                }
                else if (userpol.AssignRole == UserManageRoles.PollingUser)
                {
                    ddl.PolStation = db.BureauVotes.Where(w => w.ID_Bureauvote == userpol.AssignID).Select(s => new DropDown { Id = s.ID_Bureauvote, Value = s.Nom }).ToList();
                    ddl.Commune = db.Communes.Where(w => w.ID_Commune == db.BureauVotes.Where(z => z.ID_Bureauvote == userpol.AssignID).Select(s => s.ID_Commune).FirstOrDefault()).Select(s => new DropDown { Id = s.ID_Commune, Value = s.Nom }).ToList();
                    ddl.Territoire = db.Territoires.Where(w => w.ID_Territoire == db.Communes.Where(z => z.ID_Commune == userpol.AssignID).Select(s => s.ID_Territoire).FirstOrDefault()).Select(s => new DropDown { Id = s.ID_Territoire, Value = s.Nom }).ToList();
                    ddl.Province = db.Provinces.Where(w => w.ID_Province == db.Communes.Where(z => z.ID_Commune == userpol.AssignID).Select(s => s.ID_Province).FirstOrDefault()).Select(s => new DropDown { Id = s.ID_Province, Value = s.Nom }).ToList();
                }

            }
            return ddl;
        }
        public dynamic UserIndexList(string UserId)
        {
            UserDisplay display = accountService.FindUserDisplay("Id", UserId);
            if (display != null && display.Role == UserManageRoles.SuperAdmin)
            {
                var results = (from res in db.Results
                               join ce in db.Communes on res.BureauVote.ID_Commune equals ce.ID_Commune
                               join te in db.Territoires on ce.ID_Territoire equals te.ID_Territoire
                               join pr in db.Provinces on te.ID_Province equals pr.ID_Province
                               join ca in db.Candidats on res.ID_Candidat equals ca.ID_Candidat
                               join pa in db.Parties on res.ID_Party equals pa.ID_Party
                               select new { Party = pa.Nom, Candidats = ca.Nom, Provinces = pr.Nom, Territoires = te.Nom, res.Voix, res.Votants, res.Nuls, res.Exprimes, res.Total_Votes });
                return results;
            }
            else
            {
                var results = (from res in db.Results
                               join ce in db.Communes on res.BureauVote.ID_Commune equals ce.ID_Commune
                               join te in db.Territoires on ce.ID_Territoire equals te.ID_Territoire
                               join pr in db.Provinces on te.ID_Province equals pr.ID_Province
                               join ca in db.Candidats on res.ID_Candidat equals ca.ID_Candidat
                               join pa in db.Parties on res.ID_Party equals pa.ID_Party
                               where res.UserId == UserId
                               select new { Party = pa.Nom, Candidats = ca.Nom, Provinces = pr.Nom, Territoires = te.Nom, res.Voix, res.Votants, res.Nuls, res.Exprimes, res.Total_Votes });
                return results;
            }
        }
        public dynamic SaveListRecord(tbl_Results obj)
        {
            if (obj.ID_Result == 0)
            {
                List<tbl_Results> isExist = db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).ToList();//&& w.ID_Candidat == obj.ID_Candidat
                if (isExist.Count == 0)
                {
                    obj.Pourcentage = 100;
                    obj.Exprimes = (obj.Total_Votes + obj.Abstentions + obj.Nuls);
                    db.Results.Add(obj);
                    db.SaveChanges();
                }
                else
                {

                    tbl_Results isdublicate = isExist.Where(w => w.ID_Candidat == obj.ID_Candidat).FirstOrDefault();
                    if (isdublicate != null)
                    {
                        db.Results.RemoveRange(db.Results.Where(w => w.ID_Result == isdublicate.ID_Result).ToList());
                        db.SaveChanges();
                        //Total_Votes = db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).Sum(s => s.Voix);
                    }

                    int Total_Votes = 0;
                    if (db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).Count() > 0)
                    {
                        var tem_total = db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).Sum(s => s.Voix);
                        Total_Votes = string.IsNullOrEmpty(Convert.ToString(tem_total)) ? 0 : tem_total;
                    }


                    double Perc = Double.Parse(((obj.Voix * 100.00) / (Total_Votes + obj.Voix)).ToString("0.00"));
                    obj.Pourcentage = Perc;
                    obj.Total_Votes = Total_Votes + obj.Voix;
                    db.Results.Add(obj);
                    db.SaveChanges();
                    Total_Votes = Total_Votes + obj.Voix;

                    foreach (var it in isExist)
                    {
                        tbl_Results re = new tbl_Results();
                        re = isExist.Where(w => w.ID_Result == it.ID_Result).FirstOrDefault();
                        double Percc = Double.Parse(((re.Voix * 100.00) / Total_Votes).ToString("0.00"));
                        re.Pourcentage = Percc;
                        re.Total_Votes = Total_Votes;
                        re.Abstentions = obj.Abstentions;
                        re.Nuls = obj.Nuls;
                        re.Exprimes = (re.Total_Votes + obj.Abstentions + obj.Nuls);

                        db.SaveChanges();
                    }
                }
            }
            var res = db.Results.Where(w => w.ID_Bureauvote == obj.ID_Bureauvote).Select(s => new { s.ID_Result, s.ID_Candidat, Candidate= s.Candidat.Nom, s.ID_Party, Party = s.Party.Sigle, s.Pourcentage, Votes= s.Voix, s.Exprimes, s.Nuls, s.Abstentions, s.Total_Votes ,s.ID_Bureauvote}).ToList();
            //DataSet dt = new DataSet();
            //dt = _db.GetDatatable("proc_GetProvinceResult", "");
            _Helper.SendNotification();

            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
            //hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));

            SqlNotification objRepo = new SqlNotification();
            objRepo.GetAllMessages();

            return res;
        }
        public List<ResultViewModel> ResultViewList()
        {
            List<ResultViewModel> res = new List<ResultViewModel>();
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand(_smodel.vw_resultlist, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ResultViewModel r = new ResultViewModel();
                        r.ID_Result = Convert.ToInt32(rdr["ID_Result"]);
                        r.Code_SV = Convert.ToInt32(rdr["Code_SV"]);
                        r.UserId = Convert.ToString(rdr["UserId"]);
                        r.Candidat = Convert.ToString(rdr["Candidat"]);
                        r.Party = Convert.ToString(rdr["Party"]);
                        r.Voix = Convert.ToInt32(rdr["Voix"]);
                        r.Pourcentage = Convert.ToDouble(rdr["Pourcentage"]);
                        r.Votants = Convert.ToInt32(rdr["Votants"]);
                        r.Abstentions = Convert.ToInt32(rdr["Abstentions"]);
                        r.Nuls = Convert.ToInt32(rdr["Nuls"]);
                        r.Exprimes = Convert.ToInt32(rdr["Exprimes"]);
                        r.Total_Votes = Convert.ToInt32(rdr["Total_Votes"]);
                        r.FirstName = Convert.ToString(rdr["FirstName"]);
                        r.Province = Convert.ToString(rdr["Province"]);
                        r.Territoire = Convert.ToString(rdr["Territoire"]);
                        r.Commune = Convert.ToString(rdr["Commune"]);
                        r.PolStation = Convert.ToString(rdr["PolStation"]);
                        res.Add(r);
                    }
                    con.Close();

                }

            }
            return res;
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
        public int Code_SV { get; set; }
        //public int ID_Province { get; set; }
        //public int ID_Territoire { get; set; }

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