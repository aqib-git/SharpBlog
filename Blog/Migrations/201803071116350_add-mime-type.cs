namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmimetype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "Mime", c => c.String());
            DropColumn("dbo.Media", "Extension");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Media", "Extension", c => c.String());
            DropColumn("dbo.Media", "Mime");
        }
    }
}
