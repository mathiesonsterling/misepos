using System;
using System.Threading;
using System.Threading.Tasks;

using Mise.Core.Repositories;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Services;

namespace Mise.Core.Client.UnitTests
{
	public class NewTestUnitOfWork : IUnitOfWork
	{
		const int WAIT_FOR_REPOS_SLEEP_TIME = 100;
		public ICheckRepository CheckRepository{ get; set;}
		public IEmployeeRepository EmployeeRepository{ get; set;}

		public bool Start (Guid terminalID, IEmployee employee, ICheck check)
		{
			var checkRes = true;
			var empRes = true;
			if (check != null) {
				checkRes = CheckRepository.StartTransaction (check.Id);
			}
			if(employee != null){
				empRes = EmployeeRepository.StartTransaction (employee.Id);
			}

			return checkRes && empRes;
		}

		public async Task Commit (Guid terminalID, IEmployee employee, ICheck check)
		{
			if (employee != null) {
				await EmployeeRepository.Commit (employee.Id).ConfigureAwait (false);
			}
			if (check != null) {
				await CheckRepository.Commit (check.Id).ConfigureAwait(false);
			}
		}

		public Task Cancel (Guid terminalID, IEmployee employee, ICheck check)
		{
			CheckRepository.CancelTransaction (check.Id);
			EmployeeRepository.CancelTransaction (employee.Id);

			return Task.FromResult (true);
		}

		public void DoWhenRepositoryIsAvailable (Guid terminalID, IEmployee employee, ICheck check, Action<IEmployee, ICheck> action)
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
			});
		}
	}
}

