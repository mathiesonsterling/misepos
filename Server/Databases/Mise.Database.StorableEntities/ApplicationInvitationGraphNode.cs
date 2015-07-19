using System;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public class ApplicationInvitationGraphNode : IStorableEntityGraphNode
    {
        public ApplicationInvitationGraphNode() { }

        public ApplicationInvitationGraphNode(IApplicationInvitation source)
        {
            ID = source.ID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            Application = source.Application.ToString();
            Status = source.Status.ToString();
        }

        public IApplicationInvitation Rehydrate(Guid restID, RestaurantName restName, IEmployee invitingEmployee, 
            IEmployee destEmployee, EmailAddress destEmail)
        {
            return new ApplicationInvitation
            {
                ID = ID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),

                Application = (MiseAppTypes) Enum.Parse(typeof (MiseAppTypes), Application),
                Status = (InvitationStatus) Enum.Parse(typeof (InvitationStatus), Status),
                DestinationEmail = destEmail,
                DestinationEmployeeID = destEmployee != null ? (Guid?)destEmployee.ID : null,
                InvitingEmployeeID = invitingEmployee.ID,
                InvitingEmployeeName = invitingEmployee.Name,
                RestaurantID = restID,
                RestaurantName = restName
            };
        }
        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }

        public string Application
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
    }
}
