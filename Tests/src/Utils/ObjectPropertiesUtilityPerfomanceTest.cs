using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Avalarin.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Utils {
    [TestClass]
    public class ObjectPropertiesUtilityPerformanceTest {

        [TestMethod]
        public void OneType() {
            var count = 100000;
            Console.WriteLine("Конвертация {0} объектов одного типа в Dictionary:", count);

            var sw = new Stopwatch();
            var random = new Random();

            sw.Start();
            for (var i = 0; i < count; i++) {
                var obj = new {
                    strValue = StringUtility.RandomString(random, 50),
                    intValue = random.Next(int.MinValue, int.MaxValue),
                    dblValue = random.NextDouble(),
                    dateValue = DateTime.Now.AddSeconds(random.Next(int.MinValue, int.MaxValue))
                };
                var dict = new Dictionary<string, object>();
                foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.CanRead)) {
                    dict[property.Name] = property.GetValue(obj);
                }

                dict.Clear();
            }
            sw.Stop();
            Console.WriteLine("Простая рефлексия: " + sw.Elapsed);
            sw.Reset();

            sw.Start();
            for (var i = 0; i < count; i++) {
                var dict = new Dictionary<string, object>();
                dict["strValue"] = StringUtility.RandomString(random, 50);
                dict["intValue"] = random.Next(int.MinValue, int.MaxValue);
                dict["dblValue"] = random.NextDouble();
                dict["dateValue"] = DateTime.Now.AddSeconds(random.Next(int.MinValue, int.MaxValue));

                dict.Clear();
            }
            sw.Stop();
            Console.WriteLine("Простое создание словаря: " + sw.Elapsed);
            sw.Reset();

            sw.Start();
            for (var i = 0; i < count; i++) {
                var obj = new {
                    strValue = StringUtility.RandomString(random, 50),
                    intValue = random.Next(int.MinValue, int.MaxValue),
                    dblValue = random.NextDouble(),
                    dateValue = DateTime.Now.AddSeconds(random.Next(int.MinValue, int.MaxValue))
                };
                var dict = obj.ToDictionary();

                dict.Clear();
            }
            sw.Stop();
            Console.WriteLine("ObjectPropertiesUtility.EachProperties: " + sw.Elapsed);
            sw.Reset();


        }

    }
}
