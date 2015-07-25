using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems.Reports;

namespace Mise.Inventory.Reports
{
    public class AmountUsedInTimeReport : BaseReport
    {
        private readonly IInventory _startingInventory;
        private readonly IEnumerable<IInventory> _inventoriesInTime;
        private readonly IEnumerable<IReceivingOrder> _receivingOrdersInTime;
        private readonly DateTimeOffset _start;
        private readonly DateTimeOffset _end;
        public AmountUsedInTimeReport(DateTimeOffset start, DateTimeOffset end, IInventory startingInventory, 
            IEnumerable<IReceivingOrder> receivingOrdersInTime, IEnumerable<IInventory> inventoriesInTime)
        {
            if (startingInventory != null && startingInventory.DateCompleted.HasValue == false)
            {
                throw new ArgumentException("Inventory must be complete!");
            }

            var tempInv = inventoriesInTime.OrderBy(i => i.DateCompleted).ToList();
            if (tempInv.Any(i => i.DateCompleted.HasValue == false))
            {
                throw new ArgumentException("All inventories must be complete");
            }

            _startingInventory = startingInventory;
            _receivingOrdersInTime = receivingOrdersInTime;
            _inventoriesInTime = tempInv;
            _start = start;
            _end = end;
        }

        public override ReportTypes ReportType
        {
            get { return ReportTypes.AmountUsed; }
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

            //for each inventory, we need to get the one previous as our baseline
            foreach (var invInTime in _inventoriesInTime)
            {
                //find the most prior inventory to this
                var priorInv =
                    allPossiblePastInventoryies.FirstOrDefault(i => i.DateCompleted < invInTime.DateCompleted);
                var thisInventory = invInTime;
                var rosInPeriod =
                    _receivingOrdersInTime.Where(
                        ro =>
                            ro.DateReceived < thisInventory.DateCompleted &&
                            (priorInv == null || ro.DateReceived > priorInv.DateCompleted));

                var allROInPeriodLineItems = rosInPeriod.SelectMany(ro => ro.GetBeverageLineItems()).ToList();

                var amountsUsedThisPeriod = GetAmountUsedByInventoryPeriod(priorInv, allROInPeriodLineItems,
                    thisInventory);

                amountsUsedAllPeriods.Add(amountsUsedThisPeriod);
            }

            //sum up the amounts for each item

            //return the result
            return null;
        }

        private Dictionary<string, LiquidAmount> GetAmountUsedByInventoryPeriod(IInventory previousInventory,
            ICollection<IReceivingOrderLineItem> roLineItemsInPeriod, IInventory currentInventory)
        {
            var res = new Dictionary<string, LiquidAmount>();

            //first dedup any line items we have!
            foreach (var currentLineItem in currentInventory.GetBeverageLineItems())
            {
                var key = GetListItemKey(currentLineItem);
                if (res.ContainsKey(key)) {continue;}
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
                var amountUsed = previousAmount.Subtract(currentLineItem.CurrentAmount);
                res.Add(key, amountUsed);
            }

            return res;
        } 
    }
}
