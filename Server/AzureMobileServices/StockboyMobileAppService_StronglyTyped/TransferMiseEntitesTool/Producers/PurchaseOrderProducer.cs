namespace TransferMiseEntitesTool.Producers
{
    class PurchaseOrderProducer : BaseAzureEntitiesProducer
    {
        protected override string EntityTypeString => "Mise.Core.Common.Entities.Inventory.PurchaseOrder";
    }
}
