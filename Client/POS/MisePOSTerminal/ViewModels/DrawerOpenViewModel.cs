using System.Windows.Input;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Services;
namespace MisePOSTerminal.ViewModels
{
	public class DrawerOpenViewModel : BaseViewModel
	{
		private bool _isNoSale;
		/// <summary>
		/// If we're opening the drawer for a no sale
		/// </summary>
		/// <value><c>true</c> if this instance is no sale; otherwise, <c>false</c>.</value>
		public bool IsNoSale {
			get{ return _isNoSale; }
			set {
				_isNoSale = value;
				OnPropertyChanged ("IsNoSale");
			}
		}

	    private bool _isFastCash;
	    public bool IsFastCash
	    {
            get { return _isFastCash; }
	        set
	        {
	            _isFastCash = value;
	            OnPropertyChanged("IsFastCash");
	        }
	    }

	    public decimal ChangeDue{
			get{
				return Model.SelectedCheck.ChangeDue.Dollars;
			}
		}

	    public ICommand UndoFastCash;

		public DrawerOpenViewModel (ILogger logger, ITerminalApplicationModel model) : base(logger, model)
		{
			IsNoSale = Model.CurrentTerminalViewTypeToDisplay == TerminalViewTypes.NoSale;
            

		}

		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped (Mise.Core.ValueItems.CreditCard card)
		{
			//TODO expected behavior is to close this screen, or open a new tab?
			throw new System.NotImplementedException ();
		}

		#endregion
	}
}

