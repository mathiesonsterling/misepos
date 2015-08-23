using System.Threading.Tasks;

namespace Mise.Core.Common.Services.WebServices
{
	/// <summary>
	/// Holder for a service which can do all our subservices
	/// </summary>
	public interface IInventoryApplicationWebService : IInventoryEmployeeWebService, 
	IInventoryRestaurantWebService, IVendorWebService,
	IParWebService, IInventoryWebService,
	IReceivingOrderWebService, IPurchaseOrderWebService,
	IApplicationInvitationWebService, IAccountWebService
	{
		/// <summary>
		/// Attempt to get us in line with the server
		/// </summary>
		/// <returns>The with server.</returns>
		Task<bool> SynchWithServer();
	}
}

