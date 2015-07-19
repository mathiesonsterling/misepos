using System;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    [Route("/invitations/{Email}")]
    [Route("/restaurants/{RestaurantID}/invitations")]
    public class ApplicationInvitation :IReturn<ApplicationInvitationResponse>
    {
        public Guid? RestaurantID { get; set; }
        public string Email { get; set; }

    }
}
