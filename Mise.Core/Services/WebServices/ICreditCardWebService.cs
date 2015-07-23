using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace Mise.Core.Services.WebServices
{
    public interface ICreditCardWebService
    {
        /// <summary>
        /// Ask the server if the credit card has come back from the processor yet
        /// </summary>
        /// <param name="paymentID"></param>
        /// <returns></returns>
        Task<CreditCard> GetCardForPaymentID(string paymentID);

        /// <summary>
        /// Prime our provider to do a cc checkout, and get the payment ID back
        /// </summary>
        /// <typeparam name="TProcessorDataSet"></typeparam>
        /// <param name="destUrl"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        Task<string> PostPaymentIDToWebService<TProcessorDataSet>(Uri destUrl, TProcessorDataSet dataSet);
    }
}
