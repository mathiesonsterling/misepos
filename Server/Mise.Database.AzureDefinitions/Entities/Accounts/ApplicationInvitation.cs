using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Database.AzureDefinitions.Entities.People;
using Mise.Database.AzureDefinitions.ValueItems;
namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class ApplicationInvitation : BaseDbEntity<IApplicationInvitation, Core.Common.Entities.ApplicationInvitation>
    {
        public MiseAppTypes Application
        {
            get;
            set;
        }
        public InvitationStatus Status
        {
            get;
            set;
        }

        public Employee DestinationEmployee
        {
            get;
            set;
        }
        public Employee InvitingEmployee
        {
            get;
            set;
        }

        public Restaurant.Restaurant Restaurant
        {
            get;
            set;
        }

        protected override Core.Common.Entities.ApplicationInvitation CreateConcreteSubclass()
        {
            return new Core.Common.Entities.ApplicationInvitation
            {
                Application = Application,
                Status = Status,
                DestinationEmail = DestinationEmployee?.PrimaryEmail.ToValueItem(),
                DestinationEmployeeID = DestinationEmployee?.EntityId,
                InvitingEmployeeID = InvitingEmployee.EntityId,
                InvitingEmployeeName = InvitingEmployee?.GetName(),
                RestaurantName = Restaurant.Name,
                RestaurantID = Restaurant.RestaurantID
            };
        }
    }
}
