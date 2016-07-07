namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeliveries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Deliveries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Destination = c.String(maxLength: 20),
                        DeliveryStatus = c.String(maxLength: 100),
                        ServiceId = c.String(maxLength: 50),
                        Correlator = c.Int(nullable: false),
                        TraceUniqueId = c.String(maxLength: 100),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Destination)
                .Index(t => t.DeliveryStatus)
                .Index(t => t.TimeStamp);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Deliveries", new[] { "TimeStamp" });
            DropIndex("dbo.Deliveries", new[] { "DeliveryStatus" });
            DropIndex("dbo.Deliveries", new[] { "Destination" });
            DropTable("dbo.Deliveries");
        }
    }
}
