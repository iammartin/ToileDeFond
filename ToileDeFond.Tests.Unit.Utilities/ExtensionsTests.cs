using System.Collections.Generic;
using NUnit.Framework;
using ToileDeFond.Utilities;

namespace ToileDeFond.Tests.Unit.Utilities
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void MergeDictionaries()
        {
            var parent = new Dictionary<string, string> { { "1", "1" }, { "2", "2" }, { "4", "4" } };
            var child = new Dictionary<string, string> { { "2", "3" }, { "3", "3" } };
            var lastChild = new Dictionary<string, string> { { "3", "4" }, { "4", "5" }, { "5", "5" } };

            var merged = parent.MergeLeft(child).MergeLeft(lastChild);

            Assert.That(merged["1"], Is.EqualTo("1"));
            Assert.That(merged["2"], Is.EqualTo("3"));
            Assert.That(merged["3"], Is.EqualTo("4"));
            Assert.That(merged["4"], Is.EqualTo("5"));
            Assert.That(merged["5"], Is.EqualTo("5"));
        }
    }
}
