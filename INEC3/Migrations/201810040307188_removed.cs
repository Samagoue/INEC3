namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removed : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tbl_Candidat", "PresidentialElection");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbl_Candidat", "PresidentialElection", c => c.Boolean(nullable: false));
        }
    }
}
