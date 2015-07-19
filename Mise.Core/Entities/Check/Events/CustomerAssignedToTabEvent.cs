using Mise.Core.Entities.Check;
using Mise.Core.Entities.People;

namespace Mise.Core.Common.Events
{
	public class CustomerAssignedToTabEvent : BaseCheckEvent
	{
		public override string EventType {
			get {
				return "CustomerAssignedToTab";
			}
		}

		public Customer Customer{ get; set;}

	    public CustomerAssignedToTabEvent()
		{
		}

		public CustomerAssignedToTabEvent(Customer customer)
		{
			Customer = customer;
		}

		public override ICheck ApplyEvent (ICheck check)
		{
			if (check != null) {
                check = base.ApplyEvent(check);
				check.Customer = Customer;
			}
			return check;
		}
	}
}

