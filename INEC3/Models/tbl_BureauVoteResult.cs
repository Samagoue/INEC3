using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class tbl_BureauVoteResult
    {
        [Key]
        public int ID_BVResult { get; set; }

        [ForeignKey("BureauVote")]
        public int ID_Bureauvote { get; set; }
        public virtual tbl_BureauVote BureauVote { get; set; }

        [ForeignKey("Candidat")]
        public int ID_Candidat { get; set; }
        public virtual tbl_Candidat Candidat { get; set; }

        [ForeignKey("Party")]
        public int ID_Party { get; set; }
        public virtual tbl_Party Party { get; set; }

        public int votants { get; set; }
        public int BulletinBlancs { get; set; }
        public int Nuls { get; set; }
        public int Exprimes { get; set; }

        public int Voix { get; set; }


    }
}