using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Client.ViewModel;

namespace MiseGTKPostTerminal.GTKViews
{
    /// <summary>
    /// Callback so our views can tell the application to update
    /// </summary>
    public delegate void UpdateViewCallback();

    /// <summary>
    /// Basic class that allows us to separate out the functions to construct each 'view' like a WPF page or android intent
    /// </summary>
    public interface IGTKTerminalView
    {
        TerminalViewTypes Type { get; }
        void LoadView();
    }
}
