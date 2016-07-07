namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOutboundMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OutboundMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Destination = c.String(maxLength: 20),
                        ServiceId = c.String(maxLength: 50),
                        LinkId = c.String(maxLength: 100),
                        Correlator = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OutboundMessages");
        }
    }
}
