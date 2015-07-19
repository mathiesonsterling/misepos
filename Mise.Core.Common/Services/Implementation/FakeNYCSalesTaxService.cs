using System;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
namespace Mise.Core.Common.Services.Implementation
{
	public class FakeNYCSalesTaxService : ISalesTaxCalculatorService
	{
		public FakeNYCSalesTaxService ()
		{
		}

		#region ISalesTaxCalculatorService implementation

		public Money CalculateSalesTax (ICheck check)
		{
			return CalculateSalesTax (check.Total);
		}

	    public Money CalculateSalesTax(IPurchaseOrder purchaseOrder)
	    {
	        throw new NotImplementedException();
	    }

	    public Money CalculateSalesTax(Money amount){
			return amount.Multiply (.08875M);
		}
		#endregion
	}
}

