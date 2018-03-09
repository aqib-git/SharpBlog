namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Media", "Mime", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Media", "Mime", c => c.String());
        }
    }
}
