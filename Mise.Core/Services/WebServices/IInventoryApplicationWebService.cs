using System;

namespace Mise.Core.Services.WebServices
{
	/// <summary>
	/// Holder for a service which can do all our subservices
	/// </summary>
	public interface IInventoryApplicationWebService : IInventoryEmployeeWebService, 
	IInventoryRestaurantWebService, IVendorWebService,
	IPARWebService, IInventoryWebService,
	IReceivingOrderWebService, IPurchaseOrderWebService,
	IApplicationInvitationWebService, IAccountWebService
	{
	}
}

