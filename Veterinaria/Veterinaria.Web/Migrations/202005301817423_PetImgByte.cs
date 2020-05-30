namespace Veterinaria.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PetImgByte : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pets", "ImgUrl", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pets", "ImgUrl");
        }
    }
}
