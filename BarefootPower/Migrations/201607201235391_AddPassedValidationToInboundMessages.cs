namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPassedValidationToInboundMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InboundMessages", "PassedValidation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InboundMessages", "PassedValidation");
        }
    }
}
