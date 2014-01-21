using System;
using System.Threading.Tasks;
using Avalarin.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Utils {

    [TestClass]
    public sealed class ObjectPropertiesUtilityTest {

        [TestMethod]
        public void EachProperties() {
            var random = new Random();
            var objects = new dynamic[10000];
            for (var i = 0; i < objects.Length; i++) {
                objects[i] = new {
                    x1 = random.Next(0, int.MaxValue),
                    x2 = StringUtility.RandomString(random, random.Next(10, 30)),
                    x3 = DateTime.Now.AddDays(random.Next(-50, 50)),
                    x4 = Guid.NewGuid(),
                    x5 = random.NextDouble()
                };
            }

            foreach (var o in objects) {
                var dict = ObjectPropertiesUtility.ToDictionary(o);
                Assert.IsTrue(dict.ContainsKey("x1"));
                Assert.IsTrue(dict.ContainsKey("x2"));
                Assert.IsTrue(dict.ContainsKey("x3"));
                Assert.IsTrue(dict.ContainsKey("x4"));
                Assert.IsTrue(dict.ContainsKey("x5"));
                Assert.AreEqual(o.x1, dict["x1"]);
                Assert.AreEqual(o.x2, dict["x2"]);
                Assert.AreEqual(o.x3, dict["x3"]);
                Assert.AreEqual(o.x4, dict["x4"]);
                Assert.AreEqual(o.x5, dict["x5"]);
            }

            Parallel.ForEach(objects, o => {
                var dict = ObjectPropertiesUtility.ToDictionary(o);
                Assert.IsTrue(dict.ContainsKey("x1"));
                Assert.IsTrue(dict.ContainsKey("x2"));
                Assert.IsTrue(dict.ContainsKey("x3"));
                Assert.IsTrue(dict.ContainsKey("x4"));
                Assert.IsTrue(dict.ContainsKey("x5"));
                Assert.AreEqual(o.x1, dict["x1"]);
                Assert.AreEqual(o.x2, dict["x2"]);
                Assert.AreEqual(o.x3, dict["x3"]);
                Assert.AreEqual(o.x4, dict["x4"]);
                Assert.AreEqual(o.x5, dict["x5"]);
            });
        }

    }
}