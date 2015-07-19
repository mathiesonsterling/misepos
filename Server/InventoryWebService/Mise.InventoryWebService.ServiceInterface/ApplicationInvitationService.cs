using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Mise.Core.Repositories;
using Mise.Core.ValueItems;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;
using ApplicationInvitation = Mise.InventoryWebService.ServiceModelPortable.Responses.ApplicationInvitation;

namespace Mise.InventoryWebService.ServiceInterface
{
    public class ApplicationInvitationService : Service
    {
        private readonly IApplicationInvitationRepository _applicationInvitationRepository;

        public ApplicationInvitationService(IApplicationInvitationRepository applicationInvitationRepository)
        {
            _applicationInvitationRepository = applicationInvitationRepository;
        }

        public async Task<ApplicationInvitationResponse> Get(ApplicationInvitation request)
        {
            if (_applicationInvitationRepository.Loading)
            {
                Thread.Sleep(100);
                if (_applicationInvitationRepository.Loading)
                {
                    throw new HttpError(HttpStatusCode.ServiceUnavailable, "Server has not yet loaded");
                }
            }

            if (string.IsNullOrEmpty(request.Email) == false)
            {
                var email = new EmailAddress(request.Email);
                var items = await _applicationInvitationRepository.GetOpenInvitesForEmail(email);
                return new ApplicationInvitationResponse
                {
                    Results = items.Cast<Core.Common.Entities.ApplicationInvitation>()
                };
            }

            if (request.RestaurantID.HasValue)
            {
                var items = await _applicationInvitationRepository.GetInvitesForRestaurant(request.RestaurantID.Value);
                return new ApplicationInvitationResponse
                {
                    Results = items.Cast<Core.Common.Entities.ApplicationInvitation>()
                };
            }

            throw new ArgumentException("You must provide either the restaurant ID or destination email");
        }
    }
}
