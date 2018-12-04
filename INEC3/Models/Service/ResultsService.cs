using INEC3.DbConn;
using INEC3.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INEC3.Models.Service
{
    public class ResultsService
    {
        private inecDBContext db;
        private Sqldbconn _db;
        private _Helper _Helper;
        //private AuthContext _context;

        public ResultsService()
        {
            //_context = new AuthContext();
            db = new inecDBContext();
            _db = new Sqldbconn();
            _Helper = new _Helper();
        }


        public dynamic GetParty(int candidateid)
        {
            return db.Candidats.Where(w => w.ID_Candidat == candidateid).Select(s => new { s.ID_Party, s.Party.Color }).FirstOrDefault();

        }
        public dynamic PolStationCahngeGet(int polingstationid)
        {
            var res = new Dictionary<string, string>();
            var voters = db.BureauVotes.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.Commune.Enroles }).FirstOrDefault();
            var exprims = db.Results.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.Abstentions, s.Exprimes, s.Nuls, s.Total_Votes }).FirstOrDefault();
            var res1 = db.Results.Where(w => w.ID_Bureauvote == polingstationid).Select(s => new { s.ID_Result, s.ID_Candidat, s.Candidat.Nom, s.ID_Party, Party = s.Party.Sigle, s.Pourcentage, s.Voix }).ToList();
            res.Add("voters", JsonConvert.SerializeObject(voters));
            res.Add("list", JsonConvert.SerializeObject(res1));
            if (exprims != null)
            {
                res.Add("exprims", JsonConvert.SerializeObject(exprims));
            }
            return res;
        }

        public dynamic UserPolingStationGet()
        {
            //return db.BureauVotes.Where(w => w.ID_Commune == CommuneId).Select(s => new { s.ID_Bureauvote, s.Nom }).ToList();
            return db.BureauVotes.Select(s => new { Id=s.ID_Bureauvote,Value= s.Nom }).ToList();
             
        }
    }
}