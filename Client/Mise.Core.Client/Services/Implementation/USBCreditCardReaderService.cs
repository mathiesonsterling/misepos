using System;
using System.Collections.Generic;
using System.Linq;

using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Services.Implementation
{
	public class USBCreditCardReaderService : ICreditCardReaderService
	{
		private ILogger _logger;
		private DateTime lastAdded = DateTime.MinValue;

		public TimeSpan MaxInputLength{ get; private set;}
		public USBCreditCardReaderService(ILogger logger){
			_logger = logger;
			MaxInputLength = new TimeSpan (0, 0, 1);
		}
		#region ICreditCardHardwareService implementation

		public bool Enabled {
			get {
				return true;
			}
		}

		public CreditCardReaderType CreditCardReaderType {
			get {
				return CreditCardReaderType.USBKeyboardSwipe;
			}
		}

		public void AddCard (CreditCard card)
		{
			throw new NotImplementedException ();
		}

		public bool Reading {
			get {
				return true;
			}
		}

		public event SwipeOccurred CreditCardSwiped;

		public void StartRead ()
		{
			//do nothing for this service
		}

		public void StopRead ()
		{
			//do nothing here as well
		}

		#endregion
		public static CreditCard ExtractCreditCardFromString(string res){
			var ccStrings = res.Split (new []{ '^' }, 3);

			if (ccStrings.Length > 1) {
				//first is account number, should start with %B
				if(ccStrings [0].StartsWith ("%B", StringComparison.Ordinal) == false){
					//log it!
				}
				string acctNum = ccStrings [0].Substring (2);

				//second is name
				var nameStrings = ccStrings [1].Split (new []{'/'}, 2);
				var lastName = nameStrings [0];
				if(lastName.Contains (" ")){
					lastName = lastName.Substring (0, lastName.IndexOf (" ", StringComparison.Ordinal));
				}
				string fName = string.Empty;
				if(nameStrings.Length > 1){
					fName = nameStrings [1];
				}

				//third holds exp date
				//TODO fix this in 2099
				var expYearS = "20" + ccStrings [2].Substring (0, 2);
				var expMonthS = ccStrings [2].Substring (2, 2);

				var card = new CreditCard {
                    Name = new PersonName
                    {
                        FirstName = fName,
                        LastName = lastName
                    },
					ExpMonth = int.Parse (expMonthS),
					ExpYear = int.Parse (expYearS)
				};
				return card;
			}

			return null;
		}

		readonly List<char> addedChars = new List<char> ();
		int numCaretsFound = 0;
		public void AddChars(char c){
			//TODO change this to hit on the second question mark
			if(c == '?'){
				//we've hit the end if we have all our carets here
				if (numCaretsFound > 1) {
					if (CreditCardSwiped != null) {
						//break into the card here
						var valids = addedChars.Where (ch => ch != '\0');
						var res = new string (valids.ToArray ());

						var card = ExtractCreditCardFromString (res);
						if (card != null) {
							CreditCardSwiped (card);
						}
					}
					addedChars.Clear ();
					numCaretsFound = 0;
				} 
			} else {
				//check if we've restarted!
				if (addedChars.Any ()) {
					//is our last time over our threshold?
					if ((DateTime.UtcNow - lastAdded) > MaxInputLength) {
						addedChars.Clear ();
						numCaretsFound = 0;
					} else {
						if (c == 'B' && addedChars.Last () == '%') {
							numCaretsFound = 0;
							addedChars.Clear ();
							addedChars.Add ('%');
						}
					}
				}

				if(c == '^'){
					numCaretsFound++;
				}
				addedChars.Add (c);
				lastAdded = DateTime.UtcNow;
			}
		}
	}
}

