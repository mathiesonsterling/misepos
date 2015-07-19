using System;

namespace Mise.Core.ValueItems
{
	/// <summary>
	/// Type of credit card readers this application supports
	/// </summary>
	[Flags]
	public enum CreditCardReaderType
	{
		/// <summary>
		/// No credit cards taken as payment
		/// </summary>
		None = 0,

		/// <summary>
		/// Credit cards are handled by an external system
		/// </summary>
		External = 1,

		/// <summary>
		/// The USB keyboard swipe
		/// </summary>
		USBKeyboardSwipe = 2,

		/// <summary>
		/// Audio reader, similar to square or paypal, that uses the audio jack to take input
		/// </summary>
		AudioReader = 4,

		/// <summary>
		/// Camera reader, like card.io
		/// </summary>
		CameraReader = 8
	}
}

