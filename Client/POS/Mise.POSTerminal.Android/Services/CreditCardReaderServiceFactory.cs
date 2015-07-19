using System;

using Mise.Core.Client;
using Mise.Core.Client.Services;
using Mise.Core.Client.Services.Implementation;
using Mise.Core.ValueItems;
using Mise.Core.Services;

namespace Mise.POSTerminal.Android.Services
{
	public static class CreditCardReaderServiceFactory
	{
		public static ICreditCardReaderService GetCreditCardReaderService(CreditCardReaderType type, ILogger logger)
		{
			//todo for multiple flags do a multiple one?
			switch (type) {
			case CreditCardReaderType.None:
				return new NoCreditCardReaderService(logger);
			case CreditCardReaderType.External:
				//TODO create the external reader
				throw new NotImplementedException();
			case CreditCardReaderType.USBKeyboardSwipe:
				return new USBCreditCardReaderService(logger);
			case CreditCardReaderType.AudioReader:
				return new AudioCardReaderService(logger);
			case CreditCardReaderType.CameraReader:
				return new CreditIOReaderService(logger);
			default:
				throw new ArgumentException("Don't have a credit card reader service for type " + type);
			}
		}
	}
}

