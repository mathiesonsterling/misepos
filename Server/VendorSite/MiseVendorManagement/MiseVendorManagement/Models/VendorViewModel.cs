using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Vendors;

namespace MiseVendorManagement.Models
{
    public class VendorViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string StreetAddressNumber { get; set; }
        public string StreetDirection { get; set; }
        public string UnitNumber { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        public string Email { get; set; }
        public string PhoneAreaCode { get; set; }
        public string PhoneNumber { get; set; }

        public string StreetName { get; set; }

        public bool HasItems { get; set; }

        public IEnumerable<string> PossibleStates { get; private set; }

        public VendorViewModel()
        {
            PossibleStates = Mise.Core.ValueItems.State.GetUSStates().Select(s => s.Name).OrderBy(s => s);
        }

        public VendorViewModel(IVendor v) : this()
        {
            Id = v.ID;
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
        }
    }
}
