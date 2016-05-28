using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.ValueItems.Inventory;

namespace Mise.Core.Client.Entities.Inventory.LineItems
{
    public abstract class BaseLiquidLineItemEntity<TEntityType, TConcrete> : BaseDbEntity<TEntityType, TConcrete> 
        where TEntityType : IEntityBase , IBaseBeverageLineItem
        where TConcrete : BaseBeverageLineItem, TEntityType
    {
        protected BaseLiquidLineItemEntity()
        {
            Container = new LiquidContainer();
        }

        protected BaseLiquidLineItemEntity(TEntityType source, IEnumerable<Categories.InventoryCategory> categories) 
            : base(source)
        {
            BaseLineItem = new BaseLineItem
            {
                DisplayName = source.DisplayName,
                CaseSize = source.CaseSize,
                MiseName = source.MiseName,
                Quantity = source.Quantity > 10000 
                    ? 10000 
                    : source.Quantity,
                UPC = source.UPC
            };

            Container = new LiquidContainer(source.Container);

            var thisItemCategoryIds = source.GetCategories().Select(c => c.Id).Distinct().ToList();
            Categories = categories
                .Where(c => thisItemCategoryIds.Contains(c.EntityId))
                .Select(c => new EntityCategoryOwnership(EntityId, c)).ToList();
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
