using System;
using System.Threading.Tasks;

using Mise.Core.ValueItems;
namespace Mise.Core.Common.Services
{
    public interface IClientStripeFacade
    {
        Task<CreditCardProcessorToken> SendForToken(PersonName cardName, CreditCardNumber number);
    }
}

