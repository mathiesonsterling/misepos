using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class StreetAddressNumberDb : Core.ValueItems.StreetAddressNumber, IDbValueItem<Core.ValueItems.StreetAddressNumber>
    {
        public Core.ValueItems.StreetAddressNumber ToValueItem()
        {
            return new Core.ValueItems.StreetAddressNumber
            {
                ApartmentNumber = ApartmentNumber,
                Direction = Direction,
                Latitude = Latitude,
                Longitude = Longitude,
                Number = Number
            };
        }
    }

    [ComplexType]
    public class Street : Core.ValueItems.Street, IDbValueItem<Core.ValueItems.Street>
    {
        public Core.ValueItems.Street ToValueItem()
        {
            return new Core.ValueItems.Street
            {
                Name = Name
            };
        }
    }

    [ComplexType]
    public class City : Core.ValueItems.City
    {
    }

    [ComplexType]
    public class State : Core.ValueItems.State { }

    [ComplexType]
    public class Country : Core.ValueItems.Country { }

    [ComplexType]
    public class ZipCode : Core.ValueItems.ZipCode { }

    [ComplexType]
    public class StreetAddress : IDbValueItem<Core.ValueItems.StreetAddress>
    {
        public StreetAddressNumberDb StreetAddressNumberDb { get; set; }
        public Street Street { get; set; }
        public City City { get; set; }
        public State State { get; set; }
        public Country Country { get; set; }
        public ZipCode Zip { get; set; }

        public StreetAddress()
        {
            StreetAddressNumberDb = new StreetAddressNumberDb();
            Street = new Street();
            City = new City();
            State = new State();
            Country = new Country();
            Zip = new ZipCode();
        }

        public Core.ValueItems.StreetAddress ToValueItem()
        {
            return new Mise.Core.ValueItems.StreetAddress
            {
                City = City,
                Street = Street,
                Country = Country,
                State = State,
                StreetAddressNumber = StreetAddressNumberDb,
                Zip = Zip
            };
        }
    }
}
