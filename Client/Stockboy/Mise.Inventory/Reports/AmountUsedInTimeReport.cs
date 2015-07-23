using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _startingInventory = startingInventory;
            _receivingOrdersInTime = receivingOrdersInTime;
            _inventoriesInTime = inventoriesInTime;
            _start = start;
            _end = end;
        }

        public override ReportTypes ReportType
        {
            get { return ReportTypes.AmountUsed; }
        }

        public override ReportResult RunReport()
        {
            var amountsUsed = new Dictionary<string, LiquidAmount>();

            //get our baseline date
            //get our baseline.  If the first inventory is null, then all our amounts are zero
           
            //get each inventory, and the ROs between them.  Then compare this to the next inventory
             
            throw new NotImplementedException();
        }
    }
}
