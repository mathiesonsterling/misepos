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
        public COGSReportRedux(IEnumerable<IInventory> inventories,
            IEnumerable<IReceivingOrder> receivingOrdersInPeriod) : base(inventories, receivingOrdersInPeriod, LiquidAmountUnits.Milliliters)
        {
        }

        #region implemented abstract members of BaseReport

        protected override ReportResult CreateReport()
        {
            //get the amounts used
            var amountsUsed = GetTotalAmounts(_inventories, _receivingOrders);

            //get the prices
            var priceDic = GetPriceDictionary(_inventories, _receivingOrders);

            var allPossibleKeys = GetAllPossibleKeys(_inventories, _receivingOrders);

            var res = new Dictionary<KeyAndItemName, Money>();
            foreach (var itemKey in amountsUsed.Keys.OrderBy(k => k))
            {
                if (priceDic.ContainsKey(itemKey))
                {
                    //get the key
                    var key = allPossibleKeys[itemKey];
                    var numberOfBottlesUsed = amountsUsed[itemKey].Divide(key.Container.AmountContained);

                    //find the last price
                    var possiblePrices = priceDic[itemKey];
                    var lastPriceDate = possiblePrices.Keys.Max();
                    var price = possiblePrices[lastPriceDate];

                    //multiply price times number of bottles to get cost
                    var cost = price.Multiply(numberOfBottlesUsed);

                    res.Add(key, cost);
                }

            }

            var reportLineItems = res.Select(kv => 
                new ReportResultLineItem(
                    kv.Key.ItemName, kv.Key.Container.DisplayName, 
                    kv.Value.Dollars, kv.Value.Dollars >= 0));

            return new ReportResult(ReportTypes.COGS, "COGS", reportLineItems, 0);

        }
            
        private Dictionary<string, Dictionary<DateTimeOffset, Money>> GetPriceDictionary(IEnumerable<IInventory> inventories,
            IEnumerable<IReceivingOrder> ros)
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

            foreach (var inv in inventories.Where(i => i.DateCompleted.HasValue))
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

