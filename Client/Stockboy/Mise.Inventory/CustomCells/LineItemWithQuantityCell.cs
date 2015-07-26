using System;
using Xamarin.Forms;

namespace Mise.Inventory.CustomCells
{
	public class LineItemWithQuantityCell : ViewCell
	{
		public Guid? LineItemID{ get{
				if(IDLabel == null){
					return null;
				}

				var text = IDLabel.Text;
				return Guid.Parse (text);
			}}

		private readonly Label IDLabel;
		public LineItemWithQuantityCell(){
			var mainGrid = new Grid {
				Padding = new Thickness(5, 10, 0, 0),
				RowSpacing = 5,
				VerticalOptions = LayoutOptions.FillAndExpand,
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition{ Width = new GridLength(50, GridUnitType.Absolute)}, 
				},
				RowDefinitions = {
					new RowDefinition (),
					new RowDefinition ()
				}
			};

			var nameLabel = new Label{LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.FillAndExpand};
			nameLabel.SetBinding (Label.TextProperty, "DisplayName");
         
			nameLabel.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
			nameLabel.FontAttributes = FontAttributes.Bold;
			nameLabel.HorizontalOptions = LayoutOptions.FillAndExpand;
            nameLabel.SetBinding(Label.TextColorProperty, "TextColor");

			var detail = new Label();
			detail.SetBinding (Label.TextProperty, "DetailDisplay");
			detail.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
			detail.HorizontalOptions = LayoutOptions.FillAndExpand;
            detail.SetBinding(Label.TextColorProperty, "TextColor");

			var quantity = new Label();
			quantity.SetBinding (Label.TextProperty, "Quantity");
			quantity.HorizontalOptions = LayoutOptions.FillAndExpand;
			quantity.FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label));


			mainGrid.Children.Add (nameLabel, 0, 0);
			mainGrid.Children.Add (detail, 0, 1);
			mainGrid.Children.Add (quantity, 1, 0);

			IDLabel = new Label ();
			IDLabel.SetBinding (Label.TextProperty, "ID");

			this.View = mainGrid;
		}
	}
}

