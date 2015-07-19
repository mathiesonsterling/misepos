using System;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Inventory.Events
{
	/// <summary>
	/// Base event to alter an inventory
	/// </summary>
	public interface IInventoryEvent : IEntityEventBase
	{
        Guid InventoryID { get; }
	}
}

