using System;

namespace Mise.Core.Common.Services.Implementation
{
	public static class StripePaymentProviderSettingsFactory
	{
		public static IStripePaymentProviderSettings GetSettings(){
			return new StripePaymentProviderSettingsTest();
		}
			
	}

	public interface IStripePaymentProviderSettings
	{
		string PublishableKey{get;}
        string PrivateKey{get;}

        decimal SalesTaxRate { get; }
	}

	public class StripePaymentProviderSettingsTest : IStripePaymentProviderSettings
	{
		public string PublishableKey => "pk_test_uoq4zFLF74q0PbjzEIdNbLdQ";
	    public string PrivateKey => "sk_test_2M2KGQZ9oS1UHNGe1nF3Uzir";

	    public decimal SalesTaxRate => 8.88M;
	}
}

