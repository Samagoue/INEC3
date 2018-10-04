namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedelectiontypetocandidat : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tbl_Candidat", "tbl_ElectionType_ID_ElectionType", "dbo.tbl_ElectionType");
            DropIndex("dbo.tbl_Candidat", new[] { "tbl_ElectionType_ID_ElectionType" });
            RenameColumn(table: "dbo.tbl_Candidat", name: "tbl_ElectionType_ID_ElectionType", newName: "ID_ElectionType");
            AlterColumn("dbo.tbl_Candidat", "ID_ElectionType", c => c.Int(nullable: false));
            CreateIndex("dbo.tbl_Candidat", "ID_ElectionType");
            AddForeignKey("dbo.tbl_Candidat", "ID_ElectionType", "dbo.tbl_ElectionType", "ID_ElectionType", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbl_Candidat", "ID_ElectionType", "dbo.tbl_ElectionType");
            DropIndex("dbo.tbl_Candidat", new[] { "ID_ElectionType" });
            AlterColumn("dbo.tbl_Candidat", "ID_ElectionType", c => c.Int());
            RenameColumn(table: "dbo.tbl_Candidat", name: "ID_ElectionType", newName: "tbl_ElectionType_ID_ElectionType");
            CreateIndex("dbo.tbl_Candidat", "tbl_ElectionType_ID_ElectionType");
            AddForeignKey("dbo.tbl_Candidat", "tbl_ElectionType_ID_ElectionType", "dbo.tbl_ElectionType", "ID_ElectionType");
        }
    }
}
