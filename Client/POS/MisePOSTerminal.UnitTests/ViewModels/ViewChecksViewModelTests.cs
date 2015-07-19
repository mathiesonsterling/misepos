using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Services;
using Mise.Core.Client.Services;
using Mise.Core.ValueItems;
using MisePOSTerminal.ViewModels;
using Moq;
using NUnit.Framework;

namespace MisePOSTerminal.UnitTests.ViewModels
{
    [TestFixture]
    public class ViewChecksViewModelTests
    {
        /// <summary>
        /// Swiping a card that doesn't have a check creates a check, swiping it again just opens it
        /// </summary>
        [Test]
        public void SwipingAnExistingCheckWithCreditCardJustLoadsIt()
        {
            //ASSEMBLE
            var moqSwiper = new Mock<ICreditCardReaderService>();

            var card = new CreditCard
            {
                CardNumber = "1111222233334444",
                FirstName = "Testy",
                LastName = "Tester"

            };

            var check = new Mock<ICheck>();
            check.Setup(c => c.CreditCards).Returns(new[] {card});
            ICheck clickedCheck = null;


            var checksInRepos = new List<ICheck>();
            //we need an actual model here!
            var appModel = new Mock<ITerminalApplicationModel>();
            appModel.Setup(am => am.CreateNewCheck(card)).Returns(check.Object).Callback(() => checksInRepos.Add(check.Object));
            appModel.Setup(am => am.CheckClicked(It.IsAny<ICheck>())).Callback<ICheck>(c => clickedCheck = c);
            appModel.SetupGet(am => am.CreditCardReaderService).Returns(moqSwiper.Object);
            appModel.Setup(am => am.OpenChecks).Returns(checksInRepos);
            var logger = new Mock<ILogger>();
            var vm = new ViewChecksViewModel(logger.Object, appModel.Object);


            //ACT
            vm.CreditCardSwiped(card);
            vm.CreditCardSwiped(card);

            //ASSERT
            appModel.Verify(am => am.CreateNewCheck(card), Times.Once);
            appModel.Verify(am => am.CheckClicked(It.IsAny<ICheck>()), Times.Once);
            Assert.IsNotNull(clickedCheck);
            Assert.AreEqual(clickedCheck, check.Object);
        }
    }
}
