using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [DisplayName("Area Code")]
        public string PhoneAreaCode { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }


        public bool HasItems { get; set; }

        public IEnumerable<string> PossibleStates { get; private set; }

        public IEnumerable<VendorItemForSaleViewModel> ItemsForSale { get; private set; }
         
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
            }
            HasItems = v.GetItemsVendorSells().Any();
            ItemsForSale = v.GetItemsVendorSells().Select(li => new VendorItemForSaleViewModel(li));
        }

    }
}
