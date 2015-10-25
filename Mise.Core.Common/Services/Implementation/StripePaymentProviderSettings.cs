using System;

namespace Mise.Core.Common
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
	}

	public class StripePaymentProviderSettingsTest : IStripePaymentProviderSettings
	{
		public string PublishableKey{get{ return "pk_test_uoq4zFLF74q0PbjzEIdNbLdQ"; }}
	}
}

