namespace Mise.Database.AzureDefinitions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InventorySectionIds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "RestaurantInventorySectionIds", c => c.String());
            AddColumn("dbo.RestaurantInventorySections", "RestaurantId", c => c.Guid(nullable: false));
            AddColumn("dbo.InventorySections", "InventoryId", c => c.Guid(nullable: false));
            AddColumn("dbo.InventoryBeverageLineItems", "InventorySectionId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryBeverageLineItems", "InventorySectionId");
            DropColumn("dbo.InventorySections", "InventoryId");
            DropColumn("dbo.RestaurantInventorySections", "RestaurantId");
            DropColumn("dbo.Restaurants", "RestaurantInventorySectionIds");
        }
    }
}
