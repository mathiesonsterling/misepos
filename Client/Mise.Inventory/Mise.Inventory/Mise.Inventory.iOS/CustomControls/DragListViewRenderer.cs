using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Mise.Inventory.CustomControls;
using Mise.Inventory.ViewModels;
using Mise.Inventory.iOS.CustomControls;
using UIKit;
using Foundation;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(DragListView), typeof(DragListViewRenderer))]

namespace Mise.Inventory.iOS.CustomControls
{
	public class DragListViewRenderer : ListViewRenderer
	{
		private class ReorderableTableViewSource : UITableViewSource
		{
			public WeakReference<ListView> View { get; set; }

			public UITableViewSource Source { get; set; }

			#region A replacement UITableViewSource which enables drag and drop to reorder rows

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {
				return Source.GetCell(tableView, indexPath);
			}

			public override nfloat GetHeightForHeader(UITableView tableView, nint section) {
				return Source.GetHeightForHeader(tableView, section);
			}

			public override UIView GetViewForHeader(UITableView tableView, nint section) {
				return Source.GetViewForHeader(tableView, section);
			}

			public override nint NumberOfSections(UITableView tableView) {
				return Source.NumberOfSections(tableView);
			}

			public override void RowDeselected(UITableView tableView, NSIndexPath indexPath) {
				Source.RowDeselected(tableView, indexPath);
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath) {
				Source.RowSelected(tableView, indexPath);
			}

			public override nint RowsInSection(UITableView tableview, nint section) {
				return Source.RowsInSection(tableview, section);
			}

			public override string[] SectionIndexTitles(UITableView tableView) {
				return Source.SectionIndexTitles(tableView);
			}

			public override string TitleForHeader(UITableView tableView, nint section) {
				return Source.TitleForHeader(tableView, section);
			}

			public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
			{
				return true;
			}

			#endregion

			public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath) {
				// Don't call the base method, which is the key: the same method in
				// ListViewRenderer.ListViewDataSource throws which prevents the rows to become moveable

				ListView listView;
				if (!View.TryGetTarget (out listView)) {
					return;
				}

				//TODO get a generic base type for this
				var sources = (List<InventoryViewModel.InventoryLineItemDisplayLine>)listView.ItemsSource;
				var item = sources [sourceIndexPath.Row];
				var deleteAt = sourceIndexPath.Row;
				var insertAt = destinationIndexPath.Row;

				if (destinationIndexPath.Row < sourceIndexPath.Row) {
					deleteAt += 1;
				} else {
					insertAt += 1;
				}

				sources.Insert (insertAt, item);
				sources.RemoveAt (deleteAt);

				listView.ItemsSource = new List<InventoryViewModel.InventoryLineItemDisplayLine>();
				listView.ItemsSource = sources;

				var lv = listView as DragListView;
				if(lv == null){
					return;
				}
				lv.FireDragEnd(sourceIndexPath.Row, destinationIndexPath.Row);
			}
		}

		private new DragListView Element => base.Element as DragListView;

		protected override void OnElementChanged(ElementChangedEventArgs<ListView> args) {
			
			base.OnElementChanged(args);
			if (args.OldElement == null) {
				var listView = (DragListView)args.NewElement;
				// Make row reorderable
				Control.Source = new ReorderableTableViewSource { View = new WeakReference<ListView>(Element), Source = Control.Source };

			}
		}
	}
}

