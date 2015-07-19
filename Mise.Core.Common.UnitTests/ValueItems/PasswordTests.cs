using System;

using NUnit.Framework;
using Mise.Core.ValueItems;


namespace Mise.Core.Common.UnitTests
{
	[TestFixture]
	public class PasswordTests
	{
		[TestCase("test", "a94a8fe5ccb19ba61c4c0873d391e987982fbbd3")]
		[TestCase("chicken", "cef7e59218e3a7e18aaf7faa4a23bcd964323a66")]
		[TestCase("I don't like cake", "41bd4293fc3b8ba342baaa341ebcd63f588db4c3")]
		[Test]
		public void PasswordShouldHashValue(string input, string hash){
			var password = new Password (input);

			//ASSERT
			Assert.AreNotEqual(input, password.HashValue);
			Assert.AreEqual (hash.ToUpper(), password.HashValue);
		}

		[Test]
		public void IsValidShouldRefuseNullsButNotThrow(){
			var res = Password.IsValid (null);

			Assert.IsFalse (res);
		}

	    [Test]
	    public void SetValueWithNullShouldThrow()
	    {
	        var underTest = new Password();

	        Assert.Throws<ArgumentException>(() => underTest.SetValue(null));
	        Assert.Throws<ArgumentException>(() => underTest.SetValue(string.Empty));
	    }
	}
}

