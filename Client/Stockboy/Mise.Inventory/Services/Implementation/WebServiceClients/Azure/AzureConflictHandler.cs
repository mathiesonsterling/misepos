using System;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using Mise.Core.Services.UtilityServices;


namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
	public class AzureConflictHandler : IMobileServiceSyncHandler
	{
		readonly ILogger _logger;
		public AzureConflictHandler(ILogger logger){
			_logger = logger;
		}
		#region IMobileServiceSyncHandler implementation

		public Task OnPushCompleteAsync (MobileServicePushCompletionResult result)
		{
			return Task.FromResult (false);
		}

		public async Task<JObject> ExecuteTableOperationAsync (IMobileServiceTableOperation operation)
		{
		    try{
				await operation.ExecuteAsync ();
				return null;
			}                 
			catch (MobileServiceConflictException ex)
			{
			    _logger.HandleException (ex);
			}
			catch (MobileServicePreconditionFailedException ex)
			{
			    _logger.HandleException (ex);

				//https://codemilltech.com/why-cant-we-be-friends-conflict-resolution-in-azure-mobile-services/
			}
			catch(Exception e){
				_logger.HandleException (e);
				throw;
			}

		    try
		    {
                //retry so we'll take the client value
		        await operation.ExecuteAsync();
		        return null;
		    }
		    catch (MobileServicePreconditionFailedException e)
		    {
		        _logger.HandleException(e, LogLevel.Fatal);
		        return e.Value;
		    }
		}

		#endregion


	}
}

