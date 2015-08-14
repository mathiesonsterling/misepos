using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems
{
    public class PersonName : IEquatable<PersonName>, ITextSearchable
    {
        public PersonName()
        {
            FirstName = string.Empty;
            MiddleName = string.Empty;
            LastName = string.Empty;
        }

		public PersonName(string completeName){
			if(string.IsNullOrWhiteSpace (completeName)){
				throw new ArgumentException ("Name is empty");
			}
			var seps = completeName.Split (' ');
			if(seps.Count () == 2){
				FirstName = seps [0];
				LastName = seps [1];
			} else{
				if(seps.Count () == 3){
					FirstName = seps [0];
					MiddleName = seps [1];
					LastName = seps [2];
				}
			}
		}
        public PersonName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public PersonName(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        public string ToDatabaseString()
        {
            return FirstName + "|" + MiddleName + "|" + LastName;
        }

        public string ToSingleString()
        {
            if (string.IsNullOrWhiteSpace(MiddleName))
            {
                return FirstName + " " + LastName;
            }

            return FirstName + " " + MiddleName + " " + LastName;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public bool Equals(PersonName other)
        {
            return other != null
                   && String.Equals(FirstName, other.FirstName, StringComparison.CurrentCultureIgnoreCase)
                   && String.Equals(MiddleName, other.MiddleName, StringComparison.CurrentCultureIgnoreCase)
                   && String.Equals(LastName, other.LastName, StringComparison.CurrentCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            var other = obj as PersonName;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + FirstName.ToUpper().GetHashCode();
                hash = hash * 23 + MiddleName.ToUpper().GetHashCode();
                hash = hash * 23 + LastName.ToUpper().GetHashCode();
                return hash;
            }
        }

        public bool ContainsSearchString(string searchString)
        {
            var sUpper = searchString.ToUpper();
            return (FirstName != null && FirstName.ToUpper().Contains(sUpper))
                || (MiddleName != null && MiddleName.ToUpper().Contains(sUpper))
                || (LastName != null && LastName.ToUpper().Contains(sUpper));
        }

        /// <summary>
        /// Gets a value that we can use for testing
        /// </summary>
        public static PersonName TestName
        {
            get
            {
                return new PersonName("Testy", "McTesterson");
            }
        }
    }
}
