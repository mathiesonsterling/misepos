using System;

using Mise.Core.Client.Services;
using Mise.Core.ValueItems;
using Mise.Core.Services;


namespace Mise.POSTerminal.Android.Services
{
	public class AudioCardReaderService : ICreditCardReaderService
	{
		ILogger _logger;

		public AudioCardReaderService(ILogger logger)
		{
			_logger = logger;
		}

		#region ICreditCardReaderService implementation

		public bool Enabled {
			get {
				//TODO can we test if the device is here?
				return true;
			}
		}

		public CreditCardReaderType CreditCardReaderType {
			get {
				return CreditCardReaderType.AudioReader;
			}
		}

		public event SwipeOccurred CreditCardSwiped;

		public void StartRead()
		{
			Reading = true;
		}

		public void StopRead()
		{
			Reading = false;
		}

		public void AddChars(char c)
		{
			throw new NotImplementedException();
		}

		public void AddCard(CreditCard card)
		{
			if (CreditCardSwiped != null) {
				CreditCardSwiped(card);
			}
		}

		public bool Reading {
			get;
			private set;
		}

		#endregion


	}
}

