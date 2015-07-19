using System;
using Mise.Core.Entities.Payments;
using NUnit.Framework;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.ValueItems;


namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
	[TestFixture]
	public class PaymentSerializationTests
	{
		IJSONSerializer _jsonSer;

		[SetUp]
		public void Setup(){
			_jsonSer = new JsonNetSerializer ();
		}

		[Test]
		public void TestCashPayment(){
			var checkID = Guid.NewGuid ();
			var empID = Guid.NewGuid ();
			var cp = new CashPayment {
				ID = Guid.NewGuid (),
				AmountPaid = new Money (10.0M),
				AmountTendered = new Money (50.0M),
				ChangeGiven = new Money (40.0M),
				CheckID = checkID,
				EmployeeID = empID
			};

			var json = _jsonSer.Serialize (cp);
			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = _jsonSer.Deserialize<CashPayment> (json);
			Assert.IsNotNull (res);
			Assert.AreEqual (checkID, res.CheckID);
			Assert.AreEqual (empID, res.EmployeeID);
			Assert.AreEqual (10.0M, res.AmountPaid.Dollars);
		}
	}
}

