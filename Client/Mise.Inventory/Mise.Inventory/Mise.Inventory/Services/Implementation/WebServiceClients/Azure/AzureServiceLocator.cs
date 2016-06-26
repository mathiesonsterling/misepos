using System;

namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
	public class AzureServiceLocation{
		public AzureServiceLocation(string loc, string key){
			Uri = new Uri (loc);
			AppKey = key;
		}

		public Uri Uri{get;private set;}
        /// <summary>
        /// Public key of the service
        /// </summary>
		public string AppKey{get;private set;}
	}

	public static class AzureServiceLocator
	{
        public static AzureServiceLocation GetAzureMobileServiceLocation(BuildLevel level, bool strong = false){
            if (strong)
            {
                return new AzureServiceLocation("http://stockboymobileappservice.azurewebsites.net/", "");
            }

			if(level == BuildLevel.Demo){
				return null;
			}
				
            /*
            const string applicationURL = @"https://stockboymobile.azure-mobile.net/";
            const string applicationKey = @"zjnThZMLqPYplzheWvyqPaosgWpnrH41";
            const string localDbPath    = "localstore.db";*/
			switch(level){
			case BuildLevel.Debugging:
				return new AzureServiceLocation("http://localhost:50778", "vvECpsmISLzAxntFjNgSxiZEPmQLLG42");
			case BuildLevel.Development:
			case BuildLevel.QA:
                    return new AzureServiceLocation (@"https://stockboymobile.azure-mobile.net/", 
                        @"zjnThZMLqPYplzheWvyqPaosgWpnrH41");
			case BuildLevel.Production:
                    return new AzureServiceLocation(@"https://stockboymobile.azure-mobile.net/", 
                        @"zjnThZMLqPYplzheWvyqPaosgWpnrH41");
			default:
				throw new ArgumentException ();
			}
		}
	}
}

