using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Mise.Inventory.Services.Implementation;


namespace Mise.Inventory.UnitTests.Services
{
	[TestFixture]
	public class FunFactServiceTests
	{
		[TestCase(100000)]
		[Test]
		public async Task RandomNeverThrowsOutOfRange(int numTimesCalled){
			var underTest = new FunFactService ();

			for(var i=0;i < numTimesCalled; i++){
				var res = await underTest.GetRandomFunFact ();
				Assert.IsFalse (string.IsNullOrWhiteSpace (res));
			}
		}
	}
}

