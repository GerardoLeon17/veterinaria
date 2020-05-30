namespace Veterinaria.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Petimg : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Pets", "ImgUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pets", "ImgUrl", c => c.String());
        }
    }
}
