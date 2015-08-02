using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Services.WebServices
{
	/// <summary>
	/// Holder for a service which can do all our subservices
	/// </summary>
	public interface IInventoryApplicationWebService : IInventoryEmployeeWebService, 
	IInventoryRestaurantWebService, IVendorWebService,
	IParWebService, IInventoryWebService,
	IReceivingOrderWebService, IPurchaseOrderWebService,
	IApplicationInvitationWebService, IAccountWebService, IResendEventsWebService
	{
	}
}

