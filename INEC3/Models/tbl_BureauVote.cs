using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INEC3.Models
{

    public class tbl_BureauVote
    {

        [Key]
        public int ID_Bureauvote { get; set; }

        public string Nom { get; set; }

        public string Addresse { get; set; }

        public int Enroles { get; set; }


        [ForeignKey("Commune")]
        public int ID_Commune { get; set; }
        public virtual tbl_Commune_Chefferie Commune { get; set; }

        public int Code_SV { get; set; }

        public int Abstentions { get; set; }
        public int Votants { get; set; }
        public int bulletinsAnnules { get; set; }
        public int votesExprimes { get; set; }


    }

}