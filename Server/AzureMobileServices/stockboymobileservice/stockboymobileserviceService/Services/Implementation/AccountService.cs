using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Events;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using stockboymobileserviceService.DataObjects;
using stockboymobileserviceService.Models;

namespace stockboymobileserviceService.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IEntityDataTransportObjectFactory _dtoFactory;
        private readonly IInventoryAppEventFactory _eventFactory;
        public AccountService()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _eventFactory = new InventoryAppEventFactory("MobileServices", MiseAppTypes.MobileService);
        }

        public AccountService(IEntityDataTransportObjectFactory dtoFactory, IInventoryAppEventFactory eventFactory)
        {
            _dtoFactory = dtoFactory;
            _eventFactory = eventFactory;
        }

        public async Task<IEnumerable<IAccount>> GetAccountsWaitingForPaymentPlan()
        {
            var accountType = typeof(RestaurantAccount).ToString();
            const string MISSING_ACCOUNTS_TAG = "\"PaymentPlanSetupWithProvider\":false";

            IEnumerable<AzureEntityStorage> accountAIs;
            using (var context = new stockboymobileserviceContext())
            {
                accountAIs = await context.AzureEntityStorages.Where(a => a.MiseEntityType == accountType && a.EntityJSON.Contains(MISSING_ACCOUNTS_TAG))
                    .ToListAsync();
            }

            if (accountAIs == null)
            {
                return null;
            }

            var dtos = accountAIs.Select(ai => ai.ToRestaurantDTO());
            var accounts = dtos.Select(dto => _dtoFactory.FromDataStorageObject<RestaurantAccount>(dto));
            return accounts;
        }

        public async Task MarkAccountsAsHavingPaymentPlan(IEnumerable<IAccount> accounts)
        {
            var fullAccounts = accounts.ToList();
            //update the accounts
            foreach (var account in fullAccounts)
            {
                var ev = _eventFactory.CreateAccountHasPaymentPlanSetupEvent(account);
                account.When(ev);

                //todo - save the event into our DB as well

                //save the change!
                await UpdateAccount(account);
            }
        }

        private async Task UpdateAccount(IAccount account)
        {
            var downgrade = account as RestaurantAccount;
            if (downgrade == null)
            {
                return;
            }

            var typeString = typeof (RestaurantAccount).ToString();
            using (var db = new stockboymobileserviceContext())
            {
                var oldVer =
                    await
                        db.AzureEntityStorages.FirstOrDefaultAsync(
                            ai => ai.EntityID == account.Id && ai.MiseEntityType == typeString);
                if (oldVer == null)
                {
                    throw new InvalidOperationException("Unable to find existing account to update");
                }
                var dto = _dtoFactory.ToDataTransportObject(downgrade);

                var newVer = new AzureEntityStorage(dto);
                oldVer.EntityJSON = newVer.EntityJSON;
                oldVer.LastUpdatedDate = DateTimeOffset.UtcNow;
                db.Entry(oldVer).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
