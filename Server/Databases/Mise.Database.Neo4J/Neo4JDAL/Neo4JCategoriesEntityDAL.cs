using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;

namespace Mise.Neo4J.Neo4JDAL
{
    public partial class Neo4JEntityDAL
    {
        public async Task CreateCategories(ICollection<ICategory> categories)
        {
            //find our top level ones first
            var topLevels = categories.Where(c => c.ParentCategoryID.HasValue == false);

            //create these first
            foreach (var topLevelCat in topLevels)
            {
                var catID = topLevelCat.ID;
                var existingNode = await _graphClient.Cypher
                    .Match("(ic:ItemCategory)")
                    .Where((ItemCategory ic) => ic.ID == catID)
                    .Return(ic => ic.As<ItemCategory>())
                    .ResultsAsync;

                if (existingNode.Any() == false)
                {
                    await _graphClient.Cypher
                        .Create("(ic:ItemCategory {itemCat})")
                        .WithParam("itemCat", topLevelCat)
                        .ExecuteWithoutResultsAsync()
                        .ConfigureAwait(false);
                }

                await CreateSubLevelCat(catID, categories);
            }

        }

        private async Task CreateSubLevelCat(Guid parentID, ICollection<ICategory> allCats)
        {
            var subCats = allCats.Where(c => c.ParentCategoryID.HasValue && c.ParentCategoryID == parentID);
            foreach (var sub in subCats)
            {
                var subCatID = sub.ID;
                var existingNode = await _graphClient.Cypher
                    .Match("(ic:ItemCategory)")
                    .Where((ItemCategory ic) => ic.ID == subCatID)
                    .Return(ic => ic.As<ItemCategory>())
                    .ResultsAsync;

                if (existingNode.Any() == false)
                {
                    //create the node
                    await _graphClient.Cypher
                        .Create("(ic:ItemCategory {itemCat})")
                        .WithParam("itemCat", sub)
                        .ExecuteWithoutResultsAsync()
                        .ConfigureAwait(false);

                    //tie it to the parent
                    await _graphClient.Cypher
                        .Match("(s:ItemCategory), (parent:ItemCategory)")
                        .Where((ItemCategory s) => s.ID == subCatID)
                        .AndWhere((ItemCategory parent) => parent.ID == parentID)
                        .CreateUnique("(s)-[:SUBCATEGORY]->(parent)")
                        .ExecuteWithoutResultsAsync();
                }

                //also get the subs of this!
                await CreateSubLevelCat(subCatID, allCats);
            }
        }

        private async Task SetCategoryOnLineItem(IBaseBeverageLineItem lineItem)
        {
            foreach (var category in lineItem.GetCategories())
            {
                var catID = category.ID;
                //does our category exist?
                var existingNode = await _graphClient.Cypher
                    .Match("(ic:ItemCategory)")
                    .Where((ItemCategory ic) => ic.ID == catID)
                    .Return(ic => ic.As<ItemCategory>())
                    .ResultsAsync;

                if (existingNode.Any() == false)
                {
                    await CreateCategories(new[] {category});
                }

                await _graphClient.Cypher
                    .Match("(c:ItemCategory), (item)")
                    .Where((ItemCategory c) => c.ID == catID)
                    .AndWhere("item.ID = '" + lineItem.ID + "'")
                    .CreateUnique("(item)-[:IN_CATEGORY]->(c)")
                    .ExecuteWithoutResultsAsync();
            }
        }

        private async Task<IEnumerable<ItemCategory>> GetCategoriesForItem(Guid itemID)
        {
            var cats = await _graphClient.Cypher
                .Match("(i)-[:IN_CATEGORY]->(c)")
                .Where("i.ID = {guid}")
                .WithParam("guid", itemID)
                .Return(c => c.As<ItemCategory>())
                .ResultsAsync;

            return cats;
        }
    }
}
