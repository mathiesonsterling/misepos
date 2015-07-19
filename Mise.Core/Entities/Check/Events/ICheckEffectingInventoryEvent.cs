namespace Mise.Core.Entities.Check.Events
{
	/// <summary>
	/// Labels that this event should be considered by the inventory systems as well
	/// </summary>
	public interface ICheckEffectingInventoryEvent : ICheckEvent
	{
	}
}

