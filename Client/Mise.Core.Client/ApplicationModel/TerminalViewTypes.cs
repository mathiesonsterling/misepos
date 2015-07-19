

namespace Mise.Core.Client.ApplicationModel
{
	/// <summary>
	/// Represents all possible views in our system
	/// </summary>
	public enum TerminalViewTypes
	{
		/// <summary>
		/// Clockin to the system and clock out, select which employee is currently controlling the POS
		/// </summary>
		ClockInPage,

		/// View a logged in employee's open tabs and allow an employee to clock out.
		EmployeePage,

		/// <summary>
		/// Clock in and out, view tabs that are available
		/// </summary>
        	ViewChecks,

		ClosedChecks,
		/// <summary>
		/// Add items to a tab
		/// </summary>
        	OrderOnCheck,

		/// <summary>
		/// Modify or void an already existing order item
		/// </summary>
		ModifyOrder,

		/// <summary>
		/// Not yet implemented - cover with an advertisement
		/// </summary>
		AdvertisementScreen,

		/// <summary>
		/// Make payments onto a check
		/// </summary>
		PaymentScreen,

		/// <summary>
		/// Add tips for previously entered credit card payments
		/// </summary>
		AddTips,

		/// <summary>
		/// Display change due to customer
		/// </summary>
		DisplayChange,

		NoSale,

		/// <summary>
		/// Initial setup for the terminal
		/// </summary>
		Setup,
	}
}
