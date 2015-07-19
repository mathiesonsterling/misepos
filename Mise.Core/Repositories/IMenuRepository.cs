using System.Collections.Generic;
using Mise.Core.Entities.Menu;

namespace Mise.Core.Repositories
{
    public interface IMenuRepository
    {
		/// <summary>
		/// Gets the menu that should be currently active, under our rules
		/// </summary>
		/// <returns>The current menu.</returns>
		Menu GetCurrentMenu ();

        IEnumerable<Menu> GetAll();
    }
}
