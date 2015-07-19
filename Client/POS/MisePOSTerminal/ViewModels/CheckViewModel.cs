using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Core.Entities.Check;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MisePOSTerminal.ViewModels
{
	public class CheckViewModel : BaseViewModel
	{
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped(CreditCard card)
		{

		}

		#endregion

		private string _total = "";

		public string Total {
			get { return _total; }
			set {
				if (_total == value) {
					return;
				}

				_total = value;

				OnPropertyChanged("Total");
			}
		}

		/// <summary>
		/// Gets the order items of the currently selected check.
		/// </summary>
		/// <value>The order items.</value>
		public IEnumerable<OrderItem> OrderItems {
			get { return Model.SelectedCheck.OrderItems; }
		}

		public CheckViewModel(ILogger logger, ITerminalApplicationModel model) : base(logger, model)
		{
		}
	}
}

