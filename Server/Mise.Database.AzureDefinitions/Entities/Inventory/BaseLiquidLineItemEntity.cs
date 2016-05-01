using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.ValueItems.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public abstract class BaseLiquidLineItemEntity<TEntityType, TConcrete> : BaseDbEntity<TEntityType, TConcrete> 
        where TEntityType : IEntityBase 
        where TConcrete : BaseBeverageLineItem, TEntityType
    {
        public BaseLineItem BaseLineItem { get; set;}

        /// <summary>
        /// Size of the container it comes in, which also has other information in it
        /// </summary>
        LiquidContainer Container { get; set; }

        public List<Categories.InventoryCategory> Categories { get; set; }

        protected override TConcrete CreateConcreteSubclass()
        {
            var concrete = CreateConcreteLineItemClass();

            concrete.Categories = Categories.Select(c => c.ToBusinessEntity()).ToList();
            concrete.Container = Container.ToValueItem();

            concrete.CaseSize = BaseLineItem.CaseSize;
            concrete.DisplayName = BaseLineItem.DisplayName;
            concrete.MiseName = BaseLineItem.MiseName;
            concrete.Quantity = BaseLineItem.Quantity;
            return concrete;
        }

        protected abstract TConcrete CreateConcreteLineItemClass();
    }
}
