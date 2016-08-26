namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveEmailandMiddleNameFromAgents : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Agents", "MiddleName");
            DropColumn("dbo.Agents", "Email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Agents", "Email", c => c.String(maxLength: 100));
            AddColumn("dbo.Agents", "MiddleName", c => c.String(maxLength: 50));
        }
    }
}
