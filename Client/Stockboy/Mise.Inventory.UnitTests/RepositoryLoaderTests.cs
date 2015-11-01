using System;
using System.Linq;

using NUnit.Framework;
using Moq;
using Mise.Core.Repositories;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services.UtilityServices;

using Autofac;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Inventory.Services;


namespace Mise.Inventory.UnitTests
{
	[TestFixture]
	public class RepositoryLoaderTests
	{
		[Test]
		public async Task LoadRepositoriesWithRestaurantShouldCallEachRepositoryToLoadAndSetLastEventID(){
			//ASSEMBLE
			var empRepos = MakeMockRepository<IEmployeeRepository> ();
			var vendRepos = MakeMockRepository<IVendorRepository> ();
			var restRepos = MakeMockRepository<IRestaurantRepository> ();
			var invRepos = MakeMockRepository<IInventoryRepository> ();
			var parRepos = MakeMockRepository<IParRepository> ();
			var poRepos = MakeMockRepository<IPurchaseOrderRepository> ();
			var roRepos = MakeMockRepository<IReceivingOrderRepository> ();
			var invite = MakeMockRepository<IApplicationInvitationRepository> ();

			List<EventID> eventIDs = null;
			var eventFactory = new Mock<IInventoryAppEventFactory> ();
			eventFactory.Setup (ef => ef.SetLastEventID (It.IsAny<IEnumerable<EventID>> ()))
				.Callback<IEnumerable<EventID>> (en => eventIDs = en.ToList ());

			var bevItemService = new Mock<IBeverageItemService> ();
			bevItemService.Setup (b => b.ReloadItemCache ()).Returns (Task.FromResult (false));

			var logger = new Mock<ILogger> ();
		    var underTest = new RepositoryLoader(logger.Object, empRepos.Object, invite.Object, vendRepos.Object, 
				eventFactory.Object, restRepos.Object, parRepos.Object, invRepos.Object,
		        roRepos.Object, poRepos.Object, bevItemService.Object);

			var restID = Guid.NewGuid ();

			//ACT
			await underTest.LoadRepositories (restID);

			//ASSERT
			empRepos.Verify (r => r.Load (restID), Times.Once ());
			vendRepos.Verify (r => r.Load (restID), Times.Once ());
			restRepos.Verify (r => r.Load (restID), Times.Once ());
			invRepos.Verify (r => r.Load (restID), Times.Once ());
			parRepos.Verify (r => r.Load (restID), Times.Once ());
			poRepos.Verify (r => r.Load (restID), Times.Once ());
			roRepos.Verify (r => r.Load(restID), Times.Once ());

			Assert.NotNull (eventIDs);
			var maxID = eventIDs.Max (ei => ei.OrderingID);
			Assert.AreEqual (8, maxID);
		}

		static long lastID = 1;
		static Mock<T> MakeMockRepository<T>() where T:class, IRepository {
			var mock = new Mock<T> ();
			mock.Setup(mr => mr.Load(It.IsAny<Guid?> ()))
				.Returns(Task.Run(() => {}));
			mock.Setup (r => r.GetLastEventID ())
				.Returns (new EventID{ AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = lastID++ });
			return mock;
		}
			
		[Test]
		public async Task SaveShouldCallAllRepositoriesEvenWithExceptions(){
			var inventoryRepos = new Mock<IInventoryRepository> ();
			inventoryRepos.Setup (r => r.CommitAll ()).ThrowsAsync (new InvalidOperationException ());
			inventoryRepos.Setup (r => r.Dirty).Returns (true);

			var parRepos = new Mock<IParRepository> ();
			parRepos.Setup (r => r.CommitAll ()).ThrowsAsync (new InvalidOperationException ());
			parRepos.Setup (r => r.Dirty).Returns (true);

			var roRepos = new Mock<IReceivingOrderRepository> ();
			roRepos.Setup (r => r.CommitAll ()).ThrowsAsync (new InvalidOperationException ());
			roRepos.Setup (r => r.Dirty).Returns (true);

			var logger = new Mock<ILogger> ();

			var underTest = new RepositoryLoader (logger.Object, null, null, null, null, null, parRepos.Object,
				                inventoryRepos.Object, roRepos.Object, null, null);

			//ACT
			Exception thrown = null;
			try{
				await underTest.SaveOnSleep();
			} catch(Exception e){
				thrown = e;
			}

			//ASSERT
			Assert.NotNull(thrown);
			inventoryRepos.Verify(r => r.CommitAll(), Times.Once());
			parRepos.Verify (r => r.CommitAll (), Times.Once ());
			roRepos.Verify (r => r.CommitAll (), Times.Once ());

			logger.Verify (l => l.HandleException (It.IsAny<Exception> (), LogLevel.Error), Times.AtLeast (3));
		}
	}
}

