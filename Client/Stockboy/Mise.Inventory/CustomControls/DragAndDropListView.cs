using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace Mise.Inventory.CustomControls
{
    public class DragAndDropListView : ListView
    {
        public class ItemChangedPositionEventArgs : EventArgs 
        {
            public ItemChangedPositionEventArgs(Guid itemId, int oldIndex, int newIndex)
            {
                ItemId = itemId;
                OldIndex = oldIndex;
                NewIndex = newIndex;
            }

            public Guid ItemId { get; private set; }
            public int OldIndex { get; private set; }
            public int NewIndex { get; private set; }
        }

        public delegate void ItemDraggedToNewPositionEventHandler(object sender, ItemChangedPositionEventArgs args);

        public event ItemDraggedToNewPositionEventHandler ItemDraggedToNewPosition;

        public class SwipedLeftOnItemEventArgs : EventArgs
        {
            public SwipedLeftOnItemEventArgs(Guid itemId)
            {
                ItemId = itemId;
            }

            public Guid ItemId { get; private set; }
        }

        public delegate void SwipedLeftOnItemEventHandler(object sender, SwipedLeftOnItemEventArgs args);

        public event SwipedLeftOnItemEventHandler SwipedLeftOnItem;
    }
}
