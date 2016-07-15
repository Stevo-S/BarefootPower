namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveStatusToAgents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Agents", "isActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Agents", "isActive");
        }
    }
}
