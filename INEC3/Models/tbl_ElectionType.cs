using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class tbl_ElectionType
    {
        [Key]
        public int ID_ElectionType { get; set; }
        [Display(Name = "ElectionType")]

        public string Scrutin { get; set; }
        public virtual ICollection<tbl_Candidat> Candidat { get; set; }
    }
}