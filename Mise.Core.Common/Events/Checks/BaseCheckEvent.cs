using System;
using Mise.Core.Entities;
using Mise.Core.Entities.Check.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Checks
{
	public abstract class BaseCheckEvent : ICheckEvent
	{
	    /// <summary>
		/// The class type, reflected in the object
		/// </summary>
		/// <value>The type of the event.</value>
		public abstract MiseEventTypes EventType{ get;}
        public virtual bool IsEntityCreation { get { return false; } }
        public virtual bool IsAggregateRootCreation { get { return false; } }

		#region ICheckEvent implementation
		public Guid CheckID {
			get;
			set;
		}

		public Guid CausedById{ get; set; }

		private Guid _empID;
		public Guid EmployeeID {
			get{return _empID;}
			set{
				_empID = value;
				CausedById = value;
			}
		}

		public string DeviceId {
			get;
			set;
		}

		public Guid RestaurantId {
			get;
			set;
		}

		#endregion

		#region IEntityEvent implementation
		public Guid Id{ get; set;}

		public EventID EventOrder {
			get;
			set;
		}

		public int VersionGeneratedFrom {
			get;
			set;
		}

		public DateTimeOffset CreatedDate {
			get;
			set;
		}

		#endregion
		protected BaseCheckEvent()
		{
			CreatedDate = DateTime.UtcNow;
			Id = Guid.NewGuid ();
		}
	}
}

