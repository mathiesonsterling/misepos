using System;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Common.Entities
{
	/// <summary>
	/// Class with the default settings used for a bar terminal
	/// </summary>
	public class MiseTerminalDevice : RestaurantEntityBase, IMiseTerminalDevice
    {
        public string DisplayName { get; set; }
	    public string FriendlyID { get; set; }
	    public string MachineID { get; set; }

	    public bool IsActive { get; set; }

	    public bool IsMe { get; set; }

		public MiseAppTypes Application {get;set;}

	    public bool RequireEmployeeSignIn { get; set; }
		public Guid? TopLevelCategoryID { get; set; }

		public bool TableDropChecks { get; set;}

	    public bool PrintKitchenDupes { get; set; }

	    public CreditCardReaderType CreditCardReaderType {
			get;
			set;
		}

		public bool HasCashDrawer { get; set;}

		public bool WaitForZToCloseCards {
			get;
			set;
		}
    }
}
