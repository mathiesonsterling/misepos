using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;

using Mise.Core.Common.Entities;
using Mise.Core.Services.WebServices;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Entities.Inventory.Events;

using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;

namespace Mise.Core.Common.Services.Implementation.FakeServices
{
	public class FakeInventoryWebService: IInventoryApplicationWebService
	{
		private const MiseAppTypes FAKE_APP_CODE = MiseAppTypes.DummyData;
		private readonly List<IEmployee> _emps;
	    readonly List<IRestaurant> _restaurants;
	    readonly List<IVendor> _vendors;
	    readonly List<IPAR> _pars;
	    readonly List<IInventory> _inventories;
	    readonly List<IReceivingOrder> _receivingOrders;

	    private readonly List<IApplicationInvitation> _invitations; 

	    #region IInventoryEmployeeWebService implementation
		public FakeInventoryWebService(){
		    var restID = Guid.Empty;
            var townsendID = Guid.Parse("c7c61794-03db-4e20-a87e-8483f4960286");
		    var qaID = Guid.Parse("d6d3ae32-4b28-4e8d-b88d-94edf9958030");
			var marketingID = Guid.Parse ("441b0100-9a33-41dc-bb18-e8f74292a81d");
			var accountID = Guid.NewGuid();
			var section1ID = Guid.NewGuid();
		

			_restaurants = CreateRestaurants (restID, accountID, section1ID, townsendID, qaID, marketingID);
			_emps = CreateEmployees (restID, townsendID, qaID, marketingID);
			_vendors = CreateVendors (restID, marketingID);

			var empID = _emps.First ().ID;
			_pars = CreatePars (restID, empID);
			_inventories = CreateInventories ();

			var purchaseOrders = CreatePurchaseOrders (restID, empID, _vendors.First().ID);
			_receivingOrders = CreateReceivingOrders (restID, empID, purchaseOrders.FirstOrDefault().ID);

		    _invitations = CreateInvitations();
		}
			

	    static List<IRestaurant> CreateRestaurants(Guid restID, Guid accountID, Guid section1ID, 
			Guid townsendID, Guid qaID, Guid marketingID){
			return new List<IRestaurant>
			{
				new Restaurant
				{
					AccountID = accountID,
					Name = new RestaurantName("MainTestRestaurant"),
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 2},
					ID = restID,
					InventorySections = new List<RestaurantInventorySection>
					{
						new RestaurantInventorySection
						{
							ID = Guid.NewGuid(),
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 2},
							Name="Stock Room",
							RestaurantID = restID
						}
					},
					StreetAddress = new StreetAddress
					{
						StreetAddressNumber = new StreetAddressNumber{ Number = "699"},
						Street = new Street{Name = "Graham"},
						City = new City{Name = "Queens"},
						State = new State{Name = "New York"},
						Country = Country.UnitedStates,
						Zip = new ZipCode{Value = "11425"}
					}
				},
				new Restaurant
				{
					AccountID = null,
					Name = new RestaurantName("The Townsend", "Townsend"),
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
					ID = townsendID,
                    InventorySections = new List<RestaurantInventorySection>
                    {
						new RestaurantInventorySection
						{
							ID = Guid.NewGuid(),
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 2},
							Name="Stock Room",
							RestaurantID = townsendID
						}
                    },
                    StreetAddress = new StreetAddress("718", "", "Congress Ave", "Austin", "Texas", Country.UnitedStates.Name, "78701"),
                    PhoneNumber = new PhoneNumber("512", "887-8778"),
				},
                new Restaurant
                {
                    AccountID = null,
                    Name = new RestaurantName("Antidote", "Antidote"),
                    Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 102},
                    ID = qaID,
                    InventorySections = new List<RestaurantInventorySection>(),
                    StreetAddress = new StreetAddress("1160", "", "Bismark Drive", "Campbell", "California", Country.UnitedStates.Name, "95008"),
                    PhoneNumber = new PhoneNumber("111", "222-3344")
                },
				new Restaurant {
					AccountID = null,
					Name = new RestaurantName("Couch Party USA", "Couch Party"),
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1020},
					ID = marketingID,
					InventorySections = new List<RestaurantInventorySection>{
						new RestaurantInventorySection
						{
							ID = Guid.NewGuid(),
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 2},
							Name="Stock Room",
							RestaurantID = marketingID
						}
					},
					StreetAddress = new StreetAddress("666", "" , "Angora St", "Dallas", "Texas", 
						Country.UnitedStates.Name, "75218"),
					PhoneNumber = new PhoneNumber("512", "1112222")
				}
			};
		}

		static List<IEmployee> CreateEmployees(Guid restID, Guid townsendID, Guid qaID, Guid marketingID)
		{
            //list for our generic test restaurant
			var testRestaurantList = new Dictionary<Guid, IList<MiseAppTypes>>
			{
			    {restID, new List<MiseAppTypes> {MiseAppTypes.StockboyMobile, MiseAppTypes.DummyData}},
			};
		    return new List<IEmployee>
			{
                new Employee
                {
                    ID = Guid.NewGuid(),
                    Name = new PersonName("QA", "Tester"),
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>
                    {
                        {qaID, new[]{MiseAppTypes.StockboyMobile }}
                    },
                    Password = new Password("tester"),
                    PrimaryEmail = new EmailAddress("qa@misepos.com"),
                    Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 12}
                },
                new Employee
                {
                    ID = Guid.NewGuid(),
                    Name = new PersonName("Justin", "Elliott"),
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>
                    {
                       {townsendID, new List<MiseAppTypes>{MiseAppTypes.StockboyMobile}}
                    },
                    Password = new Password("honestgirl"),
                    PrimaryEmail = new EmailAddress("justin.elliott@thetownsendaustin.com"),
                    Emails = new List<EmailAddress>{new EmailAddress("justin.elliott@thetownsendaustin.com")},
                    Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 10}
                },
				new Employee {
					ID = Guid.NewGuid (),
					Name = new PersonName("Emily", "Perkins"),
					CreatedDate = DateTime.UtcNow,
					LastUpdatedDate = DateTime.UtcNow,
					Password = new Password("shiftdrink"),
					PrimaryEmail = new EmailAddress("emily@misepos.com"),
					RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
						{marketingID, new List<MiseAppTypes>{MiseAppTypes.StockboyMobile}}
					},
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 18}
				},
				new Employee
				{
					ID = Guid.NewGuid(),
                    Name = new PersonName("Mise", "Development"),
                    Passcode = "1111",
					CompBudget = new Money(50.0M),
					WhenICanVoid =
						new List<OrderItemStatus> {OrderItemStatus.Ordering, OrderItemStatus.Added, OrderItemStatus.Sent},
					CreatedDate = DateTime.UtcNow,
					LastUpdatedDate = DateTime.UtcNow,
					RestaurantsAndAppsAllowed =  new Dictionary<Guid, IList<MiseAppTypes>>
			        {
			            {restID, new List<MiseAppTypes> {MiseAppTypes.StockboyMobile, MiseAppTypes.DummyData}},
				        {qaID, new List<MiseAppTypes>{MiseAppTypes.StockboyMobile}},
				        {marketingID, new List<MiseAppTypes>{MiseAppTypes.StockboyMobile}}
			        },
					Revision = new EventID {AppInstanceCode = FAKE_APP_CODE, OrderingID = 100},
					Password = new Password("dev"),
                    PrimaryEmail = new EmailAddress("dev@misepos.com"),
                    Emails = new[]{new EmailAddress("dev@misepos.com"), new EmailAddress("developer@misepos.com")  }
				},
				new Employee {
					PrimaryEmail = new EmailAddress{Value = "test@misepos.com"},
					Emails = new List<EmailAddress>{ new EmailAddress{Value = "test@misepos.com"} },
					Password = new Password("test"),
					ID = Guid.NewGuid(),
                    Name = new PersonName("Testy", "McTesterson"),
					RestaurantsAndAppsAllowed = testRestaurantList,
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 120201}
				},
			};
		}

		List<IVendor> CreateVendors(Guid restID, Guid marketingID){
			var ml750 = new LiquidContainer {
				AmountContained = new LiquidAmount{ Milliliters = 750 },
			};
			var ml118 = new LiquidContainer {
				AmountContained = new LiquidAmount{ Milliliters = 118 }
			};

            //TODO replace these with standard sizes from LiquidContainer!
			var oz30 = new LiquidContainer { DisplayName = "30 oz",AmountContained = new LiquidAmount{ Milliliters = 887.206M } };
			var ml375 = new LiquidContainer { AmountContained = new LiquidAmount{ Milliliters = 375 } };
			var ml20 = new LiquidContainer { AmountContained = new LiquidAmount{ Milliliters = 20 } };
			var l175 = new LiquidContainer { DisplayName = "1.75 L", AmountContained = new LiquidAmount{ Milliliters = 1750 } };

		    var favoriteID = Guid.NewGuid();
			var vendors =  new List<Vendor>
			{
				new Vendor
				{
					ID = Guid.NewGuid(),
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
					Name = "Bob's Liquor Barn",
					VendorBeverageLineItems = new List<VendorBeverageLineItem>(),
					StreetAddress = new StreetAddress
					{
						StreetAddressNumber = new StreetAddressNumber{Direction = "N", Number = "6021"},
						Street = new Street{Name = "Ocean"},
						City = new City{Name = "Brooklyn"},
						State = new State{Name = "New York"},
						Country = Country.UnitedStates,
						Zip = new ZipCode{Value = "11226"}
					},
					RestaurantsAssociatedIDs = new List<Guid>{restID}
				},
				new Vendor 
				{
					ID = Guid.NewGuid (),
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 100},
					Name = "Siegal's",
					VendorBeverageLineItems = new List<VendorBeverageLineItem>{
						new VendorBeverageLineItem {
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Mount Gay Black Barrel",
							MiseName = "mountgayblackbarrel750ml",
							Container = ml750,
							Categories = new List<ItemCategory>{
								CategoriesService.Rum
							}
						},
						new VendorBeverageLineItem{
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Cointreau",
							MiseName = "cointreau750ml",
							Container = ml750,
							Categories = new List<ItemCategory>{
								CategoriesService.Liquer
							}
						},
						new VendorBeverageLineItem {
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Bruichladdich Port Charlotte SB",
							MiseName = "bruichladdichportcharlottesb",
							Container = ml750,
							Categories = new List<ItemCategory>{
								CategoriesService.WhiskeyScotch
							}
						},
						new VendorBeverageLineItem {
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Pimms",
							MiseName = "pimms750ml",
							Container = ml750,
							Categories = new List<ItemCategory>{
								CategoriesService.Liquer
							}
						},
						new VendorBeverageLineItem{
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Botanist Gin",
							MiseName = "botanistgin750ml",
							Container = ml750,
							Categories = new List<ItemCategory>{
								CategoriesService.Gin
							}
						},
						new VendorBeverageLineItem {
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Angostura Bitters",
							MiseName = "angosturabitters118ml",
							Container = ml118,
							Categories = new List<ItemCategory>{
								CategoriesService.NonAlcoholic
							}
						},
						new VendorBeverageLineItem {
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Cointreau Guignolet",
							MiseName = "cointreauguignolet375ml",
							Container = ml375,
							Categories = new List<ItemCategory>{
								CategoriesService.Liquer
							}
						},
						new VendorBeverageLineItem {
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Old Granddad 80 Proof",
							MiseName = "oldgranddad80proof1750ml",
							Container = l175,
							Categories = new List<ItemCategory>{
								CategoriesService.WhiskeyAmerican
							}
						}
					},
					StreetAddress = new StreetAddress("2800", "", "Routh St", "Dallas", "TX",
						Country.UnitedStates.Name, "75201"),
					RestaurantsAssociatedIDs = new List<Guid>{marketingID}
				},
				new Vendor{
					ID = Guid.NewGuid (),
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 100},
					Name = "FreshPoint",
					VendorBeverageLineItems = new List<VendorBeverageLineItem>{
						new VendorBeverageLineItem {
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 1,
							ID = Guid.NewGuid (),
							DisplayName = "Perfect Puree Passionfruit",
							MiseName = "perfectpureepassionfruit30oz",
							Container = oz30,
							Categories = new List<ItemCategory>{
								CategoriesService.NonAlcoholic
							}
						}
					},
					StreetAddress = new StreetAddress("4721", "", "Simonton Rd", "Dallas", "TX", 
						Country.UnitedStates.Name, "75244"),
					RestaurantsAssociatedIDs = new List<Guid>{marketingID}
				},
				new Vendor{
					ID = favoriteID,
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 100},
					Name = "Favorite Brands",
					//13755 Diplomat Dr #100
					//Farmers Branch, TX 75234
					StreetAddress = new StreetAddress("13755", "", "Diplomat Dr", "Farmers Branch",
						"TX", Country.UnitedStates.Name, "75234"),
					RestaurantsAssociatedIDs = new List<Guid>{marketingID},
					VendorBeverageLineItems = new List<VendorBeverageLineItem>{
						new VendorBeverageLineItem {
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							CaseSize = 12,
							ID = Guid.NewGuid (),
							DisplayName = "Underberg",
							Container = ml20,
                            VendorID = favoriteID,
							Categories = new List<ItemCategory>{
								CategoriesService.Liquer
							}
						}
					}
				}
			};

            //hack
		    foreach (var v in vendors)
		    {
		        foreach (var li in v.VendorBeverageLineItems)
		        {
		            li.VendorID = v.ID;
		        }
		    }

		    return vendors.ToList<IVendor>();
		}
			
		List<IPAR> CreatePars(Guid restID, Guid empID){
			var container = new LiquidContainer { AmountContained = new LiquidAmount{ Milliliters = 750 } };
			var lis750 = new Dictionary<string, string> {

				{"Ayelsbury Duck","1"},
				{"Boyd & Blair","1"},
				{"Tito's","1"},
				{"Tenneyson","2"},
				{"Vieux Pontralier","2"},
				{"Linie Aquavit","2"},
				{"Bols Genever","2"},
				{"Botanist Gin","2"},
				{"Ford's Gin","2"},
				{"Genius 'Naval Strength'","2"},
				{"Hayman's Old Tom","2"},
				{"Plymouth","2"}, 
				{"Reisetbauer Blue","2"},
				{"Ricard Pastis","2"},
				{"La Favorite Coeur de Canne Blanc","3"},
				{"Neisson Eleve Sous Bois","3"},
				{"Bacardi 151","3"},
				{"Balcones Rumble","3"},
				{"Batavia Arrack","3"},
				{"Cana Brava","3"},
				{"El Dorado 12 yr","3"},
				{"El Dorado 5 yr","3"},
				{"Hamilton Dark Jamaican","3"},
				{"Lemon Hart 151","3"},
				{"Mount Gay Black Brl","3"},
				{"Mount Gay XO","3"},
				{"Smith & Cross","3"},
				{"Alipus San Balthazar","4"},
				{"Alipus Santa Ana","4"},
				{"Del Maguey Iberico","4"},
				{"Del Maguey Minero","4"},
				{"Del Maguey Tepextate","4"},
				{"Mezcal Vago Elote","4"},
				{"Mezcal Vago Cuixe","4"},
				{"Hacienda del Chihuahua Plata","4"},
				{"Siembra Azul Repo","4"},
				{"Siembra Azul Anejo","4"},
				{"Siete Leguas Anejo","4"},
				{"Tapatio","4"},
				{"Tequila Ocho Reposado","4"},
				{"Booker's","5"},
				{"E.H. Taylor Single","5"},
				{"E.H. Taylor Uncut","5"},
				{"Garrison Bros","5"},
				{"Old Forester","5"},
				{"Old Grand-dad","5"},
				{"Old Weller 107,","5"},
				{"Russell's Reserve","5"},
				{"Very Old  Barton","5"},
				{"Jameson","5"},
				{"Hakushu 12 yr","5"},
				{"Hibiki 12 yr","5"},
				{"Kiuchi No Shizuku","5"},
				{"Nikka 12 yr 'Miyagikyo'","5"},
				{"Nikka 12 yr 'Taketsuru'","5"},
				{"Nikka 17 yr 'Taketsuru'","5"},
				{"Nikka 21 yr 'Taketsuru'","5"},
				{"Nikka 15 yr 'Yoichi'","5"},
				{"Nikka Coffey Still","5"},
				{"Yamazaki 12 yr","5"},
				{"Yamazaki 18 yr","5"},
				{"Angel's Envy Rye","5"},
				{"Dickel Rye","5"},
				{"Rittenhouse Rye","5"},
				{"Russell's Reserve Rye","5"},
				{"Whistle Pig Boss Hog","5"},
				{"Wild Turkey Rye","5"},
				{"Willett 4yr","5"},
				{"Black Grouse","5"},
				{"Bruichladdich Octomore","5"},
				{"Laphroaig 1/4 cask","5"},
				{"Springbank 15 yr","5"},
				{"Yokaichi Barley","5"},
				{"Honkaku Sweet Pot","5"},
				{"Navazos Malt","5"},
				{"Navazos Grain","5"}, 
				{"Clear Creek Apple Brandy","6"},
				{"Laird's Apple Brandy","6"},
				{"Marie Duffau 'Napoleon'","6"},
				{"Remy 1738","6"},
				{"Dudognan Reserve","6"},
				{"Kinsman Apricot","6"},
				{"Reisetbauer Ginger","6"},
				{"Reisetbauer Pear","6"},
				{"St. Remy","6"},
				{"Barsol Pisco Primera","6"},
				{"Benedictine","7"},
				{"Camomille","7"},
				{"Cointreau","7"},
				{"Combier Pomplemousse","7"},
				{"Combier Rouge","7"},
				{"Drambuie","7"},
				{"Dolin Genepy","7"},
				{"Galliano","7"},
				{"Giffard Banane","7"},
				{"Giffard Cassis","7"},
				{"Giffard Ginger","7"},
				{"Giffard Lichi-Li","7"},
				{"Giffard Menthe","7"},
				{"Green Chartreuse","7"},
				{"Guignolet","7"},
				{"Hayman's Sloe Gin","7"},
				{"Luxardo Maraschino","7"},
				{"Metaxa","7"},
				{"PF Dry Curacao","7"},
				{"Pimm's","7"},
				{"R&W Crème de Violette","7"},
				{"R&W Orchard Apricot","7"},
				{"R&W Orchard Cherry","7"},
				{"St. Elizabeth Allspice Dram","7"},
				{"St. Germain","7"},
				{"Tempus Fugit Crème de Cacao","7"},
				{"Tempus Fugit Crème de Menthe","7"},
				{"Trenel Crème de Cassis","7"},
				{"Trenel Crème de Mure","7"},
				{"Trenel Peche","7"},
				{"Yellow Chartreuse","7"},
				{"Amaro Montenegro","8"},
				{"Amaro Nonino","8"},
				{"Amaro Sibilia","8"},
				{"Aperol","8"},
				{"Bigallet China China","8"},
				{"Bittermens Commonwealth","8"},
				{"Branca Menta","8"},
				{"Braulio","8"},
				{"Campari","8"},
				{"Cynar","8"},
				{"Fernet Branca","8"},
				{"Gran Classico","8"},
				{"Nardini Bassano","8"},
				{"Zucca","8"},
				{"Valdespino 'El Candado'","9"},
				{"Valdespino 'Tio Diego'","9"},
				{"Lustau Amontillado","9"},
				{"Ragnaud Pineau","9"},
				{"Cocchi Americano","9"},
				{"Cocchi Americano Rosa","9"},
				{"Cocchi Vermouth di Turino","9"},
				{"Dolin Blanc","9"},
				{"Dolin Dry","9"},
				{"Perucchi Red","9"},
				{"Punt E Mes","9"}, 

			};
				
			var full750Items = lis750.Keys.Select (key => 
				new PARBeverageLineItem { 
					RestaurantID = restID,
					CaseSize = null,
					Container = container,
					DisplayName = key,
					ID = Guid.NewGuid (),
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
					MiseName = key + "750ml",
					Quantity = int.Parse (lis750[key]),
					Categories = new List<ItemCategory>{
						CategoriesService.Unknown
					}
				});


			var pars= new List<IPAR>
			{
				new PAR
				{
					Revision = new EventID {AppInstanceCode = FAKE_APP_CODE, OrderingID = 100},
					CreatedByEmployeeID = empID,
					CreatedDate = DateTime.Now,
					ID = Guid.NewGuid(),
					RestaurantID = restID,
					IsCurrent = true,
					LastUpdatedDate = DateTime.UtcNow,
					ParLineItems = full750Items.ToList ()
				}
			};

			return pars;
		}

		static IEnumerable<IPurchaseOrder> CreatePurchaseOrders(Guid restID, Guid empID, Guid vendorID){
			return new List<IPurchaseOrder>
			{
				new PurchaseOrder
				{
					ID = Guid.NewGuid(),
					CreatedDate = DateTimeOffset.UtcNow,
					CreatedByEmployeeID = empID,
					RestaurantID = restID,
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
					PurchaseOrdersPerVendor = new List<PurchaseOrderPerVendor>{
						new PurchaseOrderPerVendor {
							Status = PurchaseOrderStatus.Created,
							LineItems = new List<PurchaseOrderLineItem>
							{
								new PurchaseOrderLineItem
								{
									ID = Guid.NewGuid(),
									Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 10101},
									UPC = "1112j2jdjjdsss",
									MiseName = "testPOLineItem",
									CreatedDate = DateTimeOffset.UtcNow.AddDays(-2),
									VendorID = vendorID,
									CaseSize = 12,
									Container = new LiquidContainer
									{
										AmountContained = new LiquidAmount{Milliliters = 192, SpecificGravity = .08M},
										WeightEmpty = new Weight{Grams = 102},
										WeightFull = new Weight{Grams = 183}

									},
									Categories = new List<ItemCategory>{
										CategoriesService.Unknown
									}
								}
							}
						}
					}
				}
			};
		}

		static List<IReceivingOrder> CreateReceivingOrders(Guid restID, Guid empID, Guid? poID){
			return new List<IReceivingOrder>
			{
				new ReceivingOrder
				{
					ID = Guid.NewGuid(),
					Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
					CreatedDate = DateTimeOffset.UtcNow,
					Status = ReceivingOrderStatus.Created,
					LastUpdatedDate = DateTimeOffset.UtcNow,
					RestaurantID = restID,
					Notes = "this is a test RO, enjoy!",
					ReceivedByEmployeeID = empID,
					PurchaseOrderID = poID,
					LineItems = new List<ReceivingOrderLineItem>
					{
						new ReceivingOrderLineItem
						{
							ID = Guid.NewGuid(),
							Revision = new EventID{AppInstanceCode = FAKE_APP_CODE, OrderingID = 1},
							RestaurantID = restID,
							CaseSize = null,
							Container = new LiquidContainer
							{
								AmountContained = new LiquidAmount {Milliliters = 375}
							},
							DisplayName = "Wild Turkey 101",
							MiseName = "wildturkey101kentuckybourbon750ml",
							LineItemPrice =  new Money(10.0M),
							Quantity = 100,
							Categories = new List<ItemCategory>{CategoriesService.WhiskeyAmerican}
						}
					}
				}
			};
		}

		static List<IInventory> CreateInventories(){
			return new List<IInventory> ();
		}

		public Task<IEnumerable<IEmployee>> GetEmployeesAsync()
		{
			return Task.FromResult(_emps.AsEnumerable());
		}

		public Task<IEnumerable<IEmployee>> GetEmployeesForRestaurant(Guid restaurantID)
		{
			return Task.FromResult(_emps.Where(e => e.GetRestaurantIDs().Contains(restaurantID)));
		}

		public Task<IEmployee> GetEmployeeByPrimaryEmailAndPassword(EmailAddress email, Password password)
		{
			if (password == null || email == null) {
				return Task.FromResult (null as IEmployee);
			}
			var emp = _emps.FirstOrDefault (e => e.PrimaryEmail != null && e.PrimaryEmail.Value == email.Value && e.Password != null && password.HashValue == e.Password.HashValue);
			return Task.FromResult (emp);
		}

		#endregion

		public Task<IEnumerable<IRestaurant>> GetRestaurants (Location deviceLocation)
		{
			return Task.FromResult (_restaurants.AsEnumerable());
		}

		public Task<IRestaurant> GetRestaurant (Guid restaurantID)
		{
			return Task.FromResult (_restaurants.FirstOrDefault (r => r.ID == restaurantID));
		}

		#region IEventStoreWebService implementation

		public Task<bool> SendEventsAsync(IEmployee emp, IEnumerable<IEmployeeEvent> events)
		{
			return Task.FromResult (true);
		}

		public Task<bool> SendEventsAsync(IRestaurant rest, IEnumerable<IRestaurantEvent> events)
		{
			return Task.FromResult (true);
		}

		public Task<bool> SendEventsAsync(IVendor vendor, IEnumerable<IVendorEvent> events)
		{
			return Task.FromResult (true);
		}
		public Task<bool> SendEventsAsync(IPurchaseOrder po, IEnumerable<IPurchaseOrderEvent> events)
		{
			return Task.FromResult (true);
		}
		public Task<bool> SendEventsAsync(IInventory inv, IEnumerable<IInventoryEvent> events)
		{
			return Task.FromResult (true);
		}
		public Task<bool> SendEventsAsync(IReceivingOrder ro, IEnumerable<IReceivingOrderEvent> events)
		{
			return Task.FromResult (true);
		}
		public Task<bool> SendEventsAsync(IPAR par, IEnumerable<IPAREvent> events)
		{
			return Task.FromResult (true);
		}

        public Task<bool> SendEventsAsync(IApplicationInvitation invite, IEnumerable<IApplicationInvitationEvent> events)
        {
            return Task.FromResult(true);
        }

		public Task<bool> SendEventsAsync(IAccount account, IEnumerable<IAccountEvent> events)
        {
            return Task.FromResult(true);
        }
		#endregion

		public Task<IEnumerable<IVendor>> GetVendorsWithinSearchRadius (Location currentLocation, Distance radius)
		{
			return Task.FromResult (_vendors.AsEnumerable());
		}

		public Task<IEnumerable<IVendor>> GetVendorsBySearchString (string searchString)
		{
			return Task.FromResult (_vendors.Where (v => v.Name.Contains (searchString)));
		}

		public Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant (Guid restaurantID)
		{
			var res = new List<IVendor> ();
			foreach(var v in _vendors){
				var restIds = v.GetRestaurantIDsAssociatedWithVendor ();
				if(restIds.Contains (restaurantID)){
					res.Add (v);
				}
			}

			return Task.FromResult (res.AsEnumerable ());
		}

		public Task<IPAR> GetCurrentPAR (Guid restaurantID)
		{
			return Task.FromResult (_pars.FirstOrDefault (p => p.RestaurantID == restaurantID && p.IsCurrent));
		}

		public Task<IEnumerable<IPAR>> GetPARsForRestaurant (Guid restaurantID)
		{
			return Task.FromResult (_pars.Where (p => p.RestaurantID == restaurantID));
		}

		public Task<IEnumerable<IInventory>> GetInventoriesForRestaurant (Guid restaurantID)
		{
			return Task.FromResult (_inventories.Where (i => i.RestaurantID == restaurantID));
		}

		public Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersForRestaurant (Guid restaurantID)
		{
			return Task.FromResult (_receivingOrders.Where (ro => ro.RestaurantID == restaurantID));
		}

        private List<IApplicationInvitation> CreateInvitations()
        {
            var fromEmp = _emps.FirstOrDefault();

            var rest = _restaurants.FirstOrDefault(r => r.Name.FullName == "MainTestRestaurant");
            if (fromEmp != null && rest != null)
            {
                var invite = new ApplicationInvitation
                {
                    ID = Guid.NewGuid(),
                    DestinationEmail = new EmailAddress { Value = "joe@test.com" },
                    InvitingEmployeeID = fromEmp.ID,
                    InvitingEmployeeName = fromEmp.Name,
                    RestaurantID = rest.ID,
                    RestaurantName = rest.Name,
                    Application = MiseAppTypes.StockboyMobile,
                    Revision = new EventID { AppInstanceCode = FAKE_APP_CODE, OrderingID = 10203 },
                };
                var list = new List<IApplicationInvitation> {
					invite
				};

                return list;
            }

            return new List<IApplicationInvitation>();
        }

	    public Task<IEnumerable<IApplicationInvitation>> GetInvitationsForRestaurant(Guid restaurantID)
	    {
	        return Task.FromResult(_invitations.Where(i => i.RestaurantID == restaurantID));
	    }

	    public Task<IEnumerable<IApplicationInvitation>> GetInvitationsForEmail(EmailAddress email)
	    {
	        return Task.FromResult(_invitations.Where(i => i.DestinationEmail.Equals(email)));
	    }

	}
}

