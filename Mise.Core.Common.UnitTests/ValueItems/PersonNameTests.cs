using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;
using Mono.Security.X509;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.ValueItems
{
    [TestFixture]
    public class PersonNameTests
    {
        [Test]
        public void FirstAndLastShouldEquate()
        {
            var name1 = new PersonName("Bob", "Jones");
            var name2 = new PersonName("Bob", "Jones");

            Assert.AreEqual(name1, name2);
        }

        [Test]
        public void FirstAndLastAndMiddleShouldEquate()
        {
            var name1 = new PersonName("Bob", "Thomas","Jones");
            var name2 = new PersonName("Bob", "Thomas","Jones");

            Assert.AreEqual(name1, name2);
        }

        [Test]
        public void FirstAndLastShouldNotEquateIfMiddleInOne()
        {
            var name1 = new PersonName("Bob", "Jones");
            var name2 = new PersonName("Bob", "Thomas","Jones");

            Assert.AreNotEqual(name1, name2);
        }

        [Test]
        public void SearchShouldFind()
        {
            var name = new PersonName("Zach", "Attack");

            var res = name.ContainsSearchString("Za");

            Assert.True(res);
        }

        [Test]
        public void SearchShouldFindCaseInsensitive()
        {
            var name = new PersonName("Zach", "Attack");

            var res = name.ContainsSearchString("ZAC");

            Assert.True(res);
        }

        [Test]
        public void ContainsShouldBeCaseInsensitive()
        {
            var names = new List<PersonName>
            {
                new PersonName("Thomas", "Allen", "Jones")
            };

            var upper = new PersonName("THOMAS", "ALLEN", "JONES");
            var other = new PersonName("Justin", "Elliott");

            var upperFound = names.Contains(upper);
            var notFound = names.Contains(other);

            Assert.IsTrue(upperFound, "Upper");
            Assert.IsFalse(notFound, "not found");
        }
    }
}
