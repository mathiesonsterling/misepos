using System;

namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
	public class AzureServiceLocation{
		public AzureServiceLocation(string loc, string key){
			Uri = new Uri (loc);
			AppKey = key;
		}

		public Uri Uri{get;private set;}
		public string AppKey{get;private set;}
	}

	public static class AzureServiceLocator
	{
		public static AzureServiceLocation GetAzureMobileServiceLocation(BuildLevel level){
			if(level == BuildLevel.Demo){
				return null;
			}
				
			switch(level){
			case BuildLevel.Debugging:
				return new AzureServiceLocation("http://localhost:50778", "vvECpsmISLzAxntFjNgSxiZEPmQLLG42");
				break;
			case BuildLevel.Development:
			case BuildLevel.QA:
				return new AzureServiceLocation ("https://stockboymobileservice.azure-mobile.net/", "vvECpsmISLzAxntFjNgSxiZEPmQLLG42");
				break;
			case BuildLevel.Production:
				return new AzureServiceLocation("https://stockboymobileservice.azure-mobile.net/", "vvECpsmISLzAxntFjNgSxiZEPmQLLG42");
				break;
			default:
				throw new ArgumentException ();
			}
		}
	}
}

