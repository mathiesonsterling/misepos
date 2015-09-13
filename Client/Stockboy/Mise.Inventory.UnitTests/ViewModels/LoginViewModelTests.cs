using System;
using Mise.Core.Entities;
using Mise.Core.Services.UtilityServices;
using Moq;
using NUnit.Framework;
using Mise.Core.ValueItems;
using Mise.Core.Entities.People;
using Mise.Core.Common.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mise.Inventory.ViewModels;
using Mise.Inventory.Services;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Services;
using Mise.Inventory.ViewModels.Modals;


namespace Mise.Inventory.UnitTests.ViewModels
{
	[TestFixture]
	public class LoginViewModelTests
	{

		[Test]
		public async Task ShouldLoginAUserWithCorrectPassword()
		{
			EmailAddress givenEmail = null;
			Password givenPassword = null;
			//ASSEMBLE
			var loginService = new Mock<ILoginService> ();
			loginService.Setup(s => s.LoginAsync (It.IsAny<EmailAddress> (), It.IsAny<Password> ()))
				.Callback<EmailAddress, Password> ((email, pwd) =>{
					givenEmail = email;
					givenPassword = pwd;
				})
				.Returns(() => 
					Task.FromResult(new Employee () as IEmployee)
				);
			loginService.Setup (s => s.GetPossibleRestaurantsForLoggedInEmployee ())
				.Returns (Task.FromResult(
					new List<IRestaurant>{ new Restaurant () }.AsEnumerable()
				));
			
			var navigationService = new Mock<IAppNavigation> ();
			navigationService.Setup (ns => ns.ShowMainMenu ())
				.Returns (Task.FromResult(true));


			var logger = new Mock<ILogger> ();
		    var insights = new Mock<IInsightsService>();
			var underTest = new LoginViewModel (logger.Object, loginService.Object, navigationService.Object, insights.Object)
			{
			    Username = "test@misepos.com",
			    Password = "password"
			};

		    //ACT
			//underTest.LoginCommand.Execute (null);
			await underTest.Login();

			//ASSERT
			loginService.Verify (ls => ls.LoginAsync (It.IsAny<EmailAddress> (), It.IsAny<Password> ()), Times.Once ());
			//also assert that our password was hashed on the way in, and isn't in cleartest
			Assert.NotNull (givenEmail);
			Assert.AreEqual ("test@misepos.com", givenEmail.Value);

			Assert.NotNull (givenPassword);
			Assert.AreNotEqual ("password", givenPassword.HashValue, "password is in cleartext!");
		}

		[Test]
		public async Task ShouldNotLoginAUserWithIncorrectPasswordAndDisplayError()
		{
			//ASSEMBLE
		    IEmployee noEmp = null;
			var loginService = new Mock<ILoginService> ();
			loginService.Setup(s => s.LoginAsync (It.IsAny<EmailAddress> (), It.IsAny<Password> ()))
				.Returns(() => 
					Task.FromResult(noEmp)
				);
			loginService.Setup (s => s.GetPossibleRestaurantsForLoggedInEmployee ())
				.Returns (Task.FromResult( 
					new List<IRestaurant>{ new Restaurant () }.AsEnumerable()
				));
			
			var navigationService = new Mock<IAppNavigation> ();
			navigationService.Setup (ns => ns.ShowMainMenu())
				.Returns (Task.FromResult(true));

			var logger = new Mock<ILogger> ();
		    var insights = new Mock<IInsightsService>();

		    var messageCalled = false;
		    Func<ErrorMessage, Task> messageFunc = message =>
		    {
		        messageCalled = true;
		        return Task.FromResult(false);
		    };

			var underTest = new LoginViewModel (logger.Object, loginService.Object, navigationService.Object, insights.Object)
			{
			    Username = "test@misepos.com",
			    Password = "password",
                DisplayMessage = messageFunc
			};

		    //ACT
			await underTest.Login();
			//underTest.LoginCommand.Execute (null);

			//ASSERT
			loginService.Verify (ls => ls.LoginAsync (It.IsAny<EmailAddress> (), It.IsAny<Password> ()), Times.Once ());
			navigationService.Verify (ns => ns.ShowMainMenu(), Times.Never (), "didnt go to main menu");
            Assert.IsTrue(messageCalled);
		}

		[Test]
		public async Task ShouldNavigateToRestaurantLoadOnLoginWithOneRestaurant()
		{
			//ASSEMBLE
			var loginService = new Mock<ILoginService> ();
			loginService.Setup(s => s.LoginAsync (It.IsAny<EmailAddress> (), It.IsAny<Password> ()))
				.Returns(() => 
					Task.FromResult(new Employee () as IEmployee)
				);
			loginService.Setup (s => s.GetPossibleRestaurantsForLoggedInEmployee ())
				.Returns (Task.FromResult(
					new List<IRestaurant>{ new Restaurant () }.AsEnumerable()
				));

			var appNavigation = new Mock<IAppNavigation> ();
			appNavigation.Setup (ns => ns.ShowMainMenu ()).Returns (Task.FromResult(true));


			var logger = new Mock<ILogger> ();
		    var insights = new Mock<IInsightsService>();
			var underTest = new LoginViewModel (logger.Object, loginService.Object, appNavigation.Object, insights.Object) {Username = "test@misepos.com", Password = "password"};

		    //ACT
			//underTest.LoginCommand.Execute (null);
			await underTest.Login();

			//ASSERT
			loginService.Verify (ls => ls.LoginAsync (It.IsAny<EmailAddress> (), It.IsAny<Password> ()), Times.Once ());
			appNavigation.Verify (ns => ns.ShowRestaurantLoading(), Times.Once(), "didnt go to main menu");
			appNavigation.Verify (ns => ns.ShowSelectRestaurant (), Times.Never (), "select restaurant was called");
		}

		[Test]
		public async Task ShouldNavigateToSelectRestaurantOnLoginWithMultipleRestaurants()
		{
			//ASSEMBLE
			var loginService = new Mock<ILoginService> ();
			loginService.Setup(s => s.LoginAsync (It.IsAny<EmailAddress> (), It.IsAny<Password> ()))
				.Returns(() => 
					Task.FromResult(new Employee () as IEmployee)
				);
			loginService.Setup (s => s.GetPossibleRestaurantsForLoggedInEmployee ())
				.Returns (Task.FromResult(
					new List<IRestaurant>{ new Restaurant (), new Restaurant () }.AsEnumerable()
				));

			var navigationService = new Mock<IAppNavigation> ();
			navigationService.Setup (ns => ns.ShowMainMenu ())
				.Returns (Task.Factory.StartNew (() => {}));

			var logger = new Mock<ILogger> ();
		    var insights = new Mock<IInsightsService>();
			var underTest = new LoginViewModel (logger.Object, loginService.Object, navigationService.Object, insights.Object);
			underTest.Username = "test@misepos.com" ;
			underTest.Password = "password";

			//ACT
			//underTest.LoginCommand.Execute (null);
			await underTest.Login();

			//ASSERT
			loginService.Verify (ls => ls.LoginAsync (It.IsAny<EmailAddress> (), It.IsAny<Password> ()), 
				Times.Once ());
			navigationService.Verify (ns => ns.ShowMainMenu(), Times.Never(), "went to main menu");
			navigationService.Verify (ns => ns.ShowSelectRestaurant (), Times.Once(), 
				"select restaurant was not called");

		}

        [Test]
        public async Task ShouldSelectEmployeeCorrectRestaurant()
        {
            //ASSEMBLE
            var goodRest = Guid.NewGuid();
            var defaultRest = Guid.Empty;

            var loginService = new Mock<ILoginService>();
            loginService.Setup(s => s.LoginAsync(It.IsAny<EmailAddress>(), It.IsAny<Password>()))
                .Returns(Task.FromResult(
                    new Employee
                    {
                        RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
                            {goodRest, new[]{MiseAppTypes.StockboyMobile, MiseAppTypes.UnitTests, MiseAppTypes.POSMobile}}
                        }
                    } as IEmployee
                ));
            loginService.Setup(s => s.GetPossibleRestaurantsForLoggedInEmployee())
                .Returns(Task.FromResult(
                    new List<IRestaurant> { new Restaurant
                    {
                        ID = goodRest
                    }, new Restaurant
                    {
                        ID = defaultRest
                    }
                    }.AsEnumerable()
                ));

            var navigationService = new Mock<IAppNavigation>();
            navigationService.Setup(ns => ns.ShowMainMenu())
                .Returns(Task.Factory.StartNew(() => { }));

            var logger = new Mock<ILogger>();
            var insights = new Mock<IInsightsService>();
            var underTest = new LoginViewModel(logger.Object, loginService.Object, navigationService.Object, insights.Object)
            {
                Username = "test@misepos.com",
                Password = "password"
            };

            //ACT
            //underTest.LoginCommand.Execute (null);
            await underTest.Login();

            //ASSERT
            loginService.Verify(ls => ls.LoginAsync(It.IsAny<EmailAddress>(), It.IsAny<Password>()),
                Times.Once());
            navigationService.Verify(ns => ns.ShowMainMenu(), Times.Never(), "went to main menu");
            navigationService.Verify(ns => ns.ShowSelectRestaurant(), Times.Once(),
                "select restaurant was not called");

        }
	}
}

