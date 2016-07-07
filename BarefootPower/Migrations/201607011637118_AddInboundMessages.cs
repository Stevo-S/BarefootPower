namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInboundMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InboundMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Sender = c.String(maxLength: 20),
                        ServiceId = c.String(maxLength: 50),
                        LinkId = c.String(maxLength: 100),
                        TraceUniqueId = c.String(maxLength: 100),
                        Correlator = c.String(maxLength: 50),
                        ShortCode = c.String(maxLength: 6),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InboundMessages");
        }
    }
}
