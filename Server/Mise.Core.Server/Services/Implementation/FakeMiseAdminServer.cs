using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.MenuItems;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;

namespace Mise.Core.Server.Services.Implementation
{
    public class FakeMiseAdminServer : IMiseAdminServer
    {
        public bool SendEvents(Guid restaurantID, IEnumerable<IEntityEventBase> events)
        {
            return true;
        }

        public IEnumerable<ICheck> GetCheckSnapshots(Guid restaurantID)
        {
            return new List<ICheck>
            {
                new RestaurantCheck
                {
                    Id = Guid.NewGuid(),
                    ChangeDue = Money.None,
                    ClosedDate = null,
                    CreatedByServerID = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    Customer = new Customer
                    {
                        Name = new PersonName("John", "Smith"),
                    }
                }
            };
        }

        public IEnumerable<ICheck> GetAllChecks()
        {
            return GetCheckSnapshots(Guid.Empty);
        }

        public IEnumerable<IEmployee> GetEmployeeSnapshots(Guid restaurantID)
        {
            return new List<IEmployee> {
                new Employee {
                    Id = Guid.NewGuid(),
                    Name = new PersonName("Leo", "Dominee"),
                    Passcode = "1111",


                    CompBudget = new Money(50.0M),
                    WhenICanVoid = new List<OrderItemStatus>{OrderItemStatus.Ordering, OrderItemStatus.Added, OrderItemStatus.Sent}
                },
                new Employee {
                    Id = Guid.NewGuid (),
                    Passcode = "2222",
                    Name = new PersonName("Sheri", "Dominee"),
                    WhenICanVoid = new List<OrderItemStatus>{OrderItemStatus.Ordering, OrderItemStatus.Added, OrderItemStatus.Sent}
                },
                new Employee {
                    Id = Guid.NewGuid (),
                    Passcode = "3333",
                    Name = new PersonName("Jordan", "Ramirez")
                },
                new Employee {
                    Id = Guid.NewGuid (),
                    Passcode = "4444",
                    Name = new PersonName("J.D.", "Elliott")
                }
            };
        }

        public IEnumerable<IEmployee> GetAllEmployees()
        {
            return GetEmployeeSnapshots(Guid.Empty);
        }

        public IEnumerable<MenuRule> GetMenuRules(Guid restaurantID)
        {
            var startTime = DateTime.Now.AddDays(-1).Date;
            var endTime = DateTime.Now.Date;
            return new List<MenuRule>
            {
                new MenuRule
                {
                    CreatedDate = DateTime.Now,
                    MenuID = Guid.Empty,
                    Id = Guid.NewGuid(),
                    RestaurantID = Guid.Empty,
                    Revision = new EventID{OrderingID = 0},
                    DaysAndTimesAvailable = new List<Tuple<DayOfWeek, DateTime, DateTime>>
                    {
                        new Tuple<DayOfWeek, DateTime, DateTime>(DayOfWeek.Sunday, startTime, endTime),
                        new Tuple<DayOfWeek, DateTime, DateTime>(DayOfWeek.Monday, startTime, endTime),
                        new Tuple<DayOfWeek, DateTime, DateTime>(DayOfWeek.Tuesday, startTime, endTime),
                        new Tuple<DayOfWeek, DateTime, DateTime>(DayOfWeek.Wednesday, startTime, endTime),
                        new Tuple<DayOfWeek, DateTime, DateTime>(DayOfWeek.Thursday, startTime, endTime),
                        new Tuple<DayOfWeek, DateTime, DateTime>(DayOfWeek.Friday, startTime, endTime),
                        new Tuple<DayOfWeek, DateTime, DateTime>(DayOfWeek.Saturday, startTime, endTime),
                    }
                }
            };
        }

        public IEnumerable<MenuRule> GetAllMenuRules()
        {
            return GetMenuRules(Guid.Empty);
        }

        public Task<IEnumerable<Menu>> GetMenus(Guid restaurantID)
        {

            var kitchenDest = new OrderDestination
            {
                Name = "kitchen"
            };

            var foodDests = new List<OrderDestination> { kitchenDest };

            var basicMods = new MenuItemModifierGroup
            {
                Id = Guid.NewGuid(),
                Required = false,
                Exclusive = true,
                Modifiers = new List<MenuItemModifier> {
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Up"},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Neat"},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name="Rocks"},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Shot"},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Tall"},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Double", PriceMultiplier = 1.5M}
                }
            };

            var mixerMods = new MenuItemModifierGroup
            {
                Id = Guid.NewGuid(),
                DisplayName = "Mixers",
                Required = false,
                Exclusive = true,
                Modifiers = new List<MenuItemModifier> {
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Soda", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Tonic", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Ginger", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Cola", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "LemonLime", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Diet", PriceChange = new Money(1.0M)},

                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Cranberry", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "O.J.", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Grapefruit", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Pineapple", PriceChange = new Money(1.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Sour", PriceChange = new Money(2.0M)},

                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Collins", PriceChange = new Money(2.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Mule", PriceChange = new Money(2.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Bloody", PriceChange = new Money(2.0M)},
                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "RedBull", PriceChange = new Money(3.0M)},
                }
            };

            var garnishMods = new MenuItemModifierGroup
            {
                Id = Guid.NewGuid(),
                DisplayName = "Garnish",
                Required = false,
                Exclusive = false,
                Modifiers = new List<MenuItemModifier>{
                    new MenuItemModifier{Id=Guid.NewGuid (), Name = "Lime Wedge"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name = "Lemon Wedge"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name = "Orange Slice"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name="Lemon Peel"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name="Orange Peel"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name="Olives"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name="Onion"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name="Mint"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name="Cucumber"},
                    new MenuItemModifier{Id=Guid.NewGuid (), Name="Cherry"},
                }
            };
            //make our modifiers for each liquor group please!
            var whiskeyMods = new List<MenuItemModifierGroup>{
                basicMods,
                mixerMods,
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Cocktails",
                    Required = false,
                    Exclusive = true,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier {Id = Guid.NewGuid (), Name = "Manhattan", PriceChange = new Money(3.00M)},
                        new MenuItemModifier {Id = Guid.NewGuid (), Name = "Old Fashioned", PriceChange = new Money(3.00M)},
                    }
                },
                garnishMods,
            };
            var vodkaMods = new List<MenuItemModifierGroup>{
                basicMods,
                mixerMods,
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Cocktails",
                    Required = false,
                    Exclusive = true,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Cosmo", PriceChange = new Money(3.00M)},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name = "Martini", PriceChange = new Money(3.00M)},
                    },
                },
                garnishMods,
            };
            var ginMods = new List<MenuItemModifierGroup> {
                basicMods,
                mixerMods,
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Cocktails",
                    Required = false,
                    Exclusive = true,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name = "Martini", PriceChange = new Money(3.00M)}
                    },
                },
                garnishMods,
            };

            var tequilaMods = new List<MenuItemModifierGroup> {
                basicMods,
                mixerMods,
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Cocktails",
                    Required = false,
                    Exclusive = true,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name = "Margarita", PriceChange = new Money(3.00M)},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name = "Gimlet", PriceChange = new Money(3.00M)},
                    },
                },
                garnishMods,
            };

            var rumMods = new List<MenuItemModifierGroup> {
                basicMods,
                mixerMods,
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Cocktails",
                    Required = false,
                    Exclusive = true,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name = "Daquiri", PriceChange = new Money(3.00M)},
                    }
                },
                garnishMods,
            };

            var burgerMods = new List<MenuItemModifierGroup>{
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Temperature",
                    Exclusive = true,
                    Required = true,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Rare"},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Medium Rare"},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name = "Medium"},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name= "Medium Well"},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Well Done"}
                    }
                },
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Cheese",
                    Exclusive = false,
                    Required = false,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="American", PriceChange = new Money(1M)},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Cheddar", PriceChange = new Money(1M)},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Blue", PriceChange = new Money(1M)}
                    }
                },
                new MenuItemModifierGroup{
                    DisplayName = "burgercondiments",
                    Exclusive = false,
                    Required = false,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Mayo"},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Ketchup"}
                    }
                },
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Extra",
                    Exclusive = false,
                    Required = false,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Bacon", PriceChange = new Money(1.0M)},
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Avocado", PriceChange = new Money(1.0M)}
                    }
                }
            };

            var saladMods = new List<MenuItemModifierGroup> {
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Dressing",
                    Exclusive = true,
                    Required = true,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name="Ranch"}
                    }
                },
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Add Protein",
                    Exclusive = false,
                    Required = false,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id = Guid.NewGuid (), Name = "Add Chicken", PriceChange = new Money(5.0M)},
                        new MenuItemModifier{Id = Guid.NewGuid (), Name = "Add Shrimp", PriceChange = new Money(5.0M)}
                    }
                }
            };

            var quesadillaMods = new List<MenuItemModifierGroup> {
                new MenuItemModifierGroup{
                        Id = Guid.NewGuid (),
                        DisplayName = "Protein",
                        Exclusive = true,
                        Required = true,
                        Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name = "Chicken"},
                        new MenuItemModifier{Id = Guid.NewGuid (), Name = "Veggie"}
                        }
                    }
            };

            var nachoMods = new List<MenuItemModifierGroup> {
                new MenuItemModifierGroup{
                    Id = Guid.NewGuid (),
                    DisplayName = "Protein",
                    Exclusive = true,
                    Required = true,
                    Modifiers = new List<MenuItemModifier>{
                        new MenuItemModifier{Id=Guid.NewGuid (), Name = "Chicken"},
                        new MenuItemModifier{Id = Guid.NewGuid (), Name = "Beef"}
                    }
                }
            };

            var menuId = Guid.NewGuid();
            var topCategoryId = Guid.NewGuid();
            var menu = new Menu
            {
                Name = "RegularMenu",
                CreatedDate = DateTime.Now,
                Id = menuId,
                RestaurantID = restaurantID,
                Categories = new List<MenuItemCategory>
                {
                    new MenuItemCategory
                    {
                        Id = topCategoryId,
                        Name = "Regular",
                        SubCategories = new List<MenuItemCategory>
                        {
                            new MenuItemCategory
                            {
                                Id = Guid.NewGuid (),
                                Name = "Beer",
                                MenuItems = new List<MenuItem>
                                {
                                    new MenuItem{
                                        Id=Guid.NewGuid (),
                                        Name="Pabst Blue Ribbon",
                                        ButtonName = "PBR",
                                        OIListName = "PBR",
                                        Price=new Money(3.00M),
                                        DisplayWeight = 200,
                                        PossibleModifiers = new List<MenuItemModifierGroup>{
                                            new MenuItemModifierGroup{
                                                Id = Guid.NewGuid (),
                                                DisplayName = "Shot",
                                                Required = false,
                                                Exclusive = true,
                                                Modifiers = new List<MenuItemModifier>{
                                                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Whiskey", PriceChange = new Money(3.0M)},
                                                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Tequila", PriceChange = new Money(3.0M)},
                                                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Jager", PriceChange = new Money(3.0M)},
                                                }
                                            }
                                        }
                                    }
                                },
                                SubCategories = new List<MenuItemCategory>{
                                    new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Drafts",
                                        MenuItems = new List<MenuItem>{
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Abita Amber", ButtonName = "Abita", OIListName = "Abita", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Singlecut", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Sweet Action", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Lagunitas", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Brooklyn", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Alagash", Price = new Money(7.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Guinness", Price = new Money(6.0M)},
                                        },
                                        SubCategories = new List<MenuItemCategory>{
                                            new MenuItemCategory{
                                                Id = Guid.NewGuid (),
                                                Name = "Pitchers",
                                                MenuItems = new List<MenuItem>{
                                                    new MenuItem{Id = Guid.NewGuid (), Name = "Ptch Abita", Price = new Money(18.0M)},
                                                    new MenuItem{Id = Guid.NewGuid (), Name = "Ptch Singlecut", Price = new Money(18.0M)},
                                                    new MenuItem{Id = Guid.NewGuid (), Name = "Ptch Sweet Act", Price = new Money(18.0M)},
                                                    new MenuItem{Id = Guid.NewGuid (), Name = "Ptch Lagunit", Price = new Money(18.0M)},
                                                    new MenuItem{Id = Guid.NewGuid (), Name = "Ptch Brookl", Price = new Money(18.0M)},
                                                    new MenuItem{Id = Guid.NewGuid (), Name = "Ptch Alaga", Price = new Money(22.0M)},
                                                }
                                            }
                                        }
                                    },//end draft beer
									new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Bottles",
                                        MenuItems = new List<MenuItem>{
                                            new MenuItem{Id = Guid.NewGuid (), Name = "High Life", Price = new Money(4.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "LoneStar", Price = new Money(5.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Shiner", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Coors Light", Price = new Money(5.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Bud Light", Price = new Money(5.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Budweiser", ButtonName ="Bud", OIListName = "Bud", Price = new Money(5.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Corona", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Heineken", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Amstel Light", ButtonName="Amstel", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Peroni", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Hoegaarden", Price = new Money(6.0M)},
                                            new MenuItem{Id = Guid.NewGuid (), Name = "Magners Cider", ButtonName="Magners", OIListName = "Magners", Price = new Money(6.0M)},
                                        }
                                    }
                                }
                            }, //end beer
							new MenuItemCategory{
                                Id = Guid.NewGuid (),
                                Name = "Wine",
                                MenuItems = new List<MenuItem>{
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Sangria Glass",
                                        Price = new Money(5.0M)
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Sangria Pitcher",
                                        Price = new Money(25.0M),
                                    }
                                },//end wine menu items
								SubCategories = new List<MenuItemCategory>{
                                    new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "White Wine",
                                        MenuItems = new List<MenuItem>{
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
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
                                                Id = Guid.NewGuid (),
                                                Price = new Money(7.0M),
                                                Name = "Riff Pinot Grigio 2010",
                                                OIListName = "Pinot Grigio",
                                                ButtonName = "Pinot Grigio",
                                                VintageYear = 2010,
                                                Location = "Tre-Venezie",
                                                Country = new Country("Italy"),
                                                DisplayWeight = 50,
                                            },
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Red Tail Ridge Reisling Semi-Dry 2009",
                                                VintageYear = 2009,
                                                OIListName = "Reisling",
                                                ButtonName = "Reisling",
                                                Price = new Money(10.0M),
                                                Location = "Finger Lakes, NY",
                                                Country = Country.UnitedStates,
                                            },
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
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
                                        Id = Guid.NewGuid (),
                                        Name = "Red Wine",
                                        MenuItems = new List<MenuItem>{
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Cartilidge And Browne Pinot Noir 2010",
                                                OIListName = "Pinot Noir",
                                                ButtonName = "Pinot Noir",
                                                Price = new Money(10.0M),
                                                VintageYear = 2010,
                                                Location = "North Coast, CA",
                                                Country = Country.UnitedStates
                                            },
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Errazuriz Cabernet Sauvignon 2010",
                                                OIListName = "Cab Sauv",
                                                ButtonName = "Cab Sauv",
                                                Price = new Money(8M),
                                                VintageYear = 2010,
                                                Location = "Aconcagua Valley",
                                                Country = new Country("Chile")
                                            },
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Elsa Bianchi Malbec 2011",
                                                OIListName = "Malbec",
                                                ButtonName = "Malbec",
                                                Price = new Money(7M),
                                                VintageYear = 2011,
                                                Location = "Mendoza",
                                                Country = new Country("Argentina")
                                            },
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Paringa Shiraz 2009",
                                                OIListName = "Shiraz",
                                                ButtonName = "Shiraz",
                                                Price = new Money(8M),
                                                VintageYear = 2009,
                                                Location = "South",
                                                Country = new Country("Australia")
                                            }
                                        }
                                    }, //end redwine
									new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Sparkling",
                                        MenuItems = new List<MenuItem>{
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Adami Prosecco",
                                                Price = new Money(18.00M),
                                                ButtonName = "Prosecco",
                                                OIListName = "Prosecco",
                                                Location = "Veneto",
                                                Country = Country.UnitedStates,
                                            },
                                            new WineMenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Jaume Serra",
                                                Price = new Money(12M),
                                                ButtonName = "Cava",
                                                OIListName = "Cava",
                                                Location = "Penedes",
                                                Country = new Country("Spain")
                                            }
                                        }
                                    }
                                }, //end wine sub cats
							},//end wine
							new MenuItemCategory{
                                Id = Guid.NewGuid (),
                                Name = "Food",
                                SubCategories = new List<MenuItemCategory>{
                                    new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Small Plates",
                                        MenuItems = new List<MenuItem>{
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Hot Dog",
                                                Price = new Money(3.0M),
                                                PossibleModifiers = new List<MenuItemModifierGroup>{
                                                    new MenuItemModifierGroup{
                                                        Id = Guid.NewGuid (),
                                                        Exclusive = false,
                                                        Required = false,
                                                        Modifiers = new List<MenuItemModifier>{
                                                            new MenuItemModifier{Id = Guid.NewGuid (), Name = "Ketchup"}
                                                        }
                                                    }
                                                },
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "French Fries",
                                                Price = new Money(3.0M),
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Soft Taco",
                                                Price = new Money(3.0M),
                                                PossibleModifiers = new List<MenuItemModifierGroup>{
                                                    new MenuItemModifierGroup{
                                                        Id = Guid.NewGuid (),
                                                        Exclusive = true,
                                                        Required = true,
                                                        Modifiers = new List<MenuItemModifier>{
                                                            new MenuItemModifier{Id = Guid.NewGuid (), Name = "Chicken"},
                                                            new MenuItemModifier{Id = Guid.NewGuid (), Name = "Beef"},
                                                            new MenuItemModifier{Id = Guid.NewGuid (), Name = "Black Bean"}
                                                        }
                                                    }
                                                },
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Buffalo Wings Half Dozen",
                                                ButtonName = "6 Wings",
                                                OIListName = "6 Wings",
                                                Price = new Money(6.0M),
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Buffalo Wings Dozen",
                                                ButtonName = "12 Wings",
                                                OIListName = "12 Wings",
                                                Price = new Money(9.0M),
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Tortilla Chips",
                                                ButtonName = "Chips",
                                                OIListName = "Chips",
                                                Price = new Money(8.0M),
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Mussels",
                                                Price = new Money(9M),
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Quesadilla",
                                                Price = new Money(10M),
                                                Destinations = foodDests,
                                                PossibleModifiers = quesadillaMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Mozzarella Sticks",
                                                ButtonName = "Moz Sticks",
                                                OIListName = "Moz Sticks",
                                                Destinations = foodDests,
                                                Price = new Money(7.0M)
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Grilled Shrimp",
                                                Destinations = foodDests,
                                                Price = new Money(9M)
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Nachos",
                                                Destinations = foodDests,
                                                Price = new Money(10M),
                                                PossibleModifiers = nachoMods
                                            }
                                        }
                                    },//end small plates
									new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Sandwiches",
                                        MenuItems = new List<MenuItem>{
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Skirt Steak Sandwich",
                                                OIListName = "Steak Sandwich",
                                                ButtonName = "Steak Sand",
                                                Price = new Money(13.0M),
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "California Chicken Sandwich",
                                                OIListName = "Chicken Sandwich",
                                                ButtonName = "Chicken Sand",
                                                Price = new Money(11.0M),
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Smoked Salmon Sandwich",
                                                OIListName = "Salmon Sandwich",
                                                ButtonName = "Salmon Sand",
                                                Price = new Money(10.0M),
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Grilled Portabello Sandwich",
                                                OIListName = "Portabello Sandwich",
                                                ButtonName = "Portabello Sand",
                                                Price = new Money(10.0M),
                                                Destinations = foodDests,
                                            }
                                        }
                                    },
                                    new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Salads",
                                        MenuItems = new List<MenuItem>{
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Caeser Salad",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = saladMods,
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "House Salad",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = saladMods,
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Greek Salad",
                                                Price = new Money(8.0M),
                                                PossibleModifiers = saladMods,
                                                Destinations = foodDests,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
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
                                        Id = Guid.NewGuid () ,
                                        Name="Beef Burger",
                                        Price=new Money(10.00M),
                                        DisplayWeight = 900,
                                        PossibleModifiers = burgerMods,
                                        Destinations = foodDests,
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Turkey Burger",
                                        Price = new Money(10.0M),
                                        PossibleModifiers = burgerMods,
                                        Destinations = foodDests,
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Lentil Veggie Burger",
                                        OIListName = "Veggie Burger",
                                        ButtonName = "Veggie Burger",
                                        Price = new Money(10.0M),
                                        PossibleModifiers = burgerMods,
                                        Destinations = foodDests,
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Burrito",
                                        Price = new Money(10.0M),
                                        PossibleModifiers = new List<MenuItemModifierGroup>{
                                            new MenuItemModifierGroup{
                                                Id = Guid.NewGuid (),
                                                DisplayName = "Protein",
                                                Required = true,
                                                Exclusive = true,
                                                Modifiers = new List<MenuItemModifier>{
                                                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Chicken"},
                                                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Beef"},
                                                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Veggie"},
                                                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Steak", PriceChange = new Money(2.0M)},
                                                    new MenuItemModifier{Id = Guid.NewGuid (), Name = "Shrimp", PriceChange = new Money(2.0M)},
                                                }
                                            }
                                        },
                                        Destinations = foodDests,
                                    }
                                } //end food menu items
							},//end food subcat
							new MenuItemCategory {
                                Id = Guid.NewGuid (),
                                Name = "Whiskey",
                                DisplayOrder = 10,
                                MenuItems = new List<MenuItem>
                                {
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Whiskey",
                                        Price=new Money(4.00M),
                                        PossibleModifiers = whiskeyMods,
                                        DisplayWeight = 100,
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Crown Royal",
                                        Price=new Money(6.00M),
                                        PossibleModifiers = whiskeyMods
                                    },
                                },
                                SubCategories = new List<MenuItemCategory>
                                {
                                    new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Irish",
                                        DisplayOrder = 100,
                                        MenuItems = new List<MenuItem>
                                        {
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Bushmills",
                                                Price = new Money(7.00M),
                                                PossibleModifiers = whiskeyMods,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Bushmills Honey",
                                                ButtonName = "Bush Honey",
                                                OIListName = "Bush Honey",
                                                Price = new Money(7.00M),
                                                PossibleModifiers = whiskeyMods,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Tillamore",
                                                ButtonName = "Tillamore",
                                                OIListName = "Tillamore",
                                                Price = new Money(7.00M),
                                                PossibleModifiers = whiskeyMods,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Paddy",
                                                ButtonName = "Paddy",
                                                OIListName = "Paddy",
                                                Price = new Money(8.00M),
                                                PossibleModifiers = whiskeyMods,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Powers",
                                                ButtonName = "Powers",
                                                OIListName = "Powers",
                                                Price = new Money(6.00M),
                                                PossibleModifiers = whiskeyMods,
                                                DisplayWeight = 100,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Jameson",
                                                ButtonName = "Jameson",
                                                OIListName = "Jameson",
                                                Price=new Money(7.00M),
                                                PossibleModifiers = whiskeyMods,
                                                DisplayWeight = 1000,
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Jameson Black",
                                                ButtonName = "Jame Black",
                                                OIListName = "Jame Black",
                                                Price=new Money(8.00M),
                                                PossibleModifiers = whiskeyMods,
                                                DisplayWeight = 50
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Jameson 12 year",
                                                OIListName = "Jameson 12",
                                                ButtonName = "Jameson 12",
                                                Price=new Money(10.00M),
                                                PossibleModifiers = whiskeyMods,
                                                DisplayWeight = 10
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Red Breast",
                                                Price=new Money(10.00M),
                                                PossibleModifiers = whiskeyMods
                                            }
                                        }
                                    },//end irish whiskeys subcat
									new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Scotch",
                                        DisplayOrder = 200,
                                        MenuItems = new List<MenuItem>
                                        {
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Johnnie Walker Black Label",
                                                ButtonName = "Johnnie Black",
                                                OIListName = "Johnnie Black",
                                                Price = new Money(10.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Glenlivet 12",
                                                Price = new Money(12.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Macallan 12",
                                                Price = new Money(11.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Ballantine's",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
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
                                        Id = Guid.NewGuid (),
                                        Name = "Bourbon",
                                        DisplayOrder = 300,
                                        MenuItems = new List<MenuItem>
                                        {
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Jack Daniel's",
                                                ButtonName = "Jack",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Jack Daniel's Tennessee Honey",
                                                ButtonName = "Jack Honey",
                                                OIListName = "Jack Honey",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Jim Beam",
                                                Price = new Money(6.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Wild Turkey 81",
                                                ButtonName = "Wild Turkey",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Maker's Mark",
                                                ButtonName = "Maker's",
                                                OIListName = "Maker's",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Redemption Bourbon",
                                                ButtonName = "Redemption",
                                                OIListName = "Redemption",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Redemption Straight Bourbon",
                                                ButtonName = "Redemption S",
                                                OIListName = "Redemption S",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Bulleit",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Buffalo Trace",
                                                OIListName = "Buffalo",
                                                ButtonName = "Buffalo",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Knob Creek",
                                                Price = new Money(9.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Woodford Reserve",
                                                OIListName = "Woodford",
                                                ButtonName = "Woodford",
                                                Price = new Money(9.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                        }
                                    },//end bourbon
									new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Cognac",
                                        DisplayOrder = 400,
                                        MenuItems = new List<MenuItem>
                                        {
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Courvoisier",
                                                Price = new Money(10.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Hennessy",
                                                Price = new Money(10.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Remy Martin",
                                                Price = new Money(10.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                        }
                                    },//end cognac
									new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Rye",
                                        DisplayOrder = 500,
                                        MenuItems = new List<MenuItem>
                                        {
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Bulleit Rye",
                                                Price = new Money(7.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name = "Templeton Rye",
                                                Price = new Money(10.0M),
                                                PossibleModifiers = whiskeyMods
                                            },
                                        }
                                    },//end rye
								}
                            },//end whiskey
							new MenuItemCategory {
                                Id = Guid.NewGuid (),
                                Name = "Vodka",
                                MenuItems = new List<MenuItem>{
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Absolut",
                                        Price= new Money(7.00M),
                                        PossibleModifiers = vodkaMods
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Tito's",
                                        Price= new Money(7.00M),
                                        PossibleModifiers = vodkaMods
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Grey Goose",
                                        Price= new Money(9.00M),
                                        PossibleModifiers = vodkaMods
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Ketel One",
                                        Price= new Money(7.00M),
                                        PossibleModifiers = vodkaMods
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Stoli",
                                        Price=new Money(7.00M),
                                        PossibleModifiers = vodkaMods,
                                        DisplayWeight = 34
                                    },

                                },
                                SubCategories = new List<MenuItemCategory>{
                                    new MenuItemCategory{
                                        Id = Guid.NewGuid (),
                                        Name = "Flavored",
                                        MenuItems = new List<MenuItem>{
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Absolut Citron",
                                                Price= new Money(7.00M),
                                                PossibleModifiers = vodkaMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Stoli Orange",
                                                Price=new Money(7.00M),
                                                PossibleModifiers = vodkaMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Stoli Wild Cherry",
                                                ButtonName = "Stoli Cherry",
                                                OIListName = "Stoli Cherry",
                                                Price=new Money(7.00M),
                                                PossibleModifiers = vodkaMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
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
                                Id = Guid.NewGuid (),
                                Name = "Gin",
                                MenuItems = new List<MenuItem>{
									/*-Hendricks ($10)*/
									new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Tanquary",
                                        Price = new Money(7.0M),
                                        PossibleModifiers = ginMods
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Hendricks",
                                        Price = new Money(10.0M),
                                        PossibleModifiers = ginMods
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Bombay Sapphire",
                                        ButtonName = "Bombay",
                                        OIListName = "Bombay",
                                        Price = new Money(7.0M),
                                        PossibleModifiers = ginMods
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Beefeater",
                                        Price = new Money(6.0M),
                                        PossibleModifiers = ginMods
                                    },
                                }
                            },
                            new MenuItemCategory
                            {

                                Id = Guid.NewGuid (),
                                Name = "Cordials",
                                MenuItems = new List<MenuItem>{
                                    new MenuItem{Id = Guid.NewGuid (), Name="Jäger", Price= new Money(6.00M), DisplayWeight = 60},
                                    new MenuItem{Id = Guid.NewGuid (), Name="Grand Manier", Price= new Money(10.00M)},
                                    new MenuItem{Id = Guid.NewGuid (), Name="Ruby Port", Price= new Money(6.00M)},
                                    new MenuItem{Id = Guid.NewGuid (), Name="Tawny Port", Price= new Money(6.00M)},
                                    new MenuItem{Id = Guid.NewGuid (), Name="Ricard", Price= new Money(7.00M)},
                                    new MenuItem{Id = Guid.NewGuid (), Name="Campari", Price= new Money(7.00M)},
                                    new MenuItem{Id = Guid.NewGuid (), Name="Baileys", Price= new Money(7.00M), DisplayWeight = 10},
                                    new MenuItem{Id = Guid.NewGuid (), Name="Sweet Vermouth", Price= new Money(5.00M)},
                                    new MenuItem{Id = Guid.NewGuid (), Name="Dry Vermouth", Price= new Money(5.00M)},
                                    new MenuItem{Id = Guid.NewGuid (),
                                        Name="Fireball Cinnamon Whiskey",
                                        ButtonName = "Fireball",
                                        OIListName = "Fireball",
                                        Price= new Money(6.00M),
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Firefly Sweet Tea Vodka",
                                        ButtonName = "Firefly",
                                        OIListName = "Firefly",
                                        Price= new Money(7.00M)},
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name="Fernet Branca",
                                        ButtonName = "Fernet",
                                        OIListName = "Fernet",
                                        Price= new Money(7.00M),
                                        DisplayWeight = 10},
                                }
                            },
                            new MenuItemCategory
                            {
                                Id = Guid.NewGuid (),
                                Name = "Tequila",
                                MenuItems = new List<MenuItem>
                                {

                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                                Name="Patron Silver",
                                                Price=new Money(9.00M),
                                                PossibleModifiers = tequilaMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Cafe Patron",
                                                Price=new Money(9.00M),
                                                PossibleModifiers = tequilaMods
                                            },
                                            new MenuItem{
                                                Id = Guid.NewGuid (),
                                                Name="Patron XO Dark",
                                                ButtonName = "Patron XO",
                                                OIListName = "Patron XO",
                                                Price=new Money(9.00M),
                                                PossibleModifiers = tequilaMods
                                            },
                                            new MenuItem{
                                        Id = Guid.NewGuid (),
                                                Name="Patron Anejo",
                                                ButtonName = "Patron Anejo",
                                                OIListName = "Patron Anejo",
                                                Price=new Money(9.00M),
                                                PossibleModifiers = tequilaMods
                                            },
                                            new MenuItem{
                                        Id = Guid.NewGuid (),
                                                Name="El Toroso",
                                                Price=new Money(8.00M),
                                                PossibleModifiers = tequilaMods
                                            },
                                            new MenuItem{
                                        Id = Guid.NewGuid (),
                                                Name="Jose Cuervo Silver",
                                                ButtonName = "Cuervo Silver",
                                                OIListName = "Cuervo Silver",
                                                Price=new Money(7.00M),
                                                PossibleModifiers = tequilaMods,
                                                DisplayWeight = 10,
                                            },
                                            new MenuItem{
                                        Id = Guid.NewGuid (),
                                                Name="Jose Cuervo Silver",
                                                ButtonName = "Cuervo Gold",
                                                OIListName = "Cuervo Gold",
                                                Price=new Money(7.00M),
                                                PossibleModifiers = tequilaMods,
                                                DisplayWeight = 100,
                                            },
                                            new MenuItem{
                                        Id = Guid.NewGuid (),
                                                Name="Correlejo Silver",
                                                ButtonName = "Corr Silver",
                                                OIListName = "Corr Silver",
                                                Price=new Money(8.00M),
                                                PossibleModifiers = tequilaMods,
                                            },
                                            new MenuItem{
                                        Id = Guid.NewGuid (),
                                                Name="Correlejo Reposado",
                                                ButtonName = "Corr Repo",
                                                OIListName = "Corr Repo",
                                                Price=new Money(8.00M),
                                                PossibleModifiers = tequilaMods,
                                            },
                                }
                            }, //end tekilla
							new MenuItemCategory{
                                Id = Guid.NewGuid (),
                                Name = "Rum",
                                MenuItems = new List<MenuItem>{
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Bacardi",
                                        Price = new Money(6.0M),
                                        PossibleModifiers = rumMods,
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Captain Morgan Spiced Rum",
                                        ButtonName = "Captain",
                                        OIListName = "Captain",
                                        Price = new Money(6.0M),
                                        PossibleModifiers = rumMods,
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Captain Morgan White Rum",
                                        ButtonName = "Captain White",
                                        OIListName = "Captain White",
                                        Price = new Money(6.0M),
                                        PossibleModifiers = rumMods,
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
                                        Name = "Captain Morgan Dark Rum",
                                        ButtonName = "Captain Dark",
                                        OIListName = "Captain Dark",
                                        Price = new Money(6.0M),
                                        PossibleModifiers = rumMods,
                                    },
                                    new MenuItem{
                                        Id = Guid.NewGuid (),
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

            return Task.Factory.StartNew(() => new List<Menu> { menu }.AsEnumerable());
        }

        public Task<IEnumerable<Menu>> GetAllMenus()
        {
            return GetMenus(Guid.Empty);
        }


        private IRestaurant _restaurant;

        public bool AddNewTerminal(Guid restaurantID, IMiseTerminalDevice terminal)
        {
            if (_restaurant == null)
            {
                return false;
            }

            _restaurant.AddTerminal(terminal);

            return true;
        }

        public IRestaurant GetRestaurantSnapshot(Guid restaurantID)
        {

            //TODO remove after connected to real DB
            _restaurant = new Restaurant
            {
                Name = new RestaurantName("Dominie's LIC", "Dom's"),
                FriendlyID = "dominieslic",
                RestaurantServerLocation = new Uri("http://restaurantserver.misepos.com/"),
                Id = restaurantID,
                DiscountPercentageAfterMinimumCashTotals = new List<DiscountPercentageAfterMinimumCashTotal>{
                    new DiscountPercentageAfterMinimumCashTotal
                    {
                        Id = Guid.NewGuid(),
                        Name = "Cash over $50",
                        Percentage = -0.10M,
                        MinCheckAmount = new Money(50.0M),
                    }
                },
                DiscountPercentages = new List<DiscountPercentage>{
                    new DiscountPercentage
                    {
                        Id = Guid.NewGuid(),
                        Name = "8+ People",
                        Percentage = .18M
                    }
                },
                Terminals = new List<MiseTerminalDevice>
                {
                    new MiseTerminalDevice
                    {
                        TopLevelCategoryID = Guid.Empty,
                        CreatedDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                        RequireEmployeeSignIn = false,
                        FriendlyID = "mainBar",
                        RestaurantID = restaurantID
                    },
                     new MiseTerminalDevice
                    {
                        TopLevelCategoryID = Guid.Empty,
                        CreatedDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                        RequireEmployeeSignIn = false,
                        FriendlyID = "serverSide",
                        RestaurantID = restaurantID
                    },

                }
            };
            return _restaurant;
        }

        public IEnumerable<IRestaurant> GetAllRestaurants()
        {
            return new[] { GetRestaurantSnapshot(Guid.Empty) };
        }
    }
}
