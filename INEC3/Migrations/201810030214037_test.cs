namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.tbl_BureauVoteResults", newName: "tbl_BureauVoteResult");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.tbl_BureauVoteResult", newName: "tbl_BureauVoteResults");
        }
    }
}
