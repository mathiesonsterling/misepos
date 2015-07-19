using System.Collections.Generic;
using Mise.Core.Entities.Menu;

namespace Mise.Core.Repositories
{
    public interface IMenuRulesRepository
    {
        IEnumerable<MenuRule> GetAll();
    }
}
