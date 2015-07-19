using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems.Inventory;
using Mise.Database.StorableEntities;
using Mise.Database.StorableEntities.Inventory;
using Mise.Neo4J.Relationships;
using Neo4jClient;

namespace Mise.Neo4J.Neo4JDAL
{
    public partial class Neo4JEntityDAL
    {
        public async Task<IEnumerable<IVendor>> GetVendorsAsync()
        {
            var nodes = await _graphClient.Cypher
                .Match("(v:Vendor)")
                .Return(v => v.As<VendorGraphNode>())
                .ResultsAsync;

            var res = new List<IVendor>();
            foreach (var n in nodes)
            {
                var lis = await GetVendorBeverageLineItemsAsync(n.ID);
                var address = await GetAddressAsync(n.ID);
                var restaurants = await GetRestaurantIDsAssociatedWithVendor(n.ID);

                var emails = await GetEmailsAssociatedWithEntity(n.ID, "ORDERING_EMAIL");
                res.Add(n.Rehydrate(restaurants, address, lis, emails.FirstOrDefault()));
            }

            return res;
        }

        public async Task AddVendorAsync(IVendor vendor)
        {
            var node = new VendorGraphNode(vendor);
            await _graphClient.Cypher
                .Create("(v:Vendor {ven})")
                .WithParam("ven", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            var comparer = new LiquidContainer.ContainerComparer();
            //we need to do the containers FIRST, otherwise they'll be done in parallel and create multiple copies
            var containers = vendor.GetItemsVendorSells().Select(li => li.Container).Distinct(comparer);
            foreach (var container in containers)
            {
                await CreateLiquidContainerIfNotAlreadyPresent(container);
            }

            var tasks = new List<Task>();
            //add our address
            if (vendor.StreetAddress != null)
            {
                await SetAddress(vendor.StreetAddress, vendor.ID, new List<string>());
            }

            if (vendor.EmailToOrderFrom != null)
            {
                await SetEmailAddressOnEntityAsync(vendor.EmailToOrderFrom, vendor.ID, "ORDERING_EMAIL");
            }

            //add relationship to any restaurant ID listed - first, so our line items can add prices if needed
            tasks.AddRange(vendor.GetRestaurantIDsAssociatedWithVendor().Select(rID => AssociateVendorWithRestaurantAsync(vendor.ID, rID)));

            //add the line items
            var liTasks = vendor.GetItemsVendorSells().Select(li => SetVendorLineItem(li, vendor.ID));
            tasks.AddRange(liTasks);

            //add employee relationship
            var empTask = _graphClient.Cypher
                .Match("(e:Employee), (v:Vendor)")
                .Where((EmployeeGraphNode e) => e.ID == vendor.CreatedByEmployeeID)
                .AndWhere((VendorGraphNode v) => v.ID == vendor.ID)
                .CreateUnique("(e)-[:ADDED_VENDOR]->(v)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(empTask);

            foreach (var t in tasks)
            {
                await t.ConfigureAwait(false);
            }
        }

        public async Task UpdateVendorAsync(IVendor vendor)
        {
            //delete all the line items as well
            var liIDs = vendor.GetItemsVendorSells().Select(li => li.ID);
            foreach (var liID in liIDs)
            {
                var id = liID;
                var query = _graphClient.Cypher
                    .Match("(li:VendorBeverageLineItem)-[r]-()")
                    .Where((VendorBeverageLineItemGraphNode li) => id == li.ID)
                    .Delete("li, r");

                await query.ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            var delQuery = _graphClient.Cypher
                .Match("(v:Vendor)-[r]-()")
                .Where((VendorGraphNode v) => v.ID == vendor.ID)
                .Delete("v, r");
            await delQuery.ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            await AddVendorAsync(vendor);
        }

        public Task<IEnumerable<Guid>> GetRestaurantIDsAssociatedWithVendor(Guid vendorID)
        {
            return _graphClient.Cypher
                .Match("(v:Vendor)-[:SELLS_TO]-(r:Restaurant)")
                .Where((VendorGraphNode v) => v.ID == vendorID)
                .Return(r => r.As<RestaurantGraphNode>().ID)
                .ResultsAsync;
        }

        private async Task<IEnumerable<VendorBeverageLineItem>> GetVendorBeverageLineItemsAsync(Guid vendorID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(v:Vendor)-[:HAS_LINE_ITEM]->(il:VendorBeverageLineItem)")
                .Where((VendorGraphNode v) => v.ID == vendorID)
                .Return(il => il.As<VendorBeverageLineItemGraphNode>())
                .ResultsAsync;

            var res = new List<VendorBeverageLineItem>();
            foreach (var n in nodes)
            {
                var restaurantPrices = new Dictionary<Guid, decimal>();
                var id = n.ID;
                var priceResults = await _graphClient.Cypher
                    .Match("(li:VendorBeverageLineItem)-[rel:PAID_FOR_ITEM]->(r:Restaurant)")
                    .Where((VendorBeverageLineItemGraphNode li) => li.ID == id)
                    .Return((r, rel) => new { Restaurant = r.As<RestaurantGraphNode>().ID, RelAtts = rel.As<RelationshipInstance<PricePaidForItemPayload>>() })
                    .ResultsAsync;
                foreach (var dbRes in priceResults)
                {
                    if (dbRes.RelAtts != null)
                    {
                        var price = dbRes.RelAtts.Data.Price;
                        restaurantPrices[dbRes.Restaurant] = price;
                    }
                }

                var containers = await GetLiquidContainersForEntityAsync(n.ID);
                var categories = await GetCategoriesForItem(n.ID);
                res.Add(n.Rehydrate(containers.FirstOrDefault(), categories, restaurantPrices));
            }
            return res;
        }

        private async Task SetVendorLineItem(IVendorBeverageLineItem lineItem, Guid vendorID)
        {
            var node = new VendorBeverageLineItemGraphNode(lineItem);
            await _graphClient.Cypher
                .Create("(li:VendorBeverageLineItem {liParam})")
                .WithParam("liParam", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            await SetLiquidContainerAsync(lineItem.Container, lineItem.ID).ConfigureAwait(false);

            //also add the relationship
            await _graphClient.Cypher
                .Match("(m:Vendor), (li:VendorBeverageLineItem)")
                .Where("m.ID = {guid}")
                .AndWhere("li.ID = {liGuid}")
                .WithParam("guid", vendorID)
                .WithParam("liGuid", lineItem.ID)
                .CreateUnique("m-[:HAS_LINE_ITEM]->(li)")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            //any restaurant specific prices should be added as relationships
            var restsAndPrices = lineItem.GetPricesForRestaurants();
            foreach (var restID in restsAndPrices.Keys)
            {
                var id = restID;
                await _graphClient.Cypher
                    .Match("(li:VendorBeverageLineItem), (r:Restaurant)")
                    .Where((VendorBeverageLineItemGraphNode li) => li.ID == lineItem.ID)
                    .AndWhere((RestaurantGraphNode r) => r.ID == id)
                    .CreateUnique("(li)-[:PAID_FOR_ITEM{Price : {price}}]->(r)")
                    .WithParam("price", restsAndPrices[restID].Dollars)
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            //any categories
            await SetCategoryOnLineItem(lineItem);
        }

        private async Task AssociateVendorWithRestaurantAsync(Guid vendorID, Guid restaurantID)
        {
            await _graphClient.Cypher
                .Match("(r:Restaurant), (v:Vendor)")
                .Where((RestaurantGraphNode r) => r.ID == restaurantID)
                .AndWhere((VendorGraphNode v) => v.ID == vendorID)
                .CreateUnique("(v)-[:SELLS_TO]->(r)")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);
        }
    }
}
