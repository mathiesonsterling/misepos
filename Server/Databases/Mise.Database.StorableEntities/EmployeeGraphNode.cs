using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public sealed class EmployeeGraphNode : IStorableEntityGraphNode
    {
        public EmployeeGraphNode(IEmployee source) 
        {

            DisplayName = source.DisplayName;
            CurrentlyClockedInToPOS = source.CurrentlyClockedInToPOS;
            Passcode = source.Passcode;
            CompBudget = source.CompBudget != null?source.CompBudget.Dollars:0.0M;
            CanCompAmount = source.CanCompAmount;
            PreferredColorName = source.PreferredColorName;
            EmployeeIconUri = source.EmployeeIconUri;
            FirstName = source.Name.FirstName;
            MiddleName = source.Name.MiddleName;
            LastName = source.Name.LastName;
            LastTimeLoggedIntoInventoryApp = source.LastTimeLoggedIntoInventoryApp;
            CurrentlyLoggedIntoInventoryApp = source.CurrentlyLoggedIntoInventoryApp;
            PasswordHash = source.Password != null ? source.Password.HashValue : null;
            PrimaryEmail = source.PrimaryEmail != null ? source.PrimaryEmail.Value : string.Empty;
            LastDeviceIDLoggedInWith = source.LastDeviceIDLoggedInWith;

            ID = source.ID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();
        }


        public EmployeeGraphNode()
        {
            
        }

        public IEmployee Rehydrate(IEnumerable<EmailAddress> emailsAddresses, IDictionary<Guid, IList<MiseAppTypes>> restaurantIDsAndApps)
        {
            var emp =  new Employee
            {
                DisplayName = DisplayName,
                CurrentlyClockedInToPOS = CurrentlyClockedInToPOS,
                Passcode = Passcode,
                CompBudget = new Money(CompBudget),
                CanCompAmount = CanCompAmount,
                PreferredColorName = PreferredColorName,
                EmployeeIconUri = EmployeeIconUri,
                Name = new PersonName(FirstName, MiddleName, LastName),
                RestaurantsAndAppsAllowed = restaurantIDsAndApps,
                ID = ID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),
                LastTimeLoggedIntoInventoryApp = LastTimeLoggedIntoInventoryApp,
                CurrentlyLoggedIntoInventoryApp = CurrentlyLoggedIntoInventoryApp,
                Password = new Password { HashValue = PasswordHash},
                PrimaryEmail = new EmailAddress { Value = PrimaryEmail},
                LastDeviceIDLoggedInWith = LastDeviceIDLoggedInWith,
                Emails = emailsAddresses.ToList()
            };
            return emp;
        }

        public string PrimaryEmail { get; set; }

        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }

        public Guid? RestaurantID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        /// <summary>
        /// Allows us to override the employee's name for display on the POS
        /// </summary>
        public string DisplayName { get; set; }


        public bool CurrentlyClockedInToPOS { get; set; }

        /// <summary>
        /// Passcode this employee uses to clockin and out of the system
        /// </summary>
        /// <value>The passcode.</value>
        public string Passcode { get; set; }

        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets the remaining amount today that the employee can comp today
        /// </summary>
        /// <value>The comp budget.</value>
        public decimal CompBudget { get; set; }

        /// <summary>
        /// If true, the employee can comp amounts directly on the check, rather than through items
        /// </summary>
        public bool CanCompAmount { get; set; }

        public string PreferredColorName { get; set; }

        /// <summary>
        /// Where we can get the icon for the employee from
        /// </summary>
        /// <value>The employee icon URI.</value>
        public Uri EmployeeIconUri { get; set; }


        public bool CurrentlyLoggedIntoInventoryApp { get; set; }

        public DateTimeOffset? LastTimeLoggedIntoInventoryApp { get; set; }
        public string LastDeviceIDLoggedInWith { get; set; }
    }
}
