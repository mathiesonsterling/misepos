using System;
using Mise.Core.Entities;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Employee
{
	public abstract class BaseEmployeeEvent : IEmployeeEvent
	{

		protected BaseEmployeeEvent ()
		{
			CreatedDate = DateTime.UtcNow;
			ID = Guid.NewGuid ();
		}


		#region IEmployeeEvent implementation
		private Guid _empID;
		public Guid EmployeeID {
			get{return _empID;}
			set{
				_empID = value;
				CausedByID = value;
			}
		}

		/// <summary>
		/// Device that this event occurred on
		/// </summary>
		/// <value>The device I.</value>
		public string DeviceID {
			get;
			set;
		}
		public Guid RestaurantID {
			get;
			set;
		}
			
		#endregion

		#region IEntityEventBase implementation
		public Guid ID{get;set;}

	    public abstract MiseEventTypes EventType { get;}
		public EventID EventOrderingID {
			get;
			set;
		}
		public Guid CausedByID {
			get;
			set;
		}
		public DateTimeOffset CreatedDate {
			get;
			set;
		}
		#endregion
	}
}

