namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedmediaurl : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Media", "Url");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Media", "Url", c => c.String());
        }
    }
}
