using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems.Reports;

namespace Mise.Core.Common.Entities.Reports
{
    public class AmountUsedInTimeReport : BaseReport
    {
        private readonly IInventory _startingInventory;
        private readonly IEnumerable<IInventory> _inventoriesInTime;
        private readonly IEnumerable<IReceivingOrder> _receivingOrdersInTime;
        private readonly DateTimeOffset _start;
        private readonly DateTimeOffset _end;
		private readonly LiquidAmountUnits _unit;
        public AmountUsedInTimeReport(DateTimeOffset start, DateTimeOffset end, IInventory startingInventory, 
            IEnumerable<IReceivingOrder> receivingOrdersInTime, IEnumerable<IInventory> inventoriesInTime, LiquidAmountUnits unit)
        {
            if (startingInventory != null && startingInventory.DateCompleted.HasValue == false)
            {
                throw new ArgumentException("Inventory must be complete!");
            }

            var tempInv = inventoriesInTime.OrderBy(i => i.DateCompleted).ToList();
			if(tempInv.Any() == false){
				throw new ArgumentException ("There are no inventories in the selected time period, cannot run!");
			}
            if (tempInv.Any(i => i.DateCompleted.HasValue == false))
            {
                throw new ArgumentException("All inventories must be complete");
            }

            _startingInventory = startingInventory;
            _receivingOrdersInTime = receivingOrdersInTime;
            _inventoriesInTime = tempInv;
            _start = start;
            _end = end;
			_unit = unit;
        }

        public override ReportTypes ReportType
        {
            get { return ReportTypes.AmountUsed; }
        }

		private class LineItemsAndKey : Dictionary<string, IBaseBeverageLineItem>{
			public void AddIfDoesntExist(Dictionary<string, IBaseBeverageLineItem> keysAndItems){
				var needsAdds = keysAndItems.Where (i => ContainsKey (i.Key) == false);
				foreach(var item in needsAdds){
                    try{
					    Add (item.Key, item.Value);
                    } catch(Exception e){
                        throw;
                    }
				}
			}

		}
            
        protected override ReportResult CreateReport()
        {
            var allPossiblePastInventoryies = new List<IInventory>();
            if (_startingInventory != null)
            {
                allPossiblePastInventoryies.Add(_startingInventory);
            }
            allPossiblePastInventoryies.AddRange(_inventoriesInTime);
            allPossiblePastInventoryies =
                allPossiblePastInventoryies.OrderByDescending(i => i.DateCompleted).ToList();

            var amountsUsedAllPeriods = new List<Dictionary<string, LiquidAmount>>();

			var allItemsAndKeys = new LineItemsAndKey ();
            //for each inventory, we need to get the one previous as our baseline
            foreach (var invInTime in _inventoriesInTime)
            {
                //find the most prior inventory to this
                var priorInv =
                    allPossiblePastInventoryies.FirstOrDefault(i => i.DateCompleted < invInTime.DateCompleted);
				if(priorInv != null){
                    var itemsAndKeys = new Dictionary<string, IBaseBeverageLineItem>();
                    foreach (var item in priorInv.GetBeverageLineItems())
                    {
                        var key = GetListItemKey(item);
                        while (itemsAndKeys.ContainsKey(key))
                        {
                            key = key + "n";
                        }
                        itemsAndKeys.Add(key, item);

                    }
					allItemsAndKeys.AddIfDoesntExist (itemsAndKeys);
				}
                var thisInventory = invInTime;
                var newDic = new Dictionary<string, IBaseBeverageLineItem>();
                foreach (var item in thisInventory.GetBeverageLineItems())
                {
                    var key = GetListItemKey(item);
                    while (newDic.ContainsKey(key))
                    {
                        key = key + "n";
                    }
                    newDic.Add(key, item);
                }
                allItemsAndKeys.AddIfDoesntExist(newDic);

                var rosInPeriod =
                    _receivingOrdersInTime.Where(
                        ro =>
                            ro.DateReceived < thisInventory.DateCompleted &&
                            (priorInv == null || ro.DateReceived > priorInv.DateCompleted));

                var allROInPeriodLineItems = rosInPeriod.SelectMany(ro => ro.GetBeverageLineItems()).ToList();
				allItemsAndKeys.AddIfDoesntExist (allROInPeriodLineItems
					.Select (li => li as IBaseBeverageLineItem)
					.ToDictionary (li => GetListItemKey (li))
				);

                var amountsUsedThisPeriod = GetAmountUsedByInventoryPeriod(priorInv, allROInPeriodLineItems,
                    thisInventory);

                amountsUsedAllPeriods.Add(amountsUsedThisPeriod);
            }

            //sum up the amounts for each item
			var totalAmounts = new Dictionary<string, LiquidAmount> ();
			foreach(var periodUse in amountsUsedAllPeriods){
				foreach(var lineItem in periodUse){
					if(totalAmounts.ContainsKey (lineItem.Key)){
						//add the amounts
						totalAmounts[lineItem.Key] = totalAmounts[lineItem.Key].Add(lineItem.Value);
					} else{
						totalAmounts.Add (lineItem.Key, lineItem.Value);
					}
				}
			}

            //transform to result object
			var res = new List<ReportResultLineItem>();
			foreach(var amount in totalAmounts){
				//get the LI this refers to
				var li = allItemsAndKeys[amount.Key];

				var quantity = _unit == LiquidAmountUnits.OuncesLiquid 
					? amount.Value.GetInLiquidOunces () 
					: amount.Value.GetInMilliliters ();
				var reportLine = new ReportResultLineItem (li.DisplayName, li.Container.DisplayName, quantity, quantity >= 0);
				res.Add (reportLine);
			}

			var title = "Amount used in "
				+ (_unit == LiquidAmountUnits.OuncesLiquid ? "oz" : "ml")
			            + " between "
			            + _start.ToLocalTime ().ToString ("d")
			            + " and "
			            + _end.ToLocalTime ().ToString ("d");

			var checkSum = (decimal)res.Sum (rl => rl.Quantity);
			var result = new ReportResult (ReportTypes.AmountUsed, title, res, checkSum);
			return result;
        }

        private Dictionary<string, LiquidAmount> GetAmountUsedByInventoryPeriod(IInventory previousInventory,
            ICollection<IReceivingOrderLineItem> roLineItemsInPeriod, IInventory currentInventory)
        {
            var res = new Dictionary<string, LiquidAmount>();

            //first dedup any line items we have!
            foreach (var currentLineItem in currentInventory.GetBeverageLineItems())
            {
                var key = GetListItemKey(currentLineItem);
                if (res.ContainsKey(key)) {
                    continue;
                }
                var thisLineItem = currentLineItem;
                var previousAmount = LiquidAmount.None;
                //get the matching line for the previous inventory, if it exists
                if (previousInventory != null)
                {
                    var prevLineItems =
                        previousInventory.GetBeverageLineItems().Where(
                            pli => BeverageLineItemEquator.AreSameBeverageLineItem(pli, thisLineItem))
                            .ToList();
                    foreach (var pLI in prevLineItems)
                    {
                        previousAmount = previousAmount.Add(pLI.CurrentAmount);
                    }
                }

                //get any ROs between the two, add them to the previous
                var roS =
                    roLineItemsInPeriod.Where(
                        roLI => BeverageLineItemEquator.AreSameBeverageLineItem(roLI, thisLineItem));
                foreach (var roLI in roS)
                {
                    previousAmount = previousAmount.Add(roLI.Container.AmountContained);
                }

                //save the difference
                if (!previousAmount.Equals(LiquidAmount.None))
                {
                    var amountUsed = previousAmount.Subtract(currentLineItem.CurrentAmount);
                    res.Add(key, amountUsed);
                }
            }

            return res;
        } 
    }
}
