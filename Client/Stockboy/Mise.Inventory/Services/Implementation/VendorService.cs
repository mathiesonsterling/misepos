using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Vendors;
using Mise.Core.Repositories;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events;
using Mise.Core.Services;
using Mise.Core.Entities.Vendors.Events;

namespace Mise.Inventory.Services.Implementation
{
	public class VendorService : IVendorService
	{
		public Guid? RestaurantID{ get; set; }

		//readonly IDeviceLocationService _deviceLocationService;
		readonly ILogger _logger;
		readonly ILoginService _loginService;
		readonly IVendorRepository _vendorRepository;
		readonly IInventoryAppEventFactory _eventFactory;
		private IVendor _selectedVendor;
		public VendorService(ILogger logger, ILoginService loginService, 
			IVendorRepository vendorRepository, IInventoryAppEventFactory eventFactory)
		{
			_logger = logger;
			//_deviceLocationService = deviceLocationService;
			_vendorRepository = vendorRepository;
			_loginService = loginService;
			_selectedVendor = null;
			_eventFactory = eventFactory;
		}

		public Task<IEnumerable<IVendor>> GetPossibleVendors ()
		{
			var vendors = _vendorRepository.GetAll ();
			return Task.FromResult (vendors);
		}

		public Task SelectVendor (IVendor vendor)
		{
			_selectedVendor = vendor;
			return Task.FromResult (true);
		}

		public Task<IVendor> GetSelectedVendor ()
		{
			return Task.FromResult (_selectedVendor);
		}

	    /// <summary>
	    /// Add the specified vendor.
	    /// </summary>
	    /// <param name="name">Name.</param>
	    /// <param name = "address"></param>
	    /// <param name = "phoneNumber"></param>
	    /// <param name="email"></param>
	    public async Task<IVendor> AddVendor(string name, StreetAddress address, PhoneNumber phoneNumber, EmailAddress email)
		{
			var emp = await _loginService.GetCurrentEmployee ();

			//make the create event
			var create = _eventFactory.CreateVendorCreatedEvent(emp, name, address, phoneNumber, email);

			_selectedVendor = _vendorRepository.ApplyEvent (create);

			//TODO - check we don't get any name conflicts
			var alreadyExists = _vendorRepository.GetAll().Any(v => v.IsSameVendor(_selectedVendor));
			if(alreadyExists){
				_vendorRepository.CancelTransaction (_selectedVendor.ID);
				_selectedVendor = null;
				throw new ArgumentException ("This vendor already exists!");
			}
			await _vendorRepository.Commit (_selectedVendor.ID);

			return _selectedVendor;
		}
			
		/// <summary>
		/// List the vendors that deal with the given restaurant
		/// </summary>
		/// <param name="restaurant">Restaurant.</param>
		public Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant(IRestaurant restaurant)
		{
			return _vendorRepository.GetVendorsAssociatedWithRestaurant (restaurant.ID);
		}

		public Task<IVendor> GetVendorWithLowestPriceForItem (IBaseBeverageLineItem li, decimal quantity, Guid? restaurantID)
		{
			var lowestPrice = new Money{Dollars = decimal.MaxValue};
			IVendor lowestVendor = null;
			foreach(var vendor in _vendorRepository.GetAll ()){
				var vendorPrice = vendor.GetPriceForLineItem (li, quantity, restaurantID);
				if(vendorPrice != null){
					if(lowestVendor == null || lowestPrice.GreaterThan (vendorPrice)){
						lowestVendor = vendor;
						lowestPrice = vendorPrice;
					} 
				}
			}

		    if (lowestVendor != null)
		    {
		        return Task.FromResult(lowestVendor);
		    }
		    //is there only ONE vendor with the item?
		    var anyVendors = _vendorRepository.GetAll ().Where(v => v.DoesCarryItem (li, quantity)).ToList();
		    if(anyVendors.Count == 1){
		        lowestVendor = anyVendors.First();
		    }
		    return Task.FromResult(lowestVendor);
		}

		/// <summary>
		/// If we've received an order from a vendor, we'll want to make sure it's listed among the vendors
		/// </summary>
		/// <returns>The line item to vendor if doesnt exist.</returns>
		/// <param name="vendorID">Vendor I.</param>
		/// <param name="lis">Li.</param>
		public async Task AddLineItemsToVendorIfDontExist (Guid vendorID, IEnumerable<IReceivingOrderLineItem> lineItems)
		{
			var vendor = _vendorRepository.GetByID (vendorID);
			if(vendor != null){
				var emp = await _loginService.GetCurrentEmployee ();

				var lis = lineItems.Where (li => li.ZeroedOut == false)
					.OrderBy (li => li.DisplayName)
					.ToList();
				//TODO only take verified events in the future
				foreach(var lineItem in lis){
					var item = vendor.GetItemsVendorSells ()
						.OrderBy(li => li.DisplayName)
						.FirstOrDefault (vLI => BeverageLineItemEquator.AreSameBeverageLineItem(vLI, lineItem));

					if (item == null) {
						var newLIEv = _eventFactory.CreateVendorLineItemAddedEvent (emp, lineItem, vendor);
						var updatedVendor = _vendorRepository.ApplyEvent (newLIEv);
						item = updatedVendor.GetItemsVendorSells ()
							.Where(li => li.DisplayName == lineItem.DisplayName)
							.FirstOrDefault (vLI => BeverageLineItemEquator.AreSameBeverageLineItem(vLI, lineItem));
					}

					if (item == null) {
						_logger.Error ("Unable to resolve line item " + lineItem.DisplayName + " id " + lineItem.ID);
					} else {
						//TODO - if we've got a price being reported that is GREATER than the public price, there's a problem!
						var priceEv = _eventFactory.CreateRestaurantSetPriceEvent (emp, item, vendor, lineItem.UnitPrice);
						vendor = _vendorRepository.ApplyEvent (priceEv);
					}
				}
					
				await _vendorRepository.Commit (vendor.ID);
			}
		}
	}
}


