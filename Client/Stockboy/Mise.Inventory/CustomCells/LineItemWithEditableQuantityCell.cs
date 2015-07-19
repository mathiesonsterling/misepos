using System;

using Xamarin.Forms;
namespace Mise.Inventory
{
	public class LineItemWithEditableQuantityCell : ViewCell
	{
		public Guid? LineItemID{ get{
				if(IDLabel == null){
					return null;
				}

				var text = IDLabel.Text;
				return Guid.Parse (text);
			}}

		private readonly Label IDLabel;

		public event EventHandler QuantityUpdated;
		public LineItemWithEditableQuantityCell ()
		{
			var mainGrid = new Grid {
				Padding = new Thickness(5, 10, 0, 0),
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) },
					new ColumnDefinition{ Width = new GridLength(10, GridUnitType.Star)}, 
				},
			};

			var nameLabel = new Label();
			nameLabel.SetBinding (Label.TextProperty, "DisplayName");

			var containerPicker = new Label();
			containerPicker.SetBinding (Label.TextProperty, "ContainerDisplayName");

			var quantStepper = new Entry ();
			quantStepper.Keyboard = Keyboard.Numeric;
			quantStepper.SetBinding (Entry.TextProperty, "Quantity");
			quantStepper.TextChanged += (sender, e) => {
				if(QuantityUpdated != null){
					QuantityUpdated.Invoke (this, e);
				}
			};

			mainGrid.Children.Add (nameLabel, 0, 0);
			mainGrid.Children.Add (containerPicker, 1, 0);
			mainGrid.Children.Add (quantStepper, 2, 0);

			IDLabel = new Label ();
			IDLabel.SetBinding (Label.TextProperty, "ID");

			this.View = mainGrid;
		}
	}
}

