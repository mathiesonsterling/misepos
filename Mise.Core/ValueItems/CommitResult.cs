using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems
{
    /// <summary>
    /// Represents what can happen when we save an event 
    /// </summary>
    public enum CommitResult
    {
        /// <summary>
        /// We sent the item to the server, all happy
        /// </summary>
        SentToServer,
        /// <summary>
        /// We couldn't send, so we stored it in the DB to send later
        /// </summary>
        StoredInDB,
        /// <summary>
        /// We didn't have any events to commit
        /// </summary>
        NothingToCommit,
        /// <summary>
        /// Error we couldn't recover from
        /// </summary>
        Error,
    }
}
