using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.ValueItems
{
    [TestFixture]
    public class PhoneNumberTests
    {
        [Test]
        public void EqualsShouldIgnoreParenthesesAndHypens()
        {
            var number1 = new PhoneNumber
            {
                AreaCode = "(602) ",
                Number = " 265 - 6111 "
            };

            var number2 = new PhoneNumber
            {
                AreaCode = "602",
                Number = "2656111"
            };

            //ACT
            var res = number1.Equals(number2);

            //ASSERT
            Assert.IsTrue(res, "are equal!");
        }

        [Test]
        public void IsValidWithJustNumbers()
        {
            var res = PhoneNumber.IsValid("602", "2656111");

            Assert.IsTrue(res);
        }

        [Test]
        public void IsValidWithFormatting()
        {
            var res = PhoneNumber.IsValid("(602)", "265 - 6111");

            Assert.IsTrue(res);
        }

        [Test]
        public void ShouldCreateWithEmptyValues()
        {
            //ACT
            var phone = new PhoneNumber("", "");

            //ASSERT
            Assert.AreEqual("", phone.AreaCode);
            Assert.AreEqual("", phone.Number);
        }
    }
}
