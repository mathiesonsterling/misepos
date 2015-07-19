using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Server.Services.Implementation;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Neo4J.Neo4JDAL;

namespace DeveloperTools.Commands
{
    public class PopulateInventoryDatabaseCommand : BaseProgressReportingCommand
    {
        private readonly ILogger _logger;
        private readonly Uri _uri;
        public PopulateInventoryDatabaseCommand(ILogger logger, Uri uri, IProgress<ProgressReport> progress) : base(progress)
        {
            _logger = logger;
            _uri = uri;
        }

        public override int NumSteps
        {
            get { return 31; }
        }

        public override async Task Execute()
        {
            var config = new DevToolsConfigs { Neo4JConnectionDBUri = _uri };
            var graphDAL = new Neo4JEntityDAL(config, _logger);

            Report("Reset database");
            graphDAL.ResetDatabase();

            Report("Populated states in DB");
            await PopulateStates(graphDAL);

            Report("Creating Categories");
            var categoriesService = new CategoriesService();
            var cats = categoriesService.GetIABIngredientCategories();
            await graphDAL.CreateCategories(cats.ToList());
            Report("Categories Created");

            //get the fake service, and populate all parts of it!
            var fakeService = new FakeInventoryServiceDAL();

            //accounts
            var accts = (await fakeService.GetAccountsAsync()).ToList();
            foreach (var acct in accts)
            {
                await graphDAL.AddAccountAsync(acct);
            }
            Report("Added "+accts.Count()+" accounts");


            //restaurants
            var rests = (await fakeService.GetRestaurantsAsync()).ToList();

            foreach (var rest in rests)
            {
                await graphDAL.AddRestaurantAsync(rest);

            }
            Report("Added "+rests.Count+" Restaurants");

            var vendors = await fakeService.GetVendorsAsync();
            foreach (var v in vendors)
            {
                if (v.ID == Guid.Empty)
                {
                    throw new Exception("Vendor does not have ID");
                }
                await graphDAL.AddVendorAsync(v);
            }
            Report("Added Vendors");

            foreach (var rest in rests)
            {
                var emps = await fakeService.GetEmployeesAsync(rest.ID);
                foreach (var emp in emps)
                {
                    if (emp.ID == Guid.Empty)
                    {
                        throw new Exception("Employee does not have ID");
                    }
                    try
                    {
                        await graphDAL.AddEmployeeAsync(emp);
                    }
                    catch (Exception ex)
                    {
                        _logger.HandleException(ex);
                        throw;
                    }
                }
                Report("Added employees to " + rest.Name.ShortName);


                var inventories = await fakeService.GetInventoriesAsync(rest.ID);
                foreach (var inv in inventories)
                {
                    if (inv.ID == Guid.Empty)
                    {
                        throw new Exception("Inventory does not have ID");
                    }
                    await graphDAL.AddInventoryAsync(inv);
                }
                Report("Added inventories to " + rest.Name.ShortName);

                var ros = await fakeService.GetReceivingOrdersAsync(rest.ID);
                foreach (var ro in ros)
                {
                    if (ro.ID == Guid.Empty)
                    {
                        throw new Exception("RO does not have ID");
                    }
                    await graphDAL.AddReceivingOrderAsync(ro);
                }
                //pgBar.Value += perRestAmt / NUM_INNER_STEPS;
                Report("Added receiving orders to " + rest.Name.ShortName);
                var pos = await fakeService.GetPurchaseOrdersAsync(rest.ID);
                foreach (var po in pos)
                {
                    if (po.ID == Guid.Empty)
                    {
                        throw new Exception("PO does not have ID");
                    }
                    await graphDAL.AddPurchaseOrderAsync(po);
                }
                Report("Added purchase orders to " + rest.Name.ShortName);
                //pgBar.Value += perRestAmt / NUM_INNER_STEPS;
                var pars = await fakeService.GetPARsAsync(rest.ID);
                foreach (var par in pars)
                {
                    if (par.ID == Guid.Empty)
                    {
                        throw new Exception("PAR does not have ID");
                    }
                    await graphDAL.AddPARAsync(par);
                }
                Report("Added PARs to " + rest.Name.ShortName);
                //pgBar.Value += perRestAmt / NUM_INNER_STEPS;

            }
            var invites = await fakeService.GetApplicationInvitations();
            foreach (var invite in invites)
            {
                await graphDAL.AddApplicationInvitiation(invite);
            }
            Report("Added invitations");
            Finish();
        }


        /// <summary>
        /// Populate US states
        /// </summary>
        /// <param name="graphDAL"></param>
        /// <returns></returns>
        public static async Task PopulateStates(Neo4JEntityDAL graphDAL)
        {
            var country = new Country
            {
                Name = "United States of America"
            };


            foreach (var state in State.GetUSStates())
            {
                await graphDAL.SetState(state, country);
            }
        }

    }
}
