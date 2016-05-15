using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.Categories;
using Mise.Database.AzureDefinitions.ValueItems.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public abstract class BaseLiquidLineItemEntity<TEntityType, TConcrete> : BaseDbEntity<TEntityType, TConcrete> 
        where TEntityType : IEntityBase , IBaseBeverageLineItem
        where TConcrete : BaseBeverageLineItem, TEntityType
    {
        protected BaseLiquidLineItemEntity()
        {
            Container = new LiquidContainer();
        }

        protected BaseLiquidLineItemEntity(TEntityType source, IEnumerable<Categories.InventoryCategory> categories) : base(source)
        {
            BaseLineItem = new BaseLineItem
            {
                DisplayName = source.DisplayName,
                CaseSize = source.CaseSize,
                MiseName = source.MiseName,
                Quantity = source.Quantity,
                UPC = source.UPC
            };

            Container = new LiquidContainer(source.Container);

            Categories = categories.Select(c => new EntityCategoryOwnership {EntityId = EntityId, InventoryCategory = c}).ToList();
        }  

        public BaseLineItem BaseLineItem { get; set;}

        /// <summary>
        /// Size of the container it comes in, which also has other information in it
        /// </summary>
        LiquidContainer Container { get; }

        public List<EntityCategoryOwnership> Categories { get; set; }

        protected override TConcrete CreateConcreteSubclass()
        {
            var concrete = CreateConcreteLineItemClass();

            concrete.Categories = Categories.Select(c => c.InventoryCategory.ToBusinessEntity()).ToList();
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
