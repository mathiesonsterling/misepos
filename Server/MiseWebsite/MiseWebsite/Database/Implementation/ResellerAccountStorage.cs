using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mise.Core.Entities.Accounts;

namespace MiseWebsite.Database.Implementation
{
    [Table("ResellerAccounts", Schema = "stockboymobile")]
    public class ResellerAccountStorage : BaseAccountStorage
    {
        public ResellerAccountStorage()
        {
            
        }

        public ResellerAccountStorage(IResellerAccount source) : base(source)
        {
            ResellerUnderId = source.ResellerUnderId;
            IsActive = source.IsActive;
        }

        public Guid? ResellerUnderId { get; set; }
        public bool IsActive { get; set; }
    }
}