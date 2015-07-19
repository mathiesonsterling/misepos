using System;
using System.Collections.Generic;
using System.Runtime;
using NUnit.Framework;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.UnitTests
{
	[TestFixtureAttribute]
	public class EmailAddressTests
	{
		[Test]
		public void IsValidShouldRefuseNullsButNotThrow(){
			var res = EmailAddress.IsValid (null);

			Assert.IsFalse (res);
		}

	    [Test]
	    public void EqualsShouldBeCaseInsensitive()
	    {
	        var email1 = new EmailAddress("testing@test.com");
	        var email2 = new EmailAddress("TEStInG@test.com");

	        var oneWay = email1.Equals(email2);
	        var other = email2.Equals(email1);

            Assert.True(oneWay);
            Assert.True(other);
	    }

	    [Test]
	    public void NonEqualShouldNotEqual()
	    {
	        var email1 = new EmailAddress("bob@bob.com");
	        var email2 = new EmailAddress("joe@test.com");

	        var res = email1.Equals(email2);

            Assert.False(res);
	    }

	    [Test]
	    public void ContainsIsCaseInsensitive()
	    {
	        var emails = new List<EmailAddress>
	        {
	            new EmailAddress("test@test.com")
	        };

	        var other = new EmailAddress("TEST@TEST.COM");

	        var contains = emails.Contains(other);

            Assert.IsTrue(contains);
	    }
	}
}

