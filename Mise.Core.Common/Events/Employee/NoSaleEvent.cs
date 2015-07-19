using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events
{
	public class NoSaleEvent : BaseEmployeeEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.NoSale;
			}
		}
	}
}

