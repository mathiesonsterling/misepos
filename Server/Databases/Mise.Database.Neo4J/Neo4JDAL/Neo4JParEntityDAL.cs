using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Database.StorableEntities;
using Mise.Database.StorableEntities.Inventory;

namespace Mise.Neo4J.Neo4JDAL
{
    public partial class Neo4JEntityDAL
    {
        #region PAR
        public async Task AddPARAsync(IPAR par)
        {
            //create our node
            var node = new PARGraphNode(par);
            await _graphClient.Cypher
                .Create("(p:PAR {pa})")
                .WithParam("pa", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);


            //create any unknown containers
            var containers = par.GetBeverageLineItems().Select(bli => bli.Container).Distinct();
            foreach (var container in containers)
            {
                await CreateLiquidContainerIfNotAlreadyPresent(container);
            }

            //tie us to our restuarant
            await TiePARToRestaurantAsync(par).ConfigureAwait(false);

            //to our employee
            await _graphClient.Cypher
                .Match("(e:Employee), (p:PAR)")
                .Where((EmployeeGraphNode e) => e.ID == par.CreatedByEmployeeID)
                .AndWhere((PARGraphNode p) => p.ID == par.ID)
                .CreateUnique("(e)-[:CREATED]->(p)")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            //create the line items - do this in parallel if possible
            var tasks = par.GetBeverageLineItems().Select(li => SetLineItemsForPARAsync(li, par.ID));
            foreach (var task in tasks)
            {
                await task.ConfigureAwait(false);
            }
            /*foreach (var li in par.GetBeverageLineItems())
            {
                await SetLineItemsForPARAsync(li, par.ID);
            }*/
        }

        public async Task<IEnumerable<IPAR>> GetPARsAsync(Guid restaurantID)
        {
            var graphNodes = await _graphClient.Cypher
                .Match("(p:PAR)")
                .Where((PARGraphNode p) => p.RestaurantID == restaurantID)
                .Return(p => p.As<PARGraphNode>())
                .ResultsAsync;

            var res = new List<IPAR>();
            foreach (var node in graphNodes)
            {
                var lineItems = await GetLineItemsForPARAsync(node.ID);
                res.Add(node.Rehydrate(lineItems));
            }
            return res;
        }

        public async Task<IEnumerable<IPAR>> GetPARsAsync()
        {
            var graphNodes = await _graphClient.Cypher
                .Match("(p:PAR)")
                .Return(p => p.As<PARGraphNode>())
                .ResultsAsync;

            var res = new List<IPAR>();
            foreach (var node in graphNodes)
            {
                var lineItems = await GetLineItemsForPARAsync(node.ID);
                res.Add(node.Rehydrate(lineItems));
            }

            return res;
        }

        public async Task UpdatePARAsync(IPAR par)
        {
            //delete our LIs
            //TODO - since PARs can be very big, change this to instead only update items which are changed?
            await _graphClient.Cypher
                .Match("(m)-[r2]-(li:PARBeverageLineItem)-[r:HAS_LINE_ITEM]-(p:PAR)")
                .Where((PARGraphNode p) => p.ID == par.ID)
                .Delete("r2, r, li")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            await _graphClient.Cypher
                .Match("(p:PAR)-[r]-(m)")
                .Where((PARGraphNode p) => p.ID == par.ID)
                .Delete("r, p")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            await AddPARAsync(par);
        }

        private async Task TiePARToRestaurantAsync(IPAR par)
        {
            var relationship = par.IsCurrent ? "CURRENT_PAR" : "PAST_PAR";
            await _graphClient.Cypher
                .Match("(r:Restaurant), (p:PAR)")
                .Where((RestaurantGraphNode r) => r.ID == par.RestaurantID)
                .AndWhere((PARGraphNode p) => p.ID == par.ID)
                .CreateUnique("(r)-[:" + relationship + "]->(p)")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);
        }

        private async Task<IEnumerable<PARBeverageLineItem>> GetLineItemsForPARAsync(Guid parID)
        {
            var lineNodes = await _graphClient.Cypher
                .Match("(p:PAR)-[:HAS_LINE_ITEM]->(il:PARBeverageLineItem)")
                .Where((PARGraphNode p) => p.ID == parID)
                .Return(il => il.As<ParLineItemGraphNode>())
                .ResultsAsync;

            var nodesAndContainers = new List<Tuple<ParLineItemGraphNode, IEnumerable<LiquidContainer>, IEnumerable<ItemCategory>>>();
            foreach (var node in lineNodes.AsParallel())
            {
                var containers = await GetLiquidContainersForEntityAsync(node.ID);
                var categories = await GetCategoriesForItem(node.ID);
                nodesAndContainers.Add(
                    new Tuple<ParLineItemGraphNode, IEnumerable<LiquidContainer>, IEnumerable<ItemCategory>>(node, containers, categories));
            }
            //feed em in
            return nodesAndContainers.Select(nc => nc.Item1.Rehydrate(nc.Item2.FirstOrDefault(), nc.Item3));
        }

        public async Task UpdatePARLineItemAsync(IPARBeverageLineItem lineItem)
        {
            var node = new ParLineItemGraphNode(lineItem);
            await _graphClient.Cypher
                .Match("(li:PARBeverageLineItem)")
                .Where((InventoryBeverageLineItemGraphNode li) => li.ID == lineItem.ID)
                .Set("li = {liParam}")
                .WithParam("liParam", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            await UpdateLiquidContainerAsync(lineItem.Container, lineItem.ID).ConfigureAwait(false);
        }

        public async Task DeletePARLineItemAsync(Guid lineItemID)
        {
            await _graphClient.Cypher
                .Match("(li:PARBeverageLineItem)-[r]-()")
                .Where("li.ID = {guid}")
                .WithParam("guid", lineItemID)
                .Delete("li, r")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);
        }

        public async Task SetLineItemsForPARAsync(IPARBeverageLineItem lineItem, Guid parID)
        {
            var node = new ParLineItemGraphNode(lineItem);
            await _graphClient.Cypher
                .Create("(li:PARBeverageLineItem {liParam})")
                .WithParam("liParam", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            var containerTask = SetLiquidContainerAsync(lineItem.Container, lineItem.ID);

            //also add the relationship
            var assocQuery = _graphClient.Cypher
                .Match("(m:PAR), (li:PARBeverageLineItem)")
                .Where("m.ID = {guid}")
                .AndWhere("li.ID = {liGuid}")
                .WithParam("guid", parID)
                .WithParam("liGuid", lineItem.ID)
                .CreateUnique("m-[:HAS_LINE_ITEM]->(li)")
                .ExecuteWithoutResultsAsync();

            //add the category
            await SetCategoryOnLineItem(lineItem);

            await containerTask.ConfigureAwait(false);
            await assocQuery.ConfigureAwait(false);
        }
        #endregion
    }
}
