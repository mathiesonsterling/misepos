using System;

namespace Mise.Core.Common.Services.Implementation
{
	public class MercuryPaymentProviderSettings
	{
		private readonly bool _isDevelopment;
		public MercuryPaymentProviderSettings(bool isDevelopment){
			_isDevelopment = isDevelopment;	
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
	}


}

