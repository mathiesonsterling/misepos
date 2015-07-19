using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.UnitTests.Tools;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
	[TestFixture]
	public class InventorySerializationTests
	{
		[TestCase(SerializationType.JSONDOTNET)]
		[Test]
		public void EmptyObjectShouldSerialize(SerializationType type){
			var serializer = SerializerFactory.GetJSONSerializer(type);
			var underTest = new Common.Entities.Inventory.Inventory ();

			var json = serializer.Serialize(underTest);
			Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

			var res = serializer.Deserialize<Common.Entities.Inventory.Inventory>(json);

			Assert.AreEqual(Guid.Empty, res.ID);
			Assert.IsNotNull(res);
		}

        [TestCase(SerializationType.JSONDOTNET)]
	    [Test]
	    public void LineItemCategoriesShouldSerialAndDeserial(SerializationType type)
	    {
	        var catID = Guid.NewGuid();
            var serializer = SerializerFactory.GetJSONSerializer(type);
            var underTest = new Common.Entities.Inventory.Inventory
            {
                ID = Guid.NewGuid(),
                Sections = new List<InventorySection>
                {
                    new InventorySection
                    {
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        ID = Guid.NewGuid(), 
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                ID = Guid.NewGuid(),
                                Categories = new List<ItemCategory>
                                {
                                    new ItemCategory
                                    {
                                        ID = catID,
                                        IsCustomCategory = false,
                                        ParentCategoryID = Guid.NewGuid(),
                                        Name = "testCat"
                                    },
                                    CategoriesService.Wine
                                }
                            }
                        }
                    }
                }
            };

            var json = serializer.Serialize(underTest);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<Common.Entities.Inventory.Inventory>(json);

            Assert.NotNull(res);
            Assert.AreEqual(underTest.ID, res.ID);
	        var sections = underTest.Sections;
            Assert.AreEqual(1, sections.Count);

	        var li = sections.First().LineItems.First();
            Assert.NotNull(li);

	        var cats = li.Categories;
            Assert.NotNull(cats);
            Assert.AreEqual(2, cats.Count);

	        var firstCat = cats.FirstOrDefault(c => c.ID == catID);
            Assert.NotNull(firstCat);
            Assert.AreEqual("testCat", firstCat.Name);
	    }
	}
}

