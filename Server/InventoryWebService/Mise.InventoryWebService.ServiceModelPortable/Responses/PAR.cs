using System;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    [Route("/restaurants/{RestaurantID}/pars/")]
    [Route("/restaurants/{RestaurantID}/pars/{PARID}")]
    public class PAR : IReturn<PARResponse>
    {
        public Guid RestaurantID { get; set; }
        public Guid? PARID { get; set; }

    }
}
