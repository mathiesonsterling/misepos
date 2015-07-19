using Mise.Core.ValueItems;

namespace Mise.Core.Client.Services
{
    public delegate void SwipeOccurred(CreditCard card);

    /// <summary>
    /// Handles getting credit card info from hardware
    /// </summary>
    public interface ICreditCardReaderService
    {
		/// <summary>
		/// Whether we can trigger the swiper to read
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		bool Enabled{get;}

		/// <summary>
		/// The type of readers this service supports
		/// </summary>
		/// <value>The type of the credit card reader.</value>
		CreditCardReaderType CreditCardReaderType{get;}

		/// <summary>
		/// Fired when a credit card is swiped.  Translates into an ICreditCard.  Also signals we're done reading
		/// </summary>
		event SwipeOccurred CreditCardSwiped;

		/// <summary>
		/// Signals our service that we want to start reading credit card info
		/// </summary>
		void StartRead();

		/// <summary>
		/// Stops the read manually, stopping us from reading
		/// </summary>
		void StopRead();


		/// <summary>
		/// Adds characters from a stream to this service as part of the credit card
		/// </summary>
		/// <param name="c">C.</param>
		void AddChars(char c);

		/// <summary>
		/// If we're provided a card by the OS somehow, take it and then fire events
		/// </summary>
		/// <param name="card">Card.</param>
		void AddCard (CreditCard card);
	}
}
