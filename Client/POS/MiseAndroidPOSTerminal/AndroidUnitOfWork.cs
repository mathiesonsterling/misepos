using System;
using Mise.Core;
using Mise.Core.Services;
using Mise.Core.Repositories;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;
using System.Threading.Tasks;
using Mise.Core.Common.Repositories;
using Mise.Core.Client.Repositories;
using Java.Lang;


namespace MiseAndroidPOSTerminal
{
	public class AndroidUnitOfWork : IUnitOfWork
	{
		const int WAIT_FOR_REPOS_SLEEP_TIME = 100;

		ICheckRepository _terminalCheckRepository;
		public ICheckRepository CheckRepository{ 
			get{ return _terminalCheckRepository;} 
			set{ _terminalCheckRepository = value;}
		}
		public IEmployeeRepository EmployeeRepository{ get; set;}
		public ILogger Logger{get;set;}

		public bool Start (Guid terminalID, IEmployee employee, ICheck check)
		{
			return CheckRepository.StartTransaction (check.ID);
		}

		public Task Commit (Guid terminalID, IEmployee employee, ICheck check)
		{
			return Task.Factory.StartNew (() => {
				Task checkTask = null, empTask = null;
				if (check != null) {
					checkTask = CheckRepository.Commit (check.ID);
				}

				if (employee != null) {
					empTask = EmployeeRepository.Commit (employee.ID);
				}

				if (checkTask != null) {
					checkTask.Wait ();
				}
				if (empTask != null) {
					empTask.Wait ();
				}
			});
		}

		public Task Cancel (Guid terminalID, IEmployee employee, ICheck check)
		{
			return Task.Factory.StartNew (() => {
				_terminalCheckRepository.CancelTransaction (check.ID);
				EmployeeRepository.CancelTransaction (employee.ID);
			});
		}
						

		public void DoWhenRepositoryIsAvailabe (Guid terminalID, IEmployee employee, ICheck check, 
			Action<IEmployee, ICheck> action)
		{
			Task.Factory.StartNew (() => {
				//get the reposes
				var canStart = false;
				while(canStart == false){
					canStart = Start(terminalID, employee, check);
					if(canStart == false){
						Thread.Sleep(WAIT_FOR_REPOS_SLEEP_TIME);
					}
				}
					
				//we're now good to go
				if(action != null){
					action(employee, check);
				}

				Commit(terminalID, employee, check);
			});
		}
	}
}

