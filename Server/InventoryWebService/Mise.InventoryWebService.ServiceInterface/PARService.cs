using System;
using System.Linq;
using System.Net;
using System.Threading;
using Mise.Core.Repositories;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;
using PAR = Mise.InventoryWebService.ServiceModelPortable.Responses.PAR;

namespace Mise.InventoryWebService.ServiceInterface
{
    public class PARService : Service
    {
        private readonly IPARRepository _parRepository;
        public PARService(IPARRepository parRepository)
        {
            _parRepository = parRepository;
        }

        public PARResponse Get(PAR request)
        {
            if (_parRepository.Loading)
            {
                Thread.Sleep(100);
                if (_parRepository.Loading)
                {
                    throw new HttpError(HttpStatusCode.ServiceUnavailable, "Server has not yet loaded");
                }
            }

            if (request.PARID.HasValue)
            {
                return new PARResponse
                {
                    Results = new[] {_parRepository.GetByID(request.PARID.Value) as Core.Common.Entities.Inventory.PAR}
                };
            }

            var items = _parRepository.GetAll().Where(p => p.RestaurantID == request.RestaurantID).Cast<Core.Common.Entities.Inventory.PAR>();


            return new PARResponse
            {
                Results = items
            };
        }
    }
}
