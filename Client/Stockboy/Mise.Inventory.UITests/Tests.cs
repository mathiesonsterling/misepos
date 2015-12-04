using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Mise.Inventory.UITests
{
    [Ignore("Needs fix to address built version")]
	[TestFixture (Platform.Android)]
	[TestFixture (Platform.iOS)]
	public class Tests
	{
		IApp app;
		Platform platform;

		public Tests (Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest ()
		{
			app = AppInitializer.StartApp (platform);
		}

		[Test]
		public void AppLaunchesAndLoginAppearsDisabled ()
		{

			var res = app.Query (e => e.Button ("Login"));

			Assert.AreEqual (1, res.Length);
			Assert.False (res.First().Enabled, "Login not enabled");

			var regQuery = app.Query (e => e.Button ("Register"));
			Assert.True(regQuery.First ().Enabled, "Register enabled");

			var enterField = app.Query (e => e.TextField ().Marked ("Email"));
			Assert.AreEqual (1, enterField.Length);
		}
	}
}

