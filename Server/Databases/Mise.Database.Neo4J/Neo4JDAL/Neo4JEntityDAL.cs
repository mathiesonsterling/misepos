using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;
using Mise.Core.Server;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Database.StorableEntities;
using Mise.Database.StorableEntities.Accounts;
using Mise.Database.StorableEntities.Inventory;
using Mise.Database.StorableEntities.ValueItems;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Mise.Neo4J.Neo4JDAL
{
    public partial class Neo4JEntityDAL : IEntityDAL
    {
        private const string ID_INDEX = "IDIndex";
        private readonly GraphClient _graphClient;
        private readonly ILogger _logger;
        public Neo4JEntityDAL(IConfig config, ILogger logger)
        {
            _logger = logger;

            var uri = config.Neo4JConnectionDBUri;
            _logger.Log("Attempting to connect to Neo4JServer at " + uri, LogLevel.Info);
            _graphClient = new GraphClient(uri);

            _graphClient.Connect();

            _logger.Log("Connected to " + uri, LogLevel.Info);
        }

        /// <summary>
        /// Deletes the entire graph database. USE CAREFULLY!
        /// </summary>
        public void ResetDatabase()
        {
            var query = _graphClient.Cypher
                .Start(new { n = All.Nodes })
                .OptionalMatch("(n)-[r]-(x)")
                .With("n, r")
                .Delete("n, r");

            query.ExecuteWithoutResults();

            if (_graphClient.CheckIndexExists(ID_INDEX, IndexFor.Node) == false)
            {
                var config = new IndexConfiguration { Provider = IndexProvider.lucene, Type = IndexType.fulltext };
                _graphClient.CreateIndex(ID_INDEX, config, IndexFor.Node);
            }
        }

        #region IAdminServerDAL


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

            var results = new List<IRestaurant>();
            foreach (var data in nodes)
            {
                var inventorySections = await GetRestaurantInventorySections(data.ID);
                var address = await GetAddressAsync(data.ID);
                var lastMeasuredInventoryID = await GetLastMeasuredInventoryForRestaurant(data.ID);
                var currentInventoryID = await GetCurrentInventoryForRestaruant(data.ID);

                var rest = data.Rehydrate(address, new List<MiseTerminalDevice>(), null, inventorySections,
                    currentInventoryID, lastMeasuredInventoryID);

                results.Add(rest);
            }

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
            //create

            //link

            //add beacon
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

        public async Task UpdateRestaurantAsync(IRestaurant restaurant)
        {
            _logger.Warn("Update restaurant, will ONLY update basic fields for now");

            var updateNode = new RestaurantGraphNode(restaurant);

            await _graphClient.Cypher
                .Match("(r:Restaurant)")
                .Where((RestaurantGraphNode r) => r.ID == restaurant.ID)
                .Set("r = {rNode}")
                .WithParam("rNode", updateNode)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);
        }
        #endregion

        #region Menus

        private async Task<Menu> LoadMenu(MenuGraphNode menuNode)
        {
            //get all the categories for this menu
            var catsQuery = _graphClient.Cypher
                .Match("(menu:Menu)-[:HAS_CATEGORY]->(mic:MenuItemCategory), (menu:Menu)-[:DEFAULT_MISE_ITEM]->(mi:MenuItem)")
                .Where((MenuGraphNode menu) => menu.ID == menuNode.ID)
                .Return(mic => mic.As<MenuItemCategoryGraphNode>());

            var catNodes = await catsQuery.ResultsAsync;
            var cats = new List<MenuItemCategory>();
            foreach (var catNode in catNodes)
            {
                var loadedCat = await LoadCategory(catNode);
                cats.Add(loadedCat);
            }


            //get the menu items that are default mise items as well
            var defaultMiseItemsQuery = _graphClient.Cypher
                .Match("(menu:Menu)-[:DEFAULT_MISE_ITEM]->(menuItem:MenuItem)")
                .Where((MenuGraphNode menu) => menu.ID == menuNode.ID)
                .Return(menuItem => menuItem.As<MenuItemGraphNode>().ID);

            var defaultIDs = await defaultMiseItemsQuery.ResultsAsync;

            return menuNode.Rehydrate(defaultIDs, cats);
        }

        // ReSharper disable once UnusedParameter.Local
        private Task<MenuItemCategory> LoadCategory(MenuItemCategoryGraphNode catNode)
        {
            throw new NotImplementedException();
            //get our menu items

            //get our sub categories

            // return catNode.Rehydrate()
        }

        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        private Task<MenuItem> LoadMenuItem(MenuItemGraphNode menuItemNode)
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Menu>> GetMenusAsync(Guid restaurantID)
        {
            _logger.Log("Retrieving menus for restaurant " + restaurantID);
            //TODO get the subparts to rehydrate.  Since we need the structure, can't do this in one query yet

            var nodes = await _graphClient.Cypher
                .Match("(menu:Menu)")
                .Where((MenuGraphNode menu) => menu.RestaurantID == restaurantID)
                .Return(m => m.As<MenuGraphNode>())
                .ResultsAsync;

            var res = new List<Menu>();
            foreach (var mg in nodes)
            {
                var loaded = await LoadMenu(mg);
                res.Add(loaded);
            }
            return res;
        }



        public Task<Menu> GetMenuByIDAsync(Guid menuID)
        {
            return GetMenuByID(menuID);
        }

        private async Task AddMenu(Menu menu)
        {
            _logger.Log("Adding menu to Neo4J");

            //create if it doesn't exist!
            var existing = await _graphClient.Cypher
                .Match("(m:Menu)")
                .Where((MenuGraphNode m) => m.ID == menu.ID)
                .Return(m => m.Id())
                .ResultsAsync
                .ConfigureAwait(false);

            if (existing.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(menu:Menu {menuG})")
                    .WithParam("menuG", new MenuGraphNode(menu))
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);

                //link to restaurant
                await _graphClient.Cypher
                    .Match("(rest:Restaurant), (m:Menu)")
                    .Where((RestaurantGraphNode rest) => rest.ID == menu.RestaurantID)
                    .AndWhere((MenuGraphNode m) => m.ID == menu.ID)
                    .CreateUnique("(rest)-[:HAS_MENU]->(m)")
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            //add each category
            foreach (var c in menu.GetMenuItemCategories())
            {
                await AddTopLevelSubcat(c, menu.ID);
            }

            //add the default mise items, its just relations to each menu item
            foreach (var mi in menu.GetDefaultMiseItems())
            {
                await AddMenuItemAsDefaultMiseItem(mi, menu.ID);
            }
        }

        private async Task AddMenuItemAsDefaultMiseItem(MenuItem mi, Guid menuID)
        {
            //menu item should already exist, but should we be clean about it?
            var menuItemNodes = await _graphClient.Cypher
                .Match("(menuItem:MenuItem)")
                .Where((MenuItemGraphNode menuItem) => menuItem.ID == mi.ID)
                .Return(menuItem => menuItem.Id())
                .ResultsAsync;
            if (menuItemNodes.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(menuItem:MenuItem {menuItem})")
                    .WithParam("menuItem", new MenuItemGraphNode(mi))
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            //link it up
            var relQuery = _graphClient.Cypher
                .Match("(menu:Menu)", "(menuItem:MenuItem)")
                .Where((MenuGraphNode menu) => menu.ID == menuID)
                .AndWhere((MenuItemGraphNode menuItem) => menuItem.ID == mi.ID)
                .CreateUnique("(menu)-[:DEFAULT_MISE_ITEM]->(menuItem)");

            await relQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);
        }

        private async Task AddTopLevelSubcat(MenuItemCategory cat, Guid menuID)
        {
            var subNodes = await _graphClient.Cypher
                .Match("(menuItemCategory:MenuItemCategory)")
                .Where((MenuItemCategoryGraphNode menuItemCategory) => menuItemCategory.ID == cat.ID)
                .Return(menuItemCategory => menuItemCategory.Id())
                .ResultsAsync
                .ConfigureAwait(false);

            if (subNodes.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(menuItemCategory:MenuItemCategory {menuItemCategory})")
                    .WithParam("menuItemCategory", new MenuItemCategoryGraphNode(cat))
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            //link it up
            await _graphClient.Cypher
                .Match("(menu:Menu)", "(category:MenuItemCategory)")
                .Where((MenuGraphNode menu) => menu.ID == menuID)
                .AndWhere((MenuItemCategoryGraphNode category) => category.ID == cat.ID)
                .CreateUnique("(menu)-[:HAS_CATEGORY]->(category)")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            //get a complete list of all menu items, and then a distinct list of order destinations
            //add the menu items
            foreach (var menuItem in cat.MenuItems)
            {
                await SetMenuItem(menuItem, cat);
            }

            //add the subcats of this
            foreach (var subCat in cat.SubCategories)
            {
                await AddSubcategory(subCat, cat);
            }
        }

        private async Task AddSubcategory(MenuItemCategory subCategory, MenuItemCategory parentNode)
        {
            var subNodes = await _graphClient.Cypher
                .Match("(menuItemCategory:MenuItemCategory)")
                .Where((MenuItemCategoryGraphNode menuItemCategory) => menuItemCategory.ID == subCategory.ID)
                .Return(menuItemCategory => menuItemCategory.Id())
                .ResultsAsync;
            if (subNodes.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(menuItemCategory:MenuItemCategory {menuItemCategory})")
                    .WithParam("menuItemCategory", new MenuItemCategoryGraphNode(subCategory))
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            //link it up
            var relQuery = _graphClient.Cypher
                .Match("(parent:MenuItemCategory)", "(sub:MenuItemCategory)")
                .Where((MenuItemCategoryGraphNode parent) => parent.ID == parentNode.ID)
                .AndWhere((MenuItemCategoryGraphNode sub) => subCategory.ID == sub.ID)
                .CreateUnique("(category)-[:HAS_CATEGORY]->(sub)");

            await relQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);

            foreach (var menuItem in subCategory.MenuItems)
            {
                await SetMenuItem(menuItem, subCategory);
            }

            foreach (var subOfThisCategory in subCategory.SubCategories)
            {
                await AddSubcategory(subOfThisCategory, subCategory);
            }
        }

        private async Task SetMenuItem(MenuItem mi, MenuItemCategory cat)
        {
            var menuItemNodes = await _graphClient.Cypher
                .Match("(menuItem:MenuItem)")
                .Where((MenuItemGraphNode menuItem) => menuItem.ID == mi.ID)
                .Return(menuItem => menuItem.Id())
                .ResultsAsync;
            if (menuItemNodes.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(menuItem:MenuItem {menuItem})")
                    .WithParam("menuItem", new MenuItemGraphNode(mi))
                    .ExecuteWithoutResultsAsync().ConfigureAwait(false);
            }

            //link it up
            var relQuery = _graphClient.Cypher
                .Match("(category:MenuItemCategory)", "(menuItem:MenuItem)")
                .Where((MenuItemCategoryGraphNode category) => cat.ID == category.ID)
                .AndWhere((MenuItemGraphNode menuItem) => menuItem.ID == mi.ID)
                .CreateUnique("(category)-[:MENU_ITEM]->(menuItem)");

            await relQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);


            //add the modifier groups
            foreach (var modGroup in mi.PossibleModifiers)
            {
                await SetMenuItemModifierGroup(modGroup, mi);
            }
        }

        private async Task SetMenuItemModifierGroup(MenuItemModifierGroup menuItemModifierGroup, MenuItem mi)
        {
            var modGroupNodes = await _graphClient.Cypher
                .Match("(migNode:MenuItemModifierGroup)")
                .Where((MenuItemModifierGroupGraphNode migNode) => migNode.ID == menuItemModifierGroup.ID)
                .Return(migNode => migNode.Id())
                .ResultsAsync;

            if (modGroupNodes.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(migNode:MenuItemModifierGroup {menuItemModifierGroup})")
                    .WithParam("menuItemModifierGroup", new MenuItemModifierGroupGraphNode(menuItemModifierGroup))
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            //link it up
            var relQuery = _graphClient.Cypher
                .Match("(miNode:MenuItem)", "(migNode:MenuItemModifierGroup)")
                .Where((MenuItemModifierGroupGraphNode migNode) => migNode.ID == menuItemModifierGroup.ID)
                .AndWhere((MenuItemGraphNode miNode) => miNode.ID == mi.ID)
                .CreateUnique("(miNode)-[:POSSIBLE_MODIFIERS]->(migNode)");

            await relQuery.ExecuteWithoutResultsAsync();


            //add each of our modifiers to the group
            foreach (var mod in menuItemModifierGroup.Modifiers)
            {
                SetMenuItemModifier(mod, menuItemModifierGroup);
            }
        }

        private void SetMenuItemModifier(MenuItemModifier mod, MenuItemModifierGroup group)
        {
            var modNodesExistingQuery = _graphClient.Cypher
                .Match("(modNode:MenuItemModifier)")
                .Where((MenuItemModifierGraphNode modNode) => modNode.ID == mod.ID)
                .Return(modNode => modNode.Id());

            var modGroupNodes = modNodesExistingQuery.Results;
            if (modGroupNodes.Any() == false)
            {
                _graphClient.Cypher
                    .Create("(modNode:MenuItemModifier {menuItemModifier})")
                    .WithParam("menuItemModifier", new MenuItemModifierGraphNode(mod))
                    .ExecuteWithoutResults();
            }

            //link it up
            var relQuery = _graphClient.Cypher
                .Match("(migNode:MenuItemModifierGroup)", "(modNode:MenuItemModifier)")
                .Where((MenuItemModifierGroupGraphNode migNode) => migNode.ID == group.ID)
                .AndWhere((MenuItemModifierGraphNode modNode) => modNode.ID == mod.ID)
                .CreateUnique("(migNode)-[:MODIFIER_CHOICE]->(modNode)");

            relQuery.ExecuteWithoutResults();
        }

        public Task AddMenuAsync(Menu menu)
        {
            return Task.Factory.StartNew(() => AddMenu(menu));
        }

        private async Task UpdateMenuProperties(Menu menu)
        {
            //to do - update fields, or delete and reinsert?
            var existing = await GetMenuByID(menu.ID);
            var hasPropChange = false;
            var updateProps = _graphClient.Cypher
                .Match("(menu:Menu)")
                .Where((Menu newMenu) => newMenu.ID == menu.ID);

            if (existing.DisplayName != menu.DisplayName)
            {
                updateProps.Set("menu.DisplayName = {displayName}")
                    .WithParam("displayName", menu.DisplayName);
                hasPropChange = true;
            }

            if (existing.LastUpdatedDate != menu.LastUpdatedDate)
            {
                updateProps.Set("menu.LastUpdatedDate = {lastUpdatedDate})")
                    .WithParam("lastUpdatedDate", menu.LastUpdatedDate);
                hasPropChange = true;
            }

            if (existing.Name != menu.Name)
            {
                updateProps.Set("menu.Name = {name}")
                    .WithParam("name", menu.Name);
                hasPropChange = true;
            }

            if (hasPropChange)
            {
                updateProps.ExecuteWithoutResults();
            }

            //check our sub items and see if those items need to be updated as well
        }

        public Task UpdateMenuAsync(Menu menu)
        {
            return Task.Factory.StartNew(() => UpdateMenuProperties(menu));
        }


        public Task<IEnumerable<MenuRule>> GetMenuRulesAsync(Guid restaurantID)
        {
            throw new NotImplementedException();
        }

        public Task AddMenuRuleAsync(MenuRule menuRule)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMenuRuleAsync(MenuRule menuRule)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Inventories








        #endregion

        #region Purchase Orders
        public async Task UpdatePurchaseOrderAsync(IPurchaseOrder purchaseOrder)
        {
            var updateNode = new PurchaseOrderGraphNode(purchaseOrder);
            await _graphClient.Cypher
            .Match("(po:PurchaseOrder)")
                .Where((PurchaseOrderGraphNode po) => po.ID == purchaseOrder.ID)
                .Set("po = {poParam}")
                .WithParam("poParam", updateNode)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            var tasks = new List<Task>();
            tasks.AddRange(UpdateSubItemsWorker(
                GetPurchaseOrderLineItemsAsync,
                SetPurchaseOrderLineItemAsync,
                DeletePurchaseOrderLineItemAsync,
                UpdatePurchaseOrderLineItemAsync,
                purchaseOrder.GetPurchaseOrderLineItems().Select(t => t as PurchaseOrderLineItem).ToList(),
                purchaseOrder.ID));


            foreach (var t in tasks)
            {
                await t.ConfigureAwait(false);
            }
        }

        public async Task AddPurchaseOrderAsync(IPurchaseOrder purchaseOrder)
        {
            var node = new PurchaseOrderGraphNode(purchaseOrder);
            await _graphClient.Cypher
                .Create("(po:PurchaseOrder {po})")
                .WithParam("po", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            //create any unknown containers
            var containers = purchaseOrder.GetPurchaseOrderLineItems().Select(bli => bli.Container).Distinct();
            foreach (var container in containers)
            {
                await CreateLiquidContainerIfNotAlreadyPresent(container);
            }


            //add the purchase order by vendor objects

            //add the line items
            var tasks = purchaseOrder.GetPurchaseOrderPerVendors().Select(pv => SetPurchaseOrderPerVendorAsync(pv, purchaseOrder.ID)).ToList();

            //associate us with the restaurant
            var assocTask = _graphClient.Cypher
                .Match("(po:PurchaseOrder), (r:Restaurant)")
                .Where((PurchaseOrderGraphNode po) => po.ID == purchaseOrder.ID)
                .AndWhere((RestaurantGraphNode r) => r.ID == purchaseOrder.RestaurantID)
                .CreateUnique("(r)-[:HAS_PURCHASE_ORDER]->(po)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(assocTask);

            //associate to the employee that created us
            var empTask = _graphClient.Cypher
                .Match("(po:PurchaseOrder), (e:Employee)")
                .Where((PurchaseOrderGraphNode po) => po.ID == purchaseOrder.ID)
                .AndWhere((EmployeeGraphNode e) => e.ID == purchaseOrder.CreatedByEmployeeID)
                .CreateUnique("(e)-[:CREATED]->(po)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(empTask);

            //assoc with employee
            foreach (var t in tasks)
            {
                await t.ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersAsync(DateTimeOffset maxTimeBack)
        {
            var nodes = await _graphClient.Cypher
            .Match("(po:PurchaseOrder)")
            .Where((PurchaseOrderGraphNode po) => po.LastUpdatedDate > maxTimeBack)
            .Return(po => po.As<PurchaseOrderGraphNode>())
            .ResultsAsync;

            var res = new List<IPurchaseOrder>();
            foreach (var n in nodes)
            {
                var pvs = await GetPurchaseOrderPerVendorsAsync(n.ID);
                res.Add(n.Rehydrate(pvs));
            }
            return res;
        }

        public async Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersAsync(Guid restaurantID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(po:PurchaseOrder)")
                .Where((PurchaseOrderGraphNode po) => po.RestaurantID == restaurantID)
                .Return(po => po.As<PurchaseOrderGraphNode>())
                .ResultsAsync;

            //get the line items, address, and restaurants for each vendor
            var nodesAndPerVendors =
                nodes.Select(n => new
                {
                    Node = n,
                    PVs = GetPurchaseOrderPerVendorsAsync(n.ID).Result
                });

            return nodesAndPerVendors.Select(nli => nli.Node.Rehydrate(nli.PVs));
        }

        private async Task<IEnumerable<PurchaseOrderPerVendor>> GetPurchaseOrderPerVendorsAsync(Guid poID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(po:PurchaseOrder)-[:PER_VENDOR]->(pv:PurchaseOrderPerVendor)")
                .Where((PurchaseOrderGraphNode po) => po.ID == poID)
                .Return(pv => pv.As<PurchaseOrderPerVendorGraphNode>())
                .ResultsAsync;

            var res = new List<PurchaseOrderPerVendor>();
            foreach (var n in nodes)
            {
                var lis = await GetPurchaseOrderLineItemsAsync(n.ID);
                res.Add(n.Rehydrate(lis));
            }
            return res;
        }

        private async Task<IEnumerable<PurchaseOrderLineItem>> GetPurchaseOrderLineItemsAsync(Guid pvID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(pv:PurchaseOrderPerVendor)-[:HAS_LINE_ITEM]->(li:PurchaseOrderLineItem)")
                .Where((PurchaseOrderPerVendorGraphNode pv) => pv.ID == pvID)
                .Return(li => li.As<PurchaseOrderLineItemGraphNode>())
                .ResultsAsync;

            var res = new List<PurchaseOrderLineItem>();
            foreach (var n in nodes)
            {
                var containers = await GetLiquidContainersForEntityAsync(n.ID);
                var categories = await GetCategoriesForItem(n.ID);
                res.Add(n.Rehydrate(containers.FirstOrDefault(), categories));
            }
            return res;
        }

        private async Task SetPurchaseOrderPerVendorAsync(IPurchaseOrderPerVendor purchaseOrderPerVendor, Guid poID)
        {
            var node = new PurchaseOrderPerVendorGraphNode(purchaseOrderPerVendor);
            await _graphClient.Cypher
                .Create("(pv:PurchaseOrderPerVendor {param})")
                .WithParam("param", node)
                .ExecuteWithoutResultsAsync();

            //associtate us to the PO
            await _graphClient.Cypher
                .Match("(m:PurchaseOrder), (pv:PurchaseOrderPerVendor)")
                .Where("m.ID = {guid}")
                .AndWhere("pv.ID = {pvGuid}")
                .WithParam("guid", poID)
                .WithParam("pvGuid", purchaseOrderPerVendor.ID)
                .CreateUnique("(m)-[:PER_VENDOR]->(pv)")
                .ExecuteWithoutResultsAsync();

            //set our line items
            var liTasks = purchaseOrderPerVendor.GetLineItems().Select(li => SetPurchaseOrderLineItemAsync(li, purchaseOrderPerVendor.ID));
            foreach (var t in liTasks)
            {
                await t.ConfigureAwait(false);
            }

            //associate us with the vendor
            if (purchaseOrderPerVendor.VendorID.HasValue)
            {
                await _graphClient.Cypher
                    .Match("(pv:PurchaseOrderPerVendor), (v:Vendor)")
                    .Where((PurchaseOrderPerVendorGraphNode pv) => pv.ID == purchaseOrderPerVendor.ID)
                    .AndWhere((VendorGraphNode v) => v.ID == purchaseOrderPerVendor.VendorID)
                    .CreateUnique("(pv)-[:PURCHASE_ORDER_FOR]->(v)")
                    .ExecuteWithoutResultsAsync();
            }

        }

        private async Task SetPurchaseOrderLineItemAsync(IPurchaseOrderLineItem poLineItem, Guid pvID)
        {
            var node = new PurchaseOrderLineItemGraphNode(poLineItem);
            await _graphClient.Cypher
                .Create("(li:PurchaseOrderLineItem {param})")
                .WithParam("param", node)
                .ExecuteWithoutResultsAsync();

            var tasks = new List<Task>();
            var containerTask = SetLiquidContainerAsync(poLineItem.Container, poLineItem.ID);
            tasks.Add(containerTask);

            //also add the relationship
            var assocQuery = _graphClient.Cypher
                .Match("(m:PurchaseOrderPerVendor), (li:PurchaseOrderLineItem)")
                .Where("m.ID = {guid}")
                .AndWhere("li.ID = {liGuid}")
                .WithParam("guid", pvID)
                .WithParam("liGuid", poLineItem.ID)
                .CreateUnique("m-[:HAS_LINE_ITEM]->(li)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(assocQuery);

            tasks.Add(SetCategoryOnLineItem(poLineItem));

            foreach (var task in tasks)
            {
                await task.ConfigureAwait(false);
            }
        }

        private Task DeletePurchaseOrderLineItemAsync(Guid lineItemID)
        {
            return _graphClient.Cypher
                .Match("(li:PurchaseOrderLineItem)-[r]-()")
                .Where("li.ID = {guid}")
                .WithParam("guid", lineItemID)
                .Delete("li, r")
                .ExecuteWithoutResultsAsync();
        }

        private async Task UpdatePurchaseOrderLineItemAsync(IPurchaseOrderLineItem lineItem)
        {
            var node = new PurchaseOrderLineItemGraphNode(lineItem);
            await  _graphClient.Cypher
                .Match("(li:PurchaseOrderLineItem)")
                .Where((PurchaseOrderLineItemGraphNode li) => li.ID == lineItem.ID)
                .Set("li = {liParam}")
                .WithParam("liParam", node)
                .ExecuteWithoutResultsAsync();
            await UpdateLiquidContainerAsync(lineItem.Container, lineItem.ID);
            await SetCategoryOnLineItem(lineItem);
        }
        #endregion


        #region ReceivingOrders

        public async Task AddReceivingOrderAsync(IReceivingOrder receivingOrder)
        {
            //create our node
            var node = new ReceivingOrderGraphNode(receivingOrder);
            await _graphClient.Cypher
                .Create("(ro:ReceivingOrder {ro})")
                .WithParam("ro", node)
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            //create any unknown containers
            var containers = receivingOrder.GetBeverageLineItems().Select(bli => bli.Container).Distinct();
            foreach (var container in containers)
            {
                await CreateLiquidContainerIfNotAlreadyPresent(container);
            }

            var tasks = new List<Task>();

            //tie us to our restuarant
            var restTask = _graphClient.Cypher
                .Match("(r:Restaurant), (ro:ReceivingOrder)")
                .Where((RestaurantGraphNode r) => r.ID == receivingOrder.RestaurantID)
                .AndWhere((ReceivingOrderGraphNode ro) => ro.ID == receivingOrder.ID)
                .CreateUnique("(r)-[:HAS_RECEIVING_ORDER]->(ro)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(restTask);

            //to our employee
            var empTask = _graphClient.Cypher
                .Match("(e:Employee), (ro:ReceivingOrder)")
                .Where((EmployeeGraphNode e) => e.ID == receivingOrder.ReceivedByEmployeeID)
                .AndWhere((ReceivingOrderGraphNode ro) => ro.ID == receivingOrder.ID)
                .CreateUnique("(e)-[:RECEIVED_ORDER]->(ro)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(empTask);

            //create the line items
            var liTasks =
                receivingOrder.GetBeverageLineItems()
                    .Select(li => SetLineItemsForReceivingOrderAsync(li, receivingOrder.ID));
            tasks.AddRange(liTasks);

            //tie us to the POs as well, if the relationship is there
            var poID = receivingOrder.PurchaseOrderID;
            if (poID.HasValue)
            {
                var roID = receivingOrder.ID;
                tasks.Add(_graphClient.Cypher
                    .Match("(ro:ReceivingOrder), (po:PurchaseOrder)")
                    .Where((PurchaseOrderGraphNode po) => po.ID == poID.Value)
                    .AndWhere((ReceivingOrderGraphNode ro) => ro.ID == roID)
                    .CreateUnique("(ro)-[:RECEIVED_FOR_PO]->(po)")
                    .ExecuteWithoutResultsAsync()
                );
            }

            //tie to the vendor
            var vendorTask = _graphClient.Cypher
                .Match("(ro:ReceivingOrder), (v:Vendor)")
                .Where((VendorGraphNode v) => v.ID == receivingOrder.VendorID)
                .AndWhere((ReceivingOrderGraphNode ro) => ro.ID == receivingOrder.ID)
                .CreateUnique("(v)-[:SHIPPED_ORDER]->(ro)")
                .ExecuteWithoutResultsAsync();
            tasks.Add(vendorTask);

            //lastly, tie the VEndor to the restaurant
            tasks.Add(AssociateVendorWithRestaurantAsync(receivingOrder.VendorID, receivingOrder.RestaurantID));

            foreach (var task in tasks)
            {
                await task.ConfigureAwait(false);
            }
        }

        public async Task UpdateReceivingOrderAsync(IReceivingOrder receivingOrder)
        {
            foreach (var liQuery in receivingOrder.GetBeverageLineItems()
                .Select(li => li.ID)
                .Select(id =>
                    _graphClient.Cypher
                        .Match("(li:ReceivingOrderBeverageLineItem)-[r]-()")
                        .Where((ReceivingOrderLineItemGraphNode li) => li.ID == id)
                        .Delete("li, r"))
            )
            {
                await liQuery.ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            var roQuery = _graphClient.Cypher
                .Match("(ro:ReceivingOrder)-[r]-()")
                .Where((ReceivingOrderGraphNode ro) => ro.ID == receivingOrder.ID)
                .Delete("ro, r");

            await roQuery.ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            await AddReceivingOrderAsync(receivingOrder);
        }

        private async Task<IEnumerable<ReceivingOrderLineItem>> GetReceivingOrderLineItems(Guid roID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(ro:ReceivingOrder)-[:HAS_LINE_ITEM]->(li:ReceivingOrderBeverageLineItem)")
                .Where((ReceivingOrderGraphNode ro) => ro.ID == roID)
                .Return(li => li.As<ReceivingOrderLineItemGraphNode>())
                .ResultsAsync;

            var res = new List<ReceivingOrderLineItem>();
            foreach (var node in nodes)
            {
                var containers = await GetLiquidContainersForEntityAsync(node.ID);
                var container = containers.FirstOrDefault();
                var categories = await GetCategoriesForItem(node.ID);
                res.Add(node.Rehydrate(container, categories));
            }
            return res;
        }

        private async Task<IReceivingOrder> LoadReceivingOrder(ReceivingOrderGraphNode node)
        {
            //get the line items

            //get the purchase order IDs as well
            var poQuery = _graphClient.Cypher
                .Match("(ro:ReceivingOrder)-[:FULFILLS_PURCHASE_ORDER]->(po:PurchaseOrder)")
                .Where((ReceivingOrderGraphNode ro) => ro.ID == node.ID)
                .Return(po => po.As<PurchaseOrderGraphNode>().ID);

            var vendorQuery = _graphClient.Cypher
                .Match("(v:Vendor)-[:SHIPPED_ORDER]->(ro:ReceivingOrder)")
                .Where((ReceivingOrderGraphNode ro) => ro.ID == node.ID)
                .Return(v => v.As<VendorGraphNode>().ID);

            var liTask = GetReceivingOrderLineItems(node.ID);

            var poIDs = await poQuery.ResultsAsync;

            var vendorRes = await vendorQuery.ResultsAsync;

            var lineItems = await liTask;

            return node.Rehydrate(lineItems, poIDs.FirstOrDefault(), vendorRes.FirstOrDefault());
        }

        private async Task SetLineItemsForReceivingOrderAsync(IReceivingOrderLineItem lineItem, Guid receivingOrderID)
        {
            //TODO add line to restaurant for the private price if it's here
            var node = new ReceivingOrderLineItemGraphNode(lineItem);
            await _graphClient.Cypher
                .Create("(li:ReceivingOrderBeverageLineItem {liParam})")
                .WithParam("liParam", node)
                .ExecuteWithoutResultsAsync();

            var containerTask = SetLiquidContainerAsync(lineItem.Container, lineItem.ID);

            //also add the relationship
            var assocQuery = _graphClient.Cypher
                .Match("(ro:ReceivingOrder), (li:ReceivingOrderBeverageLineItem)")
                .Where("ro.ID = {guid}")
                .AndWhere("li.ID = {liGuid}")
                .WithParam("guid", receivingOrderID)
                .WithParam("liGuid", lineItem.ID)
                .CreateUnique("ro-[:HAS_LINE_ITEM]->(li)")
                .ExecuteWithoutResultsAsync();


            await assocQuery.ConfigureAwait(false);
            await containerTask.ConfigureAwait(false);
            await SetCategoryOnLineItem(lineItem);
        }

        public async Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersAsync(DateTimeOffset maxTimeBack)
        {
            var nodes = await _graphClient.Cypher
                .Match("(ro:ReceivingOrder)")
                .Where((ReceivingOrderGraphNode ro) => ro.LastUpdatedDate > maxTimeBack)
                .Return(ro => ro.As<ReceivingOrderGraphNode>())
                .ResultsAsync;

            var res = new List<IReceivingOrder>();
            foreach (var node in nodes)
            {
                var loaded = await LoadReceivingOrder(node);
                res.Add(loaded);
            }
            return res;
        }

        public async Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersAsync(Guid restaurantID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(ro:ReceivingOrder)")
                .Where((ReceivingOrderGraphNode ro) => ro.RestaurantID == restaurantID)
                .Return(ro => ro.As<ReceivingOrderGraphNode>())
                .ResultsAsync;

            var res = new List<IReceivingOrder>();
            foreach (var node in nodes)
            {
                var item = await LoadReceivingOrder(node);
                res.Add(item);
            }
            return res;
        }

        public async Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersAsync(IVendor vendor)
        {
            var items = await _graphClient.Cypher
                .Match("(ro:ReceivingOrder)-[:SHIPPED_ORDER]-(v:Vendor)")
                .Where((VendorGraphNode v) => v.ID == vendor.ID)
                .Return(ro => ro.As<ReceivingOrderGraphNode>())
                .ResultsAsync;

            var res = new List<IReceivingOrder>();
            foreach (var node in items)
            {
                var loaded = await LoadReceivingOrder(node);
                res.Add(loaded);
            }

            return res;
        }
        #endregion


        #endregion

        private void AddRestaurantTerminal(IMiseTerminalDevice terminal, Guid restaurantID)
        {
            //TODO if this already exists, we just tie to it?  that should be an error
            var node = new MiseTerminalDeviceGraphNode(terminal);
            var termQuery = _graphClient.Cypher
                .Match("(rest:Restaurant)")
                .Where((IRestaurant rest) => restaurantID == rest.ID)
                .CreateUnique("rest-[:HAS_TERMINAL]->(term:MiseTerminal {term})")
                .WithParam("term", node);

            termQuery.ExecuteWithoutResults();
        }

        private async Task<Menu> GetMenuByID(Guid menuID)
        {
            var results = await _graphClient
                .Cypher
                .Match("(menu:Menu)")
                .Where((Menu menu) => menu.ID == menuID)
                .Return(m => m.As<MenuGraphNode>())
                .ResultsAsync;

            return await LoadMenu(results.FirstOrDefault());
        }

        private Task<IEnumerable<EmailAddress>> GetEmailsAssociatedWithEntity(Guid entityID, string relationship = "HAS_EMAIL_ADDRESS")
        {
            return _graphClient.Cypher
                .Match("m-[:" + relationship + "]->(email:EmailAddress)")
                .Where("m.ID = {guid}")
                .WithParam("guid", entityID)
                .Return(email => email.As<EmailAddress>())
                .ResultsAsync;
        }

        private Task<IEnumerable<EmailAddress>> GetEmailsAssociatedWithEntityAsync(Guid entityID)
        {
            return _graphClient.Cypher
                .Match("m-[:HAS_EMAIL_ADDRESS]->(email:EmailAddress)")
                .Where("m.ID = {guid}")
                .WithParam("guid", entityID)
                .Return(email => email.As<EmailAddress>())
                .ResultsAsync;
        }

        private Task RemoveEmailAddressFromEntityAsync(EmailAddress email, Guid entityID)
        {
            //remove the connection
            var removeQuery = _graphClient.Cypher
                .Match("(m)-[r:HAS_EMAIL_ADDRESS]->(e:EmailAddress)")
                .Where("m.ID = {guid}")
                .AndWhere((EmailAddress e) => e.Value == email.Value)
                .WithParam("guid", entityID)
                .Delete("r")
                .ExecuteWithoutResultsAsync();

            //TODO consider cleaning up orphaned emails?

            return removeQuery;
        }

        private async Task SetEmailAddressOnEntityAsync(EmailAddress email, Guid id, string relationship = "HAS_EMAIL_ADDRESS")
        {
            var existing = await _graphClient.Cypher
                .Match("(e:EmailAddress)")
                .Where((EmailAddress e) => e.Value == email.Value)
                .Return(e => e.Id())
                .ResultsAsync;

            var found = existing.Any();

            if (found == false)
            {
                await _graphClient.Cypher
                    .Match("(m)")
                    .Where("m.ID = {guid}")
                    .Create("(email:EmailAddress {em}), (m)-[:" + relationship + "]->(email)")
                    .WithParam("em", email)
                    .WithParam("guid", id)
                    .ExecuteWithoutResultsAsync();
            }
            else
            {
                var q = _graphClient.Cypher
                    .Match("(m), (e:EmailAddress)")
                    .Where((EmailAddress e) => e.Value == email.Value)
                    .AndWhere("m.ID={guid}")
                    .WithParam("guid", id)
                    .CreateUnique("(m)-[:" + relationship + "]->(e)");
                await q.ExecuteWithoutResultsAsync();
            }
        }

        /// <summary>
        /// Ensure the specified discount exists and is tied to the discount given
        /// </summary>
        /// <param name="discount"></param>
        /// <param name="entityID"></param>
        // ReSharper disable once UnusedMember.Local
        private async Task SetDiscountPercentage(IEntityBase discount, Guid entityID)
        {
            var existsQuery = await _graphClient.Cypher
                .Match("(dis:Discount)")
                .Where((DiscountPercentage dis) => dis.ID == discount.ID)
                .Return(dis => dis.Id())
                .ResultsAsync;

            if (existsQuery.Any())
            {
                await _graphClient.Cypher
                    .Match("(m)", "(dis:Discount)")
                    .Where("m.ID = {guid}")
                    .AndWhere((DiscountPercentage dis) => dis.ID == discount.ID)
                    .WithParam("guid", entityID)
                    .CreateUnique("(m)-[:POSSIBLE_DISCOUNT]->(dis)")
                    .ExecuteWithoutResultsAsync();
            }
            else
            {

                var disQuery = _graphClient.Cypher
                    .Match("(m)")
                    .Where("m.ID = {guid}")
                    .Create("(m)-[:POSSIBLE_DISCOUNT]->(dis:Discount {discount})")
                    .WithParam("discount", discount)
                    .WithParam("guid", entityID);
                await disQuery.ExecuteWithoutResultsAsync();
            }
        }

        private async Task SetPhoneNumber(PhoneNumber phoneNumber, Guid entityID)
        {
            var exists = await _graphClient.Cypher
                .Match("(phNum:PhoneNumber)")
                .Where(
                    (PhoneNumber phNum) => phNum.AreaCode == phoneNumber.AreaCode && phNum.Number == phoneNumber.Number)
                .Return(phNum => phNum.Id())
                .ResultsAsync;

            if (exists.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(phNum:PhoneNumber {phoneNum})")
                    .WithParam("phoneNum", phoneNumber)
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }

            await _graphClient.Cypher
                .Match("(ph:PhoneNumber), (m)")
                .Where("m.ID = {guid}")
                .AndWhere((PhoneNumber ph) => ph.AreaCode == phoneNumber.AreaCode && ph.Number == phoneNumber.Number)
                .WithParam("guid", entityID)
                .Create("(m)-[:HAS_PHONE_NUMBER]->(ph)")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LiquidContainer>> GetAllLiquidContainersAsync()
        {
            var nodes = await _graphClient.Cypher
                .Match("(cn:LiquidContainer)")
                .Return(cn => cn.As<LiquidContainerGraphNode>())
                .ResultsAsync;

            return nodes.Select(lc => lc.Rehydrate());
        }

        public async Task<IEnumerable<IApplicationInvitation>> GetOpenApplicationInvitations(EmailAddress destination)
        {
            //get nodes
            var nodes = await _graphClient.Cypher
                .Match("(ai:ApplicationInvitation)-[:INVITATION_SENT_TO]-(e:EmailAddress)")
                .Where((EmailAddress e) => destination.Equals(e))
                .AndWhere((ApplicationInvitationGraphNode ai) => ai.Status != InvitationStatus.Accepted.ToString()
                                                                 && ai.Status != InvitationStatus.Rejected.ToString())
                .Return(ai => ai.As<ApplicationInvitationGraphNode>())
                .ResultsAsync;

            return await LoadApplicationInvitationNodes(nodes);
        }


        public async Task<IEnumerable<IApplicationInvitation>> GetApplicationInvitations()
        {
            //TODO might want to limit this to open ones
            var nodes = await _graphClient.Cypher
                .Match("(ai:ApplicationInvitation)-[:INVITATION_TO]-(r:Restaurant)")
                .Return(ai => ai.As<ApplicationInvitationGraphNode>())
                .ResultsAsync;

            return await LoadApplicationInvitationNodes(nodes);
        }

        private async Task<IEnumerable<IApplicationInvitation>> LoadApplicationInvitationNodes(IEnumerable<ApplicationInvitationGraphNode> nodes)
        {
            var res = new List<IApplicationInvitation>();
            foreach (var node in nodes)
            {
                //get restaurants
                var node1 = node;
                var rests = await _graphClient.Cypher
                    .Match("(ai:ApplicationInvitation)-[:INVITATION_TO]-(r:Restaurant)")
                    .Where((ApplicationInvitationGraphNode ai) => ai.ID == node1.ID)
                    .Return(r => r.As<RestaurantGraphNode>())
                    .ResultsAsync;
                var inviteRestNode = rests.FirstOrDefault();
                if (inviteRestNode == null)
                {
                    throw new Exception("No restaurant found for invitation!");
                }

                //get employees
                var invitingEmployeeNodes = await _graphClient.Cypher
                    .Match("(e:Employee)-[:MADE_INVITATION]->(ai:ApplicationInvitation)")
                    .Where((ApplicationInvitationGraphNode ai) => ai.ID == node1.ID)
                    .Return(e => e.As<EmployeeGraphNode>())
                    .ResultsAsync;

                var empNode = invitingEmployeeNodes.FirstOrDefault();
                IEmployee invitingEmp = null;
                if (empNode != null)
                {
                    var inviterEmails = await GetEmailsAssociatedWithEntity(empNode.ID);
                    var inviteRests = await GetRestaurantIDsEmployeeWorksAt(empNode.ID);
                    invitingEmp = empNode.Rehydrate(inviterEmails, inviteRests);
                }

                var destEmpNodes = await _graphClient.Cypher
                    .Match("(ai:ApplicationInvitation)-[:INVITATION_RECEIPIENT]-(e:Employee)")
                    .Where((ApplicationInvitationGraphNode ai) => ai.ID == node1.ID)
                    .Return(e => e.As<EmployeeGraphNode>())
                    .ResultsAsync;
                var destEmpNode = destEmpNodes.FirstOrDefault();
                IEmployee destEmployee = null;
                if (destEmpNode != null)
                {
                    var destEmails = await GetEmailsAssociatedWithEntity(destEmpNode.ID);
                    var destRests = await GetRestaurantIDsEmployeeWorksAt(destEmpNode.ID);
                    destEmployee = destEmpNode.Rehydrate(destEmails, destRests);
                }

                //get emails
                var emails = await GetEmailsAssociatedWithEntity(node.ID, "INVITATION_SENT_TO");

                var restName = new RestaurantName(inviteRestNode.FullName, inviteRestNode.ShortName);
                res.Add(node.Rehydrate(inviteRestNode.ID, restName, invitingEmp, destEmployee, emails.FirstOrDefault()));
            }

            return res;
        }

        public async Task AddApplicationInvitiation(IApplicationInvitation invite)
        {
            var node = new ApplicationInvitationGraphNode(invite);
            await _graphClient.Cypher
                .Create("(ai:ApplicationInvitation {appInv})")
                .WithParam("appInv", node)
                .ExecuteWithoutResultsAsync();

            //tie to restaurant
            await _graphClient.Cypher
                .Match("(ai:ApplicationInvitation), (r:Restaurant)")
                .Where((ApplicationInvitationGraphNode ai) => ai.ID == invite.ID)
                .AndWhere((RestaurantGraphNode r) => r.ID == invite.RestaurantID)
                .CreateUnique("(ai)-[:INVITATION_TO]->(r)")
                .ExecuteWithoutResultsAsync();

            //tie to employees
            await _graphClient.Cypher
                .Match("(ai:ApplicationInvitation), (e:Employee)")
                .Where((ApplicationInvitationGraphNode ai) => ai.ID == invite.ID)
                .AndWhere((EmployeeGraphNode e) => e.ID == invite.InvitingEmployeeID)
                .CreateUnique("(e)-[:MADE_INVITATION]->(ai)")
                .ExecuteWithoutResultsAsync();

            if (invite.DestinationEmployeeID.HasValue)
            {
                await _graphClient.Cypher
                    .Match("(ai:ApplicationInvitation), (e:Employee)")
                    .Where((ApplicationInvitationGraphNode ai) => ai.ID == invite.ID)
                    .AndWhere((EmployeeGraphNode e) => e.ID == invite.DestinationEmployeeID.Value)
                    .CreateUnique("(ai)-[:INVITATION_RECEIPIENT]->(e)")
                    .ExecuteWithoutResultsAsync();
            }

            //tie to email
            await SetEmailAddressOnEntityAsync(invite.DestinationEmail, invite.ID, "INVITATION_SENT_TO");
        }

        public async Task UpdateApplicationInvitation(IApplicationInvitation invite)
        {
            //just erase and readd
            await _graphClient.Cypher
                .Match("(ai:ApplicationInvitation)-[r]-()")
                .Where((ApplicationInvitationGraphNode ai) => ai.ID == invite.ID)
                .Delete("ai, r")
                .ExecuteWithoutResultsAsync();

            await AddApplicationInvitiation(invite);
        }

        private async Task<IEnumerable<LiquidContainer>> GetLiquidContainersForEntityAsync(Guid entityID)
        {
            var nodes = await _graphClient.Cypher
                .Match("m-[:HAS_CONTAINER]->(cn:LiquidContainer)")
                .Where("m.ID = {guid}")
                .WithParam("guid", entityID)
                .Return(cn => cn.As<LiquidContainerGraphNode>())
                .ResultsAsync;

            return nodes.Select(n => n.Rehydrate());
        }

        private static bool ContainerNodeMatchesContainer(LiquidContainer container, LiquidContainerGraphNode cn)
        {
            if (container.AmountContained.Milliliters != cn.Millilters)
            {
                return false;
            }

            if (container.WeightEmpty != null)
            {
                if (container.WeightEmpty.Grams != cn.WeightEmptyGrams)
                {
                    return false;
                }
            }

            if (container.WeightFull != null)
            {
                if (container.WeightFull.Grams != cn.WeightFullGrams)
                {
                    return false;
                }
            }

            if (container.AmountContained.SpecificGravity.HasValue)
            {
                if (cn.SpecificGravity.HasValue)
                {
                    if (container.AmountContained.SpecificGravity.Value != cn.SpecificGravity.Value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<string> CreateLiquidContainerIfNotAlreadyPresent(LiquidContainer container)
        {
            var containerNodes = await _graphClient.Cypher
                .Match("(cn:LiquidContainer)")
                .Where(
                    (LiquidContainerGraphNode cn) =>
                        cn.Millilters == container.AmountContained.Milliliters
                 )
                .Return(cn => cn.As<LiquidContainerGraphNode>())
                .ResultsAsync;

            var resultsValid = containerNodes.Where(cn => ContainerNodeMatchesContainer(container, cn)).ToList();

            string containerDBName;

            if (resultsValid.Any() == false)
            {
                var node = new LiquidContainerGraphNode(container);
                //create our container
                await _graphClient.Cypher
                    .Create("(cn:LiquidContainer {container})")
                    .WithParam("container", node)
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);

                containerDBName = node.DBName;
            }
            else
            {
                containerDBName = resultsValid.First().DBName;
            }

            return containerDBName;
        }

        /// <summary>
        /// Makes sure we have only one node for each container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="entityID"></param>
        private async Task SetLiquidContainerAsync(LiquidContainer container, Guid entityID)
        {
            var containerDBName = await CreateLiquidContainerIfNotAlreadyPresent(container);

            //find our ent
            var assocQuery = _graphClient.Cypher
                .Match("(m), (cn:LiquidContainer)")
                .Where("m.ID = {guid}")
                .AndWhere(
                    (LiquidContainerGraphNode cn) => cn.DBName == containerDBName)
                .WithParam("guid", entityID)
                .CreateUnique("m-[:HAS_CONTAINER]->(cn)");

            await assocQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);
        }

        private async Task UpdateLiquidContainerAsync(LiquidContainer updatedContainer, Guid entityID)
        {
            var containerInDB = (await GetLiquidContainersForEntityAsync(entityID)).FirstOrDefault();
            if (containerInDB != null && updatedContainer.Equals(containerInDB) == false)
            {
                await RemoveLiquidContainerFromEntity(containerInDB, entityID).ConfigureAwait(false);
                await SetLiquidContainerAsync(updatedContainer, entityID).ConfigureAwait(false);
            }
        }

        private Task RemoveLiquidContainerFromEntity(LiquidContainer container, Guid entityID)
        {
            var containerNode = new LiquidContainerGraphNode(container);
            return _graphClient.Cypher
                .Match("(m)-[r:HAS_CONTAINER]->(cn)")
                .Where("m.ID = {guid}")
                .AndWhere(
                    (LiquidContainerGraphNode cn) => cn.DBName == containerNode.DBName)
                .WithParam("guid", entityID)
                .Delete("r")
                .ExecuteWithoutResultsAsync();
        }

        private async Task<StreetAddress> GetAddressAsync(Guid entityID)
        {
            var node = await _graphClient.Cypher
                .Match(
                    "(m)-[:LOCATED_IN]->(san:StreetAddressNumber)-[:LOCATED_IN]->(s:Street)-[:LOCATED_IN]->(c:City)-[:LOCATED_IN]->(st:State)-[:LOCATED_IN]->(cty:Country), (san)-[:LOCATED_IN]->(zip:ZipCode)")
                .Where("m.ID = {guid}")
                .WithParam("guid", entityID)
                .Return((san, s, c, st, cty, zip) => new StreetAddress
                {
                    StreetAddressNumber = san.As<StreetAddressNumber>(),
                    Street = s.As<Street>(),
                    City = c.As<City>(),
                    State = st.As<State>(),
                    Country = cty.As<Country>(),
                    Zip = zip.As<ZipCode>()
                })
                .ResultsAsync;

            var address = node.FirstOrDefault();
            return address;
        }

        public async Task SetState(State state, Country country)
        {
            //work outside in
            var countryNode = await _graphClient.Cypher
                .Match("(cty:Country)")
                .Where((Country cty) => cty.Name == country.Name)
                .Return(cty => cty.Id())
                .ResultsAsync;
            if (countryNode.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(ct:Country {country})")
                    .WithParam("country", country)
                    .ExecuteWithoutResultsAsync();
            }

            var stateNode = await _graphClient.Cypher
                .Match("(s:State)-[:LOCATED_IN]->(c:Country)")
                .Where((State s) => s.Name == state.Name || s.Abbreviation == state.Abbreviation)
                .AndWhere((Country c) => c.Name == country.Name)
                .Return(s => s.Id())
                .ResultsAsync;

            if (stateNode.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(s:State {state})")
                    .WithParam("state", state)
                    .ExecuteWithoutResultsAsync();

                //relate to country
                await _graphClient.Cypher
                 .Match("(cty:Country)", "(s:State)")
                 .Where((Country cty) => cty.Name == country.Name)
                 .AndWhere((State s) => s.Name == state.Name)
                 .CreateUnique("(s)-[:LOCATED_IN]->(cty)")
                 .ExecuteWithoutResultsAsync();
            }
        }

        /// <summary>
        /// Our address can have many items, many of which will already exist.  So find or create on each, and 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="entityID">ID field of the node this address chain should be added to</param>
        /// <param name="additionalRelationships">If any, additional named relations ships between our entity and the address</param>
        private async Task SetAddress(StreetAddress address, Guid entityID, IEnumerable<string> additionalRelationships)
        {

            await SetState(address.State, address.Country);

            var cityNode = await _graphClient.Cypher
                .Match("(c:City)-[:LOCATED_IN]->(s:State)")
                .Where((City c) => c.Name == address.City.Name)
                .AndWhere((State s) => s.Name == address.State.Name)
                .Return(c => c.Id())
                .ResultsAsync;
            if (cityNode.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(c:City {city})")
                    .WithParam("city", address.City)
                    .ExecuteWithoutResultsAsync();

                await _graphClient.Cypher
                     .Match("(cty:City)", "(s:State)-[:LOCATED_IN]->(ctry:Country)")
                     .Where((City cty) => cty.Name == address.City.Name)
                     .AndWhere((State s) => s.Name == address.State.Name)
                     .AndWhere((Country ctry) => ctry.Name == address.Country.Name)
                     .CreateUnique("(cty)-[:LOCATED_IN]->(s)")
                     .ExecuteWithoutResultsAsync();
            }


            //street
            var streetNode = await _graphClient.Cypher
                .Match("(street:Street)-[:LOCATED_IN]->(city:City)")
                .Where((Street street) => street.Name == address.Street.Name)
                .AndWhere((City city) => city.Name == address.City.Name)
                .Return(street => street.Id())
                .ResultsAsync;
            if (streetNode.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(s:Street {street})")
                    .WithParam("street", address.Street)
                    .ExecuteWithoutResultsAsync();

                var addStreetQuery = _graphClient.Cypher
                     .Match("(street:Street)", "(c:City)-[:LOCATED_IN]->(s:State)")
                     .Where((Street street) => street.Name == address.Street.Name)
                     .AndWhere((City c) => c.Name == address.City.Name)
                     .AndWhere((State s) => s.Name == address.State.Name)
                     .CreateUnique("(street)-[:LOCATED_IN]->(c)");
                await addStreetQuery.ExecuteWithoutResultsAsync();

            }

            //zip code ties to the street address, so make it first and street can pick it up
            var zipCodeNode = await _graphClient.Cypher
                .Match("(zipCode:ZipCode)")
                .Where((ZipCode zipCode) => zipCode.Value == address.Zip.Value)
                .Return(zipCode => zipCode.Id())
                .ResultsAsync;
            if (zipCodeNode.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(z:ZipCode {zipCode})")
                    .WithParam("zipCode", address.Zip)
                    .ExecuteWithoutResultsAsync();

                var addZipQuery = _graphClient
                    .Cypher
                    .Match("(country:Country)", "(z:ZipCode)")
                    .Where((ZipCode z) => z.Value == address.Zip.Value)
                    .AndWhere((Country country) => country.Name == address.Country.Name)
                    .CreateUnique("(z)-[:LOCATED_IN]->(country)");
                await addZipQuery.ExecuteWithoutResultsAsync();
            }

            //street number - this ties to both zipcode and street!
            var streetAddressNode = await _graphClient.Cypher
                .Match("(streetAddressNumber:StreetAddressNumber)-[:LOCATED_IN]->(street:Street)")
                .Where((Street street) => street.Name == address.Street.Name)
                .AndWhere((StreetAddressNumber streetAddressNumber) => streetAddressNumber.Number == address.StreetAddressNumber.Number && streetAddressNumber.Direction == address.StreetAddressNumber.Direction)
                .Return(street => street.Id())
                .ResultsAsync;
            if (streetAddressNode.Any() == false)
            {
                await _graphClient.Cypher
                    .Create("(san:StreetAddressNumber {streetAddressNumber})")
                     .WithParam("streetAddressNumber", address.StreetAddressNumber)
                     .ExecuteWithoutResultsAsync();

                //link to street
                var addStreetAddressNumberQuery = _graphClient.Cypher
                    .Match("(san:StreetAddressNumber)",
                        "(street:Street)-[:LOCATED_IN]->(city:City)-[:LOCATED_IN]->(state:State)")
                    .Where((StreetAddressNumber san) => san.Number == address.StreetAddressNumber.Number);
                if (string.IsNullOrEmpty(address.StreetAddressNumber.Direction) == false)
                {
                    addStreetAddressNumberQuery =
                        addStreetAddressNumberQuery.AndWhere(
                            (StreetAddressNumber san) => san.Direction == address.StreetAddressNumber.Direction);
                }
                addStreetAddressNumberQuery = addStreetAddressNumberQuery
                    .AndWhere((Street street) => street.Name == address.Street.Name)
                    .AndWhere((City city) => city.Name == address.City.Name)
                    .AndWhere((State state) => state.Name == address.State.Name)
                    .CreateUnique("(san)-[:LOCATED_IN]->(street)");

                await addStreetAddressNumberQuery.ExecuteWithoutResultsAsync();

                //link to ZIP
                var addZipQuery = _graphClient.Cypher
                    .Match("(san:StreetAddressNumber)", "(zip:ZipCode)-[:LOCATED_IN]->(country:Country)")
                    .Where((StreetAddressNumber san) => san.Number == address.StreetAddressNumber.Number);
                if (string.IsNullOrEmpty(address.StreetAddressNumber.Direction) == false)
                {
                    addZipQuery =
                        addZipQuery.AndWhere(
                            (StreetAddressNumber san) => san.Direction == address.StreetAddressNumber.Direction);
                }
                addZipQuery = addZipQuery
                    .AndWhere((Country country) => country.Name == address.Country.Name)
                    .AndWhere((ZipCode zip) => zip.Value == address.Zip.Value)
                    .CreateUnique("(san)-[:LOCATED_IN]->(zip)");

                await addZipQuery.ExecuteWithoutResultsAsync();
            }

            var newTies = new List<string> { "(m-[:LOCATED_IN]->(san))" };
            newTies.AddRange(additionalRelationships.Select(addRelationship => "(m)-[:" + addRelationship.ToUpper() + "]->(san)"));
            var createStatement = string.Join(",", newTies);
            //tie our query to the street address

            var entQuery = _graphClient.Cypher
                .Match("m", "(san:StreetAddressNumber)-[:LOCATED_IN]-()-[:LOCATED_IN]-(c:City)")
                .Where("m.ID = {guid}")
                .AndWhere((City c) => c.Name == address.City.Name)
                .AndWhere((StreetAddressNumber san) => san.Number == address.StreetAddressNumber.Number);

            if (string.IsNullOrEmpty(address.StreetAddressNumber.Direction) == false)
            {
                entQuery = entQuery.AndWhere((StreetAddressNumber san) => san.Direction == address.StreetAddressNumber.Direction);
            }
            entQuery = entQuery
                .WithParam("guid", entityID)
                .Create(createStatement);
            await entQuery.ExecuteWithoutResultsAsync();

            //todo we need to tie in our location here as well, if it exists
        }

        /// <summary>
        /// Determines which items should be added, updated, or deleted, and does so
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getFromDBFunc"></param>
        /// <param name="setFunction"></param>
        /// <param name="delFunction"></param>
        /// <param name="updateFunc"></param>
        /// <param name="updatedItems"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private static IEnumerable<Task> UpdateSubItemsWorker<T>(Func<Guid, Task<IEnumerable<T>>> getFromDBFunc,
            Func<T, Guid, Task> setFunction, Func<Guid, Task> delFunction, Func<T, Task> updateFunc, List<T> updatedItems, Guid parentID) where T : IEntityBase
        {
            var tasks = new List<Task>();
            var inDB = getFromDBFunc(parentID).Result.ToList();

            var toAdd = updatedItems.Where(si => inDB.Select(dbI => dbI.ID).Contains(si.ID) == false);
            var toDel = inDB.Where(dbI => updatedItems.Select(si => si.ID).Contains(dbI.ID) == false);
            var toUpdate = updatedItems.Where(si => inDB.Select(dbI => dbI.ID).Contains(si.ID));

            tasks.AddRange(toAdd.Select(a => setFunction(a, parentID)));
            tasks.AddRange(toDel.Select(d => delFunction(d.ID)));
            tasks.AddRange(toUpdate.Select(updateFunc));

            return tasks;
        }
    }
}
