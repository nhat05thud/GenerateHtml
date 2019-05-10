namespace Generate.EntityFrameWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitGenarateHtmlTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HtmlComponentCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HtmlComponents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        HtmlBody = c.String(),
                        ImageName = c.String(),
                        CssPath = c.String(),
                        ScriptPath = c.String(),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HtmlComponentCategories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HtmlComponents", "CategoryId", "dbo.HtmlComponentCategories");
            DropIndex("dbo.HtmlComponents", new[] { "CategoryId" });
            DropTable("dbo.HtmlComponents");
            DropTable("dbo.HtmlComponentCategories");
        }
    }
}
