using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using System.Threading;
using System.Dynamic;


namespace Mise.Core.Common
{
	/// <summary>
	/// Retrieve categories 
	/// </summary>
	public class CategoriesService : ICategoriesService
	{
		private List<ItemCategory> _allCats = new List<ItemCategory>{
			BeerWineLiquor, Consumables, Operationals, Unknown,
			Beer, Wine, Liquor,
			Whiskey, WhiskeyAmerican, WhiskeyBourbon, WhiskeyCanadian, WhiskeyRye, WhiskeyScotch, 
			WhiskeyWorld, WhiskeyIrish,
			Vodka, Gin,
			Rum, RumDark,
			Agave, AgaveMezcal, AgaveTequila,
			Brandy, Liqueur, LiquerAmaro,
			WineFortified, WineRed, WineRose, WineSparkling, WineWhite,
			NonAlcoholic, Food,
			BeerDraft, BeerPackage
		};

		#region Top Level Cats
		public static ItemCategory BeerWineLiquor{
			get{
				return new ItemCategory{
					Name = "BeerWineLiquor",
					ID = Guid.Parse ("4d17620b-fcb5-4ad0-bfd3-a9aaa1d9eb25")
				};
			}
		}

		public static ItemCategory Consumables{
			get{
				return new ItemCategory {
					Name = "Consumables",
					ID = Guid.Parse ("bf2d5626-bb54-4e99-a840-36311eb46f2f")
				};
			}
		}

		public static ItemCategory Operationals{
			get{
				return new ItemCategory {
					Name = "Operationals",
					ID = Guid.Parse ("8a4efc28-84f1-44d0-8dd3-d7525da0b2f1")
				};
			}
		}

		public static ItemCategory Unknown{get{ return new ItemCategory {
					Name = "None",
					ID = Guid.Parse ("7e05f222-9aaf-47d0-a51b-3146f2060c55")
				};
			}
		}
		#endregion
		public static ItemCategory Beer { 
			get { 
				return new ItemCategory {
					Name = "Beer",
					ID = Guid.Parse ("ceb2d033-50ca-42bd-a2aa-baeca09f5d3b"),
					ParentCategoryID = BeerWineLiquor.ID
				};
			} 
		}

		public static ItemCategory Wine{get{
				return new ItemCategory {
					Name = "Wine",
					ID = Guid.Parse ("47480a57-ca22-49dc-ae97-b0e6856fb4a3"),
					ParentCategoryID = BeerWineLiquor.ID
				};
			}
		}

		public static ItemCategory Liquor{
			get{
				return new ItemCategory {
					Name = "Liquor",
					ID = Guid.Parse ("c382ad3c-7a91-4c68-9cac-03ae5d63a823"),
					ParentCategoryID = BeerWineLiquor.ID
				};
			}
		}

		#region Standard Cats
		public static ItemCategory Whiskey{get{
				return new ItemCategory {
					Name = "Whiskey",
					ID = Guid.Parse ("9607464b-11e2-4273-a0b5-aba2de34baee"),
					ParentCategoryID = Liquor.ID
				};
			}
		}

		public static ItemCategory WhiskeyWorld{get{
				return new ItemCategory {
					Name = "Whisky (world)",
					ParentCategoryID = Whiskey.ID,
					ID = Guid.Parse ("9949f991-0ed2-4ec5-96db-c21f5ce0e54c")
				};
			}}

		public static ItemCategory WhiskeyIrish{
			get{
				return new ItemCategory {
					Name = "Irish Whiskey",
					ParentCategoryID = Whiskey.ID,
					ID = Guid.Parse ("1916f019-d98c-4f13-bc3c-0cf44bfa6f22")
				};
			}
		}

		public static ItemCategory WhiskeyAmerican{get{
				return new ItemCategory {
					Name = "Whiskey (American)",
					ParentCategoryID = Whiskey.ID,
					ID = Guid.Parse ("fd28d0ca-258f-4cd2-b3e5-2e33dae7dae6")
				};
			}}

		public static ItemCategory WhiskeyScotch{get{ 
				return new ItemCategory {
					Name = "Scotch",
					ParentCategoryID = Whiskey.ID,
					ID = Guid.Parse ("aec4cf62-094d-4d51-9003-042e361f7cb2")
				};
			}}

	    public static ItemCategory WhiskeyBourbon
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "Bourbon",
	                ParentCategoryID = WhiskeyAmerican.ID,
	                ID = Guid.Parse("d2552894-1979-4e90-9604-dca3f54e0a8d")
	            };
	        }
	    }

	    public static ItemCategory WhiskeyRye
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "Rye",
	                ParentCategoryID = Whiskey.ID,
	                ID = Guid.Parse("ed526b32-efcd-48e7-8c73-f5027e5ad0df")
	            };
	        }
	    }

	    public static ItemCategory WhiskeyCanadian
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "Canadian Whiskey",
	                ParentCategoryID = Whiskey.ID,
	                ID = Guid.Parse("92b3ad51-7c1a-4363-9bc6-a85cf75e0b38")
	            };
	        }
	    }

	    public static ItemCategory Vodka{get{
				return new ItemCategory {
					Name = "Vodka",
					ID = Guid.Parse ("99d452e5-2549-4038-922b-be8742021e30"),
					ParentCategoryID = Liquor.ID
				};
			}
		}

		public static ItemCategory Gin{get{
				return new ItemCategory {
					Name = "Gin",
					ID = Guid.Parse ("81cf06eb-00ba-40f3-8c61-a837ccc04dbb"),
					ParentCategoryID = Liquor.ID
				};
			}}

		public static ItemCategory Rum{get{
				return new ItemCategory {
					Name = "Rum",
					ID = Guid.Parse ("badc4822-f4bf-40a1-a0f5-b7f403fb417a"),
					ParentCategoryID = Liquor.ID
				};
			}}

		public static ItemCategory RumDark{get{
				return new ItemCategory {
					Name = "Dark Rum",
					ID = Guid.Parse ("b0f3cd9d-26b2-4a6f-a792-a1ef4fa00661"),
					ParentCategoryID = Rum.ID
				};
			}}

		public static ItemCategory Agave{get{
				return new ItemCategory {
					Name = "Agave",
					ID = Guid.Parse ("b752a17d-9738-44d4-897a-0e7930a5b613"),
					ParentCategoryID = Liquor.ID
				};
			}}

		public static ItemCategory AgaveTequila{get{
				return new ItemCategory {
					Name = "Tequila",
					ID = Guid.Parse ("196ff83e-7ef4-432d-9285-8390c2b88175"),
					ParentCategoryID = Agave.ID
				};
			}}

		public static ItemCategory AgaveMezcal{get{
				return new ItemCategory {
					Name = "Mezcal",
					ID = Guid.Parse ("bb1edde6-4546-4a31-91a7-afcdbcfaf67f"),
					ParentCategoryID = Agave.ID
				};
			}}

		public static ItemCategory Brandy{get{
				return new ItemCategory {
					Name = "Brandy",
					ID = Guid.Parse ("ce9652db-a575-4037-bef0-e20993203559"),
					ParentCategoryID = Liquor.ID
				};
			}}

		public static ItemCategory Liqueur{get{
				return new ItemCategory {
					Name = "Liqueur",
					ID = Guid.Parse ("435f5bf7-4828-42fe-838a-59cf5e86e0be"),
					ParentCategoryID = Liquor.ID
				};
			}}

		public static ItemCategory LiquerAmaro{get{
				return new ItemCategory {
					Name = "Amaro",
					ParentCategoryID = Liqueur.ID,
					ID = Guid.Parse ("758d1f9d-053f-4a76-958b-aebfda88f580")
				};
			}}
				

		public static ItemCategory WineFortified{get{
				return new ItemCategory {
					Name = "Fortified Wine",
					ParentCategoryID = Wine.ID,
					ID = Guid.Parse ("d3ad99ed-96b0-47e5-8044-e7886870b1a1")
				};
			}}

	    public static ItemCategory WineWhite
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "White Wine",
	                ParentCategoryID = Wine.ID,
	                ID = Guid.Parse("750f9746-5d15-4aa9-9fb3-990d347f009e")
	            };
	        }
	    }

	    public static ItemCategory WineRed
	    {
	        get
	        {
	            return new ItemCategory
	                {
	                    Name = "Red Wine",
	                    ParentCategoryID = Wine.ID,
	                    ID = Guid.Parse("9b47488d-396e-4289-9a67-20dbbaa605e8")
	                };
	        }
	    }

	    public static ItemCategory WineSparkling
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "Sparkling Wine",
	                ParentCategoryID = Wine.ID,
	                ID = Guid.Parse("f998f845-f2d1-4504-8621-47ca46f1d0ee")
	            };
	        }
	    }

	    public static ItemCategory WineRose
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "Rosé",
	                ParentCategoryID = Wine.ID,
	                ID = Guid.Parse("8d7c893f-07af-4d2a-b6a7-ca13af6ca876")
	            };
	        }
	    }

		public static ItemCategory NonAlcoholic{get{return new ItemCategory {
				Name = "Non Alcoholic",
				ID = Guid.Parse ("115952ae-b81a-424d-be99-61d51804011c"),
				ParentCategoryID = Consumables.ID
			};
			}
		}
			

	    public static ItemCategory BeerPackage
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "Package Beer",
	                ParentCategoryID = Beer.ID,
	                ID = Guid.Parse("3043c1f3-4718-4419-893a-697d6c16a5c6")
	            };
	        }
	    }

	    public static ItemCategory BeerDraft
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "Draft Beer",
	                ParentCategoryID = Beer.ID,
	                ID = Guid.Parse("b6fc4aca-0f9b-431a-a231-d1c55bb7dab6")
	            };
	        }
	    }

	    public static ItemCategory Food
	    {
	        get
	        {
	            return new ItemCategory
	            {
	                Name = "Food",
	                ID = Guid.Parse("76eff069-0340-42a7-815c-ef0ef1242635"),
					ParentCategoryID = Consumables.ID
	            };
	        }
	    }
		#endregion
		#region ICategoriesService implementation
		public IEnumerable<ICategory> GetIABIngredientCategories ()
		{
			var results = GetCategoriesUnder (BeerWineLiquor).ToList();
			results.Add (Food);
			results.Add (Unknown);
			results.Add (NonAlcoholic);
			return results;
			/*
			var res = new List<ICategory>
            {
                Whiskey, 
                WhiskeyWorld, 
                WhiskeyAmerican, 
                WhiskeyScotch, 
                
                Vodka, 
                Gin, 
                
                Rum, 
                RumDark, 
                
                Agave,
			    AgaveTequila, 
                AgaveMezcal, 
                
                Brandy, 
                Liqueur, 
                LiquerAmaro, 
                
                Wine, 
                WineFortified, 
                WineWhite, 
                WineRed, 
                WineRose, 
                WineSparkling, 
                
                Beer, 
                BeerPackage, 
                BeerDraft,
            
                Food,
                Unknown
            };
			return res;*/
		}

		public IEnumerable<ICategory> GetCustomCategoriesForRestaurant (Guid restaurantID)
		{
			return new List<ICategory> ();
		}

		public IEnumerable<ICategory> GetCategoriesUnder(ICategory cat){
			var results = new List<ICategory> ();
			var subs = _allCats.Where (c => c.ParentCategoryID.HasValue && c.ParentCategoryID.Value == cat.ID);
			foreach(var sub in subs){
				results.Add (sub);
				var recurs = GetCategoriesUnder (sub);
				results.AddRange (recurs);
			}

			return results;
		}

	    public LiquidContainerShape GetShapeForCategory(ICategory cat)
	    {
	        if (cat == null)
	        {
	            return LiquidContainerShape.DefaultBottleShape;
	        }

	        if (cat.ID == Wine.ID
	            || cat.ID == WineFortified.ID
	            || cat.ID == WineRed.ID
	            || cat.ID == WineRose.ID
	            || cat.ID == WineSparkling.ID
	            || cat.ID == WineWhite.ID)
	        {
	            return LiquidContainerShape.WineBottleShape;
	        }

	        if (cat.ID == BeerDraft.ID)
	        {
	            return LiquidContainerShape.DefaultKegShape;
	        }

	        if (cat.ID == Beer.ID)
	        {
	            return LiquidContainerShape.DefaultBeerBottleShape;
	        }

	        if (cat.ID == Food.ID || cat.ID == BeerPackage.ID)
	        {
	            return LiquidContainerShape.Box;
	        }

	        return LiquidContainerShape.DefaultBottleShape;
	    }
		#endregion
	}
}

