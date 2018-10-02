using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;

using System.Text;

using System.Threading.Tasks;



namespace INEC3.Models

{

    public class tbl_Commune_Chefferie

    {

        [Key]

        public int ID_Commune { get; set; }

        [Display(Name = "Commune / Chefferie")]

        public string Nom { get; set; }

        public int Enroles { get; set; }

        public int Sieges { get; set; }



        [ForeignKey("Territoire")]

        public int ID_Territoire { get; set; }

        public virtual tbl_Territoire Territoire { get; set; }





        [ForeignKey("Province")]

        public int ID_Province { get; set; }

        public virtual tbl_Province Province { get; set; }

    }

}