using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Avalarin.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Utils.Data {
    [TestClass]
    public sealed class ReaderUtilsTest {
        private readonly FakeDataReaderField[] _fields = new[] {
            new FakeDataReaderField("Name", typeof(string)),
            new FakeDataReaderField("Age", typeof(int)),
            new FakeDataReaderField("RegDate", typeof(DateTime)),
            new FakeDataReaderField("Session", typeof(Guid))
        };

        private readonly object[][] _values = new[] {
            new object[] {"Bulune", 24, DateTime.Now.AddYears(-1), DBNull.Value},
            new object[] {"Saizel", 22, DateTime.Now.AddYears(-2), Guid.Parse("{AFA67162-5B52-4DDA-919E-D0A3A43C4737}")},
            new object[] {"Sarn", 19, DateTime.Now.AddMonths(-7), Guid.Parse("{A391D7F2-69C2-4DFC-8F92-EAF71A222571}")},
            new object[] {"Adorabor", 28, DateTime.Now.AddMonths(-1), DBNull.Value},
            new object[] {"Taukora", 31, DateTime.Now.AddMonths(-3), Guid.Parse("{B8CB907D-EEA2-4CE5-B549-18F0474A0D2D}")}
        };

        [TestMethod]
        public void ReaderValue() {
            var reader = new FakeDataReader(_fields, _values);
            foreach (var row in _values) {
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(row[0], reader.Value<string>("Name"));
                Assert.AreEqual(row[1], reader.Value<int>("Age"));
                Assert.AreEqual(row[2], reader.Value<DateTime>("RegDate"));
                Assert.AreEqual(row[3], reader.Value<object>("Session"));
            }
        }

        [TestMethod]
        public void ReaderValueOrDefault() {
            var reader = new FakeDataReader(_fields, _values);
            foreach (var row in _values) {
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(row[0], reader.Value<string>("Name"));
                Assert.AreEqual(row[1], reader.Value<int>("Age"));
                Assert.AreEqual(row[2], reader.Value<DateTime>("RegDate"));

                var defaultSession = Guid.Parse("{86D39A5A-886C-4FF4-AC09-51801C32A885}");
                Guid expected;
                if (reader.IsDBNull(reader.GetOrdinal("Session"))) {
                    expected = defaultSession;
                }
                else {
                    expected = (Guid)row[3];
                }
                Assert.AreEqual(expected, reader.ValueOrDefault("Session", defaultSession));
            }
        }

        [TestMethod]
        public void ReadAllWithoutIndex() {
            var reader = new FakeDataReader(_fields, _values);
            var items = reader.ReadAll(r => MapItem(r, -1)).ToArray();
            Assert.AreEqual(_values.Length, items.Length);
            for (var i = 0; i < _values.Length; i++) {
                var row = _values[i];
                var item = items[i];
                CheckItem(row, item);
            }
        }

        [TestMethod]
        public void ReadAllWithIndex() {
            var reader = new FakeDataReader(_fields, _values);
            var items = reader.ReadAll(MapItem).ToArray();
            Assert.AreEqual(_values.Length, items.Length);
            for (var i = 0; i < _values.Length; i++) {
                var row = _values[i];
                var item = items[i];
                Assert.AreEqual(i, item.Index);
                CheckItem(row, item);
            }
        }

        [TestMethod]
        public void ReadFirstOrDefault() {
            var reader = new FakeDataReader(_fields, _values);
            var item = reader.ReadFirstOrDefault(r => MapItem(r, 0));
            Assert.IsNotNull(item);
            var row = _values[0];
            CheckItem(row, item);

            reader = new FakeDataReader(_fields, new object[][]{});
            item = reader.ReadFirstOrDefault(r => MapItem(r, 0));
            Assert.IsNull(item);
        }

        private static void CheckItem(object[] row, dynamic item) {
            Assert.AreEqual(row[0], item.Name);
            Assert.AreEqual(row[1], item.Age);
            Assert.AreEqual(row[2], item.RegDate);
            Assert.AreEqual(row[3] == DBNull.Value ? Guid.Empty : row[3], item.Session);
        }

        private static dynamic MapItem(IDataReader reader, int index) {
            return new {
                Index = index,
                Name = reader.Value<string>("Name"),
                Age = reader.Value<int>("Age"),
                RegDate = reader.Value<DateTime>("RegDate"),
                Session = reader.ValueOrDefault("Session", Guid.Empty)
            };
        }

        private sealed class FakeDataReader : IDataReader {
            private readonly object[][] _values;
            private readonly FakeDataReaderField[] _fields;
            private object[] _currentRow;
            private int _currentRowId = -1;

            public object[] CurrentRow {
                get {
                    if (_currentRow == null) {
                        throw new InvalidOperationException("Not readed.");
                    }
                    return _currentRow;
                }
            }

            public FakeDataReader(FakeDataReaderField[] fields, object[][] values) {
                _values = values;
                _fields = fields;
                _currentRow = null;
            }

            public void Dispose() { }

            public string GetName(int i) {
                return GetField(i).Name;
            }

            public string GetDataTypeName(int i) {
                return GetFieldType(i).Name;
            }

            public Type GetFieldType(int i) {
                return GetField(i).Type;
            }

            public object GetValue(int i) {
                if (i < 0 | i >= _fields.Length) {
                    throw new IndexOutOfRangeException();
                }
                return CurrentRow[i];
            }

            public int GetValues(object[] values) {
                CurrentRow.CopyTo(values, 0);
                return CurrentRow.Length;
            }

            public int GetOrdinal(string name) {
                if (name == null) throw new ArgumentNullException("name");
                FakeDataReaderField field = null;
                int i;
                for (i = 0; i < _fields.Length; i++) {
                    if (_fields[i].Name != name) continue;
                    field = _fields[i];
                    break;
                }
                if (field == null) {
                    throw new IndexOutOfRangeException("Field '" + name + "' not found.");
                }
                return i;
            }

            public bool GetBoolean(int i) {
                return (bool) GetValue(i);
            }

            public byte GetByte(int i) {
                return (byte)GetValue(i);
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
                throw new NotImplementedException();
            }

            public char GetChar(int i) {
                return (char)GetValue(i);
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
                throw new NotImplementedException();
            }

            public Guid GetGuid(int i) {
                return (Guid)GetValue(i);
            }

            public short GetInt16(int i) {
                return (short)GetValue(i);
            }

            public int GetInt32(int i) {
                return (int)GetValue(i);
            }

            public long GetInt64(int i) {
                return (long)GetValue(i);
            }

            public float GetFloat(int i) {
                return (float)GetValue(i);
            }

            public double GetDouble(int i) {
                return (double)GetValue(i);
            }

            public string GetString(int i) {
                return (string)GetValue(i);
            }

            public decimal GetDecimal(int i) {
                return (decimal)GetValue(i);
            }

            public DateTime GetDateTime(int i) {
                return (DateTime)GetValue(i);
            }

            public IDataReader GetData(int i) {
                throw new NotImplementedException();
            }

            public bool IsDBNull(int i) {
                return GetValue(i) == DBNull.Value;
            }

            public int FieldCount { get { return _fields.Length; } }

            object IDataRecord.this[int i] {
                get { return GetValue(i); }
            }

            object IDataRecord.this[string name] {
                get { return GetValue(GetOrdinal(name)); }
            }

            public void Close() { }

            public DataTable GetSchemaTable() {
                var dt = new DataTable();
                dt.Columns.AddRange(_fields.Select(f => new DataColumn(f.Name, f.Type)).ToArray());
                return dt;
            }

            public bool NextResult() {
                return false;
            }

            public bool Read() {
                _currentRowId++;
                if (_currentRowId >= _values.Length) return false;
                _currentRow = _values[_currentRowId];

                return true;
            }

            public int Depth { get { return 1; } }
            public bool IsClosed { get { return false; } }
            public int RecordsAffected { get { return 0; }}

            private FakeDataReaderField GetField(int i) {
                if (i < 0 | i >= _fields.Length) {
                    throw new IndexOutOfRangeException();
                }
                return _fields[i];
            }
        }

        private sealed class FakeDataReaderField {
            public string Name { get; private set; }
            public Type Type { get; private set; }

            public FakeDataReaderField(string name, Type type) {
                Name = name;
                Type = type;
            }
        }

    }
}