using System;
using Mise.Core.Client.Services;
using Mise.Core.ValueItems;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Client
{
	/// <summary>
	/// Null object implementation of ICreditCardReaderService
	/// </summary>
	public class NoCreditCardReaderService : ICreditCardReaderService
	{
		ILogger _logger;
		public NoCreditCardReaderService(ILogger logger){
			_logger = logger;
		}
		#region ICreditCardReaderService implementation

		public event SwipeOccurred CreditCardSwiped;

		public void StartRead ()
		{
			const string msg = "Attempt to StartRead when no credit card reader present";
			_logger.Log (msg);
			throw new NotImplementedException (msg);
		}

		public void StopRead ()
		{
			const string msg = "Attempt to StopRead when no credit card reader present";
			_logger.Log (msg);
			throw new NotImplementedException (msg);
		}

		public void AddChars (char c)
		{
			const string msg = "Attempt to AddChars when no credit card reader present";
			_logger.Log (msg);
			throw new NotImplementedException (msg);
		}

		public void AddCard (CreditCard card)
		{
			const string msg = "Attempt to AddCard when no credit card reader present";
			_logger.Log (msg);
			throw new NotImplementedException (msg);
		}

		public bool Enabled {
			get {
				return false;
			}
		}

		public CreditCardReaderType CreditCardReaderType {
			get {
				return CreditCardReaderType.None;
			}
		}

		public bool Reading {
			get {
				return false;
			}
		}

		#endregion
	}
}

