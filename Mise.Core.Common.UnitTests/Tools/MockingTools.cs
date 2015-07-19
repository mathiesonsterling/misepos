using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Services.WebServices;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Common.Services;
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
        public static IRestaurant GetRestaurant()
        {
            return new Restaurant { ID = RestaurantID, Name = new RestaurantName("testRestaurant") };
        }

        public static Mock<IRestaurantTerminalService> GetTerminalService(bool printDupes = false)
        {

            var tc = GetMiseDevice(printDupes);

            var regRes = new Tuple<IRestaurant, IMiseTerminalDevice>(GetRestaurant(), tc.Object);
            var service = new Mock<IRestaurantTerminalService>();
			service.Setup(s => s.RegisterClientAsync(It.IsAny<string>())).Returns(Task.Factory.StartNew (() => regRes));
			service.Setup(s => s.SendEventsAsync(It.IsAny<ICheck> (), It.IsAny<IEnumerable<ICheckEvent>>())).Returns( Task.FromResult (true));
			service.Setup(s => s.SendEventsAsync(It.IsAny<ICheck> (), It.IsAny<IEnumerable<ICheckEvent>>()))
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

			service.Setup(s => s.GetEmployeesAsync()).Returns(Task.Factory.StartNew (() => new List<IEmployee>().AsEnumerable ()));
            service.Setup(s => s.GetEmployeesAsync())
                .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new List<IEmployee> ()));

            service.Setup(s => s.GetChecksAsync()).Returns(Task<IEnumerable<ICheck>>.Factory.StartNew(() => new List<ICheck>()));
            return service;
        }

        public static Mock<IRestaurantTerminalService> GetTerminalServiceWithChecks(IEnumerable<ICheck> checks)
        {
            var meChecks = checks.ToList();
            var service = GetTerminalServiceWithMenu();
            service.Setup(s => s.GetChecksAsync()).Returns(Task<IEnumerable<ICheck>>.Factory.StartNew(() => meChecks));
            return service;
        }
         
		public static Mock<IClientDAL> GetClientDAL(){
			var dal = new Mock<IClientDAL> ();
			dal.Setup (d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IEntityBase>> ()));
		    dal.Setup(d => d.GetEntitiesAsync<IMiseTerminalDevice>()).Returns(
		        Task.Factory.StartNew(() =>
		        {
		            var miseDEv = new MiseTerminalDevice
		            {
		                ID = Guid.Empty
		            };
		            var res = new List<IMiseTerminalDevice> {miseDEv};
		            return res.AsEnumerable();
		        })
		        );
                                                       
                                                     

            dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IEntityBase>>()))
                .Returns(Task.Factory.StartNew(() => true));

			dal.Setup(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEntityEventBase>>())).Returns(new Task<bool>(() => true));
			dal.Setup(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEntityEventBase>>())).Returns(Task.Factory.StartNew(() => true));

			return dal;
		}
    }
}
