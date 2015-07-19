using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;

using Mise.Core.Services;
using Mise.Core.ValueItems;
using Xamarin.Forms;

namespace MisePOSTerminal.ViewModels
{
    /// <summary>
    /// View model 
    /// </summary>
	public class ViewChecksViewModel : BaseViewModel
    {
        public IEmployee SelectedEmployee
        {
            get { return Model.SelectedEmployee; }
        }

        public IEnumerable<IEmployee> CurrentEmployees
        {
            get { return Model.CurrentEmployees; }
        }

		public IEnumerable<ICheck> Checks{
			get{ return Model.OpenChecks; }
		}

        /// <summary>
        /// Event fired when we need to load employees from the view model again into the form
        /// </summary>
        public event ViewModelEventHandler OnLoadEmployees;
		public event ViewModelEventHandler OnLoadChecks;

		public void Reload(BaseViewModel requestingView){
			if (OnLoadChecks != null) {
				OnLoadChecks (requestingView);
			}
		}

        bool _commandsVisible;
        /// <summary>
        /// If our commands panel is visible
        /// </summary>
        /// <value><c>true</c> if commands visible; otherwise, <c>false</c>.</value>
        public bool CommandsVisible
        {
            get { return _commandsVisible; }
            protected set
            {
                _commandsVisible = value;
                OnPropertyChanged("CommandsVisible");
            }
        }

 
        bool _enteringClockinPasscode;
        bool _enteringNewCheck;
        bool _enteringNoSalePasscode;
        bool _enteringClockoutPasscode;



        public bool EnteringClockinPasscode
        {
            get { return _enteringClockinPasscode; }
            private set
            {
                _enteringClockinPasscode = value;
                OnPropertyChanged("EnteringClockinPasscode");
            }
        }

        public bool EnteringNewCheck
        {
			get { return _enteringNewCheck; }
            private set
            {
                _enteringNewCheck = value;
                OnPropertyChanged("EnteringNewCheck");
            }
        }

        public bool EnteringNoSalePasscode
        {
            get { return _enteringNoSalePasscode; }
            private set
            {
                _enteringNoSalePasscode = value;
                OnPropertyChanged("EnteringNoSalePasscode");
            }
        }

        public bool EnteringClockoutPasscode
        {
            get { return _enteringClockoutPasscode; }
            private set
            {
                _enteringClockoutPasscode = value;
                OnPropertyChanged("EnteringClockoutPasscode");
            }
        }

        string _passcode;
        public string Passcode
        {
            get { return _passcode; }
            set
            {
                _passcode = value;
                OnPropertyChanged("Passcode");
            }
        }

        string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                _customerName = value;
                OnPropertyChanged("CustomerName");
            }
        }

        public ICommand ClockinStart { get; private set; }
        void DoClockinStart()
        {
            EnteringClockinPasscode = true;
            EnteringText = true;
            CommandsVisible = false;
        }

        public ICommand ClockinDone { get; private set; }
        void DoClockinDone(string passcode)
        {
            var res = Model.EmployeeClockin(passcode);
            if (res == false) return;
            //good, let's load the employees and move the view!
            SetToNormalView();
			((Command)CheckClicked).ChangeCanExecute();
            if (OnLoadEmployees != null)
            {
                OnLoadEmployees(this);
            }
        }

        public ICommand CancelEntry { get; private set; }

        public ICommand ClockoutStart { get; private set; }
        void DoClockoutStart()
        {
            EnteringClockoutPasscode = true;
            EnteringText = true;
            CommandsVisible = false;
        }
        public ICommand ClockoutDone { get; private set; }
        void DoClockoutDone(string passcode)
        {
            var res = Model.EmployeeClockout(passcode, Model.SelectedEmployee);
            if (res)
            {
                Passcode = string.Empty;
                SetToNormalView();
				((Command)CheckClicked).ChangeCanExecute();
                if (OnLoadEmployees != null)
                {
                    OnLoadEmployees(this);
                }
            }
        }

        public ICommand NewCheckStart { get; private set; }
        void DoNewCheckStart()
        {
            EnteringNewCheck = true;
            EnteringText = true;
            CommandsVisible = false;
        }

        public ICommand NewCheckDone { get; private set; }
        void DoNewCheckDone(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var res = Model.CreateNewCheck(name);

            //move to the next panel!
            if (res != null)
            {
                //cleanup before we move
                SetToNormalView();

                //todo change this to include the order or id?
				MoveToView (TerminalViewTypes.OrderOnCheck);
            }
        }

        public ICommand NoSaleStart { get; private set; }
        void DoNoSaleStart()
        {
            EnteringNoSalePasscode = true;
            EnteringText = true;
            CommandsVisible = false;
        }
        public ICommand NoSaleDone { get; private set; }
        void DoNoSaleDone(string passcode)
        {
            var res = Model.NoSale(passcode);
            if (res)
            {
				MoveToView (TerminalViewTypes.NoSale);
                //fire no sale screen
            }
            else
            {
                Passcode = string.Empty;
            }
        }

		public ICommand ClosedChecks{ get; private set;}

		public ICommand FastCash{get;private set;}

        /// <summary>
        /// Sets the values to react to when an employee is selected
        /// </summary>
        public ICommand EmployeeSelected { get; private set; }
        public ICommand EmployeeDeselected { get; private set; }

		public ICommand CheckClicked{get;private set;}
		void DoCheckClicked(ICheck check){
			Model.CheckClicked (check);
			MoveToView (TerminalViewTypes.OrderOnCheck);
		}

        void UpdateCommandsForEmployeeStatus(IEmployee emp)
        {
            Model.SelectedEmployee = emp;
            ((Command)NewCheckStart).ChangeCanExecute();
            ((Command)NoSaleStart).ChangeCanExecute();
            ((Command)ClockoutStart).ChangeCanExecute();
			((Command)FastCash).ChangeCanExecute ();
			((Command)ClosedChecks).ChangeCanExecute ();
        }


        public ViewChecksViewModel(ILogger logger, ITerminalApplicationModel model) : base(logger, model)
        {
            EnteringText = false;

            //setup our commands
			CheckClicked = new Command<ICheck> (DoCheckClicked, (check) => Model.LastSelectedEmployee != null);

            EmployeeSelected = new Command<IEmployee>(UpdateCommandsForEmployeeStatus);
            EmployeeDeselected = new Command<IEmployee>(UpdateCommandsForEmployeeStatus);

            CancelEntry = new Command(SetToNormalView);

            ClockinStart = new Command(DoClockinStart);
            ClockinDone = new Command<string>(DoClockinDone,
                passcode => string.IsNullOrEmpty(passcode) == false);


            ClockoutStart = new Command(DoClockoutStart, () => Model.SelectedEmployee != null);
            ClockoutDone = new Command<string>(
                DoClockoutDone,
                passcode => string.IsNullOrEmpty(passcode) == false && Model.CurrentEmployees.Any() && Model.SelectedEmployee != null
            );

            NewCheckStart = new Command(DoNewCheckStart, () => Model.SelectedEmployee != null);
            NewCheckDone = new Command<string>(DoNewCheckDone, customerName => string.IsNullOrEmpty(customerName) == false);

            NoSaleStart = new Command(DoNoSaleStart, () => Model.SelectedEmployee != null);
            NoSaleDone = new Command<string>(DoNoSaleDone, passcode => string.IsNullOrEmpty(passcode) == false);

			ClosedChecks = new Command (
				() => MoveToView (TerminalViewTypes.ClosedChecks),
				() => Model.ClosedChecks.Any ()
			);

			FastCash = new Command (
				() => {},
				() => Model.SelectedEmployee != null
			);
        }

        /// <summary>
        /// Sets our normal view back
        /// </summary>
        void SetToNormalView()
        {
            Passcode = string.Empty;
            CustomerName = string.Empty;

            EnteringClockinPasscode = false;
            EnteringClockoutPasscode = false;
            EnteringNoSalePasscode = false;
            EnteringNewCheck = false;
            EnteringText = false;
            CommandsVisible = true;
        }

		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped (CreditCard card)
		{
			var startingView = Model.CurrentTerminalViewTypeToDisplay;
			if (card != null) {
				//see if we already have this card attached to a check
			    var foundCheck = Model.OpenChecks
			        .Where(c => c.CreditCards != null && c.CreditCards.Any())
			        .FirstOrDefault(c => c.CreditCards.Select(cc => cc.CardNumber).Contains(card.CardNumber));

			    if (foundCheck != null)
			    {
			        Model.CheckClicked(foundCheck);
			    }
			    else
			    {
			        Model.CreateNewCheck(card);
			    }
			}
			if(startingView != Model.CurrentTerminalViewTypeToDisplay){
				MoveToView (Model.CurrentTerminalViewTypeToDisplay);
			}
		}

		#endregion
    }
}
