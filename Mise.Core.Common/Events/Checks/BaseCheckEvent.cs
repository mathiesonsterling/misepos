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


		#region ICheckEvent implementation
		public Guid CheckID {
			get;
			set;
		}

		public Guid CausedByID{ get; set; }

		private Guid _empID;
		public Guid EmployeeID {
			get{return _empID;}
			set{
				_empID = value;
				CausedByID = value;
			}
		}

		public string DeviceID {
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

		public EventID EventOrderingID {
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
			ID = Guid.NewGuid ();
		}
	}
}

