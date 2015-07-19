using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Accounts
{
    public interface IAccountPayment : IEntityBase
    {
        Guid AccountID { get; }

        Money Amount { get; }

        PaymentType PaymentType { get; }

        PaymentProcessingStatus Status { get; }

    }
}
