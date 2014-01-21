using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Avalarin.Web.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.WebUtils {
    [TestClass]
    public sealed class Localization {
        private readonly string TestRepositoryRootPath = @"D:\dev\hazzard\localizaion\";
        private readonly CultureInfo TestCulture = CultureInfo.GetCultureInfo("ru-RU");
        private readonly CultureInfo TestCulture2 = CultureInfo.GetCultureInfo("en-US");
        private readonly string TestPath = "/shared/test";
        private readonly string TestPath2 = "/shared/test2";
        private Random Random;
        private IDictionary<string, string> TestValues;
        private FsXmlLocalizationRepository Repo;

        [TestInitialize]
        public void Init() {
            Random = new Random();
            TestValues = new Dictionary<string, string>() {
                { "key_z", StringUtility.RandomString(Random, 50) },
                { "key_u", StringUtility.RandomString(Random, 50) },
                { "key_q", StringUtility.RandomString(Random, 50) },
            };

            Guid g;
            string repositoryPath;
            do {
                g = Guid.NewGuid();
                repositoryPath = Path.Combine(TestRepositoryRootPath, g.ToString());
            } while (Directory.Exists(repositoryPath));
            Directory.CreateDirectory(repositoryPath);

            var fileName = GetFileName(TestCulture, repositoryPath, TestPath);
            WriteDictionary(fileName, TestValues);

            Repo = new FsXmlLocalizationRepository(repositoryPath, MakeDefaultValue);
        }


        [TestMethod]
        public void Reading() {
            foreach (var kvp in TestValues) {
                Assert.AreEqual(kvp.Value, Repo.Get(TestCulture, TestPath, kvp.Key));
            }
            var localValues = TestValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            localValues["key_h"] = StringUtility.RandomString(Random, 50);
            localValues["key_t"] = StringUtility.RandomString(Random, 50);
            foreach (var kvp in localValues) {
                Repo.Set(TestCulture, TestPath, kvp.Key, kvp.Value);
                Assert.AreEqual(kvp.Value, Repo.Get(TestCulture, TestPath, kvp.Key));
            }
        }

        [TestMethod]
        public void ReadingAndManualChanging() {
            foreach (var kvp in TestValues) {
                Assert.AreEqual(kvp.Value, Repo.Get(TestCulture, TestPath, kvp.Key));
            }
            var localValues = TestValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            localValues["key_h"] = StringUtility.RandomString(Random, 50);
            localValues["key_t"] = StringUtility.RandomString(Random, 50);
            var fileName = GetFileName(TestCulture, Repo.RootPath, TestPath);
            WriteDictionary(fileName, localValues);
            foreach (var kvp in localValues) {
                Assert.AreEqual(kvp.Value, Repo.Get(TestCulture, TestPath, kvp.Key));
            }
        }

        [TestMethod]
        public void ReadingNotFoundKey() {
            var key = StringUtility.RandomString(Random, 5);
            Assert.AreEqual(MakeDefaultValue(key), Repo.Get(TestCulture, TestPath, key));
        }

        [TestMethod]
        public void ReadingNotFoundPath() {
            var key = StringUtility.RandomString(Random, 5);
            Assert.AreEqual(MakeDefaultValue(key), Repo.Get(TestCulture, TestPath2, key));
        }

        [TestMethod]
        public void ReadingNotFoundCulture() {
            var key = StringUtility.RandomString(Random, 5);
            Assert.AreEqual(MakeDefaultValue(key), Repo.Get(TestCulture2, TestPath2, key));
        }

        [TestMethod]
        public void Writing() {
            Repo.Set(TestCulture, TestPath, "hello", "Testing");
            Assert.AreEqual("Testing", Repo.Get(TestCulture, TestPath, "hello"));
        }

        [TestMethod]
        public void WritingNotFoundPath() {
            Repo.Set(TestCulture, TestPath2, "hello", "Testing");
            Assert.AreEqual("Testing", Repo.Get(TestCulture, TestPath2, "hello"));
        }

        [TestMethod]
        public void WritingNotFoundCulture() {
            Repo.Set(TestCulture2, TestPath2, "hello", "Testing");
            Assert.AreEqual("Testing", Repo.Get(TestCulture2, TestPath2, "hello"));
        }

        [TestMethod]
        public void WritingSeveral() {
            var newValues = new Dictionary<string, string> {
                {"key_a", StringUtility.RandomString(Random, 50)},
                {"key_b", StringUtility.RandomString(Random, 50)},
            };
            Repo.Set(TestCulture, TestPath, newValues);
            foreach (var kvp in newValues) {
                Assert.AreEqual(kvp.Value, Repo.Get(TestCulture, TestPath, kvp.Key));
            }
        }

        [TestCleanup]
        public void Cleanup() {
            var path = Repo.RootPath;
            Repo.Dispose();
            try {
                Directory.Delete(path, true);
            }
            catch (IOException) {
                Console.WriteLine("Cannot delete directory '" + path + "'.");
            }
        }

        private string GetFileName(CultureInfo culture, string repositoryPath, string path) {
            var p = path.TrimStart(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            p = p.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return Path.Combine(repositoryPath, culture.TwoLetterISOLanguageName, p + ".xml");
        }

        private void WriteDictionary(string fileName, IDictionary<string, string> dict) {
            var xml = new XElement("items", dict.Select(kvp => new XElement("item", new XAttribute("key", kvp.Key), kvp.Value)));
            var directoryName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }
            File.WriteAllText(fileName, xml.ToString());
        }

        private string MakeDefaultValue(string s) {
            return "{{" + s + "}}";
        }
    }
}