using System;

using Xamarin.Forms;

namespace Mise.Inventory.CustomControls
{
	public class DragListView : ListView
	{
		public DragListView ()
		{
			
		}

		public delegate void ItemDraggedToNewPositionEventHandler(object sender, ItemChangedPositionEventArgs args);

		public event ItemDraggedToNewPositionEventHandler ItemDraggedToNewPosition;

		public void FireDragEnd(int oldIndex, int newIndex){
			ItemChangedPositionEventArgs args = new ItemChangedPositionEventArgs(oldIndex, newIndex);
			ItemDraggedToNewPosition (null, args);
		}
	}

	public class ItemChangedPositionEventArgs : EventArgs 
	{
		public ItemChangedPositionEventArgs(int oldIndex, int newIndex)
		{
			OldIndex = oldIndex;
			NewIndex = newIndex;
		}
		public int OldIndex { get; private set; }
		public int NewIndex { get; private set; }
	}
}


