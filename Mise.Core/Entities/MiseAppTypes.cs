namespace Mise.Core.Entities
{
	/// <summary>
	/// Different types of 
	/// </summary>
	public enum MiseAppTypes
	{
		UnitTests = 0,

		/// <summary>
		/// Default data done as a placeholder
		/// </summary>
		DummyData = 1,

		/// <summary>
		/// Our POS system, running on mobile devices
		/// </summary>
		POSMobile= 2,
		/// <summary>
		/// Stockboy inventory app, running on mobile
		/// </summary>
		StockboyMobile= 3,

        /// <summary>
        /// Imported for vendor via CSV upload
        /// </summary>
        VendorManagement = 4,

        /// <summary>
        /// Reporting site
        /// </summary>
        ManagementWebsite = 5,

        /// <summary>
        /// Backend service to support mobile client
        /// </summary>
        MobileService = 6,
	}
}

