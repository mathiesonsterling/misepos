using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.People;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class ApplicationInvitation : BaseDbEntity<IApplicationInvitation, Core.Common.Entities.ApplicationInvitation>
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
                RestaurantName = Restaurant.Name,
                RestaurantID = Restaurant.RestaurantID
            };
        }
    }
}
