using System;

namespace Mise.Core.Common.Services.Implementation
{
	public class MercuryPaymentProviderSettings
	{
		private readonly bool _isDevelopment;
		public MercuryPaymentProviderSettings(bool isDevelopment){
			_isDevelopment = isDevelopment;	
		}

		public string WebServiceUrl{
			get{
				return _isDevelopment
					? "https://hc.mercurydev.net/hcws/hcservice.asmx"
						: "https://hc.mercurypay.com/hcws/hcservice.asmx";
			}
		}

		public string MercuryCheckoutPageUrl{
			get{
				return _isDevelopment 
					? "https://hc.mercurydev.net/mobile/mCheckout.aspx"
						: "https://hc.mercurypay.com/mobile/mCheckout.aspx";
			}
		}
		public string MerchantID{
			get{
				return _isDevelopment ? 013163015566916.ToString () : "-1";
			}
		}

		public string Password{
			get{
				return _isDevelopment ? "passwordDemo" : "password";
			}
		}
			
		public string TranType{
			get{return "PreAuth";}
		}

		public string Frequency{
			get{return "Recurring";}
		}

		public string TaxAmount{
			get{return "0.00";}
		}

		public string LogoUrl{
			get{
				return "http://mise.in/images/logo.png";
			}
		}

		public string ProcessCompleteUrl{
			get{
				return "http://mise.in";
			}	
		}

		public string ReturnUrl{
			get{return "http://mise.in";}
		}

		public int StartWaitTimeForResponseInMS{
			get{return 100;}
		}

		public int MaxWaitTimeForResponseInMS{
			get{return 10000;}
		}
	}


}

