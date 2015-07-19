using System.Threading.Tasks;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Payments;

namespace Mise.Core.Services
{
	public interface IPrinterService
	{
		/// <summary>
		/// Given a check, print out the reciept for it
		/// </summary>
		/// <returns><c>true</c>, if tab reciept was printed, <c>false</c> otherwise.</returns>
		/// <param name="check">Check.</param>
		Task<bool> PrintRecieptAsync(ICheck check);

        /// <summary>
        /// Prints the dupes required to send a ticket to the kitchen
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        Task<bool> PrintDupeAsync(ICheck check);

		/// <summary>
		/// Print a credit card payment slip
		/// </summary>
		/// <returns><c>true</c>, if credit card slip was printed, <c>false</c> otherwise.</returns>
		/// <param name="ccPayment">Cc payment.</param>
		Task<bool> PrintCreditCardSlipAsync (ICreditCardPayment ccPayment);


	}
}

