namespace Mise.Database.AzureDefinitions.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationInvitations",
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
                        Status = c.Int(nullable: false),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        Application_Id = c.String(maxLength: 128),
                        DestinationEmployee_Id = c.String(maxLength: 128),
                        InvitingEmployee_Id = c.String(maxLength: 128),
                        Restaurant_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MiseApplications", t => t.Application_Id)
                .ForeignKey("dbo.Employees", t => t.DestinationEmployee_Id)
                .ForeignKey("dbo.Employees", t => t.InvitingEmployee_Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.Application_Id)
                .Index(t => t.DestinationEmployee_Id)
                .Index(t => t.InvitingEmployee_Id)
                .Index(t => t.Restaurant_Id);
            
            CreateTable(
                "dbo.MiseApplications",
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
                        AppTypeValue = c.Int(nullable: false),
                        Name = c.String(),
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
            
            CreateTable(
                "dbo.Employees",
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
                        LastTimeLoggedIntoInventoryApp = c.DateTimeOffset(precision: 7),
                        LastDeviceIDLoggedInWith = c.String(),
                        CurrentlyLoggedIntoInventoryApp = c.Boolean(nullable: false),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        PasswordHash = c.String(),
                        DisplayName = c.String(),
                        Emails = c.String(),
                        PrimaryEmail = c.String(),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
            
            CreateTable(
                "dbo.EmployeeRestaurantRelationships",
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
                        IsCurrentEmployee = c.Boolean(nullable: false),
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
                        Employee_Id = c.String(maxLength: 128),
                        Restaurant_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.Employee_Id)
                .Index(t => t.Restaurant_Id);
            
            CreateTable(
                "dbo.Restaurants",
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
                        RestaurantID = c.Guid(nullable: false),
                        AccountID = c.Guid(),
                        Name_FullName = c.String(),
                        Name_ShortName = c.String(),
                        StreetAddress_StreetAddressNumberDb_Number = c.String(),
                        StreetAddress_StreetAddressNumberDb_Direction = c.String(),
                        StreetAddress_StreetAddressNumberDb_ApartmentNumber = c.String(),
                        StreetAddress_StreetAddressNumberDb_Longitude = c.Double(nullable: false),
                        StreetAddress_StreetAddressNumberDb_Latitude = c.Double(nullable: false),
                        StreetAddress_Street_Name = c.String(),
                        StreetAddress_City_Name = c.String(),
                        StreetAddress_State_Name = c.String(),
                        StreetAddress_State_Abbreviation = c.String(),
                        StreetAddress_Country_Name = c.String(),
                        StreetAddress_Zip_Value = c.String(),
                        PhoneNumberAreaCode = c.String(),
                        PhoneNumber = c.String(),
                        IsPlaceholder = c.Boolean(nullable: false),
                        EmailsToSendReportsTo = c.String(),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
            
            CreateTable(
                "dbo.RestaurantInventorySections",
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
                        Name = c.String(),
                        AllowsPartialBottles = c.Boolean(nullable: false),
                        IsDefaultInventorySection = c.Boolean(nullable: false),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        Restaurant_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.Restaurant_Id);
            
            CreateTable(
                "dbo.RestaurantApplicationUses",
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
                        MiseApplication_Id = c.String(maxLength: 128),
                        Restaurant_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MiseApplications", t => t.MiseApplication_Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.MiseApplication_Id)
                .Index(t => t.Restaurant_Id);
            
            CreateTable(
                "dbo.Inventories",
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
                        DateCompleted = c.DateTimeOffset(precision: 7),
                        IsCurrent = c.Boolean(nullable: false),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        CreatedByEmployee_Id = c.String(maxLength: 128),
                        Restaurant_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CreatedByEmployee_Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.CreatedByEmployee_Id)
                .Index(t => t.Restaurant_Id);
            
            CreateTable(
                "dbo.InventorySections",
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
                        Name = c.String(),
                        TimeCountStarted = c.DateTimeOffset(precision: 7),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        CurrentlyInUseBy_Id = c.String(maxLength: 128),
                        Inventory_Id = c.String(maxLength: 128),
                        LastCompletedBy_Id = c.String(maxLength: 128),
                        RestaurantInventorySection_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CurrentlyInUseBy_Id)
                .ForeignKey("dbo.Inventories", t => t.Inventory_Id)
                .ForeignKey("dbo.Employees", t => t.LastCompletedBy_Id)
                .ForeignKey("dbo.RestaurantInventorySections", t => t.RestaurantInventorySection_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.CurrentlyInUseBy_Id)
                .Index(t => t.Inventory_Id)
                .Index(t => t.LastCompletedBy_Id)
                .Index(t => t.RestaurantInventorySection_Id);
            
            CreateTable(
                "dbo.InventoryBeverageLineItems",
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
                        CurrentAmount_Milliliters = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentAmount_SpecificGravity = c.Decimal(precision: 18, scale: 2),
                        CurrentAmount_Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PricePaid_Dollars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PartialBottleListing = c.String(),
                        NumFullBottles = c.Int(nullable: false),
                        MethodsMeasuredLast = c.Int(nullable: false),
                        InventoryPosition = c.Int(nullable: false),
                        BaseLineItem_DisplayName = c.String(),
                        BaseLineItem_MiseName = c.String(),
                        BaseLineItem_UPC = c.String(),
                        BaseLineItem_CaseSize = c.Int(),
                        BaseLineItem_Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        VendorBoughtFrom_Id = c.String(maxLength: 128),
                        InventorySection_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vendors", t => t.VendorBoughtFrom_Id)
                .ForeignKey("dbo.InventorySections", t => t.InventorySection_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.VendorBoughtFrom_Id)
                .Index(t => t.InventorySection_Id);
            
            CreateTable(
                "dbo.EntityCategoryOwnerships",
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
                        Entity = c.Guid(nullable: false),
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
                        InventoryCategory_Id = c.String(maxLength: 128),
                        InventoryBeverageLineItem_Id = c.String(maxLength: 128),
                        VendorBeverageLineItem_Id = c.String(maxLength: 128),
                        ParBeverageLineItem_Id = c.String(maxLength: 128),
                        PurchaseOrderBeverageLineItem_Id = c.String(maxLength: 128),
                        ReceivingOrderBeverageLineItem_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InventoryCategories", t => t.InventoryCategory_Id)
                .ForeignKey("dbo.InventoryBeverageLineItems", t => t.InventoryBeverageLineItem_Id)
                .ForeignKey("dbo.VendorBeverageLineItems", t => t.VendorBeverageLineItem_Id)
                .ForeignKey("dbo.ParBeverageLineItems", t => t.ParBeverageLineItem_Id)
                .ForeignKey("dbo.PurchaseOrderBeverageLineItems", t => t.PurchaseOrderBeverageLineItem_Id)
                .ForeignKey("dbo.ReceivingOrderBeverageLineItems", t => t.ReceivingOrderBeverageLineItem_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.InventoryCategory_Id)
                .Index(t => t.InventoryBeverageLineItem_Id)
                .Index(t => t.VendorBeverageLineItem_Id)
                .Index(t => t.ParBeverageLineItem_Id)
                .Index(t => t.PurchaseOrderBeverageLineItem_Id)
                .Index(t => t.ReceivingOrderBeverageLineItem_Id);
            
            CreateTable(
                "dbo.InventoryCategories",
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
                        Name = c.String(),
                        IsCustomCategory = c.Boolean(nullable: false),
                        IsAssignable = c.Boolean(nullable: false),
                        PreferredContainer_ContainerExclusiveToBusinessId = c.Guid(),
                        PreferredContainer_ContainerDisplayName = c.String(),
                        PreferredContainer_AmountContained_Milliliters = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreferredContainer_AmountContained_SpecificGravity = c.Decimal(precision: 18, scale: 2),
                        PreferredContainer_AmountContained_Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreferredContainer_WeightEmpty_Grams = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreferredContainer_WeightFull_Grams = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreferredContainer_Shape_Name = c.String(),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        ParentCategory_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InventoryCategories", t => t.ParentCategory_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.ParentCategory_Id);
            
            CreateTable(
                "dbo.Vendors",
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
                        StreetAddress_StreetAddressNumberDb_Number = c.String(),
                        StreetAddress_StreetAddressNumberDb_Direction = c.String(),
                        StreetAddress_StreetAddressNumberDb_ApartmentNumber = c.String(),
                        StreetAddress_StreetAddressNumberDb_Longitude = c.Double(nullable: false),
                        StreetAddress_StreetAddressNumberDb_Latitude = c.Double(nullable: false),
                        StreetAddress_Street_Name = c.String(),
                        StreetAddress_City_Name = c.String(),
                        StreetAddress_State_Name = c.String(),
                        StreetAddress_State_Abbreviation = c.String(),
                        StreetAddress_Country_Name = c.String(),
                        StreetAddress_Zip_Value = c.String(),
                        EmailToOrderFrom = c.String(),
                        Website = c.String(),
                        VendorPhoneNumberAreaCode = c.String(),
                        VendorPhoneNumber = c.String(),
                        Name_FullName = c.String(),
                        Name_ShortName = c.String(),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        CreatedBy_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CreatedBy_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.CreatedBy_Id);
            
            CreateTable(
                "dbo.VendorRestaurantRelationships",
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
                        Restaurant_Id = c.String(maxLength: 128),
                        Vendor_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .ForeignKey("dbo.Vendors", t => t.Vendor_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.Restaurant_Id)
                .Index(t => t.Vendor_Id);
            
            CreateTable(
                "dbo.VendorBeverageLineItems",
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
                        NameOfItemInVendor = c.String(),
                        PublicPricePerUnit_Dollars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BaseLineItem_DisplayName = c.String(),
                        BaseLineItem_MiseName = c.String(),
                        BaseLineItem_UPC = c.String(),
                        BaseLineItem_CaseSize = c.Int(),
                        BaseLineItem_Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        Vendor_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vendors", t => t.Vendor_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.Vendor_Id);
            
            CreateTable(
                "dbo.VendorPrivateRestaurantPrices",
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
                        PriceCharged_Dollars = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                        Restaurant_Id = c.String(maxLength: 128),
                        VendorBeverageLineItem_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .ForeignKey("dbo.VendorBeverageLineItems", t => t.VendorBeverageLineItem_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.Restaurant_Id)
                .Index(t => t.VendorBeverageLineItem_Id);
            
            CreateTable(
                "dbo.MiseEmployeeAccounts",
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
                        AccountType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        PrimaryEmail = c.String(),
                        AccountPasswordHash = c.String(),
                        AccountHolderFirstName = c.String(),
                        AccountHolderLastName = c.String(),
                        Emails = c.String(),
                        AccountPhoneNumberAreaCode = c.String(),
                        AccountPhoneNumber = c.String(),
                        ReferralCodeForAccountToGiveOut = c.String(),
                        ReferralCodeUsedToCreate = c.String(),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
            
            CreateTable(
                "dbo.Pars",
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
                        IsCurrent = c.Boolean(nullable: false),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        CreatedByEmployee_Id = c.String(maxLength: 128),
                        Restaurant_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CreatedByEmployee_Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.CreatedByEmployee_Id)
                .Index(t => t.Restaurant_Id);
            
            CreateTable(
                "dbo.ParBeverageLineItems",
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
                        BaseLineItem_DisplayName = c.String(),
                        BaseLineItem_MiseName = c.String(),
                        BaseLineItem_UPC = c.String(),
                        BaseLineItem_CaseSize = c.Int(),
                        BaseLineItem_Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        Par_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pars", t => t.Par_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.Par_Id);
            
            CreateTable(
                "dbo.PurchaseOrders",
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
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        CreatedBy_Id = c.String(maxLength: 128),
                        Restaurant_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CreatedBy_Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.Restaurant_Id);
            
            CreateTable(
                "dbo.PurchaseOrderPerVendors",
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
                        Status = c.Int(nullable: false),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        Vendor_Id = c.String(maxLength: 128),
                        PurchaseOrder_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vendors", t => t.Vendor_Id)
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrder_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.Vendor_Id)
                .Index(t => t.PurchaseOrder_Id);
            
            CreateTable(
                "dbo.PurchaseOrderBeverageLineItems",
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
                        BaseLineItem_DisplayName = c.String(),
                        BaseLineItem_MiseName = c.String(),
                        BaseLineItem_UPC = c.String(),
                        BaseLineItem_CaseSize = c.Int(),
                        BaseLineItem_Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        PurchaseOrderPerVendor_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PurchaseOrderPerVendors", t => t.PurchaseOrderPerVendor_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.PurchaseOrderPerVendor_Id);
            
            CreateTable(
                "dbo.ReceivingOrders",
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
                        DateReceived = c.DateTimeOffset(nullable: false, precision: 7),
                        Status = c.Int(nullable: false),
                        Notes = c.String(),
                        InvoiceID = c.String(),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        PurchaseOrder_Id = c.String(maxLength: 128),
                        ReceivedByEmployee_Id = c.String(maxLength: 128),
                        Restaurant_Id = c.String(maxLength: 128),
                        Vendor_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrder_Id)
                .ForeignKey("dbo.Employees", t => t.ReceivedByEmployee_Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .ForeignKey("dbo.Vendors", t => t.Vendor_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.PurchaseOrder_Id)
                .Index(t => t.ReceivedByEmployee_Id)
                .Index(t => t.Restaurant_Id)
                .Index(t => t.Vendor_Id);
            
            CreateTable(
                "dbo.ReceivingOrderBeverageLineItems",
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
                        LineItemPrice_Dollars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice_Dollars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ZeroedOut = c.Boolean(nullable: false),
                        BaseLineItem_DisplayName = c.String(),
                        BaseLineItem_MiseName = c.String(),
                        BaseLineItem_UPC = c.String(),
                        BaseLineItem_CaseSize = c.Int(),
                        BaseLineItem_Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        ReceivingOrder_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReceivingOrders", t => t.ReceivingOrder_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.ReceivingOrder_Id);
            
            CreateTable(
                "dbo.RestaurantAccounts",
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
                        BillingCycleDays = c.Int(nullable: false),
                        CurrentCard_FirstName = c.String(),
                        CurrentCard_LastName = c.String(),
                        CurrentCard_ProcessorToken_Processor = c.Int(nullable: false),
                        CurrentCard_ProcessorToken_Token = c.String(),
                        PaymentPlan = c.Int(nullable: false),
                        PaymentPlanSetupWithProvider = c.Boolean(nullable: false),
                        AccountType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        PrimaryEmail = c.String(),
                        AccountPasswordHash = c.String(),
                        AccountHolderFirstName = c.String(),
                        AccountHolderLastName = c.String(),
                        Emails = c.String(),
                        AccountPhoneNumberAreaCode = c.String(),
                        AccountPhoneNumber = c.String(),
                        ReferralCodeForAccountToGiveOut = c.String(),
                        ReferralCodeUsedToCreate = c.String(),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
            
            CreateTable(
                "dbo.AccountCharges",
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
                        App = c.Int(nullable: false),
                        AccountID = c.Guid(nullable: false),
                        Amount_Dollars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateStart = c.DateTimeOffset(nullable: false, precision: 7),
                        DateEnd = c.DateTimeOffset(nullable: false, precision: 7),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        RestaurantAccount_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RestaurantAccounts", t => t.RestaurantAccount_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.RestaurantAccount_Id);
            
            CreateTable(
                "dbo.AccountCreditCardPayments",
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
                        CardUsed_FirstName = c.String(),
                        CardUsed_LastName = c.String(),
                        CardUsed_ProcessorToken_Processor = c.Int(nullable: false),
                        CardUsed_ProcessorToken_Token = c.String(),
                        Status = c.Int(nullable: false),
                        AccountID = c.Guid(nullable: false),
                        Amount_Dollars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        RestaurantAccount_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RestaurantAccounts", t => t.RestaurantAccount_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.RestaurantAccount_Id);
            
            CreateTable(
                "dbo.AccountCredits",
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
                        ReferralCodeGiven_Code = c.String(),
                        AccountID = c.Guid(nullable: false),
                        Amount_Dollars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntityId = c.Guid(nullable: false),
                        Revision_AppInstanceCode = c.Int(nullable: false),
                        Revision_OrderingID = c.Long(nullable: false),
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
                        RestaurantAccount_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RestaurantAccounts", t => t.RestaurantAccount_Id)
                .Index(t => t.CreatedAt, clustered: true)
                .Index(t => t.RestaurantAccount_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountCredits", "RestaurantAccount_Id", "dbo.RestaurantAccounts");
            DropForeignKey("dbo.AccountCreditCardPayments", "RestaurantAccount_Id", "dbo.RestaurantAccounts");
            DropForeignKey("dbo.AccountCharges", "RestaurantAccount_Id", "dbo.RestaurantAccounts");
            DropForeignKey("dbo.ReceivingOrders", "Vendor_Id", "dbo.Vendors");
            DropForeignKey("dbo.ReceivingOrders", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.ReceivingOrders", "ReceivedByEmployee_Id", "dbo.Employees");
            DropForeignKey("dbo.ReceivingOrders", "PurchaseOrder_Id", "dbo.PurchaseOrders");
            DropForeignKey("dbo.ReceivingOrderBeverageLineItems", "ReceivingOrder_Id", "dbo.ReceivingOrders");
            DropForeignKey("dbo.EntityCategoryOwnerships", "ReceivingOrderBeverageLineItem_Id", "dbo.ReceivingOrderBeverageLineItems");
            DropForeignKey("dbo.PurchaseOrders", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.PurchaseOrderPerVendors", "PurchaseOrder_Id", "dbo.PurchaseOrders");
            DropForeignKey("dbo.PurchaseOrderPerVendors", "Vendor_Id", "dbo.Vendors");
            DropForeignKey("dbo.PurchaseOrderBeverageLineItems", "PurchaseOrderPerVendor_Id", "dbo.PurchaseOrderPerVendors");
            DropForeignKey("dbo.EntityCategoryOwnerships", "PurchaseOrderBeverageLineItem_Id", "dbo.PurchaseOrderBeverageLineItems");
            DropForeignKey("dbo.PurchaseOrders", "CreatedBy_Id", "dbo.Employees");
            DropForeignKey("dbo.Pars", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.ParBeverageLineItems", "Par_Id", "dbo.Pars");
            DropForeignKey("dbo.EntityCategoryOwnerships", "ParBeverageLineItem_Id", "dbo.ParBeverageLineItems");
            DropForeignKey("dbo.Pars", "CreatedByEmployee_Id", "dbo.Employees");
            DropForeignKey("dbo.InventorySections", "RestaurantInventorySection_Id", "dbo.RestaurantInventorySections");
            DropForeignKey("dbo.InventoryBeverageLineItems", "InventorySection_Id", "dbo.InventorySections");
            DropForeignKey("dbo.InventoryBeverageLineItems", "VendorBoughtFrom_Id", "dbo.Vendors");
            DropForeignKey("dbo.VendorPrivateRestaurantPrices", "VendorBeverageLineItem_Id", "dbo.VendorBeverageLineItems");
            DropForeignKey("dbo.VendorPrivateRestaurantPrices", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.VendorBeverageLineItems", "Vendor_Id", "dbo.Vendors");
            DropForeignKey("dbo.EntityCategoryOwnerships", "VendorBeverageLineItem_Id", "dbo.VendorBeverageLineItems");
            DropForeignKey("dbo.VendorRestaurantRelationships", "Vendor_Id", "dbo.Vendors");
            DropForeignKey("dbo.VendorRestaurantRelationships", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.Vendors", "CreatedBy_Id", "dbo.Employees");
            DropForeignKey("dbo.EntityCategoryOwnerships", "InventoryBeverageLineItem_Id", "dbo.InventoryBeverageLineItems");
            DropForeignKey("dbo.EntityCategoryOwnerships", "InventoryCategory_Id", "dbo.InventoryCategories");
            DropForeignKey("dbo.InventoryCategories", "ParentCategory_Id", "dbo.InventoryCategories");
            DropForeignKey("dbo.InventorySections", "LastCompletedBy_Id", "dbo.Employees");
            DropForeignKey("dbo.InventorySections", "Inventory_Id", "dbo.Inventories");
            DropForeignKey("dbo.InventorySections", "CurrentlyInUseBy_Id", "dbo.Employees");
            DropForeignKey("dbo.Inventories", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.Inventories", "CreatedByEmployee_Id", "dbo.Employees");
            DropForeignKey("dbo.ApplicationInvitations", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.ApplicationInvitations", "InvitingEmployee_Id", "dbo.Employees");
            DropForeignKey("dbo.ApplicationInvitations", "DestinationEmployee_Id", "dbo.Employees");
            DropForeignKey("dbo.EmployeeRestaurantRelationships", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.RestaurantApplicationUses", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.RestaurantApplicationUses", "MiseApplication_Id", "dbo.MiseApplications");
            DropForeignKey("dbo.RestaurantInventorySections", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.EmployeeRestaurantRelationships", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.ApplicationInvitations", "Application_Id", "dbo.MiseApplications");
            DropIndex("dbo.AccountCredits", new[] { "RestaurantAccount_Id" });
            DropIndex("dbo.AccountCredits", new[] { "CreatedAt" });
            DropIndex("dbo.AccountCreditCardPayments", new[] { "RestaurantAccount_Id" });
            DropIndex("dbo.AccountCreditCardPayments", new[] { "CreatedAt" });
            DropIndex("dbo.AccountCharges", new[] { "RestaurantAccount_Id" });
            DropIndex("dbo.AccountCharges", new[] { "CreatedAt" });
            DropIndex("dbo.RestaurantAccounts", new[] { "CreatedAt" });
            DropIndex("dbo.ReceivingOrderBeverageLineItems", new[] { "ReceivingOrder_Id" });
            DropIndex("dbo.ReceivingOrderBeverageLineItems", new[] { "CreatedAt" });
            DropIndex("dbo.ReceivingOrders", new[] { "Vendor_Id" });
            DropIndex("dbo.ReceivingOrders", new[] { "Restaurant_Id" });
            DropIndex("dbo.ReceivingOrders", new[] { "ReceivedByEmployee_Id" });
            DropIndex("dbo.ReceivingOrders", new[] { "PurchaseOrder_Id" });
            DropIndex("dbo.ReceivingOrders", new[] { "CreatedAt" });
            DropIndex("dbo.PurchaseOrderBeverageLineItems", new[] { "PurchaseOrderPerVendor_Id" });
            DropIndex("dbo.PurchaseOrderBeverageLineItems", new[] { "CreatedAt" });
            DropIndex("dbo.PurchaseOrderPerVendors", new[] { "PurchaseOrder_Id" });
            DropIndex("dbo.PurchaseOrderPerVendors", new[] { "Vendor_Id" });
            DropIndex("dbo.PurchaseOrderPerVendors", new[] { "CreatedAt" });
            DropIndex("dbo.PurchaseOrders", new[] { "Restaurant_Id" });
            DropIndex("dbo.PurchaseOrders", new[] { "CreatedBy_Id" });
            DropIndex("dbo.PurchaseOrders", new[] { "CreatedAt" });
            DropIndex("dbo.ParBeverageLineItems", new[] { "Par_Id" });
            DropIndex("dbo.ParBeverageLineItems", new[] { "CreatedAt" });
            DropIndex("dbo.Pars", new[] { "Restaurant_Id" });
            DropIndex("dbo.Pars", new[] { "CreatedByEmployee_Id" });
            DropIndex("dbo.Pars", new[] { "CreatedAt" });
            DropIndex("dbo.MiseEmployeeAccounts", new[] { "CreatedAt" });
            DropIndex("dbo.VendorPrivateRestaurantPrices", new[] { "VendorBeverageLineItem_Id" });
            DropIndex("dbo.VendorPrivateRestaurantPrices", new[] { "Restaurant_Id" });
            DropIndex("dbo.VendorPrivateRestaurantPrices", new[] { "CreatedAt" });
            DropIndex("dbo.VendorBeverageLineItems", new[] { "Vendor_Id" });
            DropIndex("dbo.VendorBeverageLineItems", new[] { "CreatedAt" });
            DropIndex("dbo.VendorRestaurantRelationships", new[] { "Vendor_Id" });
            DropIndex("dbo.VendorRestaurantRelationships", new[] { "Restaurant_Id" });
            DropIndex("dbo.VendorRestaurantRelationships", new[] { "CreatedAt" });
            DropIndex("dbo.Vendors", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Vendors", new[] { "CreatedAt" });
            DropIndex("dbo.InventoryCategories", new[] { "ParentCategory_Id" });
            DropIndex("dbo.InventoryCategories", new[] { "CreatedAt" });
            DropIndex("dbo.EntityCategoryOwnerships", new[] { "ReceivingOrderBeverageLineItem_Id" });
            DropIndex("dbo.EntityCategoryOwnerships", new[] { "PurchaseOrderBeverageLineItem_Id" });
            DropIndex("dbo.EntityCategoryOwnerships", new[] { "ParBeverageLineItem_Id" });
            DropIndex("dbo.EntityCategoryOwnerships", new[] { "VendorBeverageLineItem_Id" });
            DropIndex("dbo.EntityCategoryOwnerships", new[] { "InventoryBeverageLineItem_Id" });
            DropIndex("dbo.EntityCategoryOwnerships", new[] { "InventoryCategory_Id" });
            DropIndex("dbo.EntityCategoryOwnerships", new[] { "CreatedAt" });
            DropIndex("dbo.InventoryBeverageLineItems", new[] { "InventorySection_Id" });
            DropIndex("dbo.InventoryBeverageLineItems", new[] { "VendorBoughtFrom_Id" });
            DropIndex("dbo.InventoryBeverageLineItems", new[] { "CreatedAt" });
            DropIndex("dbo.InventorySections", new[] { "RestaurantInventorySection_Id" });
            DropIndex("dbo.InventorySections", new[] { "LastCompletedBy_Id" });
            DropIndex("dbo.InventorySections", new[] { "Inventory_Id" });
            DropIndex("dbo.InventorySections", new[] { "CurrentlyInUseBy_Id" });
            DropIndex("dbo.InventorySections", new[] { "CreatedAt" });
            DropIndex("dbo.Inventories", new[] { "Restaurant_Id" });
            DropIndex("dbo.Inventories", new[] { "CreatedByEmployee_Id" });
            DropIndex("dbo.Inventories", new[] { "CreatedAt" });
            DropIndex("dbo.RestaurantApplicationUses", new[] { "Restaurant_Id" });
            DropIndex("dbo.RestaurantApplicationUses", new[] { "MiseApplication_Id" });
            DropIndex("dbo.RestaurantApplicationUses", new[] { "CreatedAt" });
            DropIndex("dbo.RestaurantInventorySections", new[] { "Restaurant_Id" });
            DropIndex("dbo.RestaurantInventorySections", new[] { "CreatedAt" });
            DropIndex("dbo.Restaurants", new[] { "CreatedAt" });
            DropIndex("dbo.EmployeeRestaurantRelationships", new[] { "Restaurant_Id" });
            DropIndex("dbo.EmployeeRestaurantRelationships", new[] { "Employee_Id" });
            DropIndex("dbo.EmployeeRestaurantRelationships", new[] { "CreatedAt" });
            DropIndex("dbo.Employees", new[] { "CreatedAt" });
            DropIndex("dbo.MiseApplications", new[] { "CreatedAt" });
            DropIndex("dbo.ApplicationInvitations", new[] { "Restaurant_Id" });
            DropIndex("dbo.ApplicationInvitations", new[] { "InvitingEmployee_Id" });
            DropIndex("dbo.ApplicationInvitations", new[] { "DestinationEmployee_Id" });
            DropIndex("dbo.ApplicationInvitations", new[] { "Application_Id" });
            DropIndex("dbo.ApplicationInvitations", new[] { "CreatedAt" });
            DropTable("dbo.AccountCredits",
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
            DropTable("dbo.AccountCreditCardPayments",
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
            DropTable("dbo.AccountCharges",
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
            DropTable("dbo.RestaurantAccounts",
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
            DropTable("dbo.ReceivingOrderBeverageLineItems",
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
            DropTable("dbo.ReceivingOrders",
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
            DropTable("dbo.PurchaseOrderBeverageLineItems",
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
            DropTable("dbo.PurchaseOrderPerVendors",
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
            DropTable("dbo.PurchaseOrders",
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
            DropTable("dbo.ParBeverageLineItems",
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
            DropTable("dbo.Pars",
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
            DropTable("dbo.MiseEmployeeAccounts",
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
            DropTable("dbo.VendorPrivateRestaurantPrices",
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
            DropTable("dbo.VendorBeverageLineItems",
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
            DropTable("dbo.VendorRestaurantRelationships",
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
            DropTable("dbo.Vendors",
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
            DropTable("dbo.InventoryCategories",
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
            DropTable("dbo.EntityCategoryOwnerships",
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
            DropTable("dbo.InventoryBeverageLineItems",
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
            DropTable("dbo.InventorySections",
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
            DropTable("dbo.Inventories",
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
            DropTable("dbo.RestaurantApplicationUses",
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
            DropTable("dbo.RestaurantInventorySections",
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
            DropTable("dbo.Restaurants",
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
            DropTable("dbo.EmployeeRestaurantRelationships",
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
            DropTable("dbo.Employees",
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
            DropTable("dbo.MiseApplications",
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
            DropTable("dbo.ApplicationInvitations",
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
