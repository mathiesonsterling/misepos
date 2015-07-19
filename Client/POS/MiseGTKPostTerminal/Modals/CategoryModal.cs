using System;
using Mise.Core.Entities.Menu;
using WindowType = Gtk.WindowType;

using Mise.Core.Client.ViewModel;
namespace MiseGTKPostTerminal
{
	public class CategoryModal : Gtk.Window
	{

		private readonly ITerminalApplicationModel _viewModel;
		private readonly DefaultTheme _theme;
		public CategoryModal(ITerminalApplicationModel viewModel, DefaultTheme theme) : base(WindowType.Toplevel)
		{
			_viewModel = viewModel;
			_theme = theme;

			//set height and width here?
			base.WidthRequest = 300;
			base.HeightRequest = 300;
			ModifyBg (Gtk.StateType.Normal, _theme.WindowBackground);

			//set our window border
			BorderWidth = 10;

			//populate controls based upon the category we're on
		}

		private void LoadSubCategories()
		{
			var subCats = _viewModel.SelectedCategory.SubCategories;

			foreach (var cat in subCats) {

			}
		}
		/// <summary>
		/// The item that was selected
		/// </summary>
		/// <value>The selected item.</value>
		public IMenuItem SelectedItem{ get; private set; }
	}
}

