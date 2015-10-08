namespace Mise.Core.Entities
{
	/// <summary>
	/// Different types of 
	/// </summary>
	public enum MiseAppTypes
	{
		UnitTests,

		/// <summary>
		/// Default data done as a placeholder
		/// </summary>
		DummyData,

		/// <summary>
		/// Our POS system, running on mobile devices
		/// </summary>
		POSMobile,
		/// <summary>
		/// Stockboy inventory app, running on mobile
		/// </summary>
		StockboyMobile,

        /// <summary>
        /// Imported for vendor via CSV upload
        /// </summary>
        VendorManagement,

        /// <summary>
        /// Reporting site
        /// </summary>
        ManagementWebsite,
	}
}

