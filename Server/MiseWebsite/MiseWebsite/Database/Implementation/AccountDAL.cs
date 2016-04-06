using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace MiseWebsite.Database.Implementation
{
    public class AccountDAL : IAccountDAL
    {
        public async Task<IEnumerable<IResellerAccount>> GetActiveResellerAccounts()
        {
            using (var context = new StronglyTypedEntitiesDbContext())
            {
                var dtos = await context.ResellerAccounts.Where(ra => ra.IsActive).ToListAsync();
                return dtos.Select(Hydrate);
            }
        }

        public async Task<IEnumerable<IResellerAccount>> GetResellerAccounts(EmailAddress email)
        {
            using (var context = new StronglyTypedEntitiesDbContext())
            {
                var items =
                    await
                        context.ResellerAccounts.Where(
                            ra => ra.PrimaryEmail == email.Value || ra.Emails.Contains(email.Value))
                            .ToListAsync();
                return items.Select(Hydrate);
            }
        }


        public async Task<IResellerAccount> GetResellerAccount(Guid id)
        {
            using (var context = new StronglyTypedEntitiesDbContext())
            {
                var item = await context.ResellerAccounts.FirstOrDefaultAsync(ra => ra.Id == id);
                return item == null ? null : Hydrate(item);
            }
        }

        public async Task<IResellerAccount> AddResellerAccount(IResellerAccount acct)
        {
            var storage = new ResellerAccountStorage(acct);
            using (var context = new StronglyTypedEntitiesDbContext())
            {
                context.ResellerAccounts.Add(storage);
                await context.SaveChangesAsync();
            }
            return acct;
        }

        public async Task<IResellerAccount> UpdateResellerAccount(IResellerAccount acct)
        {
            var storage = new ResellerAccountStorage(acct);
            using (var context = new StronglyTypedEntitiesDbContext())
            {
                var existingItem = await context.ResellerAccounts.FirstOrDefaultAsync(ra => ra.Id == storage.Id);

                if (existingItem != null)
                {
                    context.Entry(existingItem).CurrentValues.SetValues(storage);
                }
                await context.SaveChangesAsync();
            }

            return acct;
        }

        private static IResellerAccount Hydrate(ResellerAccountStorage source)
        {
            var item = new ResellerAccount
            {
                AccountHolderName = new PersonName(source.FirstName, source.MiddleName, source.LastName),
                CreatedDate = source.CreatedDate,
                LastUpdatedDate = source.LastUpdatedDate,
                Emails = source.Emails.Select(e => new EmailAddress(e)),
                Id = source.Id,
                IsActive = source.IsActive,
                PhoneNumber = new PhoneNumber(source.AreaCode, source.PhoneNumber),
                PrimaryEmail = source.PrimaryEmail != null ? new EmailAddress(source.PrimaryEmail) : null,
                ReferralCodeForAccountToGiveOut =
                    source.ReferralCodeToGiveOut != null ? new ReferralCode(source.ReferralCodeToGiveOut) : null,
                ReferralCodeUsedToCreate =
                    source.ReferralCodeCreatedWith != null ? new ReferralCode(source.ReferralCodeCreatedWith) : null,
                ResellerUnderId = source.ResellerUnderId,
                Revision = new EventID(source.Revision)
            };

            return item;
        }
    }
}