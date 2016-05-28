using System;
using System.Collections.Generic;

namespace Mise.Core.Client.ValueItems
{
    public class StreetAddressNumberDb : Core.ValueItems.StreetAddressNumber, IDbValueItem<Core.ValueItems.StreetAddressNumber>
    {
	    public StreetAddressNumberDb(){}

	    public StreetAddressNumberDb(Core.ValueItems.StreetAddressNumber source)
	    {
		    ApartmentNumber = source.ApartmentNumber;
		    Direction = source.Direction;
		    Latitude = source.Latitude;
		    Longitude = source.Longitude;
		    Number = source.Number;
	    }

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
        
    public class Street : Core.ValueItems.Street, IDbValueItem<Core.ValueItems.Street>
    {
	    public Street(){}

	    public Street(Core.ValueItems.Street source)
	    {
		    Name = source.Name;
	    }
        public Core.ValueItems.Street ToValueItem()
        {
            return new Core.ValueItems.Street
            {
                Name = Name
            };
        }
    }
        
    public class City : Core.ValueItems.City
    {
	    public City(){}

	    public City(Core.ValueItems.City source)
	    {
		    Name = source.Name;
	    }
    }
        
	public class State : Core.ValueItems.State
	{
		public State(){}

		public State(Core.ValueItems.State source)
		{
			Abbreviation = source.Abbreviation;
			Name = source.Name;
		}
	}
        
	public class Country : Core.ValueItems.Country
	{
		public Country(){}

		public Country(Core.ValueItems.Country source)
		{
			Name = source.Name;
		}
	}
        
	public class ZipCode : Core.ValueItems.ZipCode
	{
		public ZipCode(){}

		public ZipCode(Core.ValueItems.ZipCode source)
		{
			Value = source.Value;
		}
	}
        
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

      	public StreetAddress(Core.ValueItems.StreetAddress source)
      	{
        	StreetAddressNumberDb = new StreetAddressNumberDb(source.StreetAddressNumber);
         	Street = new Street(source.Street);
         	City = new City(source.City);
         	Country = new Country(source.Country);
         	State = new State(source.State);
         	Zip = new ZipCode(source.Zip);
      	}

        public Core.ValueItems.StreetAddress ToValueItem()
        {
            return new Core.ValueItems.StreetAddress
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
