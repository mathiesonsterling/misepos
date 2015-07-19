using Android.App;
using Android.Content.PM;
using Android.OS;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Mise.AndroidCommon.Services;
using Mise.Core.Services;
using Mise.Core.Client.Services;
using Mise.POSTerminal;
using Mise.POSTerminal.Android;
using Mise.POSTerminal.Android.Services;
using Mise.Core.ValueItems;
using Mise.Core.Common.Services.Implementation.Serialization;

[assembly: Dependency(typeof(AndroidClientLogger))]
[assembly: Dependency(typeof(AndroidUnitOfWork))]
[assembly: Dependency(typeof(AndroidSQLLiteDAL))]
[assembly: Dependency(typeof(CashDrawerService))]
[assembly: Dependency(typeof(FakeCreditCardProcessingService))]
namespace Mise.POSTerminal.Android
{
	[Activity(
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		Label = "Mise POS",
		MainLauncher = false,
		Theme = @"@style/Theme.App"
	)]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			var logger = new AndroidClientLogger();
			var serial = new JsonNetSerializer();

			base.OnCreate(savedInstanceState);

			Insights.Initialize("8abca5f28c73a3faefd9eddb5d2053ba819ba11d", this);
			Forms.Init(this, savedInstanceState);
			var creditCardService = CreditCardReaderServiceFactory.GetCreditCardReaderService(
				                        CreditCardReaderType.AudioReader,
				                        logger
			                        );

			var dal = new AndroidSQLLiteDAL(BaseContext, logger, serial);
			LoadApplication(new App(creditCardService, dal, logger));
		}
	}
}


