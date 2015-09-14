using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
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

		    var po = new PurchaseOrder
		    {
		        Id = Guid.NewGuid(),
                PurchaseOrdersPerVendor = new List<PurchaseOrderPerVendor>
                {
                    new PurchaseOrderPerVendor
                    {
                        Id = Guid.NewGuid(),
                    }
                }
		    };
		    poService.Setup(pos => pos.CreatePurchaseOrder())
		        .Returns(Task.FromResult(po as IPurchaseOrder));

			var vendorService = new Mock<IVendorService> ();
			var loginService = new Mock<ILoginService> ();
		    var logger = new Mock<ILogger>();
		    var insights = new Mock<IInsightsService>();
			var underTest = new PurchaseOrderReviewViewModel (logger.Object, appNavi.Object, poService.Object, vendorService.Object, 
				loginService.Object, insights.Object);

			//ACT
			await underTest.OnAppearing();

			//ASSERT
            Assert.NotNull(underTest.VendorsAndPOs);
		}
	}
}

