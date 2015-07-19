using System;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;

namespace Mise.Core.Common.Events
{
	public abstract class BaseCheckEvent : ICheckEvent
	{
		/// <summary>
		/// The class type, reflected in the object
		/// </summary>
		/// <value>The type of the event.</value>
		public abstract string EventType{ get;}

	    public virtual ICheck ApplyEvent(ICheck check)
	    {
	        check.LastTouchedServerID = EmployeeID;
			//when this event was created was the last time we updated the check
			check.LastUpdatedDate = CreatedDate;

			//revision gets updated here?
			check.Revision = EventOrderingID;
	        return check;
	    }

		#region ICheckEvent implementation
		public Guid CheckID {
			get;
			set;
		}

		public Guid CausedByID{ get; set; }

		public Guid EmployeeID {
			get;
			set;
		}

		public Guid TerminalID {
			get;
			set;
		}

		public Guid RestaurantID {
			get;
			set;
		}

		#endregion

		#region IEntityEvent implementation
		public Guid ID{ get; set;}

		public long EventOrderingID {
			get;
			set;
		}

		public int VersionGeneratedFrom {
			get;
			set;
		}

		public DateTime CreatedDate {
			get;
			set;
		}

		#endregion
		protected BaseCheckEvent()
		{
			CreatedDate = DateTime.UtcNow;
			ID = Guid.NewGuid ();
		}
	}
}

