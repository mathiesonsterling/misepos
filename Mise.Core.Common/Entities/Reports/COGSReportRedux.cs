using System;
using System.Linq;
using System.Collections.Generic;

using Mise.Core.ValueItems;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems.Reports;
namespace Mise.Core.Common.Entities.Reports
{
    public class COGSReportRedux : AmountUsedRedux
    {
        private readonly IEnumerable<IInventory> _allInventoriesForPrices;
        public COGSReportRedux(IEnumerable<IInventory> inventories,
            IEnumerable<IReceivingOrder> receivingOrdersInPeriod, IEnumerable<IInventory> allInventoriesForPrices) 
            : base(inventories, receivingOrdersInPeriod, LiquidAmountUnits.Milliliters)
        {
            _allInventoriesForPrices = allInventoriesForPrices;
        }

        #region implemented abstract members of BaseReport

        protected override ReportResult CreateReport()
        {
            //get the amounts used
            var amountsUsed = GetTotalAmounts(_inventories, _receivingOrders);

            //get the prices
            var windowDates = _inventories.Select(i => i.DateCompleted.Value);
            var priceDic = GetPriceDictionary(_allInventoriesForPrices, _receivingOrders, windowDates.Min(), windowDates.Max());

            var allPossibleKeys = GetAllPossibleKeys(_inventories, _receivingOrders);

            var res = new Dictionary<KeyAndItemName, Money>();
            foreach (var itemKey in amountsUsed.Keys.OrderBy(k => k))
            {
                var key = allPossibleKeys[itemKey];
                if (key != null)
                {
                    if (priceDic.ContainsKey(itemKey))
                    {
                        //get the key
                        var numberOfBottlesUsed = amountsUsed[itemKey].Divide(key.Container.AmountContained);

                        //find the price
                        var price = GetPriceForItem(priceDic, itemKey);

                        if (price != null)
                        {
                            //multiply price times number of bottles to get cost
                            var cost = price.Multiply(numberOfBottlesUsed);

                            res.Add(key, cost);
                        }
                        else
                        {
                            res.Add(key, null);
                        }
                    }
                    else
                    {
                        res.Add(key, null);
                    }
                }
            }

            var reportLineItems = res.Select(kv => 
                new ReportResultLineItem(
                    kv.Key.ItemName, 
                    kv.Value != null ? kv.Key.Container.DisplayName : "Needs price!", 
                    kv.Value != null ?kv.Value.Dollars : (decimal?)null, 
                    kv.Value != null && kv.Value.Dollars >= 0));

            return new ReportResult(ReportTypes.COGS, "COGS", reportLineItems, 0);

        }

        private Money GetPriceForItem(Dictionary<string, Dictionary<DateTimeOffset, Money>> priceDic, string itemKey)
        {
            var possiblePrices = priceDic[itemKey];

            //first see if we have a date that was set before
            DateTimeOffset bestPriceDate = possiblePrices.Keys.Max();

            /*
            if (bestPriceDate == null)
            {
                return null;
            }*/
            var price = possiblePrices[bestPriceDate];
            return price;
        }
            
        private Dictionary<string, Dictionary<DateTimeOffset, Money>> GetPriceDictionary(IEnumerable<IInventory> inventories,
            IEnumerable<IReceivingOrder> ros, DateTimeOffset preferredStart, DateTimeOffset preferredEnd)
        {
            var res = new Dictionary<string, Dictionary<DateTimeOffset, Money>>();

            //we load the ros first
            foreach (var ro in ros)
            {
                foreach (var roLI in ro.GetBeverageLineItems())
                {
                    if (roLI.LineItemPrice != null)
                    {
                        var key = GetListItemKey(roLI);

                        Dictionary<DateTimeOffset, Money> dateAndPriceDic;
                        if (!res.ContainsKey(key))
                        {
                            dateAndPriceDic = new Dictionary<DateTimeOffset, Money>();
                            res.Add(key, dateAndPriceDic);
                        }
                        else
                        {
                            dateAndPriceDic = res[key];
                        }

                        if (!dateAndPriceDic.ContainsKey(ro.DateReceived))
                        {
                            dateAndPriceDic.Add(ro.DateReceived, roLI.UnitPrice);
                        }
                    }
                }
            }

            //do first the inventories that are BEFORE our range, then after
            var inRanges = inventories.Where(i => i.DateCompleted.Value >= preferredStart && i.DateCompleted.Value <= preferredEnd);

            foreach (var inv in inRanges)
            {
                foreach (var li in inv.GetBeverageLineItems())
                {
                    if (li.PricePaid != null)
                    {
                        var key = GetListItemKey(li);
                        Dictionary<DateTimeOffset, Money> dateAndPriceDic;
                        if (!res.ContainsKey(key))
                        {
                            dateAndPriceDic = new Dictionary<DateTimeOffset, Money>();
                            res.Add(key, dateAndPriceDic);
                        }
                        else
                        {
                            dateAndPriceDic = res[key];
                        }

                        if (!dateAndPriceDic.ContainsKey(inv.DateCompleted.Value))
                        {
                            dateAndPriceDic.Add(inv.DateCompleted.Value, li.PricePaid.Multiply(1/li.Quantity));
                        }
                    }
                }
            }
        
            var outRanges = inventories.Except(inRanges);
            foreach (var inv in outRanges)
            {
                foreach (var li in inv.GetBeverageLineItems())
                {
                    if (li.PricePaid != null)
                    {
                        var key = GetListItemKey(li);
                        Dictionary<DateTimeOffset, Money> dateAndPriceDic;
                        if (!res.ContainsKey(key))
                        {
                            dateAndPriceDic = new Dictionary<DateTimeOffset, Money>();
                            res.Add(key, dateAndPriceDic);
                        }
                        else
                        {
                            dateAndPriceDic = res[key];
                        }

                        if (!dateAndPriceDic.ContainsKey(inv.DateCompleted.Value))
                        {
                            dateAndPriceDic.Add(inv.DateCompleted.Value, li.PricePaid.Multiply(1/li.Quantity));
                        }
                    }
                }
            }
            return res;
        }
       
        public override Mise.Core.ValueItems.Reports.ReportTypes ReportType
        {
            get
            {
                return Mise.Core.ValueItems.Reports.ReportTypes.COGS;
            }
        }

        #endregion
    }
}

