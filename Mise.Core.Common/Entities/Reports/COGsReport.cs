using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ExtensionMethods;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Reports;

namespace Mise.Core.Common.Entities.Reports
{
    public class COGsReport : BaseReport
    {
        private readonly IList<IInventory> _inventories;
        private readonly IEnumerable<IReceivingOrder> _receivingOrdersInPeriod; 
        public COGsReport(IEnumerable<IInventory> inventories,
            IEnumerable<IReceivingOrder> receivingOrdersInPeriod)
        {
            _inventories = inventories.OrderBy(i => i.DateCompleted).ToList();
            _receivingOrdersInPeriod = receivingOrdersInPeriod;
        }

        public override ReportTypes ReportType => ReportTypes.COGS;

		private string GetBevItemKey(IBaseBeverageLineItem li)
		{
			return li.DisplayName + "::" 
				+ li.UPC + "::" 
				+ li.Container != null ? 
					li.Container.DisplayName + ":" + li.Container.AmountContained.Milliliters 
					: "nocon";
		}

        protected override ReportResult CreateReport()
        {
            //get the first pair of inventories
            var currentStart = 0;
            var results = new List<Tuple<IBaseBeverageLineItem, Money>>();
            foreach (var inv in _inventories)
            {
                if (currentStart < _inventories.Count - 1)
                {
                    var startInv = _inventories[currentStart];
                    var endInv = _inventories[currentStart + 1];

                    var rosInOrder =
                        _receivingOrdersInPeriod.Where(
                            ro => ro.DateReceived >= startInv.DateCompleted && ro.DateReceived <= endInv.DateCompleted);

                    var amtUsedInPeriod = GetCostOfGoodsForPeriod(startInv, endInv, rosInOrder);
                    results.AddRange(amtUsedInPeriod);
                    currentStart++;
                }
            }

			//combine all items from periods
			var allItems = new Dictionary<string, Tuple<IBaseBeverageLineItem, Money>>();
			foreach (var res in results) {
				var key = GetBevItemKey (res.Item1);

				if (allItems.ContainsKey (key)) {
					var existing = allItems [key];
					existing = new Tuple<IBaseBeverageLineItem, Money> (existing.Item1, existing.Item2.Add (res.Item2));
				} else {
					allItems.Add(key, res);
				}
			}

            //translate each of the items
            var items =
				allItems.Values.Select(
                    r => new ReportResultLineItem(r.Item1.DisplayName, r.Item1.Container.DisplayName, r.Item2.Dollars, r.Item2.Dollars < 0));

			if (results.Any ()) {
				return new ReportResult (ReportTypes.COGS, "COGS report", items, results.Sum (r => r.Item2.Dollars));
			}
			return new ReportResult(ReportTypes.COGS, "COGS report", items, 0);
        }

        private class PriceAmount
        {
            public IBaseBeverageLineItem LineItem { get; set; }

            /// <summary>
            /// How many we bought at this price
            /// </summary>
            public decimal Amount { get; set; }
            /// <summary>
            /// The price we bought them at
            /// </summary>
            public Money Price { get; set; }
        }
        private IEnumerable<Tuple<IBaseBeverageLineItem, Money>> GetCostOfGoodsForPeriod(IInventory start,
            IInventory end, IEnumerable<IReceivingOrder> rosBetween)
        {
            var rosOrdered = rosBetween.OrderBy(r => r.DateReceived);
            var results = new List<Tuple<IBaseBeverageLineItem, Money>>();

            //amount used is times in each
            foreach (var endLI in end.GetBeverageLineItems())
            {
                var priceStack = new List<PriceAmount>();
                //get the start amount
                var startLI =
					start.GetBeverageLineItems().Where(li => BeverageLineItemEquator.AreSameBeverageLineItem(li, endLI)).ToList();


                Money firstROPrice = null;
                PriceAmount firstAmount = null;
				foreach (var ro in rosOrdered) {
					var roLIs =
						ro.GetBeverageLineItems ().Where (r => BeverageLineItemEquator.AreSameBeverageLineItem (r, endLI)).ToList ();
					if (roLIs.Any ()) {
						var priceAmount = new PriceAmount {
							LineItem = roLIs.First (),
							Price = roLIs.First ().UnitPrice,
							Amount = roLIs.Sum (r => r.Quantity)
						};

						if (firstROPrice == null) {
							firstROPrice = roLIs.First ().UnitPrice;
							firstAmount = new PriceAmount {
								LineItem = endLI,
								Price = firstROPrice,
								Amount = startLI.Any () ? startLI.Sum (li => li.Quantity) : 0
							};
							priceStack.Add (firstAmount);
						}

						priceStack.Add (priceAmount);
					}
				}

				if (priceStack.Any() == false && startLI.Any())
                {
					Money defaultPrice = null;
					var firstStartWithPrice = startLI.FirstOrDefault (l => l.PricePaid != null);
					if (firstStartWithPrice != null) {
						defaultPrice = firstStartWithPrice.PricePaid;
					} else {
						if (endLI != null) {
							defaultPrice = endLI.PricePaid;
						}
					}

                    //we didn't have any ROs, so we don't have a price, but we do have start items!
                    firstAmount = new PriceAmount
                    {
                        LineItem = endLI,
                        Price = defaultPrice,
                        Amount = startLI.Sum(li => li.Quantity)
                    };
                    priceStack.Add(firstAmount);
                }

                //get our amoutn used
				if (priceStack.Any ()) {
					var amountIn = priceStack.Sum (ps => ps.Amount);
					var amountAtEnd = endLI.Quantity;
					var amountUsed = amountIn - amountAtEnd;

					//ok, now get the price for each, starting at the begining
					var quantityLeft = amountUsed;
					var totalCost = Money.None;
					foreach (var ps in priceStack) {
						if (quantityLeft > 0) {
							if (quantityLeft > ps.Amount) {
								if (ps.Price != null) {
									totalCost = totalCost.Add (ps.Price.Multiply (ps.Amount));
									quantityLeft -= ps.Amount;
								}
							} else {
								//get just the remainder
								if (ps.Price != null) {
									totalCost = totalCost.Add (ps.Price.Multiply (quantityLeft));
									quantityLeft = 0;
								}
							}
						}
					}

					results.Add (new Tuple<IBaseBeverageLineItem, Money> (endLI, totalCost));
				}
                
            }

            return results;
        } 
    }
}
