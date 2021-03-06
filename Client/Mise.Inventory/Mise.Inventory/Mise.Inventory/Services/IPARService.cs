﻿using System;
using System.Threading.Tasks;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Inventory;
namespace Mise.Inventory.Services
{
	public interface IPARService
	{
		Task AddLineItemToCurrentPAR (string name, ICategory category, string upc, int? quantity, int caseSize, 
			LiquidContainer container);
		Task AddLineItemToCurrentPAR (IBaseBeverageLineItem source, int? quantity);

		Task<IPar> GetCurrentPAR ();

		Task<IPar> CreateCurrentPAR();
		Task UpdateQuantityOfPARLineItem (IParBeverageLineItem lineItem, decimal newQuantity);

		Task SaveCurrentPAR();

		Task SetCurrentLineItem (IParBeverageLineItem li);
		Task<IParBeverageLineItem> GetCurrentLineItem ();
		Task DeleteLineItem (IParBeverageLineItem li);
	}
}

