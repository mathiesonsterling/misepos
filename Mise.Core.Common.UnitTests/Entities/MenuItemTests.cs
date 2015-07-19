
using Mise.Core.Common.Entities.MenuItems;
using Mise.Core.Entities.Menu;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities
{
	[TestFixture]
	public class MenuItemTests
	{
		[Test]
		public void TestDestinationsIsNotNull(){
			var mi = new MenuItem ();

			Assert.IsNotNull (mi.Destinations);
			Assert.IsNotNull (mi.PossibleModifiers);
		}
	}
}

