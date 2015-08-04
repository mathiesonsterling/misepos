using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;
using Mise.Core.Common.Entities.MenuItems;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.Check.Events;

namespace Mise.Core.Common.Services.Implementation
{
	public class FakeDomineesRestaurantServiceClient : IRestaurantTerminalService
    {
        readonly IList<Employee> _employees;
        readonly IList<RestaurantCheck> _tabs;
        readonly Guid _topCategoryID = Guid.NewGuid();
        /// <summary>
        /// Public to allow generation off it
        /// </summary>
        public readonly Menu Menu;

        private static readonly Guid RestaurantID = Guid.NewGuid();
		private static readonly Uri restServLoc = new Uri ("http://mise.com");
        private static readonly Guid AccountID = Guid.NewGuid();

        public static List<Employee> CreateEmployees()
        {
			var restaurantList = new Dictionary<Guid, IList<MiseAppTypes>> ();
			restaurantList.Add (RestaurantID, new List<MiseAppTypes>{ MiseAppTypes.DummyData });
            return new List<Employee> {
				new Employee {
					ID = Guid.NewGuid (),
                    Name = new PersonName("Leo","Dominee"),
					Passcode = "1111",
					CompBudget = new Money(50.0M),
					WhenICanVoid = new List<OrderItemStatus>{OrderItemStatus.Ordering, OrderItemStatus.Added, OrderItemStatus.Sent},
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    RestaurantsAndAppsAllowed = restaurantList,
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100}
				},
				new Employee {
					ID = Guid.NewGuid (),
					Passcode = "4444",
                    Name = new PersonName("Justin","Elliott"),
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
					RestaurantsAndAppsAllowed = restaurantList,
                    PrimaryEmail = new EmailAddress{Value = "justin.elliott@misepos.com"},
                    Emails = new List<EmailAddress>{new EmailAddress{Value = "justin.elliott@misepos.com"}, new EmailAddress{Value="jdanelliott@gmail.com "}},
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 109}

				},
                new Employee
                {
                    ID = Guid.NewGuid(),
                    Passcode = "6666",
                    Name = new PersonName("Mathieson", "Sterling"),
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
					RestaurantsAndAppsAllowed = restaurantList,
                    Emails = new List<EmailAddress>{new EmailAddress{Value="mathieson@misepos.com"}},
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1001}
                }
			};
        }

        public FakeDomineesRestaurantServiceClient()
        {
            _employees = CreateEmployees();

            var kitchenDest = new OrderDestination
            {
                Name = "kitchen"
            };

            var foodDests = new List<OrderDestination> { kitchenDest };

            var basicMods = new MenuItemModifierGroup
            {
                ID = Guid.NewGuid(),
                Required = false,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101 },
                Exclusive = true,
                Modifiers = new List<MenuItemModifier> {
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Up"},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Neat"},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Rocks"},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Shot"},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Tall"},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Double", PriceMultiplier = 1.5M}
				}
            };

            var mixerMods = new MenuItemModifierGroup
            {
                ID = Guid.NewGuid(),
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101 },
                DisplayName = "Mixers",
                Required = false,
                Exclusive = true,
                Modifiers = new List<MenuItemModifier> {
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Soda", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Tonic", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Ginger", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Cola", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "LemonLime", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Diet", PriceChange = new Money(1.0M)},

					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Cranberry", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "O.J.", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Grapefruit", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Pineapple", PriceChange = new Money(1.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Sour", PriceChange = new Money(2.0M)},

					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Collins", PriceChange = new Money(2.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (),Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101}, Name = "Mule", PriceChange = new Money(2.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Bloody", PriceChange = new Money(2.0M)},
					new MenuItemModifier{ID = Guid.NewGuid (),Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101}, Name = "RedBull", PriceChange = new Money(3.0M)},
				}
            };

            var garnishMods = new MenuItemModifierGroup
            {
                ID = Guid.NewGuid(),
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101 },
                DisplayName = "Garnish",
                Required = false,
                Exclusive = false,
                Modifiers = new List<MenuItemModifier>{
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Lime Wedge"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Lemon Wedge"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Orange Slice"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Lemon Peel"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Orange Peel"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Olives"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Onion"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Mint"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Cucumber"},
					new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Cherry"},
				}
            };
            //make our modifiers for each liquor group please!
            var whiskeyMods = new List<MenuItemModifierGroup>{
				basicMods,
				mixerMods,
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Cocktails",
					Required = false,
					Exclusive = true,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier {ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Manhattan", PriceChange = new Money(3.00M)},
						new MenuItemModifier {ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Old Fashioned", PriceChange = new Money(3.00M)},
					}
				},
				garnishMods,
			};
            var vodkaMods = new List<MenuItemModifierGroup>{
				basicMods,
				mixerMods,
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Cocktails",
					Required = false,
					Exclusive = true,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Cosmo", PriceChange = new Money(3.00M)},
						new MenuItemModifier{ID=Guid.NewGuid (),Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101}, Name = "Martini", PriceChange = new Money(3.00M)},
					},
				},
				garnishMods,
			};
            var ginMods = new List<MenuItemModifierGroup> {
				basicMods,
				mixerMods,
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Cocktails",
					Required = false,
					Exclusive = true,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Martini", PriceChange = new Money(3.00M)}
					},
				},
				garnishMods,
			};

            var tequilaMods = new List<MenuItemModifierGroup> {
				basicMods,
				mixerMods,
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Cocktails",
					Required = false,
					Exclusive = true,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (),Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101}, Name = "Margarita", PriceChange = new Money(3.00M)},
						new MenuItemModifier{ID=Guid.NewGuid (),Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101}, Name = "Gimlet", PriceChange = new Money(3.00M)},
					},
				},
				garnishMods,
			};

            var rumMods = new List<MenuItemModifierGroup> {
				basicMods,
				mixerMods,
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Cocktails",
					Required = false,
					Exclusive = true,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Daquiri", PriceChange = new Money(3.00M)},
					}
				},
				garnishMods,
			};

            var burgerMods = new List<MenuItemModifierGroup>{
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Temperature",
					Exclusive = true,
					Required = true,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Rare"},
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Medium Rare"},
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Medium"},
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name= "Medium Well"},
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Well Done"}
					}
				},
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Cheese",
					Exclusive = false,
					Required = false,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="American", PriceChange = new Money(1M)},
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Cheddar", PriceChange = new Money(1M)},
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Blue", PriceChange = new Money(1M)}
					}
				},
				new MenuItemModifierGroup{
                    ID = Guid.NewGuid(),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "burgercondiments",
					Exclusive = false,
					Required = false,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (),Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101}, Name="Mayo"},
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Ketchup"}
					}
				},
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Extra",
					Exclusive = false,
					Required = false,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Bacon", PriceChange = new Money(1.0M)},
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Avocado", PriceChange = new Money(1.0M)}
					}
				}
			};

            var saladMods = new List<MenuItemModifierGroup> {
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Dressing",
					Exclusive = true,
					Required = true,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Ranch"}
					}
				},
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Add Protein",
					Exclusive = false,
					Required = false,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Add Chicken", PriceChange = new Money(5.0M)},
						new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Add Shrimp", PriceChange = new Money(5.0M)}
					}
				}
			};

            var quesadillaMods = new List<MenuItemModifierGroup> {
				new MenuItemModifierGroup{
						ID = Guid.NewGuid (),
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
						DisplayName = "Protein",
						Exclusive = true,
						Required = true,
						Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Chicken"},
						new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Veggie"}
						}
					}
			};

            var nachoMods = new List<MenuItemModifierGroup> {
				new MenuItemModifierGroup{
					ID = Guid.NewGuid (),
                    Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
					DisplayName = "Protein",
					Exclusive = true,
					Required = true,
					Modifiers = new List<MenuItemModifier>{
						new MenuItemModifier{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Chicken"},
						new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Beef"}
					}
				}
			};

            var menuId = Guid.NewGuid();

            Menu = new Menu
            {
                RestaurantID = RestaurantID,
                Active = true,
                Name = "RegularMenu",
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                ID = menuId,
                Categories = new List<MenuItemCategory>
				{
					new MenuItemCategory
					{
						ID = _topCategoryID,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
						Name = "All Items",
						SubCategories = new List<MenuItemCategory>
						{
							new MenuItemCategory
							{
								ID = Guid.NewGuid (),
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Beer",
								MenuItems = new List<MenuItem>
								{
									new MenuItem{
										ID=Guid.NewGuid (), 
										Name="Pabst Blue Ribbon",
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										ButtonName = "PBR",
										OIListName = "PBR",
										Price=new Money(3.00M), 
										DisplayWeight = 200,
										PossibleModifiers = new List<MenuItemModifierGroup>{
											new MenuItemModifierGroup{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												DisplayName = "Shot",
												Required = false,
												Exclusive = true,
												Modifiers = new List<MenuItemModifier>{
													new MenuItemModifier{ID = Guid.NewGuid (), Name = "Whiskey", Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},PriceChange = new Money(3.0M)},
													new MenuItemModifier{ID = Guid.NewGuid (), Name = "Tequila", Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},PriceChange = new Money(3.0M)},
													new MenuItemModifier{ID = Guid.NewGuid (), Name = "Jager", Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},PriceChange = new Money(3.0M)},
												}
											}
										}
									}
								},
								SubCategories = new List<MenuItemCategory>{
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Drafts",
										MenuItems = new List<MenuItem>{
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Abita Amber", ButtonName = "Abita", OIListName = "Abita", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Singlecut", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Sweet Action", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Lagunitas", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Brooklyn", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Alagash", Price = new Money(7.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Guinness", Price = new Money(6.0M)},
										},
										SubCategories = new List<MenuItemCategory>{
											new MenuItemCategory{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Pitchers",
												MenuItems = new List<MenuItem>{
													new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Ptch Abita", Price = new Money(18.0M)},
													new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Ptch Singlecut", Price = new Money(18.0M)},
													new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Ptch Sweet Act", Price = new Money(18.0M)},
													new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Ptch Lagunit", Price = new Money(18.0M)},
													new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Ptch Brookl", Price = new Money(18.0M)},
													new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Ptch Alaga", Price = new Money(22.0M)},
												}
											}
										}
									},//end draft beer
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Bottles",
										MenuItems = new List<MenuItem>{
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "High Life", Price = new Money(4.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "LoneStar", Price = new Money(5.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Shiner", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Coors Light", Price = new Money(5.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Bud Light", Price = new Money(5.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Budweiser", ButtonName ="Bud", OIListName = "Bud", Price = new Money(5.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Corona", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Heineken", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Amstel Light", ButtonName="Amstel", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Peroni", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Hoegaarden", Price = new Money(6.0M)},
											new MenuItem{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Magners Cider", ButtonName="Magners", OIListName = "Magners", Price = new Money(6.0M)},
										}
									}
								}
							}, //end beer
							new MenuItemCategory{
								ID = Guid.NewGuid (),
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Wine",
								MenuItems = new List<MenuItem>{
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Sangria Glass",
										Price = new Money(5.0M)
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Sangria Pitcher",
										Price = new Money(25.0M),
									}
								},//end wine menu items
								SubCategories = new List<MenuItemCategory>{
									new MenuItemCategory{
										ID = Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "White Wine",
										MenuItems = new List<MenuItem>{
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												VintageYear = 2011,
												Location = "Marlborough",
												Country = new Country("New Zealand"),
												Price = new Money(10.0M),
												Name = "Babich Savignon Blanc 2011",
												OIListName = "Sav Blanc",
												ButtonName = "Sav Blanc",
												DisplayWeight = 200,
											},
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Price = new Money(7.0M),
												Name = "Riff Pinot Grigio 2010",
												OIListName = "Pinot Grigio",
												ButtonName = "Pinot Grigio",
												VintageYear = 2010,
												Location = "Tre-Venezie",
												Country = new Country( "Italy"),
												DisplayWeight = 50,
											},
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Red Tail Ridge Reisling Semi-Dry 2009",
												VintageYear = 2009,
												OIListName = "Reisling",
												ButtonName = "Reisling",
												Price = new Money(10.0M),
												Location = "Finger Lakes, NY",
												Country = Country.UnitedStates,
											},
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Price = new Money(8.0M),
												Name = "Mark West Chardonnay 2011",
												OIListName = "Chardonnay",
												ButtonName = "Chardonnay",
												Location = "Sonoma, CA",
												Country = Country.UnitedStates,
												DisplayWeight = 300,
											}
										},
									},//end white wine
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Red Wine",
										MenuItems = new List<MenuItem>{
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Cartilidge And Browne Pinot Noir 2010",
												OIListName = "Pinot Noir",
												ButtonName = "Pinot Noir",
												Price = new Money(10.0M),
												VintageYear = 2010,
												Location = "North Coast, CA",
												Country = Country.UnitedStates
											},
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Errazuriz Cabernet Sauvignon 2010",
												OIListName = "Cab Sauv",
												ButtonName = "Cab Sauv",
												Price = new Money(8M),
												VintageYear = 2010,
												Location = "Aconcagua Valley",
												Country = new Country("Chile")
											},
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Elsa Bianchi Malbec 2011",
												OIListName = "Malbec",
												ButtonName = "Malbec",
												Price = new Money(7M),
												VintageYear = 2011,
												Location = "Mendoza",
												Country = new Country( "Argentina")
											},
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Paringa Shiraz 2009",
												OIListName = "Shiraz",
												ButtonName = "Shiraz",
												Price = new Money(8M),
												VintageYear = 2009,
												Location = "South",
												Country = new Country( "Australia")
											}
										}
									}, //end redwine
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Sparkling",
										MenuItems = new List<MenuItem>{
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Adami Prosecco",
												Price = new Money(18.00M),
												ButtonName = "Prosecco",
												OIListName = "Prosecco",
												Location = "Veneto",
												Country = Country.UnitedStates,
											},
											new WineMenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Jaume Serra",
												Price = new Money(12M),
												ButtonName = "Cava",
												OIListName = "Cava",
												Location = "Penedes",
												Country = new Country( "Spain")
											}
										}
									}
								}, //end wine sub cats
							},//end wine
							new MenuItemCategory{
								ID = Guid.NewGuid (),
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Food",
								SubCategories = new List<MenuItemCategory>{
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Small Plates",
										MenuItems = new List<MenuItem>{
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Hot Dog",
												Price = new Money(3.0M),
												PossibleModifiers = new List<MenuItemModifierGroup>{
													new MenuItemModifierGroup{
														ID = Guid.NewGuid (),
                                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
														Exclusive = false,
														Required = false,
														Modifiers = new List<MenuItemModifier>{
															new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Ketchup"}
														}
													}
												},
												Destinations = foodDests,
											},
											new MenuItem{
												ID =Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "French Fries",
												Price = new Money(3.0M),
												Destinations = foodDests,
											},
											new MenuItem{
												ID =Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Soft Taco",
												Price = new Money(3.0M),
												PossibleModifiers = new List<MenuItemModifierGroup>{
													new MenuItemModifierGroup{
														ID = Guid.NewGuid (),
                                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
														Exclusive = true,
														Required = true,
														Modifiers = new List<MenuItemModifier>{
															new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Chicken"},
															new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Beef"},
															new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Black Bean"}
														}
													}
												},
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Buffalo Wings Half Dozen",
												ButtonName = "6 Wings",
												OIListName = "6 Wings",
												Price = new Money(6.0M),
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Buffalo Wings Dozen",
												ButtonName = "12 Wings",
												OIListName = "12 Wings",
												Price = new Money(9.0M),
												Destinations = foodDests,
											},
											new MenuItem{
												ID =Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Tortilla Chips",
												ButtonName = "Chips",
												OIListName = "Chips",
												Price = new Money(8.0M),
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Mussels",
												Price = new Money(9M),
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Quesadilla",
												Price = new Money(10M),
												Destinations = foodDests,
												PossibleModifiers = quesadillaMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Mozzarella Sticks",
												ButtonName = "Moz Sticks",
												OIListName = "Moz Sticks",
												Destinations = foodDests,
												Price = new Money(7.0M)
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Grilled Shrimp",
												Destinations = foodDests,
												Price = new Money(9M)
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Nachos",
												Destinations = foodDests,
												Price = new Money(10M),
												PossibleModifiers = nachoMods
											}
										}
									},//end small plates
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Sandwiches",
										MenuItems = new List<MenuItem>{
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Skirt Steak Sandwich",
												OIListName = "Steak Sw",
												ButtonName = "Steak Sand",
												Price = new Money(13.0M),
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "California Chicken Sandwich",
												OIListName = "Chicken Sw",
												ButtonName = "Chicken Sand",
												Price = new Money(11.0M),
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Smoked Salmon Sandwich",
												OIListName = "Salmon Sw",
												ButtonName = "Salmon Sand",
												Price = new Money(10.0M),
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Grilled Portabello Sandwich",
												OIListName = "Portabello Sw",
												ButtonName = "Portabello Sand",
												Price = new Money(10.0M),
												Destinations = foodDests,
											}
										}
									},
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Salads",
										MenuItems = new List<MenuItem>{
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Caeser Salad",
												Price = new Money(7.0M),
												PossibleModifiers = saladMods,
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "House Salad",
												Price = new Money(7.0M),
												PossibleModifiers = saladMods,
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Greek Salad",
												Price = new Money(8.0M),
												PossibleModifiers = saladMods,
												Destinations = foodDests,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Spinach Salad",
												Price = new Money(9.0M),
												PossibleModifiers = saladMods,
												Destinations = foodDests,
											},
										}
									}
								},//end food subcats
								MenuItems = new List<MenuItem>{
									new MenuItem{
										ID=Guid.NewGuid () ,
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Beef Burger", 
										Price=new Money(10.00M),
										DisplayWeight = 900,
										PossibleModifiers = burgerMods,
										Destinations = foodDests,
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Turkey Burger",
										Price = new Money(10.0M),
										PossibleModifiers = burgerMods,
										Destinations = foodDests,
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Lentil Veggie Burger",
										OIListName = "Veggie Burger",
										ButtonName = "Veggie Burger",
										Price = new Money(10.0M),
										PossibleModifiers = burgerMods,
										Destinations = foodDests,
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Burrito",
										Price = new Money(10.0M),
										PossibleModifiers = new List<MenuItemModifierGroup>{
											new MenuItemModifierGroup{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												DisplayName = "Protein",
												Required = true,
												Exclusive = true,
												Modifiers = new List<MenuItemModifier>{
													new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Chicken"},
													new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Beef"},
													new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Veggie"},
													new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Steak", PriceChange = new Money(2.0M)},
													new MenuItemModifier{ID = Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name = "Shrimp", PriceChange = new Money(2.0M)},
												}
											}
										},
										Destinations = foodDests,
									}
								} //end food menu items
							},//end food subcat
							new MenuItemCategory {
								ID = Guid.NewGuid (), 
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Whiskey",
								DisplayOrder = 10,
								MenuItems = new List<MenuItem>
								{
									new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Well Whiskey", 
										Price=new Money(4.00M),
										PossibleModifiers = whiskeyMods,
										DisplayWeight = 100,
									},
									new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Crown Royal", 
										Price=new Money(6.00M),
										PossibleModifiers = whiskeyMods
									},
								},
								SubCategories = new List<MenuItemCategory>
								{
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Irish",
										DisplayOrder = 100,
										MenuItems = new List<MenuItem>
										{
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Bushmills",
												Price = new Money(7.00M),
												PossibleModifiers = whiskeyMods,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Bushmills Honey",
												ButtonName = "Bush Honey",
												OIListName = "Bush Honey",
												Price = new Money(7.00M),
												PossibleModifiers = whiskeyMods,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Tillamore",
												ButtonName = "Tillamore",
												OIListName = "Tillamore",
												Price = new Money(7.00M),
												PossibleModifiers = whiskeyMods,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Paddy",
												ButtonName = "Paddy",
												OIListName = "Paddy",
												Price = new Money(8.00M),
												PossibleModifiers = whiskeyMods,
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Powers",
												ButtonName = "Powers",
												OIListName = "Powers",
												Price = new Money(6.00M),
												PossibleModifiers = whiskeyMods,
												DisplayWeight = 100,
											},
											new MenuItem{
												ID=Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Jameson",
												ButtonName = "Jameson",
												OIListName = "Jameson",
												Price=new Money(7.00M),
												PossibleModifiers = whiskeyMods,
												DisplayWeight = 1000,
											},
											new MenuItem{
												ID=Guid.NewGuid (), 
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Jameson Black", 
												ButtonName = "Jame Black",
												OIListName = "Jame Black",
												Price=new Money(8.00M),
												PossibleModifiers = whiskeyMods,
												DisplayWeight = 50
											},
											new MenuItem{
												ID=Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Jameson 12 year",
												OIListName = "Jameson 12",
												ButtonName = "Jameson 12",
												Price=new Money(10.00M),
												PossibleModifiers = whiskeyMods,
												DisplayWeight = 10
											},
											new MenuItem{
												ID=Guid.NewGuid (), 
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Red Breast", 
												Price=new Money(10.00M),
												PossibleModifiers = whiskeyMods
											}
										}
									},//end irish whiskeys subcat
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Scotch",
										DisplayOrder = 200,
										MenuItems = new List<MenuItem>
										{
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Johnnie Walker Black Label",
												ButtonName = "Johnnie Black",
												OIListName = "Johnnie Black",
												Price = new Money(10.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Glenlivet 12",
												Price = new Money(12.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Macallan 12",
												Price = new Money(11.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Ballantine's",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Dewer's White Label",
												OIListName = "Dewer's",
												ButtonName = "Dewer's",
												Price = new Money(8.0M),
												PossibleModifiers = whiskeyMods,
												DisplayWeight = 90
											}
										}
									},//end scotch subcat
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Bourbon",
										DisplayOrder = 300,
										MenuItems = new List<MenuItem>
										{
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Jack Daniel's",
												ButtonName = "Jack",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Jack Daniel's Tennessee Honey",
												ButtonName = "Jack Honey",
												OIListName = "Jack Honey",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Jim Beam",
												Price = new Money(6.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Wild Turkey 81",
												ButtonName = "Wild Turkey",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Maker's Mark",
												ButtonName = "Maker's",
												OIListName = "Maker's",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Redemption Bourbon",
												ButtonName = "Redemption",
												OIListName = "Redemption",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Redemption Straight Bourbon",
												ButtonName = "Redemption S",
												OIListName = "Redemption S",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Bulleit",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Buffalo Trace",
												OIListName = "Buffalo",
												ButtonName = "Buffalo",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Knob Creek",
												Price = new Money(9.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Woodford Reserve",
												OIListName = "Woodford",
												ButtonName = "Woodford",
												Price = new Money(9.0M),
												PossibleModifiers = whiskeyMods
											},
										}
									},//end bourbon
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Cognac",
										DisplayOrder = 400,
										MenuItems = new List<MenuItem>
										{
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Courvoisier",
												Price = new Money(10.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Hennessy",
												Price = new Money(10.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Remy Martin",
												Price = new Money(10.0M),
												PossibleModifiers = whiskeyMods
											},
										}
									},//end cognac
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Rye",
										DisplayOrder = 500,
										MenuItems = new List<MenuItem>
										{
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Bulleit Rye",
												Price = new Money(7.0M),
												PossibleModifiers = whiskeyMods
											},
											new MenuItem{
												ID = Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name = "Templeton Rye",
												Price = new Money(10.0M),
												PossibleModifiers = whiskeyMods
											},
										}
									},//end rye
								}
							},//end whiskey
							new MenuItemCategory {
								ID = Guid.NewGuid (), 
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Vodka",
								MenuItems = new List<MenuItem>{  
									new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Absolut", 
										Price= new Money(7.00M),
										PossibleModifiers = vodkaMods
									},
									new MenuItem{
										ID=Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Tito's", 
										Price= new Money(7.00M),
										PossibleModifiers = vodkaMods
									},
									new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Grey Goose", 
										Price= new Money(9.00M),
										PossibleModifiers = vodkaMods
									},
									new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Ketel One", 
										Price= new Money(7.00M),
										PossibleModifiers = vodkaMods
									},
									new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Stoli", 
										Price=new Money(7.00M),
										PossibleModifiers = vodkaMods,
										DisplayWeight = 34
									},

								},
								SubCategories = new List<MenuItemCategory>{
									new MenuItemCategory{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Flavored",
										MenuItems = new List<MenuItem>{
											new MenuItem{
												ID=Guid.NewGuid (), 
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Absolut Citron", 
												Price= new Money(7.00M),
												PossibleModifiers = vodkaMods
											},
											new MenuItem{
												ID=Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Stoli Orange", 
												Price=new Money(7.00M),
												PossibleModifiers = vodkaMods
											},
											new MenuItem{
												ID=Guid.NewGuid (), 
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Stoli Wild Cherry",
												ButtonName = "Stoli Cherry",
												OIListName = "Stoli Cherry",
												Price=new Money(7.00M),
												PossibleModifiers = vodkaMods
											},
											new MenuItem{
												ID=Guid.NewGuid (), 
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Stoli Vanilla", 
												Price=new Money(7.00M),
												PossibleModifiers = vodkaMods,
												DisplayWeight = 20,
											},
										}
									} //end flavored vodka
								}//end subcats
							},//end vodka
							new MenuItemCategory 
							{
								ID = Guid.NewGuid (), 
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Gin",
								MenuItems = new List<MenuItem>{
									/*-Hendricks ($10)*/
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Tanquary",
										Price = new Money(7.0M),
										PossibleModifiers = ginMods
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Hendricks",
										Price = new Money(10.0M),
										PossibleModifiers = ginMods
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Bombay Sapphire",
										ButtonName = "Bombay",
										OIListName = "Bombay",
										Price = new Money(7.0M),
										PossibleModifiers = ginMods
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Beefeater",
										Price = new Money(6.0M),
										PossibleModifiers = ginMods
									},
								}
							},
							new MenuItemCategory
							{

								ID = Guid.NewGuid (), 
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Cordials",
								MenuItems = new List<MenuItem>{
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Jäger", Price= new Money(6.00M), DisplayWeight = 60},
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Grand Manier", Price= new Money(10.00M)},
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Ruby Port", Price= new Money(6.00M)},
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Tawny Port", Price= new Money(6.00M)},
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Ricard", Price= new Money(7.00M)},
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Campari", Price= new Money(7.00M)},
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Baileys", Price= new Money(7.00M), DisplayWeight = 10},
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Sweet Vermouth", Price= new Money(5.00M)},
									new MenuItem{ID=Guid.NewGuid (), Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},Name="Dry Vermouth", Price= new Money(5.00M)},
									new MenuItem{ID=Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Fireball Cinnamon Whiskey", 
										ButtonName = "Fireball",
										OIListName = "Fireball",
										Price= new Money(6.00M),
									},
									new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Firefly Sweet Tea Vodka",
										ButtonName = "Firefly",
										OIListName = "Firefly",
										Price= new Money(7.00M)},
									new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name="Fernet Branca",
										ButtonName = "Fernet",
										OIListName = "Fernet",
										Price= new Money(7.00M), 
										DisplayWeight = 10},
								}
							},
							new MenuItemCategory
							{
								ID = Guid.NewGuid (),
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Tequila",
								MenuItems = new List<MenuItem>
								{
									
									new MenuItem{
										ID=Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Patron Silver", 
												Price=new Money(9.00M), 
												PossibleModifiers = tequilaMods
											},
											new MenuItem{
												ID=Guid.NewGuid (), 
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Cafe Patron", 
												Price=new Money(9.00M), 
												PossibleModifiers = tequilaMods
											},
											new MenuItem{
												ID=Guid.NewGuid (),
                                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Patron XO Dark",
												ButtonName = "Patron XO",
												OIListName = "Patron XO",
												Price=new Money(9.00M), 
												PossibleModifiers = tequilaMods
											},
											new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Patron Anejo",
												ButtonName = "Patron Anejo",
												OIListName = "Patron Anejo",
												Price=new Money(9.00M), 
												PossibleModifiers = tequilaMods
											},
											new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="El Toroso",
												Price=new Money(8.00M), 
												PossibleModifiers = tequilaMods
											},
											new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Jose Cuervo Silver",
												ButtonName = "Cuervo Silver",
												OIListName = "Cuervo Silver",
												Price=new Money(7.00M), 
												PossibleModifiers = tequilaMods,
												DisplayWeight = 10,
											},
											new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Jose Cuervo Gold",
												ButtonName = "Cuervo Gold",
												OIListName = "Cuervo Gold",
												Price=new Money(7.00M), 
												PossibleModifiers = tequilaMods,
												DisplayWeight = 100,
											},
											new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Correlejo Silver",
												ButtonName = "Corr Silver",
												OIListName = "Corr Silver",
												Price=new Money(8.00M), 
												PossibleModifiers = tequilaMods,
											},
											new MenuItem{
										ID=Guid.NewGuid (), 
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
												Name="Correlejo Reposado",
												ButtonName = "Corr Repo",
												OIListName = "Corr Repo",
												Price=new Money(8.00M), 
												PossibleModifiers = tequilaMods,
											},
								}
							}, //end tekilla
							new MenuItemCategory{
								ID = Guid.NewGuid (),
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
								Name = "Rum",
								MenuItems = new List<MenuItem>{
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Bacardi",
										Price = new Money(6.0M),
										PossibleModifiers = rumMods,
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Captain Morgan Spiced Rum",
										ButtonName = "Captain",
										OIListName = "Captain",
										Price = new Money(6.0M),
										PossibleModifiers = rumMods,
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Captain Morgan White Rum",
										ButtonName = "Captain White",
										OIListName = "Captain White",
										Price = new Money(6.0M),
										PossibleModifiers = rumMods,
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Captain Morgan Dark Rum",
										ButtonName = "Captain Dark",
										OIListName = "Captain Dark",
										Price = new Money(6.0M),
										PossibleModifiers = rumMods,
									},
									new MenuItem{
										ID = Guid.NewGuid (),
                                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
										Name = "Goslings",
										Price = new Money(7.0M),
										PossibleModifiers = rumMods,
									},
								},
							},//end rum
						},//end subcats list
					},
				},
            };

            var mostPopular = Menu.GetAllMenuItems()
                .OrderByDescending(m => m.DisplayWeight)
                .Skip(3)
                .Take(15)
                .ToList();


            Menu.DefaultMiseItems = mostPopular;
            _tabs = new List<RestaurantCheck>();

            /*_tabs = new List<ICheck> {
                new BarTab{
                    PaymentStatus = CheckPaymentStatus.Closed,
                    LastUpdatedDate = DateTime.UtcNow,
                    LastTouchedServerID = _employees.First().ID,
                    CreatedByServerID = _employees.First().ID,
                    CreatedDate = DateTime.UtcNow,
                    Customer = new Customer{
                        FirstName = "Doug",
                        LastName = "McTest"
                    },
                    OrderItems = new List<OrderItem >{
                        new OrderItem{
                            MenuItem = new MenuItem{
                                ButtonName = "hehehe",
                                Price = new Money(10.0M),
                            },
                            Modifiers = new List<MenuItemModifier>{
                                new MenuItemModifier{
                                    Name = "aMod",
                                    PriceChange = Money.None,
                                }
                            },
                        }
                    }
                }
            };*/

        }

        public Task<Tuple<Restaurant, IMiseTerminalDevice>> RegisterClientAsync(string deviceName)
        {
            var restaurant = new Restaurant
            {
                AccountID = AccountID,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                Name = new RestaurantName("Dominie's LIC", "Doms"),
                FriendlyID = "dominieshoeklic",
                ID = RestaurantID,
                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1023},
                RestaurantServerLocation = restServLoc,
                DiscountPercentageAfterMinimumCashTotals = new List<DiscountPercentageAfterMinimumCashTotal>{
		            new DiscountPercentageAfterMinimumCashTotal
		            {
		                ID = Guid.NewGuid(),
		                Name = "Cash over $50",
		                Percentage = -0.10M,
		                MinCheckAmount = new Money(50.0M),
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1023},
		            }
                },
                DiscountPercentages = new List<DiscountPercentage>{
		            new DiscountPercentage
		            {
		                ID = Guid.NewGuid(),
		                Name = "8+ People",
		                Percentage = .18M,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1023},
		            }
		        },
                StreetAddress = new StreetAddress
                {
                    Country = Country.UnitedStates,
                    State = new State { Name = "New York"},
                    City = new City { Name = "Long Island City"},
                    Street = new Street { Name = "Vernon Blvd"},
                    StreetAddressNumber = new StreetAddressNumber { Number = "48-17"},
                    Zip = new ZipCode { Value = "11101"}
                }
            };
            var dev = new MiseTerminalDevice
            {
                TopLevelCategoryID = _topCategoryID,
                CreatedDate = DateTime.Now,
                ID = Guid.NewGuid(),
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101 },
                RequireEmployeeSignIn = false,
                TableDropChecks = false,
				CreditCardReaderType = CreditCardReaderType.AudioReader,
                HasCashDrawer = true,
                RestaurantID = RestaurantID,
                WaitForZToCloseCards = true,
            };
            restaurant.Terminals = new List<MiseTerminalDevice> { dev };
			return Task.Factory.StartNew (() => new Tuple<Restaurant, IMiseTerminalDevice>(restaurant, dev));
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return _employees;
        }

        public Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return Task.FromResult(GetEmployees());
        }
			

        public Task<IEnumerable<RestaurantCheck>> GetChecksAsync()
        {
			return Task.FromResult(_tabs.AsEnumerable ());
        }

        public Menu GetCurrentMenu()
        {
            return Menu;
        }

        public Task<IEnumerable<Menu>> GetMenusAsync()
        {
			return Task.FromResult (new []{GetCurrentMenu()}.AsEnumerable ());
        }

		public Task<bool> SendEventsAsync(Employee emp, IEnumerable<IEmployeeEvent> empEvents)
        {
            return Task.FromResult(SendEmployeeEvents(emp, empEvents));
        }

        public Task SendOrderItemsToDestination(OrderDestination destination, OrderItem orderItem)
        {
            //do nothing!
			return Task.FromResult (true);
        }

        public Task NotifyDestinationOfVoid(OrderDestination destination, OrderItem orderItem)
        {
			return Task.FromResult (false);
        }


		public Task<bool> SendEventsAsync(RestaurantCheck check, IEnumerable<ICheckEvent> events)
        {
			return Task.FromResult (true);
        }

        public Task<bool> SendEventsAsync(Restaurant rest, IEnumerable<IRestaurantEvent> events)
        {
			return Task.FromResult (true);
        }
        public bool SendEmployeeEvents(Employee emp, IEnumerable<IEmployeeEvent> empEvents)
        {
			return true;
        }
			
		public Task<IEnumerable<Employee>> GetEmployeesForRestaurant (Guid restaurantID)
		{
			throw new NotImplementedException ();
		}
		public Task<Employee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password)
		{
			throw new NotImplementedException ();
		}


		public Task<IEnumerable<Restaurant>> GetRestaurants (Location deviceLocation, Distance max)
		{
			throw new NotImplementedException ();
		}

		public Task<Restaurant> GetRestaurant (Guid restaurantID)
		{
			throw new NotImplementedException ();
		}

	    public Task<bool> ResendEvents(ICollection<IEntityEventBase> events)
	    {
	        return Task.FromResult(true);
	    }
    }
}

