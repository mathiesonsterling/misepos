using System;

namespace Mise.Core.ValueItems
{
    public class RestaurantName : IEquatable<RestaurantName>, ITextSearchable
    {
        /// <summary>
        /// Full legal name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Max 8 chars name, used for nickname if wanted
        /// </summary>
        public string ShortName { get; set; }

        public RestaurantName() { }

        public RestaurantName(string fullName, string shortName)
        {
            FullName = fullName;
            ShortName = shortName;
        }

        public RestaurantName(string fullName)
        {
            FullName = fullName;

			var sname = fullName;
			if (sname.ToUpper ().StartsWith ("THE ")) {
				sname = sname.Replace ("The ", "").Replace ("THE ", "").Replace ("the ", "");
			}
			if (sname.Length < 9) {
				ShortName = sname;
			} else {
				sname = fullName.Substring (0, 8);
			}
        }

        public bool Equals(RestaurantName other)
        {
            return other != null
                   && FullName == other.FullName
                   && ShortName == other.ShortName;
        }

        public bool ContainsSearchString(string searchString)
        {
            return (FullName != null && FullName.Contains(searchString))
                   || (ShortName != null && ShortName.Contains(searchString));
        }

        public static RestaurantName TestName
        {
            get { return new RestaurantName("Test Restaurant", "Test");}
        }
    }
}
