using System;
using System.Collections.Generic;
using System.Text;
using Mise.Core.Entities;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Base;

namespace Mise.Core.Services
{
    public interface IMenuService
    {
        IMenuItem GetMenuItem(string menuItemID);

        IEnumerable<IMenuItem> GetHotMenuItems();

        IEnumerable<IMenuItem> GetAdminMenuItems();
    }
}
