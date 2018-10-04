namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tbl_BureauVoteResults", "ID_Bureauvote", "dbo.tbl_BureauVote");
            DropForeignKey("dbo.tbl_BureauVoteResults", "ID_Candidat", "dbo.tbl_Candidat");
            DropForeignKey("dbo.tbl_BureauVoteResults", "ID_Party", "dbo.tbl_Party");
            DropIndex("dbo.tbl_BureauVoteResults", new[] { "ID_Bureauvote" });
            DropIndex("dbo.tbl_BureauVoteResults", new[] { "ID_Candidat" });
            DropIndex("dbo.tbl_BureauVoteResults", new[] { "ID_Party" });
            AddColumn("dbo.tbl_Province", "Sieges", c => c.Int(nullable: false));
            
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.tbl_BureauVoteResults",
                c => new
                    {
                        ID_BVResult = c.Int(nullable: false, identity: true),
                        ID_Bureauvote = c.Int(nullable: false),
                        ID_Candidat = c.Int(nullable: false),
                        ID_Party = c.Int(nullable: false),
                        votants = c.Int(nullable: false),
                        BulletinBlancs = c.Int(nullable: false),
                        Nuls = c.Int(nullable: false),
                        Exprimes = c.Int(nullable: false),
                        Voix = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_BVResult);
            
            DropColumn("dbo.tbl_Province", "Sieges");
            CreateIndex("dbo.tbl_BureauVoteResults", "ID_Party");
            CreateIndex("dbo.tbl_BureauVoteResults", "ID_Candidat");
            CreateIndex("dbo.tbl_BureauVoteResults", "ID_Bureauvote");
            AddForeignKey("dbo.tbl_BureauVoteResults", "ID_Party", "dbo.tbl_Party", "ID_Party", cascadeDelete: true);
            AddForeignKey("dbo.tbl_BureauVoteResults", "ID_Candidat", "dbo.tbl_Candidat", "ID_Candidat", cascadeDelete: true);
            AddForeignKey("dbo.tbl_BureauVoteResults", "ID_Bureauvote", "dbo.tbl_BureauVote", "ID_Bureauvote", cascadeDelete: true);
        }
    }
}
