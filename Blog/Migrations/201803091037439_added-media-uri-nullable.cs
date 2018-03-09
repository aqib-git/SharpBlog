namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedmediaurinullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Media", "Uri", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Media", "Uri", c => c.String(nullable: false));
        }
    }
}
