using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INEC3.Models
{

    public class tbl_Territoire
    {

        [Key]
        public int ID_Territoire { get; set; }
        [Display(Name = "Territoire")]

        public string Nom { get; set; }

        public int Enroles { get; set; }

        public string GID_2 { get; set; }

        public string GID_1 { get; set; }

        public int Sieges { get; set; }

        [ForeignKey("Province")]
        public int ID_Province { get; set; }
        public virtual tbl_Province Province { get; set; }

    }

}