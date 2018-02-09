namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class showposts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Show", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Show");
        }
    }
}
