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
		private readonly List<InventoryCategory> _allCats = new List<InventoryCategory>{
			BeerWineLiquor, Consumables, Operationals, Unknown,
			Beer, Wine, Liquor,
			Whiskey, WhiskeyAmerican, WhiskeyBourbon, WhiskeyCanadian, WhiskeyRye,
            WhiskeyScotch, ScotchSingleMalt, ScotchBlended, ScotchGrain, ScotchSingleMaltCampbeltown, ScotchSingleMaltHighland, ScotchSingleMaltIslands,
            ScotchSingleMaltIslay, ScotchSingleMaltLowland, ScotchSingleMaltSpeyside,
			WhiskeyWorld, WhiskeyIrish, WhiskeyAmericanCorn, WhiskeyAmericanSingleMalt,
			Vodka, Gin,
			Rum, RumDark, RumTraditional, SpicedRum, RumAgricole, Cachaca,
			Agave, AgaveMezcal, AgaveTequila,
			Brandy, BrandyMisc, EauDeVie, Cognac, Pisco,
            Liqueur, LiquerAmaro, LiquerGeneric,
			WineFortified, WineRed, WineRose, WineSparkling, WineWhite, Sake,
            AromatizedWine, Vermouth, MiscAromatizedWine,
			NonAlcoholic, Food,
			BeerDraft, BeerPackage
		};

		#region Top Level Cats
		public static InventoryCategory BeerWineLiquor => new InventoryCategory{
		    Name = "BeerWineLiquor",
		    Id = Guid.Parse ("4d17620b-fcb5-4ad0-bfd3-a9aaa1d9eb25")
		};

	    public static InventoryCategory Consumables => new InventoryCategory {
	        Name = "Consumables",
	        Id = Guid.Parse ("bf2d5626-bb54-4e99-a840-36311eb46f2f")
	    };

	    public static InventoryCategory Operationals => new InventoryCategory {
	        Name = "Operationals",
	        Id = Guid.Parse ("8a4efc28-84f1-44d0-8dd3-d7525da0b2f1")
	    };

	    public static InventoryCategory Unknown => new InventoryCategory {
	        Name = "None",
	        Id = Guid.Parse ("7e05f222-9aaf-47d0-a51b-3146f2060c55")
	    };

        #endregion

        #region BLW
        public static InventoryCategory Beer => new InventoryCategory {
		    Name = "Beer",
		    Id = Guid.Parse ("ceb2d033-50ca-42bd-a2aa-baeca09f5d3b"),
		    ParentCategoryID = BeerWineLiquor.Id,
		};

	    public static InventoryCategory Wine => new InventoryCategory {
	        Name = "Wine",
	        Id = Guid.Parse ("47480a57-ca22-49dc-ae97-b0e6856fb4a3"),
	        ParentCategoryID = BeerWineLiquor.Id,
            PreferredContainers = new List<LiquidContainer>{
                LiquidContainer.Bottle750ML,
                LiquidContainer.Bottle1L,
                LiquidContainer.Bottle1_75ML,
                LiquidContainer.Bottle375ML
            }
	    };

	    public static InventoryCategory Liquor => new InventoryCategory {
	        Name = "Liquor",
	        Id = Guid.Parse ("c382ad3c-7a91-4c68-9cac-03ae5d63a823"),
	        ParentCategoryID = BeerWineLiquor.Id,
            PreferredContainers = new List<LiquidContainer>{
                LiquidContainer.Bottle750ML,
                LiquidContainer.Bottle1L,
                LiquidContainer.Bottle1_75ML,
                LiquidContainer.Bottle375ML
            }
	    };
        #endregion

        #region Standard Cats
        public static InventoryCategory Vodka => new InventoryCategory
        {
            Name = "Vodka",
            Id = Guid.Parse("99d452e5-2549-4038-922b-be8742021e30"),
            ParentCategoryID = Liquor.Id,
        };

        public static InventoryCategory Gin => new InventoryCategory
        {
            Name = "Gin",
            Id = Guid.Parse("81cf06eb-00ba-40f3-8c61-a837ccc04dbb"),
            ParentCategoryID = Liquor.Id
        };

        public static InventoryCategory Rum => new InventoryCategory
        {
            Name = "Rum",
            Id = Guid.Parse("badc4822-f4bf-40a1-a0f5-b7f403fb417a"),
            ParentCategoryID = Liquor.Id
        };

	    public static InventoryCategory RumTraditional => new InventoryCategory
	    {
	        Name = "Rum",
	        Id = Guid.Parse("cfb0cefa-925a-4799-831b-31e6e3b166a7"),
	        ParentCategoryID = Rum.Id
	    };

	    public static InventoryCategory RumAgricole => new InventoryCategory
	    {
	        Name = "Rhum Agricole",
	        Id = Guid.Parse("58503cb2-0f1a-4c10-ae8d-728b1c2bdce0"),
	        ParentCategoryID = Rum.Id
	    };
        public static InventoryCategory RumDark => new InventoryCategory
        {
            Name = "Dark Rum",
            Id = Guid.Parse("b0f3cd9d-26b2-4a6f-a792-a1ef4fa00661"),
            ParentCategoryID = Rum.Id
        };

	    public static InventoryCategory SpicedRum => new InventoryCategory
	    {
	        Name = "Spiced Rum",
	        Id = Guid.Parse("611cc7d8-63c6-4a56-86cf-c8c7572887a7"),
	        ParentCategoryID = Rum.Id
	    };

	    public static InventoryCategory Cachaca => new InventoryCategory
	    {
            Name = "Cachaca",
            Id = Guid.Parse("3e6e8e10-e09e-4739-8c9f-baaeba1d0f76"),
            ParentCategoryID = Rum.Id
	    };

        public static InventoryCategory Whiskey => new InventoryCategory {
		    Name = "Whiskey",
		    Id = Guid.Parse ("9607464b-11e2-4273-a0b5-aba2de34baee"),
		    ParentCategoryID = Liquor.Id
		};

	    public static InventoryCategory WhiskeyWorld => new InventoryCategory {
	        Name = "Whisky (World)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse ("9949f991-0ed2-4ec5-96db-c21f5ce0e54c")
	    };

	    public static InventoryCategory WhiskeyIrish => new InventoryCategory {
	        Name = "Whiskey (Irish)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse ("1916f019-d98c-4f13-bc3c-0cf44bfa6f22")
	    };

	    public static InventoryCategory WhiskeyAmerican => new InventoryCategory {
	        Name = "Whiskey (American)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse ("fd28d0ca-258f-4cd2-b3e5-2e33dae7dae6")
	    };

	    public static InventoryCategory WhiskeyScotch => new InventoryCategory {
	        Name = "Scotch",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse ("aec4cf62-094d-4d51-9003-042e361f7cb2"),
            IsAssignable = true
	    };

	    public static InventoryCategory ScotchSingleMalt => new InventoryCategory
	    {
	        Name = "Scotch (Single Malt)",
	        ParentCategoryID = WhiskeyScotch.Id,
	        Id = Guid.Parse("b7ae838a-6335-4501-b97c-56e7e9be37aa"),
            IsAssignable = true
	    };

	    public static InventoryCategory ScotchSingleMaltHighland => new InventoryCategory
	    {
	        Name = "Scotch (SM Highlands)",
	        ParentCategoryID = ScotchSingleMalt.Id,
	        Id = Guid.Parse("6efedaea-32a7-4058-816f-7676e9238710"),
	        IsAssignable = true
	    };

        public static InventoryCategory ScotchSingleMaltSpeyside => new InventoryCategory
        {
            Name = "Scotch (SM Speyside)",
            ParentCategoryID = ScotchSingleMalt.Id,
            Id = Guid.Parse("97abc4bd-22de-4e5a-a40c-d642bf6119f6"),
            IsAssignable = true
        };

        public static InventoryCategory ScotchSingleMaltCampbeltown => new InventoryCategory
        {
            Name = "Scotch (SM Campbeltown)",
            ParentCategoryID = ScotchSingleMalt.Id,
            Id = Guid.Parse("c7de68ae-bcef-4fe9-b120-7cdf90b0072d"),
            IsAssignable = true
        };

        public static InventoryCategory  ScotchSingleMaltIslands => new InventoryCategory
        {
            Name = "Scotch (SM Islands)",
            ParentCategoryID = ScotchSingleMalt.Id,
            Id = Guid.Parse("2b83edb1-4c21-454a-80c6-25d3569b4ae3"),
            IsAssignable = true
        };

        public static InventoryCategory ScotchSingleMaltIslay => new InventoryCategory
        {
            Name = "Scotch (SM Islay)",
            ParentCategoryID = ScotchSingleMalt.Id,
            Id = Guid.Parse("bd32d58f-d002-49d4-a1f6-8823ee0e1292"),
            IsAssignable = true
        };

        public static InventoryCategory ScotchSingleMaltLowland => new InventoryCategory
        {
            Name = "Scotch (SM Lowland)",
            ParentCategoryID = ScotchSingleMalt.Id,
            Id = Guid.Parse("b6aced85-4c57-4b1b-83ae-23946100571b"),
            IsAssignable = true
        };

	    public static InventoryCategory ScotchBlended => new InventoryCategory
	    {
	        Name = "Scotch (Blended)",
	        ParentCategoryID = WhiskeyScotch.Id,
	        Id = Guid.Parse("4af047e0-3f22-4df7-8ed8-13d78789a458")
	    };

	    public static InventoryCategory ScotchGrain => new InventoryCategory
	    {
	        Name = "Scotch (Grain)",
	        ParentCategoryID = WhiskeyScotch.Id,
	        Id = Guid.Parse("e376e21b-8f82-4514-885a-896bd0eb3694")
	    };

        public static InventoryCategory WhiskeyBourbon => new InventoryCategory
	    {
	        Name = "Bourbon",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse("d2552894-1979-4e90-9604-dca3f54e0a8d")
	    };

	    public static InventoryCategory WhiskeyRye => new InventoryCategory
	    {
	        Name = "Whiskey (Rye)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse("ed526b32-efcd-48e7-8c73-f5027e5ad0df")
	    };

	    public static InventoryCategory WhiskeyAmericanSingleMalt => new InventoryCategory
	    {
	        Name = "Whiskey (American Single Malt)",
            ParentCategoryID = Whiskey.Id,
            Id = Guid.Parse("10a2ae32-4756-4838-988f-a6d87de67ac7")
	    };

	    public static InventoryCategory WhiskeyAmericanCorn => new InventoryCategory
	    {
	        Name = "Whiskey (Corn)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse("e4e7a71f-861e-4f64-ae57-74034bb367eb")
	    };

	    public static InventoryCategory WhiskeyCanadian => new InventoryCategory
	    {
	        Name = "Whiskey (Canadian)",
	        ParentCategoryID = Whiskey.Id,
	        Id = Guid.Parse("92b3ad51-7c1a-4363-9bc6-a85cf75e0b38")
	    };


	    public static InventoryCategory Agave => new InventoryCategory {
	        Name = "Agave",
	        Id = Guid.Parse ("b752a17d-9738-44d4-897a-0e7930a5b613"),
	        ParentCategoryID = Liquor.Id
	    };

	    public static InventoryCategory AgaveTequila => new InventoryCategory {
	        Name = "Tequila",
	        Id = Guid.Parse ("196ff83e-7ef4-432d-9285-8390c2b88175"),
	        ParentCategoryID = Agave.Id
	    };

	    public static InventoryCategory AgaveMezcal => new InventoryCategory {
	        Name = "Mezcal",
	        Id = Guid.Parse ("bb1edde6-4546-4a31-91a7-afcdbcfaf67f"),
	        ParentCategoryID = Agave.Id
	    };

	    public static InventoryCategory Brandy => new InventoryCategory {
	        Name = "Brandy",
	        Id = Guid.Parse ("ce9652db-a575-4037-bef0-e20993203559"),
	        ParentCategoryID = Liquor.Id
	    };

	    public static InventoryCategory BrandyMisc => new InventoryCategory
	    {
	        Name = "Brandy",
	        Id = Guid.Parse("fbcbe907-4a71-45f3-a43b-756aaeedc853"),
	        ParentCategoryID = Brandy.Id
	    };

	    public static InventoryCategory Cognac => new InventoryCategory
	    {
	        Name = "Cognac",
	        Id = Guid.Parse("85f40201-4510-4023-982b-64daa554a77d"),
	        ParentCategoryID = Brandy.Id
	    };

	    public static InventoryCategory EauDeVie => new InventoryCategory
	    {
	        Name = "Eau de Vie",
	        Id = Guid.Parse("c2ec7e2c-a231-4878-b248-9f5c567f8157"),
	        ParentCategoryID = Brandy.Id,
	    };

	    public static InventoryCategory Pisco => new InventoryCategory
	    {
	        Name = "Pisco",
	        Id = Guid.Parse("42d49164-5ebc-476e-a286-aea21a97950e"),
	        ParentCategoryID = Brandy.Id
	    };
	    public static InventoryCategory Liqueur => new InventoryCategory {
	        Name = "Liqueur",
	        Id = Guid.Parse ("435f5bf7-4828-42fe-838a-59cf5e86e0be"),
	        ParentCategoryID = Liquor.Id
	    };

	    public static InventoryCategory LiquerAmaro => new InventoryCategory {
	        Name = "Amaro",
	        ParentCategoryID = Liqueur.Id,
	        Id = Guid.Parse ("758d1f9d-053f-4a76-958b-aebfda88f580")
	    };

	    public static InventoryCategory LiquerGeneric => new InventoryCategory
	    {
	        Name = "Liqueur",
	        ParentCategoryID = Liqueur.Id,
	        Id = Guid.Parse("abb08632-0d61-463c-94c3-982f974423f1")
	    };

	    public static InventoryCategory AromatizedWine => new InventoryCategory
	    {
	        Name = "Aromatized Wine",
	        ParentCategoryID = BeerWineLiquor.Id,
	        Id = Guid.Parse("1db18c0e-f28d-46a1-a864-7a2bf77b1a3f")
	    };

	    public static InventoryCategory MiscAromatizedWine => new InventoryCategory
	    {
	        Name = "Misc. Aromatized Wine",
	        ParentCategoryID = AromatizedWine.Id,
	        Id = Guid.Parse("6cf38037-dff9-47aa-a578-aad1063bb41f")
	    };

	    public static InventoryCategory Vermouth => new InventoryCategory
	    {
            Name = "Vermouth",
            ParentCategoryID = AromatizedWine.Id,
            Id = Guid.Parse("6a4a4be4-1da5-4451-8da6-22bbfdaaec80")
	    };

        public static InventoryCategory WineFortified => new InventoryCategory
        {
            Name = "Fortified Wine",
            ParentCategoryID = Wine.Id,
            Id = Guid.Parse("d3ad99ed-96b0-47e5-8044-e7886870b1a1")
        };

        public static InventoryCategory WineWhite => new InventoryCategory
	    {
	        Name = "White Wine",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse("750f9746-5d15-4aa9-9fb3-990d347f009e")
	    };

	    public static InventoryCategory WineRed => new InventoryCategory
	    {
	        Name = "Red Wine",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse("9b47488d-396e-4289-9a67-20dbbaa605e8")
	    };

	    public static InventoryCategory WineSparkling => new InventoryCategory
	    {
	        Name = "Sparkling Wine",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse("f998f845-f2d1-4504-8621-47ca46f1d0ee")
	    };

	    public static InventoryCategory WineRose => new InventoryCategory
	    {
	        Name = "Rosé",
	        ParentCategoryID = Wine.Id,
	        Id = Guid.Parse("8d7c893f-07af-4d2a-b6a7-ca13af6ca876")
	    };

	    public static InventoryCategory Sake => new InventoryCategory
	    {
            Name = "Sake",
            ParentCategoryID = Wine.Id,
            Id = Guid.Parse("08610c03-efc4-4d70-be4b-d95c1ba9102d")
	    };

        public static InventoryCategory NonAlcoholic => new InventoryCategory {
	        Name = "Non Alcoholic",
	        Id = Guid.Parse ("115952ae-b81a-424d-be99-61d51804011c"),
	        ParentCategoryID = Consumables.Id
	    };


	    public static InventoryCategory BeerPackage => new InventoryCategory
	    {
	        Name = "Package Beer",
	        ParentCategoryID = Beer.Id,
	        Id = Guid.Parse("3043c1f3-4718-4419-893a-697d6c16a5c6"),
            PreferredContainers = new List<LiquidContainer>{
                LiquidContainer.Can12Oz,
                LiquidContainer.Bottle12Oz,
                LiquidContainer.Can16Oz,
                LiquidContainer.Can10Oz,
                LiquidContainer.Bottle16Oz,
                LiquidContainer.Bottle7Oz,
                LiquidContainer.Bottle40Oz
            }
	    };

	    public static InventoryCategory BeerDraft => new InventoryCategory
	    {
	        Name = "Draft Beer",
	        ParentCategoryID = Beer.Id,
	        Id = Guid.Parse("b6fc4aca-0f9b-431a-a231-d1c55bb7dab6"),
            PreferredContainers = new List<LiquidContainer>{
                LiquidContainer.Keg,
                LiquidContainer.HalfKeg,
                LiquidContainer.TorpedoKeg,
                LiquidContainer.ImportKeg
            }
	    };

	    public static InventoryCategory Food => new InventoryCategory
	    {
	        Name = "Food",
	        Id = Guid.Parse("76eff069-0340-42a7-815c-ef0ef1242635"),
	        ParentCategoryID = Consumables.Id
	    };

	    #endregion

	    public IEnumerable<IInventoryCategory> GetAllCategories()
	    {
	        return _allCats;
	    } 

		#region ICategoriesService implementation
		public IEnumerable<IInventoryCategory> GetIABIngredientCategories ()
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

		public IEnumerable<IInventoryCategory> GetCategoriesUnder(IInventoryCategory cat){
			var results = new List<IInventoryCategory> ();
			var subs = _allCats.Where (c => c.ParentCategoryID.HasValue && c.ParentCategoryID.Value == cat.Id);
			foreach(var sub in subs){
				results.Add (sub);
				var recurs = GetCategoriesUnder (sub);
				results.AddRange (recurs);
			}

			return results;
		}

        public IEnumerable<LiquidContainer> GetPreferredContainers(IInventoryCategory cat){
            var prefCats = cat.GetPreferredContainers();
            if (prefCats != null)
            {
                return prefCats;
            } 
            var parent = _allCats.FirstOrDefault(c => c.Id == cat.ParentCategoryID);
            if (parent == null)
            {
                return null;
            }
            return GetPreferredContainers(parent);
        }

	    public LiquidContainerShape GetShapeForCategory(IInventoryCategory cat)
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

	    public IEnumerable<IInventoryCategory> GetPossibleCategories(string givenCategory)
	    {
	        return _allCats.Where(c => c.Name.Contains(givenCategory) || givenCategory.Contains(c.Name));
	    }

	    public IEnumerable<IInventoryCategory> GetAssignableCategories()
	    {
	        var parentCats = _allCats.Where(c => c.ParentCategoryID != null).Select(c => c.ParentCategoryID).Distinct().ToList();
	        return _allCats.Where(c => c.IsAssignable || (parentCats.Contains(c.Id) == false && c.ParentCategoryID.HasValue));
	    }

	    #endregion
	}
}

