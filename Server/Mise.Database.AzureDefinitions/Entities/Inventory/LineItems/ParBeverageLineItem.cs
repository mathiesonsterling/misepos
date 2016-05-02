﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class ParBeverageLineItem : BaseLiquidLineItemEntity<IParBeverageLineItem, Core.Common.Entities.Inventory.ParBeverageLineItem>
    {
        protected override Core.Common.Entities.Inventory.ParBeverageLineItem CreateConcreteLineItemClass()
        {
            return new Core.Common.Entities.Inventory.ParBeverageLineItem();
        }
    }
}
