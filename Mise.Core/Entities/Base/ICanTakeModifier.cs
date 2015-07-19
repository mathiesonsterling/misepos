using System.Collections.Generic;
using Mise.Core.Entities.Menu;

namespace Mise.Core.Entities.Base
{
    /// <summary>
    /// Represents an item which can have an IMenuItemModifier applied to it
    /// </summary>
    public interface ICanTakeModifier
    {
        /// <summary>
        /// Represents the modifiers that have been applied to the item
        /// </summary>
        IEnumerable<MenuItemModifier> Modifiers { get; }

		void AddModifier(MenuItemModifier mod);
		void RemoveModifier(MenuItemModifier mod);

        /// <summary>
        /// Name that the item displays on screen
        /// </summary>
        string Name { get; }

    }
}
