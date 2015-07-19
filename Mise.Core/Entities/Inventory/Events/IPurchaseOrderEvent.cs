using System;

using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Inventory.Events
{
	/// <summary>
	/// Event which modifies a requisition / purchase order
	/// </summary>
	public interface IPurchaseOrderEvent : IEntityEventBase
	{
        Guid PurchaseOrderID { get; }
	}
}

