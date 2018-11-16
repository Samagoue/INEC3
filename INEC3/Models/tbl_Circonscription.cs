using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class tbl_Circonscription
    {
        [Key]
        public int ID_Circonscription { get; set; }
        [Display(Name = "Circonscription")]

        public string Nom { get; set; }

        public int Enroles { get; set; }

        public int SiegesProv { get; set; }

        public int SiegesNat { get; set; }


        [ForeignKey("Territoire")]
        public int ID_Territoire { get; set; }
        public virtual tbl_Territoire Territoire { get; set; }

        [ForeignKey("Province")]
        public int ID_Province { get; set; }
        public virtual tbl_Province Province { get; set; }
    }
}