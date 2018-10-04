namespace INEC3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changetocandidat : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.tbl_Candidat", name: "tbl_ElectionType_ID_ElectionType", newName: "ID_ElectionType");
            RenameIndex(table: "dbo.tbl_Candidat", name: "IX_tbl_ElectionType_ID_ElectionType", newName: "IX_ID_ElectionType");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.tbl_Candidat", name: "IX_ID_ElectionType", newName: "IX_tbl_ElectionType_ID_ElectionType");
            RenameColumn(table: "dbo.tbl_Candidat", name: "ID_ElectionType", newName: "tbl_ElectionType_ID_ElectionType");
        }
    }
}
