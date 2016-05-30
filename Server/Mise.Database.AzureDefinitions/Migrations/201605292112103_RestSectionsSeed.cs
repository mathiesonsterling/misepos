namespace Mise.Database.AzureDefinitions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestSectionsSeed : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Restaurants", "RestaurantInventorySectionIds");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurants", "RestaurantInventorySectionIds", c => c.String());
        }
    }
}
