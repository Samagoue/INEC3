using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

using System.Linq;

using System.Text;

using System.Threading.Tasks;



namespace INEC3.Models

{

    public class tbl_Pays

    {

        [Key]

        public int ID_Pays { get; set; }

        [Display(Name = "Pays")]

        public string Nom { get; set; }

        public int Enroles { get; set; }

    }

}