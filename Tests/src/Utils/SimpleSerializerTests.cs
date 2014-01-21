using System;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using Avalarin.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Utils {
    [TestClass]
    public sealed class SimpleSerializerTests {

        [TestMethod]
        public void Serialize() {
            var random = new Random();
            var stringValue = StringUtility.RandomString(random, random.Next(30, 50));
            Assert.AreEqual(stringValue, SimpleSerializer.Serialize(stringValue));
            var intValue = random.Next(1000, 10000);
            Assert.AreEqual(intValue.ToString(CultureInfo.InvariantCulture), SimpleSerializer.Serialize(intValue));
            var doubleValue = random.NextDouble();
            Assert.AreEqual(doubleValue.ToString(CultureInfo.InvariantCulture), SimpleSerializer.Serialize(doubleValue));
            var dateValue = DateTime.Now.AddHours(random.Next(-5000, 5000));
            Assert.AreEqual(dateValue.ToString(CultureInfo.InvariantCulture), SimpleSerializer.Serialize(dateValue));

            int? nullableIntValue = random.Next(1000, 10000);
            Assert.AreEqual(nullableIntValue.Value.ToString(CultureInfo.InvariantCulture), SimpleSerializer.Serialize(nullableIntValue));
            nullableIntValue = null;
            Assert.AreEqual(string.Empty, SimpleSerializer.Serialize(nullableIntValue));

            double? nullableDoubleValue = random.NextDouble();
            Assert.AreEqual(nullableDoubleValue.Value.ToString(CultureInfo.InvariantCulture), SimpleSerializer.Serialize(nullableDoubleValue));
            nullableDoubleValue = null;
            Assert.AreEqual(string.Empty, SimpleSerializer.Serialize(nullableDoubleValue));

            DateTime? nullableDateValue = DateTime.Now.AddHours(random.Next(-5000, 5000));
            Assert.AreEqual(nullableDateValue.Value.ToString(CultureInfo.InvariantCulture), SimpleSerializer.Serialize(nullableDateValue));
            nullableDateValue = null;
            Assert.AreEqual(string.Empty, SimpleSerializer.Serialize(nullableDateValue));

            Exception exception = null;
            try {
                SimpleSerializer.Serialize(new TestStruct?(new TestStruct()));
            }
            catch (NotSupportedException e) {
                exception = e;
            }

            exception = null;
            try {
                SimpleSerializer.Serialize(new { x1 = "x1", x2 = "x2" });
            }
            catch (NotSupportedException e) {
                exception = e;
            }
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void Deserialize() {

            var random = new Random();

            Assert.AreEqual(null, SimpleSerializer.Deserialize<string>(""));
            Assert.AreEqual("empty", SimpleSerializer.Deserialize<string>("", "empty"));
            Assert.AreEqual(null, SimpleSerializer.Deserialize<string>(null));
            Assert.AreEqual(5, SimpleSerializer.Deserialize<int>("", 5));
            Assert.AreEqual((double)5, SimpleSerializer.Deserialize<double>("", (double)5));

            var stringValue = StringUtility.RandomString(random, random.Next(30, 50));
            Assert.AreEqual(stringValue, SimpleSerializer.Deserialize<string>(stringValue));
            var intValue = random.Next(1000, 10000);
            Assert.AreEqual(intValue, SimpleSerializer.Deserialize<int>(intValue.ToString(CultureInfo.InvariantCulture)));
            var doubleValue = random.NextDouble();
            Assert.IsTrue(Math.Abs(doubleValue - SimpleSerializer.Deserialize<double>(doubleValue.ToString(CultureInfo.InvariantCulture))) < 0.000000000000001);
            var dateValue = DateTime.Today.AddSeconds(random.Next(-5000, 5000));
            Assert.AreEqual(dateValue, SimpleSerializer.Deserialize<DateTime>(dateValue.ToString(CultureInfo.InvariantCulture)));

            int? nullableIntValue = random.Next(1000, 10000);
            Assert.AreEqual(nullableIntValue.Value, SimpleSerializer.Deserialize<int?>(nullableIntValue.Value.ToString(CultureInfo.InvariantCulture)).Value);

            double? nullableDoubleValue = random.NextDouble();
            Assert.IsTrue(Math.Abs(nullableDoubleValue.Value - SimpleSerializer.Deserialize<double?>(nullableDoubleValue.Value.ToString(CultureInfo.InvariantCulture)).Value) < 0.000000000000001);

            DateTime? nullableDateValue = DateTime.Today.AddSeconds(random.Next(-5000, 5000));
            Assert.AreEqual(nullableDateValue.Value, SimpleSerializer.Deserialize<DateTime?>(nullableDateValue.Value.ToString(CultureInfo.InvariantCulture)).Value);

            Exception exception = null;
            try {
                SimpleSerializer.Deserialize<TestStruct?>("1");
            }
            catch (NotSupportedException e) {
                exception = e;
            }

            exception = null;
            try {
                SimpleSerializer.Deserialize<Exception>("1");
            }
            catch (NotSupportedException e) {
                exception = e;
            }
            Assert.IsNotNull(exception);
        }

        private struct TestStruct {
            public int X1 { get; set; }
            public string X2 { get; set; }
        }
        
    }
}