using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.UnitTests.Tools;
using NUnit.Framework;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Common.Entities.MenuItems;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Menu;

namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
	[TestFixture]
	public class MenuSerializationTests
	{
        [TestCase(SerializationType.JSONDOTNET)]
		[Test]
		public void TestMenuEmpty(SerializationType type)
		{
		    var jsonSer = SerializerFactory.GetJSONSerializer(type);
			var menu = new Menu ();
		
			var json = jsonSer.Serialize (menu);
		
			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = jsonSer.Deserialize<Menu> (json);
			Assert.IsNotNull (res);
		}

        [TestCase(SerializationType.JSONDOTNET)]
		[Test]
        public void TestMenuItemCategoryEmpty(SerializationType type)
        {
			var mc = new MenuItemCategory {
				Name = "cat",
				DisplayOrder = 100,
				MenuItems = new List<MenuItem> (),
				SubCategories = new List<MenuItemCategory> ()
			};

            var jsonSer = SerializerFactory.GetJSONSerializer(type);
			var json = jsonSer.Serialize (mc);
			Assert.IsFalse (string.IsNullOrEmpty (json), "empty JSON string");

			var res = jsonSer.Deserialize<MenuItemCategory> (json);
			Assert.IsNotNull (res, "Deserialize is null");
			Assert.AreEqual ("cat", res.Name);
			Assert.AreEqual (100, res.DisplayOrder);
		}

        [TestCase(SerializationType.JSONDOTNET)]
		[Test]
        public void TestMenuItemCategoryWithSubcat(SerializationType type)
        {
			var mc = new MenuItemCategory {
				Name = "cat",
				DisplayOrder = 100,
				MenuItems = new List<MenuItem> (),
				SubCategories = new List<MenuItemCategory>{
					new MenuItemCategory{
						Name = "subCat",
						DisplayOrder = 101,
					}
				}
			};

            var jsonSer = SerializerFactory.GetJSONSerializer(type);
			var json = jsonSer.Serialize (mc);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty JSON string");

			var res = jsonSer.Deserialize<MenuItemCategory> (json);
			Assert.IsNotNull (res);
			Assert.AreEqual ("cat", res.Name);
			Assert.AreEqual (100, res.DisplayOrder);

			Assert.AreEqual (1, res.SubCategories.Count());
			Assert.AreEqual ("subCat", res.SubCategories.First().Name);
		}

		static MenuItemModifierGroup GetMenuItemModifierGroup()
		{
			return new MenuItemModifierGroup{
				Id = Guid.NewGuid (),
				DisplayName = "testgroup",
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
		}

        [TestCase(SerializationType.JSONDOTNET)]
		[Test]
        public void TestMenuItemModifierGroup(SerializationType type)
        {
			var mig = GetMenuItemModifierGroup ();

            var jsonSer = SerializerFactory.GetJSONSerializer(type);
			var json = jsonSer.Serialize (mig);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty JSON string");

			var res = jsonSer.Deserialize<MenuItemModifierGroup> (json);
			Assert.IsNotNull (res);
			Assert.AreEqual ("testgroup", res.DisplayName);
		}

        [TestCase(SerializationType.JSONDOTNET)]
		[Test]
        public void TestMenuItem(SerializationType type)
        {
			var mi = new MenuItem {
				Id = Guid.NewGuid (),
				ButtonName = "bname",
				OIListName = "oilistname",
				Name = "name",
				Price = new Money (1.0M),
				PossibleModifiers = new List<MenuItemModifierGroup> {
					GetMenuItemModifierGroup ()
				}
			};

            var jsonSer = SerializerFactory.GetJSONSerializer(type);
            var json = jsonSer.Serialize(mi);

            Assert.IsFalse(string.IsNullOrEmpty(json), "empty JSON string");

            var res = jsonSer.Deserialize<MenuItem>(json);
            Assert.IsNotNull(res);
            Assert.AreEqual("bname", res.ButtonName);
        }

        [TestCase(SerializationType.JSONDOTNET)]
		[Test]
        public void TestMenuPopulated(SerializationType type)
        {
			var fms = new FakeDomineesRestaurantServiceClient ();

			var menu = fms.Menu;
            var jsonSer = SerializerFactory.GetJSONSerializer(type);
			var json = jsonSer.Serialize (menu);

			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = jsonSer.Deserialize<Menu> (json);
			Assert.IsNotNull (res);
		}

		[TestCase(SerializationType.JSONDOTNET)]
		[Test]
		public void TestMenuItemModifierEmpty(SerializationType type){
			var mod = new MenuItemModifier ();

			var jsonSer = SerializerFactory.GetJSONSerializer (type);
			var json = jsonSer.Serialize (mod);

			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = jsonSer.Deserialize<MenuItemModifier> (json);
			Assert.IsNotNull (res);
		}

		[TestCase(SerializationType.JSONDOTNET)]
		[Test]
		public void TestMenuItemModifierGroupEmpty(SerializationType type){
			var modG = new MenuItemModifierGroup ();

			var jsonSer = SerializerFactory.GetJSONSerializer (type);
			var json = jsonSer.Serialize (modG);

			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = jsonSer.Deserialize<MenuItemModifierGroup> (json);
			Assert.IsNotNull (res);
		}
	}
}

