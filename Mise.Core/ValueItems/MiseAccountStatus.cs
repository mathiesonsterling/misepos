using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems
{
    public enum MiseAccountStatus
    {
        /// <summary>
        /// Created, but for some reason not yet activated
        /// </summary>
        NotActivated,
        /// <summary>
        /// Using a free demo period
        /// </summary>
        FreeDemoPeriod,
        Active,
        Overdue,
        /// <summary>
        /// Cancelled, but still active till next bill
        /// </summary>
        CancelledButStillActive,
        /// <summary>
        /// Cancel requested by the user
        /// </summary>
        Cancelled,
        CancelledFully,
        SuspendedByRequest,
        SuspendedForNonPayment,
    }
}
