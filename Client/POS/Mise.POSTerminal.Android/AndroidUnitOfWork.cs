using System;
using System.Threading.Tasks;

using Java.Lang;

using Mise.Core;
using Mise.Core.Client.Repositories;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.Services;

namespace Mise.POSTerminal.Android
{
	public class AndroidUnitOfWork : IUnitOfWork
	{
		const int WAIT_FOR_REPOS_SLEEP_TIME = 100;

		ITerminalCheckRepository _terminalCheckRepository;

		public ICheckRepository CheckRepository { 
			get{ return _terminalCheckRepository; } 
			set{ _terminalCheckRepository = (ITerminalCheckRepository)value; }
		}

		public IEmployeeRepository EmployeeRepository{ get; set; }

		public ILogger Logger{ get; set; }

		bool _currentlyCommitting;
		Task _commitTask;

		public bool Start(Guid terminalID, IEmployee employee, ICheck check)
		{
			if (_currentlyCommitting) {
				Logger.Log("Waiting for last commit to finish . . .");
				_commitTask.Wait();
				_currentlyCommitting = false;
			}
			return CheckRepository.StartTransaction(check.ID);
		}

		public void Commit(Guid terminalID, IEmployee employee, ICheck check)
		{
			if (check != null) {
				CheckRepository.Commit(check.ID);
			}

			if (employee != null) {
				EmployeeRepository.Commit(employee.ID);
			}
		}

		public void StartCommit(Guid terminalID, IEmployee employee, ICheck check)
		{
			_terminalCheckRepository.CommitBegin(check.ID);
			_currentlyCommitting = true;

		}

		public Task FinishCommit(Guid terminalID, IEmployee employee, ICheck check)
		{
			_commitTask = _terminalCheckRepository.CommitFinish(check.ID);

			return Task.Factory.StartNew(() => {
				_commitTask.Wait();
				EmployeeRepository.Commit(employee.ID);
				_currentlyCommitting = false;
			});
		}

		public void Cancel(Guid terminalID, IEmployee employee, ICheck check)
		{
			_terminalCheckRepository.CancelTransaction(check.ID);
			EmployeeRepository.CancelTransaction(employee.ID);
		}


		public void DoWhenRepositoryIsAvailabe(Guid terminalID, IEmployee employee, ICheck check, 
		                                       Action<IEmployee, ICheck> action)
		{
			Task.Factory.StartNew(() => {
				//get the reposes
				var canStart = false;
				while (canStart == false) {
					canStart = Start(terminalID, employee, check);
					if (canStart == false) {
						Thread.Sleep(WAIT_FOR_REPOS_SLEEP_TIME);
					}
				}

				//we're now good to go
				if (action != null) {
					action(employee, check);
				}

				Commit(terminalID, employee, check);
			});
		}
	}
}

