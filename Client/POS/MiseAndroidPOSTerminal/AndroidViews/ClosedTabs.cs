﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mise.Core.Client.ApplicationModel;
namespace MiseAndroidPOSTerminal.AndroidViews
{
	[Activity (Label = "ClosedTabs")]			
	public class ClosedTabs : BaseXamarinFormsHostPage
	{
		protected override TerminalViewTypes Type {
			get {
				return TerminalViewTypes.ClosedChecks;
			}
		}
	}
}

