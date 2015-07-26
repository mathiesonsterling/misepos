using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mise.Inventory.MVVM
{
	public class SimpleCommand : ICommand
	{
		readonly Func<bool> _canExecute;
		readonly Action _execute;
			
		public SimpleCommand(Action execute, Func<bool> canExecute)
		{
			_execute = execute ?? new Action(() => {
			});
			_canExecute = canExecute ?? new Func<bool>(() => true);
		}


		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute();
		}

		public void Execute(object parameter)
		{
			_execute();
		}

		public void RaiseCanExecuteChanged()
		{
			var cec = CanExecuteChanged;
			if (null != cec) {
				cec(this, EventArgs.Empty);
			}
		}
	}

	public class SimpleCommand<T> : ICommand
	{
		readonly Func<T, bool> _canExecute;
		readonly Action<T> _execute;

		public SimpleCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		public SimpleCommand(Action<T> execute, Func<T, bool> canExecute)
		{
			_execute = execute ?? new Action<T>(p => {
			});
			_canExecute = canExecute ?? new Func<T, bool>(p => true);
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute((T)parameter);
		}

		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			var cec = CanExecuteChanged;
			if (null != cec) {
				cec(this, EventArgs.Empty);
			}
		}
	}
}