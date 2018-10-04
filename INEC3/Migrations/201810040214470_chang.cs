namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chang : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbl_ElectionType",
                c => new
                    {
                        ID_ElectionType = c.Int(nullable: false, identity: true),
                        Scrutin = c.String(),
                    })
                .PrimaryKey(t => t.ID_ElectionType);
            
            AddColumn("dbo.tbl_Candidat", "tbl_ElectionType_ID_ElectionType", c => c.Int());
            CreateIndex("dbo.tbl_Candidat", "tbl_ElectionType_ID_ElectionType");
            AddForeignKey("dbo.tbl_Candidat", "tbl_ElectionType_ID_ElectionType", "dbo.tbl_ElectionType", "ID_ElectionType");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbl_Candidat", "tbl_ElectionType_ID_ElectionType", "dbo.tbl_ElectionType");
            DropIndex("dbo.tbl_Candidat", new[] { "tbl_ElectionType_ID_ElectionType" });
            DropColumn("dbo.tbl_Candidat", "tbl_ElectionType_ID_ElectionType");
            DropTable("dbo.tbl_ElectionType");
        }
    }
}
