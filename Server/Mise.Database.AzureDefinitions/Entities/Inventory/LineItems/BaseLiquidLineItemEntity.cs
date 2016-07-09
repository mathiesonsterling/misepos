using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.Categories;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public abstract class BaseLiquidLineItemEntity<TEntityType, TConcrete> : BaseDbEntity<TEntityType, TConcrete> 
        where TEntityType : IEntityBase , IBaseBeverageLineItem
        where TConcrete : BaseBeverageLineItem, TEntityType
    {
        protected BaseLiquidLineItemEntity()
        {
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
            ContainerId = Container.Id;
            var thisItemCategoryIds = source.GetCategories().Select(c => c.Id).Distinct().ToList();
            Categories = categories
                .Where(c => thisItemCategoryIds.Contains(c.EntityId))
                .Select(c => new EntityCategoryOwnership(EntityId, c)).ToList();
        }  

        public BaseLineItem BaseLineItem { get; set;}

        /// <summary>
        /// Size of the container it comes in, which also has other information in it
        /// </summary>
        public LiquidContainer Container { get; set; }
        [ForeignKey("Container")]
        public string ContainerId { get; set; }

        public List<EntityCategoryOwnership> Categories { get; set; }

        protected override TConcrete CreateConcreteSubclass()
        {
            var concrete = CreateConcreteLineItemClass();

            concrete.Categories = Categories.Select(c => c.InventoryCategory.ToBusinessEntity()).ToList();

            if (Container != null)
            {
                var containerEnt = Container.ToBusinessEntity();
                concrete.Container = new Core.ValueItems.Inventory.LiquidContainer(containerEnt);
            }

            concrete.CaseSize = BaseLineItem.CaseSize;
            concrete.DisplayName = BaseLineItem.DisplayName;
            concrete.MiseName = BaseLineItem.MiseName;
            concrete.Quantity = BaseLineItem.Quantity;
            return concrete;
        }

        protected abstract TConcrete CreateConcreteLineItemClass();
    }
}
