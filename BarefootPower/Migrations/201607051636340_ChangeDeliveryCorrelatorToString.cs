namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDeliveryCorrelatorToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Deliveries", "Correlator", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Deliveries", "Correlator", c => c.Int(nullable: false));
        }
    }
}
