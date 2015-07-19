using Mise.Core.Client.Services;
using Mise.Core.ValueItems;
using Android.Content;

//using Card.IO;
using System;
using Android.Views.Accessibility;
using Mise.Core.Services;

namespace MiseAndroidPOSTerminal
{
	public class CreditIOReaderService : ICreditCardReaderService
	{
		Context _context;
		Action<Intent, int> _startActivityFunc;
		ILogger _logger;

		public CreditIOReaderService(ILogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Each view will have to update this
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="startFunc">Start func.</param>
		public void SetStartActivityFunction(Context context, Action<Intent, int> startFunc)
		{
			_context = context;
			_startActivityFunc = startFunc;
		}

		#region ICreditCardReaderService implementation

		public bool Enabled {
			get {
				//TODO are we running on ARM hardware?
				return false;
			}
		}

		public CreditCardReaderType CreditCardReaderType {
			get {
				return CreditCardReaderType.CameraReader;
			}
		}

		public event SwipeOccurred CreditCardSwiped;

		public void StartRead()
		{
//			Commenting out to get a build working.

//			Reading = true;
//			var intent = new Intent(_context, typeof(CardIOActivity));
//			intent.PutExtra(CardIOActivity.ExtraAppToken, "YOUR-TOKEN-HERE");
//			intent.PutExtra(CardIOActivity.ExtraRequireExpiry, true); 	
//			intent.PutExtra(CardIOActivity.ExtraRequireCvv, true); 		
//			intent.PutExtra(CardIOActivity.ExtraRequirePostalCode, false); 
//			intent.PutExtra(CardIOActivity.ExtraUseCardioLogo, false);
//			intent.PutExtra(CardIOActivity.ExtraScanResult, true);
//
//			if (_startActivityFunc != null) {
//				_logger.Log("Switching to Card.IO Activity with token 101", LogLevel.Debug);
//				_startActivityFunc.Invoke(intent, 101);
//			}
		}

		public void StopRead()
		{
			Reading = false;
		}

		public void AddChars(char c)
		{
			throw new System.NotImplementedException();
		}

		public void AddCard(Mise.Core.ValueItems.CreditCard card)
		{
			if (card != null) {
				if (CreditCardSwiped != null) {
					CreditCardSwiped(card);
				}
			}
		}

		public bool Reading { get; private set; }

		#endregion
	}
}

