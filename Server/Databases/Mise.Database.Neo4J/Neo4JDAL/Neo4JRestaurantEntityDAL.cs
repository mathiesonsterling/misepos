using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;
using Mise.Neo4J.StorableEntities;
using Mise.Neo4J.StorableEntities.Accounts;
using Mise.Neo4J.StorableEntities.ValueItems;

namespace Mise.Neo4J.Neo4JDAL 
{
    public partial class Neo4JEntityDAL
    {
        #region Restaurant
        public async Task<IRestaurant> GetRestaurantAsync(Guid restaurantID)
        {
            var rests = await _graphClient.Cypher
                .Match("(r:Restaurant)")
                .Where((RestaurantGraphNode r) => r.ID == restaurantID)
                .Return(r => r.As<RestaurantGraphNode>())
                .ResultsAsync;

            _logger.Log("Query created");

            var data = rests.FirstOrDefault();
            if (data == null)
            {
                return null;
            }
            //var terms = data.Terminals.Select(t => t.Data.Rehydrate());

            var inventorySections = await GetRestaurantInventorySections(restaurantID);

            var address = await GetAddressAsync(restaurantID);

            //TODO - do we have an account associated with this restaurant?  If so, get the ID

            var lastMeasuredInventoryID = await GetLastMeasuredInventoryForRestaurant(restaurantID);
            var currentInventoryID = await GetCurrentInventoryForRestaruant(restaurantID);
            return data.Rehydrate(address, new List<MiseTerminalDevice>(), null, inventorySections, currentInventoryID, lastMeasuredInventoryID);
        }

        public async Task<IEnumerable<IRestaurant>> GetRestaurantsAsync()
        {
            var nodes = await _graphClient.Cypher
                .Match("(r:Restaurant)")
                .Return(r => r.As<RestaurantGraphNode>())
                .ResultsAsync;

            _logger.Log("Query created");

            var results = (from data in nodes
                           where data != null
                           let inventorySections = GetRestaurantInventorySections(data.ID)
                           let address = GetAddressAsync(data.ID)
                           let lastMeasuredInventoryID = GetLastMeasuredInventoryForRestaurant(data.ID)
                           let currentInventoryID = GetCurrentInventoryForRestaruant(data.ID)
                           select
                               data.Rehydrate(address.Result, new List<MiseTerminalDevice>(), null, inventorySections.Result,
                                   currentInventoryID.Result, lastMeasuredInventoryID.Result));

            return results;
        }

        private async Task<IEnumerable<RestaurantInventorySection>> GetRestaurantInventorySections(Guid restaurantID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(r:Restaurant)-[:HAS_INVENTORY_SECTION]->(ris:RestaurantInventorySection)")
                .Where("r.ID = {param}")
                .WithParam("param", restaurantID)
                .Return(ris => ris.As<RestaurantInventorySectionGraphNode>())
                .ResultsAsync;

            //get the beacon if any
            var rehydratedSections = new List<RestaurantInventorySection>();
            foreach (var n in nodes)
            {
                var beacon = await GetBeaconForRestaurantSection(n.ID);
                var done = n.Rehydrate(beacon);
                rehydratedSections.Add(done);
            }
            return rehydratedSections;

        }

        public async Task AddRestaurantAsync(IRestaurant restaurant)
        {

            _logger.Log("Adding restaurant " + restaurant.Name + " to graph database");

            var query = _graphClient.Cypher
                .Create(
                    "(restaurant:Restaurant {restaurant})")
                .WithParam("restaurant", new RestaurantGraphNode(restaurant));
            await query.ExecuteWithoutResultsAsync();

            //find our account and tie to it - do we want to not have our account later?
            if (restaurant.AccountID.HasValue)
            {
                var acctID = restaurant.AccountID;
                var restID = restaurant.ID;
                var accountQuery = _graphClient.Cypher
                    .Match("(acct:MiseAccount), (r:Restaurant)")
                    .Where((AccountGraphNode acct) => acct.ID == acctID)
                    .AndWhere((RestaurantGraphNode r) => r.ID == restID)
                    .CreateUnique("(r)-[:MISE_ACCOUNT]->(acct)");
                await accountQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);
            }

            //ADDRESS
            if (restaurant.StreetAddress != null)
            {
                await SetAddress(restaurant.StreetAddress, restaurant.ID, new List<string> { "PHYSICAL_LOCATION" });
            }

            //TODO renable discount percentages sometime!
            /*
            foreach (var discount in downRest.DiscountPercentages)
            {
                SetDiscountPercentage(discount, restaurant.ID);
            }

            foreach (var discount in downRest.DiscountPercentageAfterMinimumCashTotals)
            {
                //TODO check if this exists already!
                var source = new DiscountPercentageAfterMinimumCashTotalGraphNode(discount);
                var disQuery = _graphClient.Cypher
                    .Match("(rest:Restaurant)")
                    .Where((IRestaurant rest) => restaurant.ID == rest.ID)
                    .Create("rest-[:POSSIBLE_DISCOUNT]->(dis:Discount {discount})")
                    .WithParam("discount", source);
                disQuery.ExecuteWithoutResults();
            }
            */

            //TERMINALS
            foreach (var terminal in restaurant.GetTerminals())
            {
                AddRestaurantTerminal(terminal, restaurant.ID);
            }


            //INVENTORY SECTIONS
            var tasks =
                restaurant.GetInventorySections().Select(s => SetRestaurantInventorySectionAsync(s, restaurant.ID));

            foreach (var t in tasks)
            {
                await t.ConfigureAwait(false);
            }
        }

        public async Task SetRestaurantInventorySectionAsync(IRestaurantInventorySection restaurantInventorySection,
            Guid restaurantID)
        {
            var node = new RestaurantInventorySectionGraphNode(restaurantInventorySection);
            await _graphClient.Cypher
                .Create("(ris:RestaurantInventorySection {risParam})")
                .WithParam("risParam", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);


            var tasks = new List<Task>();

            //link
            var linkTask = _graphClient.Cypher
                .Match("(r:Restaurant), (ris:RestaurantInventorySection)")
                .Where((RestaurantGraphNode r) => r.ID == restaurantID)
                .AndWhere((RestaurantInventorySectionGraphNode ris) => ris.ID == restaurantInventorySection.ID)
                .CreateUnique("(r)-[:HAS_INVENTORY_SECTION]->(ris)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(linkTask);

            //add beacon
            if (restaurantInventorySection.Beacon != null)
            {
                tasks.Add(SetBeaconForRestaurantSection(restaurantInventorySection.Beacon, restaurantInventorySection.ID));
            }

            foreach (var t in tasks)
            {
                await t.ConfigureAwait(false);
            }
        }

        private async Task<Beacon> GetBeaconForRestaurantSection(Guid restaurantSectionID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(ris:RestaurantInventorySection)-[:HAS_SECTION_BEACON]->(b:Beacon)")
                .Where("ris.ID = {param}")
                .WithParam("param", restaurantSectionID)
                .Return(b => b.As<BeaconGraphNode>())
                .ResultsAsync;

            var node = nodes.FirstOrDefault();
            return node != null ? node.Rehydrate() : null;
        }

        private async Task SetBeaconForRestaurantSection(Beacon beacon, Guid inventorySectionID)
        {
            //does the beacon exist?
            var nodes = await _graphClient.Cypher
                .Match("(b:Beacon)")
                .Where(
                    (BeaconGraphNode b) => b.UUID == beacon.UUID && b.Major == beacon.Major && b.Minor == beacon.Minor)
                .Return(b => b.As<BeaconGraphNode>())
                .ResultsAsync
                .ConfigureAwait(false);

            if (nodes.Any() == false)
            {
                //create it
                await _graphClient.Cypher
                     .Create("(b:Beacon {beacon})")
                     .WithParam("beacon", new BeaconGraphNode(beacon))
                     .ExecuteWithoutResultsAsync()
                     .ConfigureAwait(false);
            }

            //link it
            await _graphClient.Cypher
                .Match("(b:Beacon), (ris:RestaurantInventorySection)")
                .Where(
                    (BeaconGraphNode b) =>
                        b.UUID == beacon.UUID && b.Major == beacon.Major && b.Minor == beacon.Minor)
                .AndWhere((RestaurantInventorySectionGraphNode ris) => ris.ID == inventorySectionID)
                .CreateUnique("(ris)-[:HAS_SECTION_BEACON]->(b)")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);
        }

        public Task UpdateRestaurantAsync(IRestaurant restaurant)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
