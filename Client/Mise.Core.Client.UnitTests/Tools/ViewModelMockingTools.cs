using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Client.Repositories;
using Mise.Core.Entities.People.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Client.ApplicationModel.Implementation;
using Moq;
using Mise.Core.Entities.People;
using Mise.Core.Services.WebServices;
using Mise.Core.Entities;
using Mise.Core.Client.Services;

namespace Mise.Core.Client.UnitTests.Tools
{
	public static class ViewModelMockingTools
	{
		public static TerminalApplicationModel CreateViewModel(){
			ICheckRepository cr;
			IEmployeeRepository er;
			return CreateViewModel (out cr, out er);
		}

		public static TerminalApplicationModel CreateViewModel(out ICheckRepository checkRepos, out IEmployeeRepository empRepos)
		{
			return CreateViewModel (null, out checkRepos, out empRepos);
		}

		public static TerminalApplicationModel CreateViewModel(IEmployee emp){
			ICheckRepository checkRepos;
			return CreateViewModel(emp, out checkRepos);
		}

		public static TerminalApplicationModel CreateViewModel(IEmployee employee, 
			out ICheckRepository checkRepos){
			IEmployeeRepository employeeRepos;
			return CreateViewModel(employee, out checkRepos, out employeeRepos);
		}
			
		public static TerminalApplicationModel CreateViewModel(IEmployee employee,
			out ICheckRepository checkRepos, out IEmployeeRepository empRepos)
		{
			return CreateViewModel (employee, out checkRepos, out empRepos, null);
		}

		public static TerminalApplicationModel CreateViewModel(IEmployee emp, 
			out ICheckRepository checkRepos,
			out IEmployeeRepository empRepos,
			ICreditCardProcessorService processorService){
			var service = MockingTools.GetTerminalServiceWithMenu ();
			service.Setup (s => s.GetEmployeesAsync ()).Returns (Task.Factory.StartNew (() => new List<IEmployee>{emp}.AsEnumerable ()));
		    service.Setup(s => s.GetEmployeesAsync())
		        .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new[] {emp}));
		    service.Setup(s => s.SendEventsAsync(It.IsAny<IEmployee> (), It.IsAny<IEnumerable<IEmployeeEvent>>()))
				.Returns(Task.Factory.StartNew (() => true));

			var settings = service.Object.RegisterClientAsync (string.Empty).Result;
			var dal = MockingTools.GetClientDAL ();
			dal.Setup (d => d.GetEntitiesAsync<IMiseTerminalDevice> ())
                .Returns(
                    Task.Factory.StartNew(() =>
                        new []{settings.Item2}.AsEnumerable()
                     )
                );

			var logger = new Mock<ILogger>();
			var cashDrawerService = new Mock<ICashDrawerService>();
			var creditHardwareService = new Mock<ICreditCardReaderService>();
			var checkReposC = new ClientCheckRepository(service.Object, dal.Object, logger.Object);
			checkReposC.Load (MockingTools.RestaurantID);
			checkRepos = checkReposC;

			var empReposC = new ClientEmployeeRepository(service.Object, dal.Object, logger.Object);
			empReposC.Load (MockingTools.RestaurantID);
			empRepos = empReposC;

			var menuRepos = new Mock<IMenuRepository> ();
		    var fakeService = new FakeDomineesRestaurantServiceClient();
		    menuRepos.Setup(mr => mr.GetCurrentMenu()).Returns(fakeService.Menu);
		    menuRepos.Setup(mr => mr.GetAll()).Returns(new[] {fakeService.Menu});

			var vm = new TerminalApplicationModel(new NewTestUnitOfWork(), service.Object, logger.Object, cashDrawerService.Object, creditHardwareService.Object, processorService,
				new FakeNYCSalesTaxService(), null, checkRepos, empRepos, menuRepos.Object, settings.Item2, settings.Item1);
			vm.SelectedEmployee = emp;

			return vm;
		}

		public static TerminalApplicationModel CreateViewModel(IEmployee emp, ICheckRepository checkRepository, 
			IEmployeeRepository empRepos){
			var service = MockingTools.GetTerminalServiceWithMenu ();
			service.Setup (s => s.GetEmployeesAsync ()).Returns (Task.Factory.StartNew (() => new List<IEmployee>{emp}.AsEnumerable ()));

			return CreateViewModel (emp, checkRepository, empRepos, 
				new Mock<ICashDrawerService> ().Object, 
				service.Object);
		}
			
		public static TerminalApplicationModel CreateViewModel(IEmployee emp, ICheckRepository checkRepos,
			IEmployeeRepository empRepos, ICashDrawerService cds){
			var service = MockingTools.GetTerminalServiceWithMenu ();
			service.Setup (s => s.GetEmployeesAsync ()).Returns (Task.Factory.StartNew (() => new List<IEmployee>{emp}.AsEnumerable ()));

			return CreateViewModel (emp, checkRepos, empRepos, 
				cds, 
				service.Object);
		}

	    public static TerminalApplicationModel CreateViewModel(IEmployee emp,
	        ICheckRepository checkRepos,
	        IEmployeeRepository empRepos,
	        ICashDrawerService cashDrawerService,
	        IRestaurantTerminalService service
	        )
	    {
	        return CreateViewModel(emp, checkRepos, empRepos, cashDrawerService, service, null);
	    }

		public static TerminalApplicationModel CreateViewModel(IEmployee emp, 
			ICheckRepository checkRepos,
			IEmployeeRepository empRepos,
			ICashDrawerService cashDrawerService,
			IRestaurantTerminalService service,
            IPrinterService printerService
		){


			var settings = service.RegisterClientAsync (string.Empty).Result;
			var dal = MockingTools.GetClientDAL ();
			dal.Setup (d => d.GetEntitiesAsync<IMiseTerminalDevice> ()).Returns(
                Task.Factory.StartNew(() => new []{settings.Item2}.AsEnumerable()
                )
            );

			var logger = new Mock<ILogger>();
			var creditHardwareService = new Mock<ICreditCardReaderService>();

			var menuRepos = new Mock<IMenuRepository> ();
		    var fakeService = new FakeDomineesRestaurantServiceClient();
		    menuRepos.Setup(mr => mr.GetCurrentMenu()).Returns(fakeService.Menu);

			var vm = new TerminalApplicationModel(new NewTestUnitOfWork(), service, logger.Object, 
				cashDrawerService, creditHardwareService.Object, null,
				new FakeNYCSalesTaxService(), printerService, checkRepos, empRepos,  menuRepos.Object, settings.Item2, settings.Item1)
			{
			    SelectedEmployee = emp
			};

		    return vm;
		}

		public static TerminalApplicationModel CreateViewModelWithEmployeeAndCreditCardProcessor(IEmployee emp){
			ICheckRepository checkRepos;
			IEmployeeRepository empRepos;

			var logger = new Mock<ILogger> ();
			var ccProcessor = new TestingCreditCardService (logger.Object);

			return CreateViewModel (emp, out checkRepos, out empRepos, ccProcessor);
		}

	}
}

