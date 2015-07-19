using System.ComponentModel;
using Mise.Core.Client.Services;
using Mise.Core.Services;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.ValueItems;
using Xamarin.Forms;


namespace MisePOSTerminal.ViewModels
{
    /// <summary>
    /// Base class for any MiseViewModel.  Gets our model to use, and sets up property notifications for binding
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
		ICreditCardReaderService _creditCardReader;

		/// <summary>
		/// When we're given swiped chars, we need to deal with them
		/// </summary>
		/// <param name="c">C.</param>
		public virtual void AddChars(char c){
			if (_creditCardReader != null) {
				_creditCardReader.AddChars (c);
			}
		}

		/// <summary>
		/// If the OS has given a credit card, take it in and pass it to the processing service
		/// </summary>
		/// <param name="cc">Cc.</param>
		public virtual void AcceptCreditCard(CreditCard cc){
			if(_creditCardReader != null){
				_creditCardReader.AddCard (cc);
			}
		}

		public Command StartCreditCardReader;
		/// <summary>
		/// If true, the reader is currently able to accept a card
		/// </summary>
		bool _creditCardReading;
		public bool CreditCardReaderReading{ get{ return _creditCardReading; }
			private set{ _creditCardReading = value;
				OnPropertyChanged ("CreditCardReading");
			}
		}

        protected readonly ITerminalApplicationModel Model;
		protected readonly ILogger Logger;
		protected BaseViewModel(ILogger logger, ITerminalApplicationModel model)
        {
			Logger = logger;
            Model = model;

			if (model.CreditCardReaderService != null) {
				_creditCardReader = model.CreditCardReaderService;
				_creditCardReader.CreditCardSwiped += CreditCardSwiped;
			}

			StartCreditCardReader = new Command(
				()=>{
						Logger.Log ("Starting credit card reader", LogLevel.Debug);
						_creditCardReader.StartRead ();
					    CreditCardReaderReading = true;
				},
				() => _creditCardReader != null && _creditCardReader.Enabled && Model.SelectedEmployee != null
			);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
        }

		public abstract void CreditCardSwiped(CreditCard card);

		#region ViewSwitching
		TerminalViewTypes _destinationView;

		/// <summary>
		/// Stores whichever our last view was
		/// </summary>
		/// <value>The previous view.</value>
		public static TerminalViewTypes PreviousView{ get; protected set; }

		public TerminalViewTypes DestinationView{ 
			get{ return _destinationView; }
			protected set{ 
				PreviousView = _destinationView;
				_destinationView = value;
				OnPropertyChanged ("DestinationView");
			} 
		}

		/// <summary>
		/// Basic event for being able to move from one view to another
		/// </summary>
		public event ViewModelEventHandler OnMoveToView;

		/// <summary>
		/// Function to trigger moving to view event.  Moves the application to the view specified
		/// </summary>
		/// <param name="destination">Destination.</param>
		protected void MoveToView(TerminalViewTypes destination){
			DestinationView = destination;
			if (OnMoveToView != null) {
				OnMoveToView (this);
			}
		}
		#endregion

		bool _enteringText;

		public bool EnteringText{
			get{return _enteringText;}
			set{
				_enteringText = value;
				OnPropertyChanged("EnteringText");
			}
		}
    }
		
	/// <summary>
	/// Represents an event that is called by the view model
	/// </summary>
	public delegate void ViewModelEventHandler(BaseViewModel calling);

	public delegate void KeyboardToggled(BaseViewModel calling, bool keyboardVisible);

}
