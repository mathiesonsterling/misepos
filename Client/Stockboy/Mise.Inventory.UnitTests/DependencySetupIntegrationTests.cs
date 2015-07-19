using System;

using NUnit.Framework;
using Moq;
namespace Mise.Inventory.UnitTests
{
	[TestFixture]
	public class DependencySetupIntegrationTests
	{
		[Test]
		public void DependencySetupShouldBuildWithoutException(){
			//ASSEMBLE
			var underTest = new DependencySetup ();

			//ACT
			underTest.CreateContainer ();

			//ASSERT
			Assert.IsTrue (true, "no exception thrown!"); 
		}
	}
}

