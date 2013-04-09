using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable RedundantUsingDirective
using System.Threading.Tasks;
// ReSharper restore RedundantUsingDirective
using NUnit.Framework;
using ToileDeFond.ContentManagement;

namespace ToileDeFond.Tests.Unit.ContentManagement
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void DateTimeToUtcPrecision()
        {
            var datetime = DateTime.Now;
            var serializedDatetime = datetime.ToUniversalTime();
            var restoredDatetime = serializedDatetime.ToLocalTime();

            Assert.That(restoredDatetime, Is.EqualTo(datetime));
        }
    }
}
