﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Mise.Core.Entities.Vendors;

namespace MiseVendorManagement.Models
{
    public class VendorViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Vendor Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Street Number")]
        public string StreetAddressNumber { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        [DisplayName("Apt / Suite")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Direction (N, S, Etc)")]
        public string StreetDirection { get; set; }

        [Required]
        [DisplayName("Street Name")]
        public string StreetName { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        public string Country { get; set; }
        public string ZipCode { get; set; }

        public string Email { get; set; }

        [Required]
        [Url]
        public string Website { get; set; }

        [DisplayName("Area Code")]
        public string PhoneAreaCode { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumberDisplay { get; set; }

        [DisplayName("Items")]
        public int NumItems { get; set; }

        public IEnumerable<string> PossibleStates { get; private set; }

        public IEnumerable<VendorItemForSaleViewModel> ItemsForSale { get; set; }
         
        public VendorViewModel()
        {
            PossibleStates = Mise.Core.ValueItems.State.GetUSStates().Select(s => s.Name).OrderBy(s => s);
            Country = Mise.Core.ValueItems.Country.UnitedStates.Name;
        }

        public VendorViewModel(IVendor v) : this()
        {
            Id = v.Id;
            Name = v.Name;
            if (v.StreetAddress != null)
            {
                StreetAddressNumber = v.StreetAddress.StreetAddressNumber.Number;
                if (v.StreetAddress.StreetAddressNumber.Longitude != 0 ||
                    v.StreetAddress.StreetAddressNumber.Latitude != 0)
                {
                    Longitude = v.StreetAddress.StreetAddressNumber.Longitude;
                    Latitude = v.StreetAddress.StreetAddressNumber.Latitude;
                }

                StreetDirection = v.StreetAddress.StreetAddressNumber.Direction;
                StreetName = v.StreetAddress.Street.Name;
                City = v.StreetAddress.City.Name;
                State = v.StreetAddress.State.Name;
                Country = v.StreetAddress.Country.Name;
                ZipCode = v.StreetAddress.Zip.Value;
            }
            if (v.EmailToOrderFrom != null)
            {
                Email = v.EmailToOrderFrom.Value;
            }
            if (v.PhoneNumber != null)
            {
                PhoneAreaCode = v.PhoneNumber.AreaCode;
                PhoneNumber = v.PhoneNumber.Number;
                PhoneNumberDisplay = v.PhoneNumber.ToFormattedString();
            }
            if (v.Website != null)
            {
                Website = v.Website.AbsoluteUri;
            }

            NumItems = v.GetItemsVendorSells().Count();
            ItemsForSale = v.GetItemsVendorSells().Select(li => new VendorItemForSaleViewModel(li));
        }

    }
}
