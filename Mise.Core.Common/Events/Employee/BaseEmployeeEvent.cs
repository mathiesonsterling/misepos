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
			Id = Guid.NewGuid ();
		}


		#region IEmployeeEvent implementation
		private Guid _empID;
		public Guid EmployeeID {
			get{return _empID;}
			set{
				_empID = value;
				CausedById = value;
			}
		}

		/// <summary>
		/// Device that this event occurred on
		/// </summary>
		/// <value>The device I.</value>
		public string DeviceId {
			get;
			set;
		}
		public Guid RestaurantId {
			get;
			set;
		}
			
		#endregion

		#region IEntityEventBase implementation
		public Guid Id{get;set;}

	    public abstract MiseEventTypes EventType { get;}

        public virtual bool IsEntityCreation { get { return false; } }
        public virtual bool IsAggregateRootCreation { get { return false; } }
		public EventID EventOrder {
			get;
			set;
		}
		public Guid CausedById {
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

