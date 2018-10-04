namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedpresidentoptio : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_Candidat", "PresidentialElection", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbl_Candidat", "PresidentialElection");
        }
    }
}
