namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageToOutboundMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OutboundMessages", "Message", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OutboundMessages", "Message");
        }
    }
}
