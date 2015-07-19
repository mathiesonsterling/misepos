using System;
using Android.Widget;
using Android.Content;

namespace MiseAndroidPOSTerminal.AndroidControls
{
	///TextView that also holds a hidden item
	class OrderItemLabel : Button
	{
		public OrderItemLabel(Android.Content.Context context) : base(context){
		}

		public Guid OrderItemID{ get; set; } 
	}
}

