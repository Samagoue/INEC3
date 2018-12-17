using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class ResultViewModel
    {
        public int ID_Result { get; set; }
        public int Code_SV { get; set; }
        public string UserId { get; set; }
        public string Candidat { get; set; }
        public string Party { get; set; }
        public int Voix { get; set; }
        public double Pourcentage { get; set; }
        public int Votants { get; set; }
        public int Abstentions { get; set; }
        public int Nuls { get; set; }
        public int Exprimes { get; set; }
        public int Total_Votes { get; set; }
        public string FirstName { get; set; }
        public string Province { get; set; }
        public string Territoire { get; set; }
        public string Commune { get; set; }
        public string PolStation { get; set; }
    }
}