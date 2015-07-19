using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core
{
    /// <summary>
    /// Represents an item which be searched for.  
    /// </summary>
    public interface ITextSearchable
    {
        bool ContainsSearchString(string searchString);
    }
}
