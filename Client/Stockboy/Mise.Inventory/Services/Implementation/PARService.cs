using System;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Repositories;
using Mise.Core.Common.Events;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services;


namespace Mise.Inventory.Services.Implementation
{
	public class PARService : IPARService
	{
		private IPAR _currentPar;

		readonly IPARRepository _parRepository;
		readonly ILoginService _loginService;
		readonly IInventoryAppEventFactory _eventFactory;
		readonly ILogger _logger;
		public PARService (ILogger logger, ILoginService loginService, IPARRepository parRespository, 
			IInventoryAppEventFactory eventFactory)
		{
			_parRepository = parRespository;
			_loginService = loginService;
			_eventFactory = eventFactory;
			_logger = logger;
		}

		#region IPARService implementation

		public async Task AddLineItemToCurrentPAR (string name, ICategory category, string upc, int? quantity, int caseSize, LiquidContainer container)
		{
			var emp = await _loginService.GetCurrentEmployee().ConfigureAwait (false);
			var curr = await GetCurrentPAR ().ConfigureAwait (false);

			var categories = new []{ category as ItemCategory };
			var addEv = _eventFactory.CreatePARLineItemAddedEvent (emp, name, upc, categories, caseSize, container, 
				quantity.HasValue ? quantity.Value:0, curr);
		
			_currentPar = _parRepository.ApplyEvent (addEv);
			await _parRepository.Commit (_currentPar.ID);
		}

		public async Task AddLineItemToCurrentPAR (IBaseBeverageLineItem source, int? quantity)
		{
			var curr = await GetCurrentPAR ().ConfigureAwait (false);
			if (curr == null) {
				curr = await CreateCurrentPAR ();
			}
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false) ;

			var addEv = _eventFactory.CreatePARLineItemAddedEvent (emp, source, quantity, curr);

			_currentPar = _parRepository.ApplyEvent (addEv);
			await _parRepository.Commit (_currentPar.ID);
		}
			
		public async Task<IPAR> GetCurrentPAR ()
		{
			if (_currentPar == null) {
				var rest = await _loginService.GetCurrentRestaurant ();
				_currentPar = await _parRepository.GetCurrentPAR (rest.RestaurantID);
			}

			return _currentPar;
		}

		public async Task SaveCurrentPAR ()
		{
			if(_parRepository.Dirty == false){
				return;
			}
			var curr = await GetCurrentPAR ();
			if (curr != null) {
				try{
					await _parRepository.Commit (curr.ID);
				} catch(Exception e){
					_logger.HandleException (e);
					throw;
				}
			}
		}

		public async Task<IPAR> CreateCurrentPAR ()
		{
			var emp = await _loginService.GetCurrentEmployee ();

			//TODO - if there's already a PAR, mark it as no longer current
			//make a new PAR, and set it as the current
			var createEv = _eventFactory.CreatePARCreatedEvent (emp);
			_currentPar = _parRepository.ApplyEvent (createEv);
			return _currentPar;
		}

		public async Task UpdateQuantityOfPARLineItem (IPARBeverageLineItem lineItem, int newQuantity)
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var par = await GetCurrentPAR ().ConfigureAwait (false);

			var updateEv = _eventFactory.CreatePARLineItemQuantityUpdatedEvent (emp, par, lineItem.ID, newQuantity);
			_currentPar = _parRepository.ApplyEvent (updateEv);
		}
		#endregion

	}
}

