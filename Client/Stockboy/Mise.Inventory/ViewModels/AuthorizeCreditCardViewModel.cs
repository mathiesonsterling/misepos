using System;
using System.Threading.Tasks;
using Mise.Core.Services;
using Mise.Inventory.Services;

namespace Mise.Inventory.ViewModels
{
	public class AuthorizeCreditCardViewModel : BaseViewModel
	{
	    private readonly ILoginService _loginService;
	    private readonly ICreditCardProcessorService _creditCardProcessorService;
	    public AuthorizeCreditCardViewModel(IAppNavigation navigationService, ILogger logger, ILoginService loginService, 
            ICreditCardProcessorService creditCardProcessorService)
            : base(navigationService, logger)
	    {
	        _creditCardProcessorService = creditCardProcessorService;
	        _loginService = loginService;
	    }

	    public string StartUrl
	    {
	        get { return "https://hc.mercurydev.net/mobile/mCheckout.aspx"; }
	    }

        //The URL we want to see as proof we're done on mercury's page
	    public string TargetUrl
	    {
            get { return "http://google.com"; }
	    }

	    public override async Task OnAppearing()
	    {
	        Processing = true;
	        await _creditCardProcessorService.SetPaymentID();
	        Processing = false;
	    }

        /// <summary>
        /// Notifies us that the web view moved, and we should start polling our items
        /// </summary>
        /// <param name="newUrl"></param>
	    public async Task UrlHasChanged(string newUrl)
        {
            try
            {
                Processing = true;
                var creditCardFromOurServer = await _creditCardProcessorService.GetCardAfterAuthorization();

                if (creditCardFromOurServer != null)
                {
                    await _loginService.CompleteRegisterAccount(creditCardFromOurServer);
                    await Navigation.ShowMainMenu();
                }
                Processing = false;
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }
	}
}

