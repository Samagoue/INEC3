using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class tbl_Candidat
    {
        [Key]
        public int ID_Candidat { get; set; }

        public string Nom { get; set; }

        [ForeignKey("Party")]
        public int ID_Party { get; set; }
        public virtual tbl_Party Party { get; set; }
    }


}