using System;

namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
	public static class AzureServiceLocator
	{
		public static Uri GetAzureMobileServiceLocation(BuildLevel level){
			if(level == BuildLevel.Demo){
				return null;
			}

			Uri uri = null;
			switch(level){
			case BuildLevel.Debugging:
				uri = new Uri ("http://localhost:43499");
				break;
			case BuildLevel.Development:
			case BuildLevel.QA:
				uri = new Uri ("https://stockboymobileservice.azure-mobile.net/");
				break;
			case BuildLevel.Production:
				uri = new Uri ("https://stockboymobileservice.azure-mobile.net/");
				break;
			default:
				throw new ArgumentException ();
			}

			return uri;
		}
	}
}

