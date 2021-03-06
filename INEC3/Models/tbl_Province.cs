﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace INEC3.Models
{

    public class tbl_Province
    {

        [Key]
        public int ID_Province { get; set; }
        [Display(Name = "Province")]

        public string Nom { get; set; }

        public int Enroles { get; set; }

        public string GID_1 { get; set; }

        public int Sieges { get; set; }


        [ForeignKey("Pays")]
        public int ID_Pays { get; set; }

        public virtual tbl_Pays Pays { get; set; }

    }

}