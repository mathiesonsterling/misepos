using System;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using NUnit.Framework;
using Moq;

using Mise.Inventory.ViewModels;
using Mise.Core.Services;
using Mise.Inventory.Services;
namespace Mise.Inventory.UnitTests.ViewModels
{
	[TestFixture]
	public class SectionAddViewModelTests
	{
		[Test]
		public void AddSectionShouldCallServiceAndMoveBackToSelect(){
			var logger = new Mock<ILogger>();
			var loginService = new Mock<ILoginService> ();
			loginService.Setup (ls => ls.AddNewSectionToRestaurant (It.IsAny<string> (), It.IsAny<bool> (), It.IsAny<bool> ()))
				.Returns (Task.FromResult (true));

			var appNav = new Mock<IAppNavigation> ();
			appNav.Setup (an => an.ShowSectionSelect (false)).Returns (Task.FromResult (true));

			var invService = new Mock<IInventoryService> ();
			invService.Setup (i => i.AddNewSection (It.IsAny<string> (), It.IsAny<bool> (), It.IsAny<bool> ()))
				.Returns (Task.FromResult (true));
			var underTest = new SectionAddViewModel (appNav.Object, loginService.Object, 
				logger.Object, invService.Object);

			//ACT
			underTest.AddSection();

			//ASSERT
			appNav.Verify(an => an.ShowSectionSelect(false), Times.Once());
		}
	}
}

