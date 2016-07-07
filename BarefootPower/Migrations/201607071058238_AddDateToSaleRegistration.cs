namespace BarefootPower.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateToSaleRegistration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaleRegistrations", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SaleRegistrations", "Date");
        }
    }
}
