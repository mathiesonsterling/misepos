﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

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
	    public async Task<IVendor> AddVendor(BusinessName name, StreetAddress address, PhoneNumber phoneNumber, EmailAddress email)
		{
			var emp = await _loginService.GetCurrentEmployee ();

			//make the create event
			var create = _eventFactory.CreateVendorCreatedEvent(emp, name, address, phoneNumber, email);

			_selectedVendor = _vendorRepository.ApplyEvent (create);

			//TODO - check we don't get any name conflicts
			var alreadyExists = _vendorRepository.GetAll().Any(v => v.IsSameVendor(_selectedVendor));
			if(alreadyExists){
				_vendorRepository.CancelTransaction (_selectedVendor.Id);
				_selectedVendor = null;
				throw new ArgumentException ("This vendor already exists!");
			}
			await _vendorRepository.Commit (_selectedVendor.Id);

			return _selectedVendor;
		}
			
		/// <summary>
		/// List the vendors that deal with the given restaurant
		/// </summary>
		/// <param name="restaurant">Restaurant.</param>
		public Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant(IRestaurant restaurant)
		{
			return _vendorRepository.GetVendorsAssociatedWithRestaurant (restaurant.Id);
		}

		public Task<IVendor> GetBestVendorForItem (IBaseBeverageLineItem li, decimal quantity, IRestaurant restaurant)
		{
			var lowestPrice = new Money(decimal.MaxValue);
            IVendor foundVendor = null;
			foreach(var vendor in _vendorRepository.GetAll ()){
                var vendorPrice = vendor.GetPriceForLineItem (li, quantity, restaurant?.Id);
				if(vendorPrice != null){
					if(foundVendor == null || lowestPrice.GreaterThan (vendorPrice)){
						foundVendor = vendor;
						lowestPrice = vendorPrice;
					} 
				}
			}

		    if (foundVendor != null)
		    {
		        return Task.FromResult(foundVendor);
		    }
		    //is there only ONE vendor with the item?
		    var anyVendors = _vendorRepository.GetAll ().Where(v => v.DoesCarryItem (li, quantity)).ToList();
            if (anyVendors.Count == 1)
            {
                foundVendor = anyVendors.First();
            }
            else
            {
                IList<IVendor> vendorsToPickFrom;
                //first find vendors in state
                if (restaurant != null && restaurant.StreetAddress != null)
                {
                    vendorsToPickFrom = anyVendors.Where(v =>
                            v.StreetAddress != null
                        && v.StreetAddress.State != null
                        && v.StreetAddress.State.Equals(restaurant.StreetAddress.State)
                    ).ToList();
                }
                else
                {
                    vendorsToPickFrom = anyVendors.ToList();
                }

                var numVendors = vendorsToPickFrom.Count;
                var rand = new Random();
                var index = rand.Next(numVendors - 1);

                foundVendor = vendorsToPickFrom[index];

            }
		    return Task.FromResult(foundVendor);
		}

		/// <summary>
		/// If we've received an order from a vendor, we'll want to make sure it's listed among the vendors
		/// </summary>
		/// <returns>The line item to vendor if doesnt exist.</returns>
		/// <param name="vendorID">Vendor I.</param>
		/// <param name="lineItems">Li.</param>
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
						_logger.Error ("Unable to resolve line item " + lineItem.DisplayName + " id " + lineItem.Id);
					} else {
						//TODO - if we've got a price being reported that is GREATER than the public price, there's a problem!
						var priceEv = _eventFactory.CreateRestaurantSetPriceEvent (emp, item, vendor, lineItem.UnitPrice);
						vendor = _vendorRepository.ApplyEvent (priceEv);
					}
				}
					
				await _vendorRepository.Commit (vendor.Id);
			}
		}
	}
}


