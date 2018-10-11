using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class tbl_Party
    {
        [Key]
        public int ID_Party { get; set; }

        public string Sigle { get; set; }
        public string Nom { get; set; }
        public string Addresse { get; set; }
        public string Dirigeant { get; set; }
        public string Color { get; set; }
    }
}