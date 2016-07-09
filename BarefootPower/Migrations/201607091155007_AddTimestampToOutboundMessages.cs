namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimestampToOutboundMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OutboundMessages", "Timestamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OutboundMessages", "Timestamp");
        }
    }
}
