using Mise.Core.Client.Entities.People;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Entities.Accounts
{
    public class ApplicationInvitation : BaseDbRestaurantEntity<IApplicationInvitation, Core.Common.Entities.ApplicationInvitation>
    {
        public ApplicationInvitation()
        {
            
        }

        public ApplicationInvitation(Core.Common.Entities.ApplicationInvitation source, MiseApplication app, Employee destEmployee, 
            Employee invitingEmployee, Restaurant.Restaurant rest) 
            : base(source)
        {
            Application = app;
            Status = source.Status;
            DestinationEmployee = destEmployee;
            InvitingEmployee = invitingEmployee;
            Restaurant = rest;
            RestaurantId = rest.Id;
        }

        public MiseApplication Application
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
                Application = (MiseAppTypes)Application.AppTypeValue,
                Status = Status,
                DestinationEmail = new EmailAddress(DestinationEmployee?.PrimaryEmail),
                DestinationEmployeeID = DestinationEmployee?.EntityId,
                InvitingEmployeeID = InvitingEmployee.EntityId,
                InvitingEmployeeName = InvitingEmployee?.GetName(),
                RestaurantName = new BusinessName(Restaurant.FullName, Restaurant.ShortName),
                RestaurantID = Restaurant.RestaurantID
            };
        }
    }
}
