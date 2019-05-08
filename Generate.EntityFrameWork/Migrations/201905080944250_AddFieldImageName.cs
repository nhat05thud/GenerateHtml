namespace Generate.EntityFrameWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldImageName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HtmlComponents", "ImageName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HtmlComponents", "ImageName");
        }
    }
}
