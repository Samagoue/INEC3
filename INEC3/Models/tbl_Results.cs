using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class tbl_Results
    {
        [Key]
        public int ID_Result { get; set; }


        [ForeignKey("Candidat")]
        public int ID_Candidat { get; set; }
        public virtual tbl_Candidat Candidat { get; set; }

        [ForeignKey("Party")]
        [Display(Name = "Party")]
        public int ID_Party { get; set; }
        public virtual tbl_Party Party { get; set; }

        [ForeignKey("BureauVote")]
        public int ID_Bureauvote { get; set; }
        public virtual tbl_BureauVote BureauVote { get; set; }

        public int Voix { get; set; }
        public double? Pourcentage { get; set; }
        public int Votants { get; set; }
        public int Total_Votes { get; set; }
        public int Abstentions { get; set; }
        public int Nuls { get; set; }
        public int Exprimes { get; set; }

    }

    public class Results
    {
        public int ID_Result { get; set; }
        public int ID_Bureauvote { get; set; }
        public int Votants { get; set; }
        public int Abstentions { get; set; }
        public int Nuls { get; set; }
        public int Exprimes { get; set; }
        public int Total_Votes { get; set; }
        public List<ResultList> ResultList { get; set; }

    }

    public class ResultList
    {
        public int ID_Candidat { get; set; }
        public int ID_Party { get; set; }
        public double Pourcentage { get; set; }
        public int Voix { get; set; }
    }

    public class ResultsLive
    {
        public int ID_Result { get; set; }
        public int ID_Bureauvote { get; set; }
        public int Votants { get; set; }
        public int Abstentions { get; set; }
        public int Nuls { get; set; }
        public int Exprimes { get; set; }
        public int Total_Votes { get; set; }
        public int ID_Candidat { get; set; }
        public int ID_Party { get; set; }
        public double Pourcentage { get; set; }
        public int Voix { get; set; }

    }
}