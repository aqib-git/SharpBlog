namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedmediauri : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "Uri", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Media", "Uri");
        }
    }
}
