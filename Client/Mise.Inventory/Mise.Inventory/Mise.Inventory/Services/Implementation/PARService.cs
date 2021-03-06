﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Repositories;
using Mise.Core.Common.Events;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.Client.Services;

namespace Mise.Inventory.Services.Implementation
{
	public class PARService : IPARService
	{
		private IPar _currentPar;
		IParBeverageLineItem _lineItem;

		readonly IParRepository _parRepository;
		readonly ILoginService _loginService;
		readonly IInventoryAppEventFactory _eventFactory;
	    private readonly IInsightsService _insights;
		readonly ILogger _logger;
		public PARService (ILogger logger, ILoginService loginService, IParRepository parRespository, 
			IInventoryAppEventFactory eventFactory, IInsightsService insights)
		{
			_parRepository = parRespository;
			_loginService = loginService;
			_eventFactory = eventFactory;
			_logger = logger;
		    _insights = insights;
		}

		#region IPARService implementation

		public async Task AddLineItemToCurrentPAR (string name, ICategory category, string upc, int? quantity, int caseSize, LiquidContainer container)
		{
			var emp = await _loginService.GetCurrentEmployee().ConfigureAwait (false);
			var curr = await GetCurrentPAR ().ConfigureAwait (false);

			var categories = new []{ category as InventoryCategory };
			var addEv = _eventFactory.CreatePARLineItemAddedEvent (emp, name, upc, categories, caseSize, container, 
				quantity.HasValue ? quantity.Value:0, curr);
		
			_currentPar = _parRepository.ApplyEvent (addEv);
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
		}
			
		public async Task<IPar> GetCurrentPAR ()
		{
			if (_currentPar == null) {
				var rest = await _loginService.GetCurrentRestaurant ();
				_currentPar = await _parRepository.GetCurrentPAR (rest.Id);
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
					await _parRepository.Commit (curr.Id);
				} catch(Exception e){
					_logger.HandleException (e);
					throw;
				}
			}
		}

		public async Task<IPar> CreateCurrentPAR ()
		{
			var emp = await _loginService.GetCurrentEmployee ();

			//TODO - if there's already a PAR, mark it as no longer current
			//make a new PAR, and set it as the current
			var createEv = _eventFactory.CreatePARCreatedEvent (emp);
			_currentPar = _parRepository.ApplyEvent (createEv);
			return _currentPar;
		}

		public async Task UpdateQuantityOfPARLineItem (IParBeverageLineItem lineItem, decimal newQuantity)
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var par = await GetCurrentPAR ().ConfigureAwait (false);

			var updateEv = _eventFactory.CreatePARLineItemQuantityUpdatedEvent (emp, par, lineItem.Id, newQuantity);
			_currentPar = _parRepository.ApplyEvent (updateEv);
            ReportNumItemsInTransaction();
		}

		public Task SetCurrentLineItem (IParBeverageLineItem li)
		{
			_lineItem = li;
			return Task.FromResult (true);
		}

		public async Task DeleteLineItem (IParBeverageLineItem li)
		{
			var emp = await _loginService.GetCurrentEmployee ();
			var par = await GetCurrentPAR ();

			var ev = _eventFactory.CreateParLineItemDeletedEvent (emp, par, li);
			_currentPar = _parRepository.ApplyEvent (ev);
			ReportNumItemsInTransaction ();
		}

		public Task<IParBeverageLineItem> GetCurrentLineItem ()
		{
			return Task.FromResult (_lineItem);
		}
		#endregion


        private const int REPORT_NUMBER_OF_EVENTS_THRESHOLD = 50;
        /// <summary>
        /// If we have a large number of events waiting for commit, let's mark an event of it.  Later we might do a bleed off
        /// </summary>
        private void ReportNumItemsInTransaction()
        {
            if (_currentPar != null)
            {
                var numItems = _parRepository.GetNumberOfEventsInTransacitonForEntity(_currentPar.Id);
                if (numItems > REPORT_NUMBER_OF_EVENTS_THRESHOLD)
                {
                    var insightsParam = new Dictionary<string, string>
	                {
	                    {"Repository", "Par"},
	                    {"Number of items", numItems.ToString()}
	                };
                    _insights.Track("Large number of events in repository", insightsParam);
                }
            }
        }
	}
}

