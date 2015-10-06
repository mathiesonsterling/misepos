using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Services.Implementation
{
	/// <summary>
	/// Retrieve categories 
	/// </summary>
	public class CategoriesService : ICategoriesService
	{
		private readonly List<ItemCategory> _allCats = new List<ItemCategory>{
			BeerWineLiquor, Consumables, Operationals, Unknown,
			Beer, Wine, Liquor,
			Whiskey, WhiskeyAmerican, WhiskeyBourbon, WhiskeyCanadian, WhiskeyRye, WhiskeyScotch, 
			WhiskeyWorld, WhiskeyIrish,
			Vodka, Gin,
			Rum, RumDark, RumLight, SpicedRum,
			Agave, AgaveMezcal, AgaveTequila,
			Brandy, Liqueur, LiquerAmaro, LiquerGeneric,
			WineFortified, WineRed, WineRose, WineSparkling, WineWhite,
			NonAlcoholic, Food,
			BeerDraft, BeerPackage
		};

		#region Top Level Cats
		public static ItemCategory BeerWineLiquor => new ItemCategory{
		    Name = "BeerWineLiquor",
		    Id = Guid.Parse ("4d17620b-fcb5-4ad0-bfd3-a9aaa1d9eb25")
		};

	    public static ItemCategory Consumables => new ItemCategory {
	        Name = "Consumables",
	        Id = Guid.Parse ("bf2d5626-bb54-4e99-a840-36311eb46f2f")
	    };

	    public static ItemCategory Operationals => new ItemCategory {
	        Name = "Operationals",
	        Id = Guid.Parse ("8a4efc28-84f1-44d0-8dd3-d7525da0b2f1")
	    };

	    public static ItemCategory Unknown => new ItemCategory {
	        Name = "None",
	        Id = Guid.Parse ("7e05f222-9aaf-47d0-a51b-3146f2060c55")
	    };

        #endregion

        #region BLW
        public static ItemCategory Beer => new ItemCategory {
		    Name = "Beer",
		    Id = Guid.Parse ("ceb2d033-50ca-42bd-a2aa-baeca09f5d3b"),
		    ParentCategoryID = BeerWineLiquor.Id
		};

	    public static ItemCategory Wine => new ItemCategory {
	        Name = "Wine",
	        Id = Guid.Parse ("47480a57-ca22-49dc-ae97-b0e6856fb4a3"),
	        ParentCategoryID = BeerWineLiquor.Id
	    };

	    public static ItemCategory Liquor => new ItemCategory {
	        Name = "Liquor",
	        Id = Guid.Parse ("c382ad3c-7a91-4c68-9cac-03ae5d63a823"),
	        ParentCategoryID = BeerWineLiquor.Id
	    };
        #endregion

        #region Standard Cats
        public static ItemCategory Vodka => new ItemCategory
        {
            Name = "Vodka",
            Id = Guid.Parse("99d452e5-2549-4038-922b-be8742021e30"),
            ParentCategoryID = Liquor.Id
        };

        public static ItemCategory Gin => new ItemCategory
        {
            Name = "Gin",
            Id = Guid.Parse("81cf06eb-00ba-40f3-8c61-a837ccc04dbb"),
            ParentCategoryID = Liquor.Id
        };

        public static ItemCategory Rum => new ItemCategory
        {
            Name = "Rum",
            Id = Guid.Parse("badc4822-f4bf-40a1-a0f5-b7f403fb417a"),
            ParentCategoryID = Liquor.Id
        };

	    public static ItemCategory RumLight => new ItemCategory
	    {
	        Name = "Rum",
	        Id = Guid.Parse("cfb0cefa-925a-4799-831b-31e6e3b166a7"),
	        ParentCategoryID = Rum.Id
	    };
        public static ItemCategory RumDark => new ItemCategory
        {
            Name = "Dark Rum",
            Id = Guid.Parse("b0f3cd9d-26b2-4a6f-a792-a1ef4fa00661"),
            ParentCategoryID = Rum.Id
        };

	    public static ItemCategory SpicedRum => new ItemCategory
	    {
	        Name = "Spiced Rum",
	        Id = Guid.Parse("611cc7d8-63c6-4a56-86cf-c8c7572887a7"),
	        ParentCategoryID = Rum.Id
	    };

	    public static ItemCategory Cachaca => new ItemCategory
	    {
            Name = "Cachaca",
            Id = Guid.Parse("3e6e8e10-e09e-4739-8c9f-baaeba1d0f76"),
            ParentCategoryID = Rum.Id
	    };

        public static ItemCategory Whiskey => new ItemCategory {
		    Name = "Whiskey",
		    Id = Guid.Parse ("9607464b-11e2-4273-a0b5-aba2de34baee"),
		    ParentCategoryID = Liquor.Id
		};

	    public static ItemCategory WhiskeyWorld => new ItemCategory {
	        Name = "Whisky (World)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse ("9949f991-0ed2-4ec5-96db-c21f5ce0e54c")
	    };

	    public static ItemCategory WhiskeyIrish => new ItemCategory {
	        Name = "Whiskey (Irish)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse ("1916f019-d98c-4f13-bc3c-0cf44bfa6f22")
	    };

	    public static ItemCategory WhiskeyAmerican => new ItemCategory {
	        Name = "Whiskey (American)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse ("fd28d0ca-258f-4cd2-b3e5-2e33dae7dae6")
	    };

	    public static ItemCategory WhiskeyScotch => new ItemCategory {
	        Name = "Scotch",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse ("aec4cf62-094d-4d51-9003-042e361f7cb2")
	    };

	    public static ItemCategory WhiskeyBourbon => new ItemCategory
	    {
	        Name = "Bourbon",
	        ParentCategoryID = WhiskeyAmerican.Id,
	        Id = Guid.Parse("d2552894-1979-4e90-9604-dca3f54e0a8d")
	    };

	    public static ItemCategory WhiskeyRye => new ItemCategory
	    {
	        Name = "Rye",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse("ed526b32-efcd-48e7-8c73-f5027e5ad0df")
	    };

	    public static ItemCategory WhiskeyCanadian => new ItemCategory
	    {
	        Name = "Canadian Whiskey",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse("92b3ad51-7c1a-4363-9bc6-a85cf75e0b38")
	    };


	    public static ItemCategory Agave => new ItemCategory {
	        Name = "Agave",
	        Id = Guid.Parse ("b752a17d-9738-44d4-897a-0e7930a5b613"),
	        ParentCategoryID = Liquor.Id
	    };

	    public static ItemCategory AgaveTequila => new ItemCategory {
	        Name = "Tequila",
	        Id = Guid.Parse ("196ff83e-7ef4-432d-9285-8390c2b88175"),
	        ParentCategoryID = Agave.Id
	    };

	    public static ItemCategory AgaveMezcal => new ItemCategory {
	        Name = "Mezcal",
	        Id = Guid.Parse ("bb1edde6-4546-4a31-91a7-afcdbcfaf67f"),
	        ParentCategoryID = Agave.Id
	    };

	    public static ItemCategory Brandy => new ItemCategory {
	        Name = "Brandy",
	        Id = Guid.Parse ("ce9652db-a575-4037-bef0-e20993203559"),
	        ParentCategoryID = Liquor.Id
	    };

	    public static ItemCategory Liqueur => new ItemCategory {
	        Name = "Liqueur",
	        Id = Guid.Parse ("435f5bf7-4828-42fe-838a-59cf5e86e0be"),
	        ParentCategoryID = Liquor.Id
	    };

	    public static ItemCategory LiquerAmaro => new ItemCategory {
	        Name = "Amaro",
	        ParentCategoryID = Liqueur.Id,
	        Id = Guid.Parse ("758d1f9d-053f-4a76-958b-aebfda88f580")
	    };

	    public static ItemCategory LiquerGeneric => new ItemCategory
	    {
	        Name = "Liqueur",
	        ParentCategoryID = Liqueur.Id,
	        Id = Guid.Parse("abb08632-0d61-463c-94c3-982f974423f1")
	    };
	    public static ItemCategory WineFortified => new ItemCategory {
	        Name = "Fortified Wine",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse ("d3ad99ed-96b0-47e5-8044-e7886870b1a1")
	    };

	    public static ItemCategory WineWhite => new ItemCategory
	    {
	        Name = "White Wine",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse("750f9746-5d15-4aa9-9fb3-990d347f009e")
	    };

	    public static ItemCategory WineRed => new ItemCategory
	    {
	        Name = "Red Wine",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse("9b47488d-396e-4289-9a67-20dbbaa605e8")
	    };

	    public static ItemCategory WineSparkling => new ItemCategory
	    {
	        Name = "Sparkling Wine",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse("f998f845-f2d1-4504-8621-47ca46f1d0ee")
	    };

	    public static ItemCategory WineRose => new ItemCategory
	    {
	        Name = "Rosé",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse("8d7c893f-07af-4d2a-b6a7-ca13af6ca876")
	    };

	    public static ItemCategory NonAlcoholic => new ItemCategory {
	        Name = "Non Alcoholic",
	        Id = Guid.Parse ("115952ae-b81a-424d-be99-61d51804011c"),
	        ParentCategoryID = Consumables.Id
	    };


	    public static ItemCategory BeerPackage => new ItemCategory
	    {
	        Name = "Package Beer",
	        ParentCategoryID = Beer.Id,
	        Id = Guid.Parse("3043c1f3-4718-4419-893a-697d6c16a5c6")
	    };

	    public static ItemCategory BeerDraft => new ItemCategory
	    {
	        Name = "Draft Beer",
	        ParentCategoryID = Beer.Id,
	        Id = Guid.Parse("b6fc4aca-0f9b-431a-a231-d1c55bb7dab6")
	    };

	    public static ItemCategory Food => new ItemCategory
	    {
	        Name = "Food",
	        Id = Guid.Parse("76eff069-0340-42a7-815c-ef0ef1242635"),
	        ParentCategoryID = Consumables.Id
	    };

	    #endregion
		#region ICategoriesService implementation
		public IEnumerable<ICategory> GetIABIngredientCategories ()
		{
			var results = GetCategoriesUnder (BeerWineLiquor).ToList();
			results.Add (Food);
			results.Add (Unknown);
			results.Add (NonAlcoholic);
			return results;
		}

		public IEnumerable<ICategory> GetCustomCategoriesForRestaurant (Guid restaurantID)
		{
			return new List<ICategory> ();
		}

		public IEnumerable<ICategory> GetCategoriesUnder(ICategory cat){
			var results = new List<ICategory> ();
			var subs = _allCats.Where (c => c.ParentCategoryID.HasValue && c.ParentCategoryID.Value == cat.Id);
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

	        if (cat.Id == Wine.Id
	            || cat.Id == WineFortified.Id
	            || cat.Id == WineRed.Id
	            || cat.Id == WineRose.Id
	            || cat.Id == WineSparkling.Id
	            || cat.Id == WineWhite.Id)
	        {
	            return LiquidContainerShape.WineBottleShape;
	        }

	        if (cat.Id == BeerDraft.Id)
	        {
	            return LiquidContainerShape.DefaultKegShape;
	        }

	        if (cat.Id == Beer.Id)
	        {
	            return LiquidContainerShape.DefaultBeerBottleShape;
	        }

	        if (cat.Id == Food.Id || cat.Id == BeerPackage.Id)
	        {
	            return LiquidContainerShape.Box;
	        }

	        return LiquidContainerShape.DefaultBottleShape;
	    }

	    public IEnumerable<ICategory> GetPossibleCategories(string givenCategory)
	    {
	        return _allCats.Where(c => c.Name.Contains(givenCategory) || givenCategory.Contains(c.Name));
	    }

	    public IEnumerable<ICategory> GetAssignableCategories()
	    {
	        var parentCats = _allCats.Where(c => c.ParentCategoryID != null).Select(c => c.ParentCategoryID).Distinct().ToList();
	        return _allCats.Where(c => parentCats.Contains(c.Id) == false && c.ParentCategoryID.HasValue);
	    }

	    #endregion
	}
}

