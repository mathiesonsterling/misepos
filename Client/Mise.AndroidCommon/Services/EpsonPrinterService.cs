using System;
using Mise.Core.Services;
using System.Threading.Tasks;

using Mise.Core.Entities.Check;
using Mise.Core.Entities.Payments;
using System.Collections;
using System.Linq;
using Com.Epson.Eposprint;
using Com.Epson.Epsonio;
using System.Collections.Generic;
using Android.Support.V4.Print;
using Android.Content;
using System.Runtime.InteropServices;
using Android.Drm;
using Android.OS;
using System.Security.Cryptography;

namespace Mise.AndroidCommon
{
	public class EpsonPrinterService : IPrinterService, IAndroidUsingContextService
	{
		private readonly ILogger _logger;
		Print _printer;
		private Context _context;
		const string ModelName = "TT20II";
		Builder builder;
		string _restaurantName;
		public EpsonPrinterService (ILogger logger, string restaurantName, Context context)
		{
			_logger = logger;
			_restaurantName = restaurantName;
		}

		public void SetContext (Context context){
			_context = context;

			try{
				_logger.Log ("Constructing Print", LogLevel.Debug);
				_printer = new Print ();
				OpenPrinter ();

				_logger.Log ("Constructing Builder", LogLevel.Debug);
				builder = new Builder (ModelName, 0, context);

				_logger.Log ("Adding FontA", LogLevel.Debug);
				builder.AddTextFont (Builder.FontA);

				_logger.Log ("Adding TextAlign");
				builder.AddTextAlign (0);

			} catch (Exception e){
				_logger.HandleException (e);
				throw;
			}
		}
		#region IPrinterService implementation

		public Task<bool> PrintRecieptAsync (ICheck check)
		{
			_logger.Log ("Entering PrintRecieptAsync", LogLevel.Debug);
			throw new NotImplementedException ();
		}

		public Task<bool> PrintDupeAsync (ICheck check)
		{
			_logger.Log ("Entering PrintDupeAsync", LogLevel.Debug);

			return PrintDupe (check);
		}

		async Task<bool> PrintDupe(ICheck check){
			//get the items with a destination of kitchen
			var kitchenOIs = new List<OrderItem> ();
			var lines = new List<string> ();
			foreach(var oi in check.OrderItems){
				foreach(var dest in oi.Destinations){
					if(dest.Name.ToUpper () == "KITCHEN"){
						kitchenOIs.Add (oi);

						lines.Add (oi.Name);
						foreach(var mod in oi.Modifiers){
							lines.Add (mod.Name);
						}
					}
				}
			}
			PrintLines (lines);
			return lines.Any();
		}

		public Task<bool> PrintCreditCardSlipAsync (ICreditCardPayment ccPayment)
		{
			return Task.Factory.StartNew (() => {
				var lines = new List<string>{
					_restaurantName,
					string.Empty,
					ccPayment.Card.FirstName + " " + ccPayment.Card.LastName,

					//add the total
					"SUB-TOTAL:  " + ccPayment.AmountCharged.ToString (),
					string.Empty,
					"TIP: " + "___________________",
					string.Empty,
					"TOTAL:  " + "_____________________", 
					string.Empty,
					"SIGNATURE",
					"__________________________________",
					ccPayment.Card.FirstName + " " + ccPayment.Card.LastName,

				};

				PrintLines (lines);
				return true;
			});

		}

		/// <summary>
		/// Given a collection of lines, push them to the printer
		/// </summary>
		/// <param name="lines">Lines.</param>
		void PrintLines(IEnumerable<string> lines){
			_logger.Log ("Printing lines . . . ");
			foreach(var line in lines){
				builder.AddText (line);
			}
			var status = new int[1];
			var battery = new int[1];

			_logger.Log ("Sending data to printer", LogLevel.Debug);
			_printer.SendData (builder, 10 * 1000, status, battery);
		}
		void OpenPrinter(){
			_logger.Log ("Opening printer", LogLevel.Debug);
			_printer.OpenPrinter(DevType.Usb, "");
		}
			
		#endregion
	}
}

