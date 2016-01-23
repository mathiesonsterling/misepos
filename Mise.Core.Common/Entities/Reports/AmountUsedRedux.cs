using System;
using System.Linq;
using System.Collections.Generic;

using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems.Reports;
namespace Mise.Core.Common.Entities.Reports
{
    public class AmountUsedRedux : BaseReport
    {
        protected readonly IEnumerable<IInventory> _inventories;
        protected readonly IEnumerable<IReceivingOrder> _receivingOrders;
        private readonly LiquidAmountUnits _unit;
        public AmountUsedRedux(IEnumerable<IInventory> inventories,
            IEnumerable<IReceivingOrder> receivingOrdersInPeriod, LiquidAmountUnits unit)
        {
            _inventories = inventories;
            _receivingOrders = receivingOrdersInPeriod;
            _unit = unit;
        }

        #region implemented abstract members of BaseReport

        protected override ReportResult CreateReport()
        {
            var amounts = GetTotalAmounts(_inventories, _receivingOrders);

            var allKeys = GetAllPossibleKeys(_inventories, _receivingOrders);

            //make the line items
            var lineItems = new List<ReportResultLineItem>();
            foreach (var amtKey in amounts.Keys.OrderBy(k => k))
            {
                if (allKeys.ContainsKey(amtKey))
                {
                    var fullKey = allKeys[amtKey];

                    var amt = amounts[amtKey];
                    if (!amt.IsEmpty)
                    {
                        var amtDec = _unit == LiquidAmountUnits.Milliliters ? amt.GetInMilliliters() : amt.GetInLiquidOunces();
                        var li = new ReportResultLineItem(fullKey.ItemName, fullKey.Container.DisplayName, amtDec, amtDec >= 0);
                        lineItems.Add(li);
                    }
                }
            }

            var title = _unit == LiquidAmountUnits.Milliliters ? "Amount in ML" : "Amount in Oz";
            return new ReportResult(ReportTypes.AmountUsed, title, lineItems, 0);
        }

        public override ReportTypes ReportType
        {
            get
            {
                return ReportTypes.AmountUsed;
            }
        }

        #endregion

        protected class KeyAndItemName
        {
            public string Key{get;private set;}
            public string ItemName{get;private set;}
            public LiquidContainer Container{get;private set;}

            public KeyAndItemName(IBaseBeverageLineItem li)
            {
                Key = GetListItemKey(li);
                ItemName = li.DisplayName;
                Container = li.Container;
            }
        }

        protected Dictionary<string, KeyAndItemName> GetAllPossibleKeys(IEnumerable<IInventory> inventories, IEnumerable<IReceivingOrder> ros)
        {
            var res = new Dictionary<string, KeyAndItemName>();
            var allLIs = inventories.SelectMany(i => i.GetBeverageLineItems());
            foreach (var li in allLIs)
            {
                var key = GetListItemKey(li);
                if (!res.ContainsKey(key))
                {
                    var kin = new KeyAndItemName(li);
                    res.Add(key, kin);
                }
            }

            var allROLis = ros.SelectMany(ro => ro.GetBeverageLineItems());
            foreach (var li in allROLis)
            {
                var key = GetListItemKey(li);
                if (!res.ContainsKey(key))
                {
                    var kin = new KeyAndItemName(li);
                    res.Add(key, kin);
                }
            }

            return res;
        }

        protected Dictionary<string, LiquidAmount> GetTotalAmounts(IEnumerable<IInventory> inventories,
            IEnumerable<IReceivingOrder> ros)
        {
            //get the cost and items, each way
            var itemAndCost = new Dictionary<string, decimal>();
            var itemsAndAmounts = new Dictionary<string, LiquidAmount>();
            var orderedInvs = _inventories.Where(i => i.DateCompleted.HasValue)
                .OrderByDescending(i => i.DateCompleted.Value);

            var usedAmount = new Dictionary<string, LiquidAmount>();
            foreach (var inventory in orderedInvs)
            {
                var previousInv = orderedInvs
                    .Where(i => i.DateCompleted < inventory.DateCompleted)
                    .OrderByDescending(i => i.DateCompleted.Value)
                    .FirstOrDefault();

                if (previousInv != null)
                {
                    //get the ROs in the period
                    var rosInPeriod = ros.Where(r => r.DateReceived < inventory.DateCompleted.Value && r.DateReceived > previousInv.DateCompleted.Value);

                    //consolidate any repeated in both inventories items
                    var endDic = GetConsolidatedItems(inventory);
                    var startDic = GetConsolidatedItems(previousInv);
                    var roAmounts = GetConsolidatedAmounts(rosInPeriod);

                    var startAmounts = AddAmountDics(startDic, roAmounts);

                    foreach (var itemKey in startAmounts.Keys)
                    {
                        LiquidAmount usedForItem;
                        if (endDic.ContainsKey(itemKey))
                        {
                            var difference = startAmounts[itemKey].Subtract(endDic[itemKey]);

                            usedForItem = difference;
                        }
                        else
                        {
                            //we used it all
                            usedForItem = startAmounts[itemKey];
                        }

                        if (usedAmount.ContainsKey(itemKey))
                        {
                            var amt = usedAmount[itemKey];
                            usedAmount[itemKey] = amt.Add(usedForItem);
                        }
                        else
                        {
                            usedAmount.Add(itemKey, usedForItem);
                        }
                    }
                } //end inventory cycle
            }

            return usedAmount;
        }

        private Dictionary<string, LiquidAmount> GetConsolidatedAmounts(IEnumerable<IReceivingOrder> ros)
        {
            var dic = new Dictionary<string, LiquidAmount>();
            foreach (var ro in ros)
            {
                foreach (var li in ro.GetBeverageLineItems())
                {
                    var key = GetListItemKey(li);
                    if (dic.ContainsKey(key))
                    {
                        var existing = dic[key];
                        dic[key] = existing.Add(li.Container.AmountContained);
                    }
                    else
                    {
                        dic[key] = li.Container.AmountContained;
                    }
                }
            }

            return dic;
        }

        private Dictionary<string, LiquidAmount> GetConsolidatedItems(IInventory inv)
        {
            var dic = new Dictionary<string, LiquidAmount>();
            foreach (var item in inv.GetBeverageLineItems())
            {
                var key = GetListItemKey(item);
                if (dic.ContainsKey(key))
                {
                    var existing = dic[key];
                    dic[key] = existing.Add(item.CurrentAmount);
                }
                else
                {
                    dic[key] = item.CurrentAmount;
                }
            }

            return dic;
        }

        private Dictionary<string, LiquidAmount> AddAmountDics(Dictionary<string, LiquidAmount> dic1, Dictionary<string, LiquidAmount> dic2)
        {
            var res = new Dictionary<string, LiquidAmount>();
            foreach (var key in dic1.Keys)
            {
                res.Add(key, dic1[key]);
            }

            foreach (var key in dic2.Keys)
            {
                if (res.ContainsKey(key))
                {
                    var existing = res[key];
                    res[key] = existing.Add(dic2[key]);
                }
                else
                {
                    res[key] = dic2[key];
                }
            }
            return res;
        }
    }
}

