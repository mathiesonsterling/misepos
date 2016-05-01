using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.People;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Base;
using Moq;
using Mise.Core.ValueItems;


namespace Mise.Core.Common.UnitTests.Tools
{
    /// <summary>
	/// Collection of commonly used mocks and other objects for tests.  Assumes use of Moq
    /// </summary>
    public static class MockingTools
    {
		public static CreditCard GetCreditCard(){
			return new CreditCard{
				ExpMonth = 12,
				ExpYear = 2014,
				Name = PersonName.TestName,
                ProcessorToken = new CreditCardProcessorToken
                {
                    Processor = CreditCardProcessors.FakeProcessor,
                    Token = "fakeToken"
                }
			};
		}


        public static Mock<IMiseTerminalDevice> GetMiseDevice(bool printDups = false)
        {
            var tc = new Mock<IMiseTerminalDevice>();
            tc.Setup(t => t.RequireEmployeeSignIn).Returns(true);
            tc.Setup(t => t.TopLevelCategoryID).Returns(Guid.Empty);
            tc.Setup(t => t.PrintKitchenDupes).Returns(printDups);
            return tc;
        }

        public static Guid RestaurantID { get { return Guid.Empty; } }
        public static Restaurant GetRestaurant()
        {
            return new Restaurant { Id = RestaurantID, Name = new BusinessName("testRestaurant") };
        }

        public static Mock<IRestaurantTerminalService> GetTerminalService(bool printDupes = false)
        {

            var tc = GetMiseDevice(printDupes);

            var regRes = new Tuple<Restaurant, IMiseTerminalDevice>(GetRestaurant(), tc.Object);
            var service = new Mock<IRestaurantTerminalService>();
			service.Setup(s => s.RegisterClientAsync(It.IsAny<string>())).Returns(Task.Factory.StartNew (() => regRes));
			service.Setup(s => s.SendEventsAsync(It.IsAny<RestaurantCheck> (), It.IsAny<IEnumerable<ICheckEvent>>())).Returns( Task.FromResult (true));
			service.Setup(s => s.SendEventsAsync(It.IsAny<RestaurantCheck> (), It.IsAny<IEnumerable<ICheckEvent>>()))
				.Returns(Task.FromResult(true));
           
            return service;
        }

        public static Mock<IRestaurantTerminalService> GetTerminalServiceWithMenu()
        {
             var cat = new Mock<MenuItemCategory>();
            var menu = new Mock<Menu>();
            menu.Setup(m => m.GetMenuItemCategories()).Returns(new List<MenuItemCategory> { cat.Object });

            var service = GetTerminalService();
            //service.Setup(s => s.GetCurrentMenu()).Returns(menu.Object);
			service.Setup(s => s.GetMenusAsync()).Returns(Task<IEnumerable<Menu>>.Factory.StartNew(() => new[]{menu.Object}));

			service.Setup(s => s.GetEmployeesAsync()).Returns(Task.Factory.StartNew (() => new List<Employee>().AsEnumerable ()));
            service.Setup(s => s.GetEmployeesAsync())
                .Returns(Task<IEnumerable<Employee>>.Factory.StartNew(() => new List<Employee> ()));

            service.Setup(s => s.GetChecksAsync()).Returns(Task<IEnumerable<RestaurantCheck>>.Factory.StartNew(() => new List<RestaurantCheck>()));
            return service;
        }

        public static Mock<IRestaurantTerminalService> GetTerminalServiceWithChecks(IEnumerable<RestaurantCheck> checks)
        {
            var meChecks = checks.ToList();
            var service = GetTerminalServiceWithMenu();
            service.Setup(s => s.GetChecksAsync()).Returns(Task<IEnumerable<RestaurantCheck>>.Factory.StartNew(() => meChecks));
            return service;
        }
    }
}
