using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.WebServices.FakeServices;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;
using Mise.Core.Server.Services.DAL;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Server.Services.Implementation
{
    /// <summary>
    /// Provides fake stuff for lookup
    /// </summary>
    public class FakeInventoryServiceDAL : IEntityDAL
    {
        #region Fields

        private List<IInventory> _restaurauntInventories;
        private List<IEmployee> _employees;

        private List<Restaurant> _restaurants;

        private List<IVendor> _vendors;
        private List<IPurchaseOrder> _purchaseOrders;
        private List<IReceivingOrder> _receivingOrders;
        private List<IPar> _pars;
        private List<IApplicationInvitation> _invitations; 
        #endregion

        #region Constructor

        private async void Load()
        {
            var fakeInventoryWebService = new FakeInventoryWebService();

            _restaurants = (await fakeInventoryWebService.GetRestaurants(new Location(), new Distance{Kilometers = 10000})).ToList();

            _employees = new List<IEmployee>();
            _vendors = new List<IVendor>();
            _restaurauntInventories = new List<IInventory>();
            _receivingOrders = new List<IReceivingOrder>();
            _pars = new List<IPar>();
            _purchaseOrders = new List<IPurchaseOrder>();
            _invitations = new List<IApplicationInvitation>();
            foreach (var rest in _restaurants)
            {
                _employees.AddRange(await fakeInventoryWebService.GetEmployeesForRestaurant(rest.Id));
                _vendors.AddRange(await fakeInventoryWebService.GetVendorsAssociatedWithRestaurant(rest.Id));
                _restaurauntInventories.AddRange(await fakeInventoryWebService.GetInventoriesForRestaurant(rest.Id));
                _receivingOrders.AddRange(await fakeInventoryWebService.GetReceivingOrdersForRestaurant(rest.Id));
                _pars.AddRange(await fakeInventoryWebService.GetPARsForRestaurant(rest.Id));
                _invitations.AddRange(await fakeInventoryWebService.GetInvitationsForRestaurant(rest.Id));
            }
        }
        public FakeInventoryServiceDAL()
        {
            Load();
        }
			

        #endregion

        public Task<IEnumerable<IAccount>> GetAccountsAsync()
        {
            var accounts = new List<IAccount>
            {
                MakeEmployeeAccount(new PersonName("Justin", "Elliott"), new EmailAddress("justin.elliott@misepos.com"), new ReferralCode("TAKEMONDAYOFF"), new PhoneNumber()),
                MakeEmployeeAccount(new PersonName("Mathieson", "Sterling"), new EmailAddress("mathieson@misepos.com"), new ReferralCode("DRANKWITHMATTY"), new PhoneNumber("718", "7152945")),
                MakeEmployeeAccount(new PersonName("Dave", "Stewart"), new EmailAddress("dave@misepos.com"), new ReferralCode("WHIZK3Y"), new PhoneNumber()),
                MakeEmployeeAccount(new PersonName("Andrew", "Siegler"), new EmailAddress("andrew@misepos.com"), new ReferralCode("CANNEDHEATBLUES"), new PhoneNumber()),
                MakeEmployeeAccount(new PersonName("Emily", "Perkins"), new EmailAddress("emily@misepos.com"), new ReferralCode("SHIFTDRINK"), new PhoneNumber())
            };
            return Task.FromResult(accounts.AsEnumerable());
        }

        private static MiseEmployeeAccount MakeEmployeeAccount(PersonName name, EmailAddress email, ReferralCode code, PhoneNumber phone)
        {
            return new MiseEmployeeAccount
            {
                AccountHolderName = name,
                AppsOnAccount = new List<MiseAppTypes> {MiseAppTypes.StockboyMobile},
                BillingCycle = TimeSpan.MaxValue,
                CreatedDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                CurrentCard = null,
                PrimaryEmail = email,
                Emails = new List<EmailAddress> {email},
                LastUpdatedDate = DateTime.UtcNow,
                ReferralCodeForAccountToGiveOut = code,
                PhoneNumber = phone,
                Status = MiseAccountStatus.Active,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.DummyData, OrderingID = 1}
            };
        }

        public Task<IAccount> GetAccountByIDAsync(Guid accountID)
        {
            throw new NotImplementedException();
        }

        public Task AddAccountAsync(IAccount account)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAccountAysnc(IAccount account)
        {
            throw new NotImplementedException();
        }

        public Task<IRestaurant> GetRestaurantAsync(Guid restaurantID)
        {
            return Task.FromResult(_restaurants.FirstOrDefault(r => r.Id == restaurantID) as IRestaurant);
        }

        public Task<IEnumerable<IRestaurant>> GetRestaurantsAsync()
        {
            return Task.FromResult(_restaurants.AsEnumerable().Cast<IRestaurant>());
        }

        public Task AddRestaurantAsync(IRestaurant restaurant)
        {
            if (_restaurants.Select(r => r.Id).Contains(restaurant.Id) == false)
            {
                _restaurants.Add(restaurant as Restaurant);
            }
            return Task.FromResult(true);
        }

        public Task UpdateRestaurantAsync(IRestaurant restaurant)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Menu>> GetMenusAsync(Guid restaurantID)
        {
            throw new NotImplementedException();
        }

        public Task<Menu> GetMenuByIDAsync(Guid menuID)
        {
            throw new NotImplementedException();
        }

        public Task AddMenuAsync(Menu menu)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMenuAsync(Menu menu)
        {
            throw new NotImplementedException();
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

        public Task<IEnumerable<IEmployee>> GetEmployeesAsync(Guid restaurantID)
        {
            var emps = new List<IEmployee>();
            foreach (var emp in _employees)
            {
                if (emps.Contains(emp) == false && emp.GetRestaurantIDs().Contains(restaurantID))
                {
                    emps.Add(emp);
                }
            }
            return Task.FromResult(emps.AsEnumerable());
        }

        public Task<IEnumerable<IEmployee>> GetInventoryAppUsingEmployeesAsync()
        {
            return Task.Run(() => _employees.AsEnumerable());
        }

        public Task AddEmployeeAsync(IEmployee employee)
        {
            return Task.Run(() => _employees.Add(employee));
        }

        public Task UpdateEmployeeAsync(IEmployee employee)
        {
            return Task.Run(() =>
            {
                var existing = _employees.FirstOrDefault(emp => employee.Id == emp.Id);
                if (existing == null) return;

                _employees.Remove(existing);
                _employees.Add(employee);
            });
        }

        public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
        {
			return Task.FromResult (true);
        }

        public Task<IEnumerable<IInventory>> GetInventoriesAsync(Guid restaurantID)
        {
            var invs = _restaurauntInventories.Where(ri => ri.RestaurantID == restaurantID);
            return Task.FromResult(invs);
        }

        public Task<IEnumerable<IInventory>> GetInventoriesAsync(DateTimeOffset dateSince)
        {
            return Task.FromResult(_restaurauntInventories.AsEnumerable());
        }

        public Task AddInventoryAsync(IInventory inventory)
        {
            if (_restaurauntInventories.Select(ri => ri.Id).Contains(inventory.Id) == false)
            {
                _restaurauntInventories.Add(inventory);
            }
			return Task.FromResult (false);
        }

        public Task UpdateInventoryAsync(IInventory inventory)
        {
            var foundInv = _restaurauntInventories.FirstOrDefault(ri => ri.Id == inventory.Id);
            if (foundInv != null)
            {
                _restaurauntInventories.Remove(foundInv);
                _restaurauntInventories.Add(inventory);
            }
			return Task.FromResult (false);
        }

        public Task UpdateVendorAsync(IVendor newVersion)
        {
            throw new NotImplementedException();
        }

        public Task AddVendorAsync(IVendor newVersion)
        {
            return Task.Run(() => _vendors.Add(newVersion));
        }

        public Task<IEnumerable<IVendor>> GetVendorsAsync()
        {
            return Task.FromResult( _vendors.AsEnumerable());
        }

        public Task UpdatePurchaseOrderAsync(IPurchaseOrder newVersion)
        {
            throw new NotImplementedException();
        }

        public Task AddPurchaseOrderAsync(IPurchaseOrder newVersion)
        {
			return Task.Run(() => _purchaseOrders.Add(newVersion));
        }

        public Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersAsync(DateTimeOffset maxTimeBack)
        {
            return Task.FromResult( _purchaseOrders.AsEnumerable());
        }

        public Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersAsync(Guid restaurantID)
        {
            return Task.FromResult(_purchaseOrders.Where(po => po.RestaurantID == restaurantID));
        }

        public Task UpdateReceivingOrderAsync(IReceivingOrder arg)
        {
            throw new NotImplementedException();
        }

        public Task AddReceivingOrderAsync(IReceivingOrder arg)
        {
            return Task.Run(() => _receivingOrders.Add(arg));
        }

        public Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersAsync(DateTimeOffset maxTimeBack)
        {
            return Task.FromResult( _receivingOrders.AsEnumerable());
        }

        public Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersAsync(Guid restaurantID)
        {
            return Task.FromResult( _receivingOrders.Where(ro => ro.RestaurantID == restaurantID));
        }

        public Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersAsync(IVendor vendor)
        {
            return Task.FromResult( _receivingOrders.Where(ro => ro.VendorID == vendor.Id));
        }

        public Task UpdatePARAsync(IPar arg)
        {
            throw new NotImplementedException();
        }

        public Task AddPARAsync(IPar arg)
        {
            return Task.Run(() => _pars.Add(arg));
        }

        public Task<IEnumerable<IPar>> GetPARsAsync(Guid restaurantID)
        {
            return Task.FromResult(_pars.Where(p => p.RestaurantID == restaurantID));
        }

        public Task<IEnumerable<IPar>> GetPARsAsync()
        {
            return Task.FromResult(_pars.AsEnumerable());
        }

        public Task<IEnumerable<LiquidContainer>> GetAllLiquidContainersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IApplicationInvitation>> GetOpenApplicationInvitations(EmailAddress destination)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IApplicationInvitation>> GetApplicationInvitations()
        {
            return Task.FromResult(_invitations.AsEnumerable());
        }

        public Task AddApplicationInvitiation(IApplicationInvitation invite)
        {
            _invitations.Add(invite);
            return Task.FromResult(true);
        }

        public Task UpdateApplicationInvitation(IApplicationInvitation invite)
        {
            var exIn = _invitations.FirstOrDefault(i => i.Id == invite.Id);
            _invitations.Remove(exIn);
            _invitations.Add(exIn);

            return Task.FromResult(true);
        }

        public Task UpdatePARLineItemAsync(IParBeverageLineItem lineItem)
        {
            var par = _pars.FirstOrDefault(p => p.GetBeverageLineItems().Select(pLi => pLi.Id).Contains(lineItem.Id));
            if (par != null)
            {
                var downCast = par as Par;
                if (downCast == null)
                {
                    return Task.FromResult(false);
                }
                var oldLI = downCast.ParLineItems.FirstOrDefault(l => l.Id == lineItem.Id);
                downCast.ParLineItems.Remove(oldLI);
                downCast.ParLineItems.Add(lineItem as ParBeverageLineItem);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task DeletePARLineItemAsync(Guid lineItemID)
        {
            throw new NotImplementedException();
        }

        public Task SetLineItemsForPARAsync(IParBeverageLineItem lineItem, Guid parID)
        {
            throw new NotImplementedException();
        }

        public Task CreateCategories(ICollection<ICategory> categories)
        {
            return Task.FromResult(true);
        }
    }
}
