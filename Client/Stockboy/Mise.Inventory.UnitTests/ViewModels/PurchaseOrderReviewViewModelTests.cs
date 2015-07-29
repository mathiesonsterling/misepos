using System;
using System.Threading.Tasks;
using Mise.Core.Services;
using Mise.Inventory.Services;
using Mise.Inventory.ViewModels;

using NUnit.Framework;
using Moq;
namespace Mise.Inventory.UnitTests.ViewModels
{
	[TestFixture]
	public class PurchaseOrderReviewViewModelTests
	{
		[Test]
		public async Task LoadShouldGenerateNewPO(){
			var appNavi = new Mock<IAppNavigation>();
			var poService = new Mock<IPurchaseOrderService> ();
			var vendorService = new Mock<IVendorService> ();
			var loginService = new Mock<ILoginService> ();
		    var logger = new Mock<ILogger>();
		    var insights = new Mock<IInsightsService>();
			var underTest = new PurchaseOrderReviewViewModel (logger.Object, appNavi.Object, poService.Object, vendorService.Object, 
				loginService.Object, insights.Object);

			//ACT
			await underTest.OnAppearing();

			//ASSERT
            throw new NotImplementedException();
		}
	}
}

