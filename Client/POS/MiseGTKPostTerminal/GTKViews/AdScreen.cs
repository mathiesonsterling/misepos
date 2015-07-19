using System;
using Mise.Core.Client.ViewModel;

namespace MiseGTKPostTerminal.GTKViews
{
	public class AdScreen : IGTKTerminalView
	{
		public AdScreen ()
		{
		}

		public TerminalViewTypes Type { get { return TerminalViewTypes.OrderOnTab; } }
		public void LoadView()
		{
			//create the ad
		}

	}
}

