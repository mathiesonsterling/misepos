using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;

namespace Mise.Core.Repositories
{
    public interface IReceivingOrderRepository : IEventSourcedEntityRepository<IReceivingOrder, IReceivingOrderEvent>
    {
    }
}
