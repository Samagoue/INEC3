namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class circo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbl_Circonscription",
                c => new
                    {
                        ID_Circonscription = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Enroles = c.Int(nullable: false),
                        SiegesProv = c.Int(nullable: false),
                        SiegesNat = c.Int(nullable: false),
                        ID_Territoire = c.Int(nullable: false),
                        ID_Province = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Circonscription)
                .ForeignKey("dbo.tbl_Province", t => t.ID_Province, cascadeDelete: true)
                .ForeignKey("dbo.tbl_Territoire", t => t.ID_Territoire, cascadeDelete: true)
                .Index(t => t.ID_Territoire)
                .Index(t => t.ID_Province);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbl_Circonscription", "ID_Territoire", "dbo.tbl_Territoire");
            DropForeignKey("dbo.tbl_Circonscription", "ID_Province", "dbo.tbl_Province");
            DropIndex("dbo.tbl_Circonscription", new[] { "ID_Province" });
            DropIndex("dbo.tbl_Circonscription", new[] { "ID_Territoire" });
            DropTable("dbo.tbl_Circonscription");
        }
    }
}
