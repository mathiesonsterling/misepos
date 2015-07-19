using System;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities
{
    /// <summary>
    /// Represents a single POS client, regardless of platform
    /// TODO extract out info for other apps that aren't terminal to a base interface
    /// </summary>
    public interface IMiseTerminalDevice : IRestaurantEntityBase
    {
        string DisplayName { get; }

        string FriendlyID { get; }

		MiseAppTypes Application{get;}

        /// <summary>
        /// If set, this is the unique machine address that is assigned to this terminal.  Allows new devices to 
        /// easily setup
        /// </summary>
        string MachineID { get; }

        /// <summary>
        /// If true, this terminal is up and ready to do business
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// This allows us to show if this is the same terminal as the client holding it
        /// </summary>
        bool IsMe { get; set; }

        /// <summary>
        /// The top level category we'll display here
        /// </summary>
		Guid? TopLevelCategoryID { get; }

		/// <summary>
		/// If true, we require the employee to sign in before altering tabs
		/// </summary>
		bool RequireEmployeeSignIn { get; }

		/// <summary>
		/// If true, we drop checks.  If not, we send directly to payments when closing
		/// </summary>
		/// <value><c>true</c> if table drop checks; otherwise, <c>false</c>.</value>
		bool TableDropChecks{get;}

		/// <summary>
		/// If true the terminal can do cash drawer stuff.  If not it's cards only!
		/// </summary>
		/// <value><c>true</c> if this instance has cash drawer; otherwise, <c>false</c>.</value>
		bool HasCashDrawer{get;}

        /// <summary>
        /// If true, this device should print kitchen tickets as well as sending them to the kitchen printer (if there is one)
        /// </summary>
        bool PrintKitchenDupes { get; }

		/// <summary>
		/// How this terminal can read or accept credit cards
		/// </summary>
		/// <value>The type of the credit card reader.</value>
		CreditCardReaderType CreditCardReaderType{get;}

		/// <summary>
		/// If true, we wait until we Z cards to send them to our processor.  Otherwise, we do once the tip amount is added.
		/// </summary>
		/// <value><c>true</c> if wait for Z to close cards; otherwise, <c>false</c>.</value>
		bool WaitForZToCloseCards{get;}

    }
}
