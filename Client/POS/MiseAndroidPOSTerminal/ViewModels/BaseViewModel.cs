using System;
using System.ComponentModel;

using Mise.Core.Client.ApplicationModel;
namespace MiseAndroidPOSTerminal.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		protected readonly ITerminalApplicationModel Model;

		protected BaseViewModel(ITerminalApplicationModel model)
		{
			Model = model;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this,
					new PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// Represents an event that is called by the view model
	/// </summary>
	public delegate void ViewModelEventHandler<T>(T calling) where T:BaseViewModel;

	/// <summary>
	/// Placeholder for ICommand till we move more stuff over
	/// </summary>
	public interface IMiseCommand<T>
	{
		void Execute(T param);
		bool CanExecute(T param);

	}

	public interface IMiseCommand{
		void Execute();
		bool CanExecute();
	}

	/// <summary>
	/// Placeholder for ICommand for now
	/// </summary>
	public class MiseCommand<T> : IMiseCommand<T>
	{
		readonly Func<T, bool> _canRun;
		readonly Action<T> _doCommand;

		public MiseCommand(Action<T> execute){
			_doCommand = execute;
		}

		public MiseCommand(Action<T> execute, Func<T, bool> canExecute){
			_canRun = canExecute;
			_doCommand = execute;
		}

		public void Execute(T param){
			_doCommand (param);
		}

		public bool CanExecute(T param){
			return _canRun == null || _canRun (param);

		}
			
	}

	public class MiseCommand : IMiseCommand {
		readonly Func<bool> _canRun;
		readonly Action _doCommand;

		public MiseCommand(Action execute){
			_doCommand = execute;
		}

		public MiseCommand(Action execute, Func<bool> canExecute){
			_canRun = canExecute;
			_doCommand = execute;
		}

		#region IMiseCommand implementation
		public void Execute ()
		{
			_doCommand ();
		}
		public bool CanExecute ()
		{
			return _canRun == null || _canRun ();

		}
		#endregion
	}
}

