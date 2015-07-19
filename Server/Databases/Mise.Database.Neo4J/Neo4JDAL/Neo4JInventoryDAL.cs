using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Database.StorableEntities;
using Mise.Database.StorableEntities.Inventory;

namespace Mise.Neo4J.Neo4JDAL
{
    public partial class Neo4JEntityDAL
    {
        public async Task<IEnumerable<IInventory>> GetInventoriesAsync(Guid restaurantID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(i:Inventory)")
                .Where((InventoryGraphNode i) => i.RestaurantID == restaurantID)
                .Return(i => i.As<InventoryGraphNode>())
                .ResultsAsync;

            var res = new List<IInventory>();
            foreach (var n in nodes)
            {
                var sections = await GetSectionsForInventoryAsync(n.ID);
                res.Add(n.Rehydrate(sections));
            }

            return res;
        }


        public async Task<IEnumerable<IInventory>> GetInventoriesAsync(DateTimeOffset dateSince)
        {
            var nodes = await _graphClient.Cypher
                .Match("(i:Inventory)")
                .Where((InventoryGraphNode i) => i.CreatedDate > dateSince)
                .Return(i => i.As<InventoryGraphNode>())
                .ResultsAsync;

            var res = new List<IInventory>();
            foreach (var node in nodes)
            {
                var sections = await GetSectionsForInventoryAsync(node.ID);
                res.Add(node.Rehydrate(sections));
            }

            return res;
        }

        public async Task AddInventoryAsync(IInventory inventory)
        {
            //if we have an inventory that is current, and this one is as well, we need to make it not
            //TODO move this to a business logic level, not the DAL
            if (inventory.IsCurrent)
            {
                var currentInventories = await _graphClient.Cypher
                    .Match("(i:Inventory)-[:INVENTORY]-(r:Restaurant)")
                    .Where((RestaurantGraphNode r) => r.ID == inventory.RestaurantID)
                    .AndWhere((InventoryGraphNode i) => i.IsCurrent == true)
                    .Return(i =>  i.As<InventoryGraphNode>())
                    .ResultsAsync;

                foreach (var currInv in currentInventories)
                {
                    var inv = currInv.ID;
                    var updateQuery = _graphClient.Cypher
                        .Match("(i:Inventory)")
                        .Where((InventoryGraphNode i) => i.ID == inv)
                        .Set("i.IsCurrent = 'false'");

                    await updateQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);
                }
            }

            var node = new InventoryGraphNode(inventory);
            await _graphClient.Cypher
                .Create("(inv:Inventory {inv})")
                .WithParam("inv", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            var comparer = new LiquidContainer.ContainerComparer();
            //we need to do the containers FIRST, otherwise they'll be done in parallel and create multiple copies
            var containers = inventory.GetBeverageLineItems().Select(li => li.Container).Distinct(comparer);
            foreach (var container in containers)
            {
                await CreateLiquidContainerIfNotAlreadyPresent(container);
            }

            var tasks = new List<Task>();
            tasks.AddRange(inventory.GetSections().Select(s => SetSectionForInventoryAsync(s, inventory.ID)));

            //associate us with the restaurant
            const string RELATIONSHIP_NAME = "INVENTORY";
            var restTask = _graphClient.Cypher
                .Match("(r:Restaurant), (inv:Inventory)")
                .Where((RestaurantGraphNode r) => r.ID == inventory.RestaurantID)
                .AndWhere((InventoryGraphNode inv) => inv.ID == inventory.ID)
                .CreateUnique("(r)-[:" + RELATIONSHIP_NAME + "]->(inv)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(restTask);

            //also our employee
            var empTask = _graphClient.Cypher
                .Match("(e:Employee), (inv:Inventory)")
                .Where((EmployeeGraphNode e) => e.ID == inventory.CreatedByEmployeeID)
                .AndWhere((InventoryGraphNode inv) => inv.ID == inventory.ID)
                .CreateUnique("(e)-[:CREATED]->(inv)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(empTask);

            foreach (var task in tasks)
            {
                await task.ConfigureAwait(false);
            }
        }

        public async Task UpdateInventoryAsync(IInventory inventory)
        {
            var invID = inventory.ID;
            //delete connections to line items, but not the lis themselves
            await _graphClient.Cypher
                .Match("(m)-[r2]-(li:InventoryBeverageLineItem)-[rSec:HAS_LINE_ITEM]-(sec:InventorySection)-[r:HAS_SECTION]-(i:Inventory)")
                .Where((InventoryGraphNode i) => i.ID == invID)
                .Delete("r2, rSec, li")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            //delete all the sections as well
            var query = _graphClient.Cypher
                .Match("(s:InventorySection)-[r:HAS_SECTION]-(i:Inventory)")
                .Where((InventoryGraphNode i) => i.ID == invID)
                .Delete("s, r");

            await query.ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            await _graphClient.Cypher
                .Match("(m)-[r]-(i:Inventory)")
                .Where((InventoryGraphNode i) => i.ID == invID)
                .Delete("r, i")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);


            await AddInventoryAsync(inventory);

        }

        private async Task<IEnumerable<InventorySection>> GetSectionsForInventoryAsync(Guid inventoryID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(i:Inventory)-[:HAS_SECTION]->(isec:InventorySection)")
                .Where((InventoryGraphNode i) => i.ID == inventoryID)
                .Return(isec => isec.As<InventorySectionGraphNode>())
                .ResultsAsync;

            var res = new List<InventorySection>();
            foreach (var n in nodes)
            {
                var lineItems = await GetLineItemsForInventorySectionAsync(n.ID);
                var restSectionID = await GetRestaurantIDForInventorySection(n.ID);
                res.Add(n.Rehydrate(lineItems, restSectionID));
            }
            return res;
        }

        private async Task<Guid> GetRestaurantIDForInventorySection(Guid invSectionID)
        {
            var items = await _graphClient.Cypher
                .Match("(isec:InventorySection)-[:INVENTORY_FOR_SECTION]-(ris:RestaurantInventorySection)")
                .Where("isec.ID = {param}")
                .WithParam("param", invSectionID)
                .Return(ris => ris.As<RestaurantInventorySectionGraphNode>())
                .ResultsAsync;

            var res = items.ToList();
            return res.Any() ? res.First().ID : Guid.Empty;
        }

        private Task DeleteInventorySectionAsync(Guid sectionID)
        {
            //find any sub items as well
            return _graphClient.Cypher
                .Match("(isec:InventorySection)-[:HAS_LINE_ITEM]->(li:InventoryLineItem)")
                .Where((InventorySectionGraphNode isec) => isec.ID == sectionID)
                .Delete("li, isec")
                .ExecuteWithoutResultsAsync();
        }

        private Task UpdateInventorySectionAsync(InventorySection section)
        {
            var updateNode = new InventorySectionGraphNode(section);
            return _graphClient.Cypher
                .Match("(isec:InventorySection)")
                .Where((InventorySectionGraphNode isec) => isec.ID == section.ID)
                .Set("isec = {secParam}")
                .WithParam("secParam", updateNode)
                .ExecuteWithoutResultsAsync()
                .ContinueWith(task =>
                {
                    var tasks = UpdateSubItemsWorker(
                        GetLineItemsForInventorySectionAsync,
                        SetLineItemsForInventorySectionAsync,
                        DeleteInventoryLineItemAsync,
                        UpdateInventoryLineItemAsync,
                        section.LineItems,
                        section.ID
                        );

                    Task.WaitAll(tasks.ToArray());
                });

        }

        private async Task SetSectionForInventoryAsync(IInventorySection section, Guid inventoryID)
        {
            //each inventory will carry its own copy of sections
            var node = new InventorySectionGraphNode(section);
            await _graphClient.Cypher
                .Create("(isec:InventorySection {param})")
                .WithParam("param", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);


            var tasks =
                section.GetInventoryBeverageLineItemsInSection()
                    .Select(li => SetLineItemsForInventorySectionAsync(li, section.ID)).ToList();

            var associateTask = _graphClient.Cypher
                .Match("(i:Inventory), (isec:InventorySection)")
                .Where((InventoryGraphNode i) => i.ID == inventoryID)
                .AndWhere((InventorySectionGraphNode isec) => isec.ID == section.ID)
                .CreateUnique("(i)-[:HAS_SECTION]->(isec)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(associateTask);

            var restSec = _graphClient.Cypher
                .Match("(isec:InventorySection), (rsec:RestaurantInventorySection)")
                .Where((InventorySectionGraphNode isec) => isec.ID == section.ID)
                .AndWhere((RestaurantInventorySection rsec) => rsec.ID == section.RestaurantInventorySectionID)
                .CreateUnique("(isec)-[:INVENTORY_FOR_SECTION]->(rsec)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(restSec);

            foreach (var t in tasks)
            {
                await t.ConfigureAwait(false);
            }
        }

        private async Task<IEnumerable<InventoryBeverageLineItem>> GetLineItemsForInventorySectionAsync(Guid inventorySectionID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(i:InventorySection)-[:HAS_LINE_ITEM]->(il:InventoryBeverageLineItem)")
                .Where((InventorySectionGraphNode i) => i.ID == inventorySectionID)
                .Return(il => il.As<InventoryBeverageLineItemGraphNode>())
                .ResultsAsync;

            var res = new List<InventoryBeverageLineItem>();
            foreach (var ln in nodes)
            {
                var containers = await GetLiquidContainersForEntityAsync(ln.ID);
                var categories = await GetCategoriesForItem(ln.ID);
                res.Add(ln.Rehydrate(containers.FirstOrDefault(), categories));
            }
            return res;
        }

        private Task UpdateInventoryLineItemAsync(IInventoryBeverageLineItem lineItem)
        {
            var node = new InventoryBeverageLineItemGraphNode(lineItem);
            return _graphClient.Cypher
                .Match("(li:InventoryBeverageLineItem)")
                .Where((InventoryBeverageLineItemGraphNode li) => li.ID == lineItem.ID)
                .Set("li = {liParam}")
                .WithParam("liParam", node)
                .ExecuteWithoutResultsAsync()
                .ContinueWith(task => UpdateLiquidContainerAsync(lineItem.Container, lineItem.ID));
        }

        private Task DeleteInventoryLineItemAsync(Guid lineItemID)
        {
            return _graphClient.Cypher
                .Match("(li:InventoryBeverageLineItem)-[r]-()")
                .Where("li.ID = {guid}")
                .WithParam("guid", lineItemID)
                .Delete("li, r")
                .ExecuteWithoutResultsAsync();
        }

        private async Task SetLineItemsForInventorySectionAsync(IInventoryBeverageLineItem lineItem, Guid inventorySectionID)
        {
            var node = new InventoryBeverageLineItemGraphNode(lineItem);
            await _graphClient.Cypher
                .Create("(li:InventoryBeverageLineItem {liParam})")
                .WithParam("liParam", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            var containerTask = SetLiquidContainerAsync(lineItem.Container, lineItem.ID);

            //also add the relationship
            var assocQuery = _graphClient.Cypher
                .Match("(m:InventorySection), (li:InventoryBeverageLineItem)")
                .Where("m.ID = {guid}")
                .AndWhere("li.ID = {liGuid}")
                .WithParam("guid", inventorySectionID)
                .WithParam("liGuid", lineItem.ID)
                .CreateUnique("m-[:HAS_LINE_ITEM]->(li)")
                .ExecuteWithoutResultsAsync();

            //add the categories
            await SetCategoryOnLineItem(lineItem);

            await containerTask.ConfigureAwait(false);
            await assocQuery.ConfigureAwait(false);
        }

        private async Task<Guid?> GetLastMeasuredInventoryForRestaurant(Guid restaurantID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(r:Restaurant)-[:MEASURED_INVENTORY]->(i:Inventory)")
                .Where((RestaurantGraphNode r) => r.ID == restaurantID)
                .Return(i => i.As<InventoryGraphNode>())
                .ResultsAsync;

            var finishedNodes = nodes
                .Where(n => n.DateCompleted.HasValue)
                .OrderByDescending(n => n.DateCompleted);

            if (finishedNodes.Any())
            {
                return (Guid?)finishedNodes.First().ID;
            }
            return null;
        }

        private async Task<Guid?> GetCurrentInventoryForRestaruant(Guid restaurantID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(r:Restaurant)-[:INVENTORY]->(i:Inventory)")
                .Where((RestaurantGraphNode r) => r.ID == restaurantID)
                .AndWhere((InventoryGraphNode i) => i.IsCurrent == true)
                .Return(i => i.As<InventoryGraphNode>().ID)
                .ResultsAsync;

            var list = nodes.ToList();
            if (list.Any())
            {
                return (Guid?)list.First();
            }
            return null;
        }

    }
}
