using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Client.ValueItems
{
    public interface IDbValueItem<out TValueItem>
    {
        TValueItem ToValueItem();
    }
}
