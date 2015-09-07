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
        /// <summary>
        /// We want to close the window, not move to another one, so we're not in the back chain
        /// </summary>
		[Test]
		public void AddSectionShouldCallServiceAndCloseSectionAdd(){
			var logger = new Mock<ILogger>();
			var loginService = new Mock<ILoginService> ();
			loginService.Setup (ls => ls.AddNewSectionToRestaurant (It.IsAny<string> (), It.IsAny<bool> (), It.IsAny<bool> ()))
				.Returns (Task.FromResult (true));

			var appNav = new Mock<IAppNavigation> ();
			appNav.Setup (an => an.ShowSectionSelect ()).Returns (Task.FromResult (true));

			var invService = new Mock<IInventoryService> ();
			invService.Setup (i => i.AddNewSection (It.IsAny<string> (), It.IsAny<bool> (), It.IsAny<bool> ()))
				.Returns (Task.FromResult (true));
			var underTest = new SectionAddViewModel (appNav.Object, logger.Object, invService.Object);

			//ACT
			underTest.AddSection();

			//ASSERT
			appNav.Verify(an => an.CloseSectionAdd(), Times.Once());
		}

		[Test]
		public void AddSectionCannotFireWithBlankName(){
			var logger = new Mock<ILogger>();

			var appNav = new Mock<IAppNavigation> ();
			appNav.Setup (an => an.ShowSectionSelect ()).Returns (Task.FromResult (true));

			var invService = new Mock<IInventoryService> ();
			invService.Setup (i => i.AddNewSection (It.IsAny<string> (), It.IsAny<bool> (), It.IsAny<bool> ()))
				.Returns (Task.FromResult (true));
			var underTest = new SectionAddViewModel (appNav.Object, logger.Object, invService.Object);

			//ACT
			underTest.SectionName = "  ";
			var canWithBlank = underTest.AddCommand.CanExecute (null);
			Assert.False (canWithBlank);

			underTest.SectionName = "A";
			var canWithSingle = underTest.AddCommand.CanExecute (null);
			Assert.False (canWithSingle);

			underTest.SectionName = "Bill";
			var realOne = underTest.AddCommand.CanExecute (null);
			Assert.True (realOne);
		}
	}
}

