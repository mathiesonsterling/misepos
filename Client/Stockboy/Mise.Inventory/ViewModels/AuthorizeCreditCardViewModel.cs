using System;
using System.Threading.Tasks;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Inventory.Services;
using Mise.Core.Common.Services.Implementation;

namespace Mise.Inventory.ViewModels
{
	public class AuthorizeCreditCardViewModel : BaseViewModel
	{
	    private readonly ILoginService _loginService;
	    private readonly ICreditCardProcessorService _creditCardProcessorService;
		readonly MercuryPaymentProviderSettings _mercurySettings;
		private string _paymentID;
	    public AuthorizeCreditCardViewModel(IAppNavigation navigationService, ILogger logger, ILoginService loginService, 
            ICreditCardProcessorService creditCardProcessorService)
            : base(navigationService, logger)
	    {
	        _creditCardProcessorService = creditCardProcessorService;
	        _loginService = loginService;

			var isDevelopment = DependencySetup.GetBuildLevel () != BuildLevel.Production;
			_mercurySettings = new MercuryPaymentProviderSettings (isDevelopment);
	    }

	    public string StartUrl
	    {
	        get { return _mercurySettings.MercuryCheckoutPageUrl + "?pid=" + _paymentID; }
	    }

	    public override async Task OnAppearing()
	    {
	        try
	        {
	            Processing = true;
	            PersonName name = null;
	            var emp = await _loginService.GetCurrentEmployee();
	            if (emp != null)
	            {
	                name = emp.Name;
	            }
	            var accountID = await _loginService.GetRegisteringAccountID();
	            if (accountID.HasValue == false)
	            {
	                throw new InvalidOperationException("No account currently set to register");
	            }
	            _paymentID = await _creditCardProcessorService.SetPaymentID(accountID.Value, name, Money.MiseMonthlyFee);
	            Processing = false;
	        }
	        catch (Exception e)
	        {
	            HandleException(e);
	        }
	    }

        /// <summary>
        /// Notifies us that the web view moved, and we should start polling our items
        /// </summary>
        /// <param name="newUrl"></param>
	    public async Task UrlHasChanged(string newUrl)
        {
            try
            {
				if(string.IsNullOrEmpty (newUrl))
				{
					return;
				}

				if(newUrl.Contains (_mercurySettings.ProcessCompleteUrl))
				{
	                Processing = true;
	                var creditCardFromOurServer = await _creditCardProcessorService.GetCardAfterAuthorization(_paymentID);

	                if (creditCardFromOurServer != null)
	                {
	                    await _loginService.CompleteRegisterAccount(creditCardFromOurServer);
	                    await Navigation.ShowMainMenu();
	                }
	                Processing = false;
				}
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }
	}
}

