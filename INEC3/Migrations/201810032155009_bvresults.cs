namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bvresults : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_Territoire", "SiegesProv", c => c.Int(nullable: false));
            AddColumn("dbo.tbl_Territoire", "SiegesNat", c => c.Int(nullable: false));
            DropColumn("dbo.tbl_Territoire", "Sieges");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbl_Territoire", "Sieges", c => c.Int(nullable: false));
            DropColumn("dbo.tbl_Territoire", "SiegesNat");
            DropColumn("dbo.tbl_Territoire", "SiegesProv");
        }
    }
}
