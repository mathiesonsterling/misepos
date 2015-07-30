using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Client.Repositories;
using Mise.Core.Entities.People.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Client.ApplicationModel.Implementation;
using Moq;
using Mise.Core.Entities.People;
using Mise.Core.Services.WebServices;
using Mise.Core.Client.Services;

namespace Mise.Core.Client.UnitTests.Tools
{
	public static class ViewModelMockingTools
	{
		public static Task<Tuple<TerminalApplicationModel, ICheckRepository, IEmployeeRepository>> CreateViewModel(IEmployee employee)
		{
			return CreateViewModel (employee, null);
		}

		public static async Task<Tuple<TerminalApplicationModel, ICheckRepository, IEmployeeRepository>> CreateViewModel(IEmployee emp, 
			ICreditCardProcessorService processorService){
			var service = MockingTools.GetTerminalServiceWithMenu ();
			service.Setup (s => s.GetEmployeesAsync ()).Returns (Task.FromResult(new List<IEmployee>{emp}.AsEnumerable ()));
		    service.Setup(s => s.GetEmployeesForRestaurant(It.IsAny<Guid>()))
		        .Returns(Task.FromResult(new List<IEmployee> {emp}.AsEnumerable()));
		    service.Setup(s => s.GetEmployeesAsync())
		        .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new[] {emp}));
		    service.Setup(s => s.SendEventsAsync(It.IsAny<IEmployee> (), It.IsAny<IEnumerable<IEmployeeEvent>>()))
				.Returns(Task.Factory.StartNew (() => true));

			var settings = service.Object.RegisterClientAsync (string.Empty).Result;
			var dal = MockingTools.GetClientDAL ();
		    dal.Setup(d => d.GetEntitiesAsync<MiseTerminalDevice>())
		        .Returns(
		            Task.FromResult(new[] {settings.Item2 as MiseTerminalDevice}.AsEnumerable()));

			var logger = new Mock<ILogger>();
			var cashDrawerService = new Mock<ICashDrawerService>();
			var creditHardwareService = new Mock<ICreditCardReaderService>();
            var checkReposC = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);
			await checkReposC.Load (MockingTools.RestaurantID);
			var checkRepos = checkReposC;

            var empReposC = new ClientEmployeeRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);
			await empReposC.Load (MockingTools.RestaurantID);
			var empRepos = empReposC;

			var menuRepos = new Mock<IMenuRepository> ();
		    var fakeService = new FakeDomineesRestaurantServiceClient();
		    menuRepos.Setup(mr => mr.GetCurrentMenu()).Returns(fakeService.Menu);
		    menuRepos.Setup(mr => mr.GetAll()).Returns(new[] {fakeService.Menu});

			var vm = new TerminalApplicationModel(new NewTestUnitOfWork(), service.Object, logger.Object, cashDrawerService.Object, creditHardwareService.Object, processorService,
				new FakeNYCSalesTaxService(), null, checkRepos, empRepos, menuRepos.Object, settings.Item2, settings.Item1);
			vm.SelectedEmployee = emp;

		    return new Tuple<TerminalApplicationModel, ICheckRepository, IEmployeeRepository>(vm, checkRepos, empRepos);
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
		    dal.Setup(d => d.GetEntitiesAsync<MiseTerminalDevice>()).Returns(
		        Task.FromResult(new[] {settings.Item2 as MiseTerminalDevice}.AsEnumerable()));

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

		public static async Task<TerminalApplicationModel> CreateViewModelWithEmployeeAndCreditCardProcessor(IEmployee emp){
			var logger = new Mock<ILogger> ();
			var ccProcessor = new TestingCreditCardService (logger.Object);

			var res = await CreateViewModel (emp, ccProcessor);
		    return res.Item1;
		}

	}
}

