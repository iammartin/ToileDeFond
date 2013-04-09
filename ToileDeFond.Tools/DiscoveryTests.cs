using System;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToileDeFond.Tools
{
    [TestFixture]
    [Ignore]
    public class DiscoveryTests
    {
        [Test]
        public void FromDictionaryToObjectWithNewtonsoftJson()
        {
            var data = new Dictionary<string, string> { { "Name", "Rusi" }, { "Age", "23" }, { "IsHot", "true" }, { "Hello", "sfds" } };

            var fakeClass = JsonConvert.DeserializeObject<FakeClass>(JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc }));

            Assert.That(fakeClass.Name, Is.EqualTo(data["Name"]));
            Assert.That(fakeClass.Age, Is.EqualTo(int.Parse(data["Age"])));
            Assert.That(fakeClass.IsHot, Is.EqualTo(true));
        }

        [Test]
        public void GuidSerialization()
        {
            var originalGuid = Guid.NewGuid();
            var json = JsonConvert.SerializeObject(originalGuid, new Newtonsoft.Json.JsonSerializerSettings { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc });

            var guid = JsonConvert.DeserializeObject<Guid>(json);

            Assert.AreEqual(originalGuid, guid);


            var strGuid = JsonConvert.DeserializeObject<string>(json);
            Assert.AreEqual(strGuid, originalGuid.ToString());
        }

        [Test]
        public void NumericSerialization()
        {
            const int originalNumeric = 12;
            var json = JsonConvert.SerializeObject(originalNumeric);

            var numeric = JsonConvert.DeserializeObject<int>(json, new Newtonsoft.Json.JsonSerializerSettings { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc });

            Assert.AreEqual(originalNumeric, numeric);


            var strNumeric = JsonConvert.DeserializeObject<string>(json, new Newtonsoft.Json.JsonSerializerSettings { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc });
            Assert.AreEqual(strNumeric, originalNumeric.ToString());
        }

        [Test]
        public void TypeSerialization()
        {
            var originalType = typeof(string);
            var json = JsonConvert.SerializeObject(originalType, new Newtonsoft.Json.JsonSerializerSettings { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc });

            var typeName = JsonConvert.DeserializeObject<string>(json);

            Assert.AreEqual(originalType.AssemblyQualifiedName, typeName);


            var finalType = JsonConvert.DeserializeObject<Type>(json);
            Assert.AreEqual(finalType, originalType);
        }


        private class FakeClass
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsHot { get; set; }
            public bool Missing { get; set; }
        }
    }
}
