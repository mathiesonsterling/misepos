namespace Mise.Database.AzureDefinitions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRestIdsToEmployees : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "RestaurantsEmployedAtIds", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "RestaurantsEmployedAtIds");
        }
    }
}
