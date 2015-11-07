namespace stockboyService.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class FixStreetAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "stockboy.Restaurants",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Id")
                                },
                            }),
                        CreatedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdatedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        MiseId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
                        RestaurantID = c.Guid(nullable: false),
                        AccountID = c.Guid(),
                        RestaurantFullName = c.String(),
                        RestaurantShortName = c.String(),
                        StreetAddressNumber = c.String(),
                        Street = c.String(),
                        StreetDirection = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        ZipCode = c.String(),
                        AreaCode = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        NumberOfActiveCashRegisters = c.Int(nullable: false),
                        NumberOfActiveCreditRegisters = c.Int(nullable: false),
                        NumberOfActiveOrderTerminals = c.Int(nullable: false),
                        FriendlyID = c.String(),
                        IsPlaceholder = c.Boolean(nullable: false),
                        CurrentInventoryID = c.Guid(),
                        LastMeasuredInventoryID = c.Guid(),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Version")
                                },
                            }),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "CreatedAt")
                                },
                            }),
                        UpdatedAt = c.DateTimeOffset(precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "UpdatedAt")
                                },
                            }),
                        Deleted = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Deleted")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedAt, clustered: true);
            
        }
        
        public override void Down()
        {
            DropIndex("stockboy.Restaurants", new[] { "CreatedAt" });
            DropTable("stockboy.Restaurants",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "CreatedAt" },
                        }
                    },
                    {
                        "Deleted",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Deleted" },
                        }
                    },
                    {
                        "Id",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Id" },
                        }
                    },
                    {
                        "UpdatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "UpdatedAt" },
                        }
                    },
                    {
                        "Version",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Version" },
                        }
                    },
                });
        }
    }
}
