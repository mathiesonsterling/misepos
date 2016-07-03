using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mise.Core.ValueItems
{
	public class StreetAddress : IEquatable<StreetAddress>, ITextSearchable
	{
		public StreetAddressNumber StreetAddressNumber{ get; set;}
		public Street Street{get;set;}
		public City City{ get; set;}
		public State State{ get; set;}
		public Country Country{ get; set;}
		public ZipCode Zip{ get; set;}

		public StreetAddress(){}

		public StreetAddress(string number, string direction, string street, string city, string state, 
			string country, string zip){
			StreetAddressNumber = new StreetAddressNumber{ Number = number, Direction = direction };
			Street = new Street{ Name = street };
			City = new City{ Name = city };
			State = new State{ Name = state };
			Country = new Country{ Name = country };
			Zip = new ZipCode{ Value = zip };
		}

	    public StreetAddress(string number, string direction, string street, string city, string state,
	        string country, string zip, double lat, double longitude)
	        : this(number, direction, street, city, state, country, zip)
	    {
	        StreetAddressNumber.Latitude = lat;
	        StreetAddressNumber.Longitude = longitude;
	    }

        public bool Equals(StreetAddress other)
	    {
	        return
	            StreetAddressNumber.Equals(other.StreetAddressNumber)
	            && Street.Equals(other.Street)
	            && City.Equals(other.City)
	            && State.Equals(other.State)
	            && Country.Equals(other.Country)
	            && Zip.Equals(other.Zip);
	    }

	    public bool ContainsSearchString(string searchString)
	    {
	        return StreetAddressNumber.ContainsSearchString(searchString)
	               || Street.ContainsSearchString(searchString)
	               || City.ContainsSearchString(searchString)
	               || State.ContainsSearchString(searchString)
	               || Zip.ContainsSearchString(searchString);
	    }

	    public string ToSingleString()
	    {
	        var sb = new StringBuilder(StreetAddressNumber.Number);
	        if (!string.IsNullOrWhiteSpace(StreetAddressNumber.Direction))
	        {
	            sb.Append(" " + StreetAddressNumber.Direction);
	        }

	        sb.Append(" " + Street.Name);
	        if (!string.IsNullOrWhiteSpace(StreetAddressNumber.ApartmentNumber))
	        {
	            sb.Append(" " + StreetAddressNumber.ApartmentNumber);
	        }

	        sb.Append(" " + City.Name);
	        sb.Append(" " + State.Name);

	        return sb.ToString();
	    }

	    public static StreetAddress TestStreetAddress => new StreetAddress("699", "", "Ocean Ave", "Brooklyn", "New York", Country.UnitedStates.Name, "11226");
	}

    public class Street : IEquatable<Street>, ITextSearchable
    {
        public string Name { get; set; }
        public bool Equals(Street other)
        {
            return Name.Equals(other.Name);
        }

        public bool ContainsSearchString(string searchString)
        {
            return Name.Contains(searchString);
        }
    }

    /// <summary>
    /// Address is small enough to where we can consider it a location
    /// </summary>
    public class StreetAddressNumber : Location, IEquatable<StreetAddressNumber>, ITextSearchable
    {
        public string Number { get; set; }
        public string Direction { get; set; }

        public string ApartmentNumber { get; set; }
        public bool Equals(StreetAddressNumber other)
        {
            return Number.Equals(other.Number)
                   && Direction.Equals(other.Direction)
                   && Latitude.Equals(other.Latitude)
                   && Longitude.Equals(other.Longitude);
        }

        public bool ContainsSearchString(string searchString)
        {
            return Number.Contains(searchString);
        }
    }

    public class City : IEquatable<City>, ITextSearchable
    {
        public City()
        {
            
        }

        public City(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool Equals(City other)
        {
            return Name.Equals(other.Name);
        }

        public bool ContainsSearchString(string searchString)
        {
            return Name.Contains(searchString);
        }
    }

    public class State : IEquatable<State>, ITextSearchable
    {
        public string Name { get; set; }

        /// <summary>
        /// Two letter code for the state
        /// </summary>
        public string Abbreviation { get; set; }

        public bool Equals(State other)
        {
			if (other == null) {
				return false;
			}

			var realThis = this;
			if (Abbreviation == null) {
				realThis = GetUSStates().FirstOrDefault(s => s.Name == Name || s.Abbreviation == Name) ?? this;
			}

			var realOther = other;
			if(other.Abbreviation == null)
			{
				realOther = GetUSStates ().FirstOrDefault (s => s.Name == other.Name || s.Abbreviation == other.Name) ?? other;
			}
			return realThis.Name == realOther.Name || realThis.Abbreviation == realOther.Abbreviation;
        }

        public bool ContainsSearchString(string searchString)
        {
            return Name.Contains(searchString) || (Abbreviation != null && Abbreviation.Contains(searchString));
        }

        public static IEnumerable<State> GetUSStates()
        {
            var states = new List<State>
            {
                new State {Name = "Alabama", Abbreviation = "AL"},
                new State {Name = "Alaska", Abbreviation = "AK"},
                new State {Name = "Arizona", Abbreviation = "AZ"},
                new State {Name = "Arkansas", Abbreviation = "AR"},
                new State {Name = "California", Abbreviation = "CA"},
                new State {Name = "Colorado", Abbreviation = "CO"},
                new State {Name = "Connecticut", Abbreviation = "CT"},
                new State {Name = "Delaware", Abbreviation = "DE"},
                new State {Name = "Florida", Abbreviation = "FL"},
                new State {Name = "Georgia", Abbreviation = "GA"},
                new State {Name = "Hawaii", Abbreviation = "HI"},
                new State {Name = "Idaho", Abbreviation = "ID"},
                new State {Name = "Illinois", Abbreviation = "IL"},
                new State {Name = "Indiana", Abbreviation = "IN"},
                new State {Name = "Iowa", Abbreviation = "IA"},
                new State {Name = "Kansas", Abbreviation = "KS"},
                new State {Name = "Kentucky", Abbreviation = "KY"},
                new State {Name = "Louisiana", Abbreviation = "LA"},
                new State {Name = "Maine", Abbreviation = "ME"},
                new State {Name = "Maryland", Abbreviation = "MD"},
                new State {Name = "Massachusetts", Abbreviation = "MA"},
                new State {Name = "Michigan", Abbreviation = "MI"},
                new State {Name = "Minnesota", Abbreviation = "MN"},
                new State {Name = "Mississippi", Abbreviation = "MS"},
                new State {Name = "Missouri", Abbreviation = "MO"},
                new State {Name = "Montana", Abbreviation = "MT"},
                new State {Name = "Nebraska", Abbreviation = "NE"},
                new State {Name = "Nevada", Abbreviation = "NV"},
                new State {Name = "New Hampshire", Abbreviation = "NH"},
                new State {Name = "New Jersey", Abbreviation = "NJ"},
                new State {Name = "New Mexico", Abbreviation = "NM"},
                new State {Name = "New York", Abbreviation = "NY"},
                new State {Name = "North Carolina", Abbreviation = "NC"},
                new State {Name = "North Dakota", Abbreviation = "ND"},
                new State {Name = "Ohio", Abbreviation = "OH"},
                new State {Name = "Oklahoma", Abbreviation = "OK"},
                new State {Name = "Oregon", Abbreviation = "OR"},
                new State {Name = "Pennsylvania", Abbreviation = "PA"},
                new State {Name = "Rhode Island", Abbreviation = "RI"},
                new State {Name = "South Carolina", Abbreviation = "SC"},
                new State {Name = "South Dakota", Abbreviation = "SD"},
                new State {Name = "Tennessee", Abbreviation = "TN"},
                new State {Name = "Texas", Abbreviation = "TX"},
                new State {Name = "Utah", Abbreviation = "UT"},
                new State {Name = "Vermont", Abbreviation = "VT"},
                new State {Name = "Virginia", Abbreviation = "VA"},
                new State {Name = "Washington", Abbreviation = "WA"},
                new State {Name = "West Virginia", Abbreviation = "WV"},
                new State {Name = "Wisconsin", Abbreviation = "WI"},
                new State {Name = "Wyoming", Abbreviation = "WY"}
            };

            return states;
        }
    }

    public class ZipCode : IEquatable<ZipCode>, ITextSearchable
    {
        public ZipCode() { }
        public ZipCode(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
        public bool Equals(ZipCode other)
        {
            if (other == null)
            {
                return false;
            }

            //handle when one is 5 digits and the other is as well
            if (Value.Length >= 5 && other.Value.Length >= 5)
            {
                return Value.Substring(0, 5).Equals(other.Value.Substring(0, 5));
            }
            return false;
        }

        public bool ContainsSearchString(string searchString)
        {
            return Value.Contains(searchString);
        }

		public static bool IsValid(string candidate){
			return string.IsNullOrEmpty (candidate) && candidate.Length > 4 && candidate.Length < 10;
		}
    }

    public class Country : IEquatable<Country>
    {
        public Country() { }

        public Country(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool Equals(Country other)
        {
            return Name.Equals(other.Name);
        }

        public static Country UnitedStates => new Country("United States of America");
    }
}

