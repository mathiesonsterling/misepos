using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Events.Payments;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events.Restaurant;
 
namespace Mise.Core.Common.Entities
{
	public class Employee : EntityBase, IEmployee
    {
		public Employee(){
			WhenICanVoid = new List<OrderItemStatus> {OrderItemStatus.Added};
            Password = new Password();
			RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>> ();
		}

        public ICloneableEntity Clone()
        {
            var newItem = CloneEntityBase(new Employee
            {
                Name = Name,
                CurrentlyClockedInToPOS = CurrentlyClockedInToPOS,
                CurrentlyLoggedIntoInventoryApp = CurrentlyLoggedIntoInventoryApp,
                LastTimeLoggedIntoInventoryApp = LastTimeLoggedIntoInventoryApp,
                Passcode = Passcode,
                Password = Password,
                PrimaryEmail = PrimaryEmail,
                CanCompAmount = CanCompAmount,
                PreferredColorName = PreferredColorName,
                EmployeeIconUri = EmployeeIconUri,
				OAuthToken = OAuthToken
            });
						
			newItem.RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>();
			foreach(var rest in RestaurantsAndAppsAllowed){
				newItem.RestaurantsAndAppsAllowed [rest.Key] = new List<MiseAppTypes> (rest.Value);
			}

            var voidableStates = WhenICanVoid != null ? WhenICanVoid.ToList() : null;
            newItem.WhenICanVoid = voidableStates;

            var emails = Emails != null ? Emails.ToList():null;
            newItem.Emails = emails;

            newItem.CompBudget = CompBudget != null? new Money(CompBudget.Dollars) : null;

            return newItem;
        }

        /// <summary>
        /// Employees may or may not have a restaurant at all times
        /// </summary>
        public IDictionary<Guid, IList<MiseAppTypes>> RestaurantsAndAppsAllowed { get; set; }

	    public IEnumerable<MiseAppTypes> GetAppsEmployeeCanUse(Guid restaurantID)
	    {
	        return RestaurantsAndAppsAllowed.ContainsKey(restaurantID)
	            ? RestaurantsAndAppsAllowed[restaurantID]
	            : new MiseAppTypes[0];
	    }

	    public PersonName Name{get;set;} 

        private string _displayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    if (Name != null)
                    {
                        return Name.FirstName + " " + Name.LastName;
                    }
                    return string.Empty;
                }
                return _displayName;
            }
            set { _displayName = value; }
        }

	    public DateTimeOffset HiredDate { get; set; }

	    public bool CurrentlyClockedInToPOS{ get; set; }

		public string Passcode { get; set;}

		/// <summary>
		/// We only store the hash, since this field goes across the wire
		/// </summary>
		/// <value>The password hash.</value>
		public Password Password {
			get;
			set;
		}

		public OAuthToken OAuthToken {
			get;
			set;
		}

		public IEnumerable<OrderItemStatus> WhenICanVoid{ get; set; }

		public bool CanVoid (OrderItemStatus status)
		{
			return WhenICanVoid.Contains (status);
		}

		public EmailAddress PrimaryEmail {
			get;
			set;
		}

		public IList<EmailAddress> Emails {
			get;
			set;
		}


	    public bool CanCompAmount { get; set; }

	    public Money CompBudget {
			get;
			set;
		}

		public string PreferredColorName {
			get;
			set;
		}
		public Uri EmployeeIconUri {
			get;
			set;
		}

		public bool CanUseAppForRestaurant (Guid restaurantID, MiseAppTypes app)
		{
			if(RestaurantsAndAppsAllowed.ContainsKey (restaurantID)){
				return RestaurantsAndAppsAllowed [restaurantID].Contains (app);
			}
			return false;
		}

	    public IEnumerable<Guid> GetRestaurantIDs()
	    {
	        return RestaurantsAndAppsAllowed.Keys;
	    } 

		public IEnumerable<Guid> GetRestaurantIDs(MiseAppTypes type)
		{
			return (from kv in RestaurantsAndAppsAllowed 
                   where kv.Value.Contains(type) 
                   select kv.Key).ToList();
		}
			

		public IEnumerable<EmailAddress> GetEmailAddresses()
		{
		    var res = Emails == null ? new List<EmailAddress>() : new List<EmailAddress>(Emails);
		    if (PrimaryEmail != null)
		    {
		        if (res.Select(e => e.Value).Contains(PrimaryEmail.Value) == false)
		        {
		            res.Add(PrimaryEmail);
		        } 
		    }

		    return res.Where(e => e != null);
		}

		public DateTimeOffset? LastTimeLoggedIntoInventoryApp { get; set; }
	    public string LastDeviceIDLoggedInWith { get; set; }

	    public bool CurrentlyLoggedIntoInventoryApp { get; set; }

	    public void When(IEmployeeEvent empEvent)
	    {
	        switch (empEvent.EventType)
	        {
				case MiseEventTypes.EmployeeCreatedEvent:
					WhenEmployeeCreated ((EmployeeCreatedEvent)empEvent);
					break;
	            case MiseEventTypes.EmployeeClockedIn:
                    WhenClockedIn((EmployeeClockedInEvent)empEvent);
	                break;
                case MiseEventTypes.EmployeeClockedOut:
                    WhenClockedOut((EmployeeClockedOutEvent)empEvent);
	                break;
                case MiseEventTypes.CompPaidDirectlyOnCheck:
	                WhenCompPaidOnCheck((CompPaidDirectlyOnCheckEvent) empEvent);
	                break;
                case MiseEventTypes.NoSale:
                    WhenNoSale((NoSaleEvent)empEvent);
                    break;
                case MiseEventTypes.InsufficientPermissions:
                    WhenInsufficientPermissions((InsufficientPermissionsEvent)empEvent);
                    break;
			case MiseEventTypes.ItemCompedGeneral:
				WhenItemComped ((ItemCompedGeneralEvent)empEvent);
				break;
			case MiseEventTypes.ItemUncomped:
				WhenItemUncomped ((ItemUncompedEvent)empEvent);
				break;
                case MiseEventTypes.EmployeeLoggedIntoInventoryAppEvent:
	                WhenEmployeeLoggedIntoInventoryApp((EmployeeLoggedIntoInventoryAppEvent) empEvent);
	                break;
                case MiseEventTypes.EmployeeLoggedOutOfInventoryApp:
	                WhenEmployeeLoggedOutOfInventoryApp((EmployeeLoggedOutOfInventoryAppEvent) empEvent);
	                break;
                case MiseEventTypes.EmployeeRegisteredForInventoryAppEvent:
	                WhenEmployeeRegisteredForInventoryAppEvent((EmployeeRegisteredForInventoryAppEvent) empEvent);
	                break;
				case MiseEventTypes.EmployeeAcceptsInvitation:
					WhenEmployeeAcceptsInvitation ((EmployeeAcceptsInvitationEvent)empEvent);
					break;
                case MiseEventTypes.EmployeeRejectsInvitation:
	                WhenEmployeeRejectsInvitiation((EmployeeRejectsInvitationEvent) empEvent);
	                break;
				case MiseEventTypes.EmployeeRegistersRestaurant:
					WhenEmployeeRegistersRestaurant ((EmployeeRegistersRestaurantEvent)empEvent);
					break;
                default:
                    throw new ArgumentException("Employee object cannot process event of type " + empEvent.EventType);
	        }
			Revision = empEvent.EventOrder;
			LastUpdatedDate = empEvent.CreatedDate;
	    }

	    private void WhenEmployeeRejectsInvitiation(EmployeeRejectsInvitationEvent empEvent)
	    {
	        //nothing to do for now
	    }

	    void WhenEmployeeCreated (EmployeeCreatedEvent ecEV)
		{
			Id = ecEV.EmployeeID;
			CreatedDate = ecEV.CreatedDate;
	        Name = ecEV.Name;
			if (Emails == null) {
				Emails = new List<EmailAddress> ();
			}
			Emails.Add (ecEV.Email);
			PrimaryEmail = ecEV.Email;
			Revision = ecEV.EventOrder;
			Password = ecEV.Password;
			OAuthToken = ecEV.OAuthToken;
	        //RestaurantsAndAppsAllowed[ecEV.RestaurantID] = new[] {ecEV.AppType};
		}			

        protected virtual void WhenEmployeeRegisteredForInventoryAppEvent(EmployeeRegisteredForInventoryAppEvent empEvent)
	    {
            if (RestaurantsAndAppsAllowed.ContainsKey(empEvent.RestaurantId) == false)
            {
				RestaurantsAndAppsAllowed.Add (empEvent.RestaurantId, new List<MiseAppTypes> ());
            }
			RestaurantsAndAppsAllowed [empEvent.RestaurantId].Add (MiseAppTypes.StockboyMobile);
	    }

		void WhenEmployeeAcceptsInvitation (EmployeeAcceptsInvitationEvent ev)
		{
			if(RestaurantsAndAppsAllowed.ContainsKey (ev.RestaurantId) == false){
				RestaurantsAndAppsAllowed.Add (ev.RestaurantId, new List<MiseAppTypes> ());
			}
			RestaurantsAndAppsAllowed [ev.RestaurantId].Add (ev.Application);
		}

		void WhenEmployeeRegistersRestaurant (EmployeeRegistersRestaurantEvent ev)
		{
			if(RestaurantsAndAppsAllowed.ContainsKey (ev.RestaurantId) == false){
				RestaurantsAndAppsAllowed.Add (ev.RestaurantId, new List<MiseAppTypes> ());
			}
			RestaurantsAndAppsAllowed [ev.RestaurantId].Add (ev.ApplicationUsed);
		}

        protected virtual void WhenEmployeeLoggedOutOfInventoryApp(EmployeeLoggedOutOfInventoryAppEvent empEvent)
        {
            CurrentlyLoggedIntoInventoryApp = false;
        }

        protected virtual void WhenEmployeeLoggedIntoInventoryApp(EmployeeLoggedIntoInventoryAppEvent empEvent)
        {
            CurrentlyLoggedIntoInventoryApp = true;
            LastDeviceIDLoggedInWith = empEvent.DeviceId;
            LastTimeLoggedIntoInventoryApp = empEvent.CreatedDate;
        }

	    protected virtual void WhenItemComped(ItemCompedGeneralEvent comp){
			if (CompBudget != null) {
				CompBudget = CompBudget.Subtract (comp.Amount);
			}
		}

		protected virtual void WhenItemUncomped(ItemUncompedEvent unc){
			if (CompBudget != null) {
				CompBudget = CompBudget.Add (unc.Amount);
			}
		}

        protected virtual void WhenNoSale(NoSaleEvent noSale)
        {
            //got nada to do
        }

        protected virtual void WhenInsufficientPermissions(InsufficientPermissionsEvent permEvent)
        {

        }

	    protected virtual void WhenClockedIn(EmployeeClockedInEvent clockIn)
	    {
	        CurrentlyClockedInToPOS = true;
	    }

	    protected virtual void WhenClockedOut(EmployeeClockedOutEvent clockOut)
	    {
	        CurrentlyClockedInToPOS = false;
	    }

	    protected virtual void WhenCompPaidOnCheck(CompPaidDirectlyOnCheckEvent comp)
	    {
			CompBudget = CompBudget.Subtract (comp.Amount);
	    }
    }
}
