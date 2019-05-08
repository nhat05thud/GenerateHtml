namespace Generate.EntityFrameWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTblHtmlComponent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HtmlComponents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        HtmlBody = c.String(),
                        CssPath = c.String(),
                        ScriptPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HtmlComponents");
        }
    }
}
