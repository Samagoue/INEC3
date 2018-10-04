﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace INEC3.Models
{

    public class inecDBContext : DbContext
    {
        

        public inecDBContext() : base("inecConn")
        {

            Database.SetInitializer<inecDBContext>(null);

        }

        public DbSet<tbl_Pays> Pays { get; set; }

        public DbSet<tbl_Province> Provinces { get; set; }

        public DbSet<tbl_Territoire> Territoires { get; set; }

        public DbSet<tbl_Commune_Chefferie> Communes { get; set; }

        public DbSet<tbl_BureauVote> BureauVotes { get; set; }

        public DbSet<tbl_Party> Parties { get; set; }    

        public DbSet<tbl_Candidat> Candidats { get; set; }

        public DbSet<tbl_ElectionType> ElectionTypes { get; set; }
    }

}