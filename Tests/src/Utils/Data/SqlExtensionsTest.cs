using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalarin.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Utils.Data {
    [TestClass]
    public sealed class SqlExtensionsTest {
        private readonly Random _random = new Random();

        [TestMethod]
        public void ExecuteSpWithOneInputParameterAndReadFirst() {
            var employeeName = StringUtility.RandomString(_random, 50);
            using (var conn = GetConnection()) {
                var employee = conn.Sp("CreateEmployee")
                    .WithParameter("name", employeeName)
                    .ExecuteAndReadFirstOrDefault(MapEmployee);
                Assert.AreEqual(employeeName, employee.Name);
            }
        }

        [TestMethod]
        public void ExecuteSpWithSomeParameters() {
            var employeeName = StringUtility.RandomString(_random, 50);
            var departmentName = StringUtility.RandomString(_random, 50);
            using (var conn = GetConnection()) {
                var employee = conn.Sp("CreateEmployee")
                    .WithParameter("name", employeeName)
                    .ExecuteAndReadFirstOrDefault(MapEmployee);
                var department = conn.Sp("CreateDepartment")
                    .WithParameter("name", departmentName)
                    .ExecuteAndReadFirstOrDefault(MapDepartment);
                conn.Sp("ChangeEmployeeDepartment")
                    .WithParameters(new {
                        employeeId = employee.Id,
                        newDepartmentId = department.Id
                    })
                    .ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void ExecuteSpWithSomeParametersDictionary() {
            var employeeName = StringUtility.RandomString(_random, 50);
            var departmentName = StringUtility.RandomString(_random, 50);
            using (var conn = GetConnection()) {
                var employee = conn.Sp("CreateEmployee")
                    .WithParameter("name", employeeName)
                    .ExecuteAndReadFirstOrDefault(MapEmployee);
                var department = conn.Sp("CreateDepartment")
                    .WithParameter("name", departmentName)
                    .ExecuteAndReadFirstOrDefault(MapDepartment);
                conn.Sp("ChangeEmployeeDepartment")
                    .WithParameters(new Dictionary<string, object>() {
                        {"employeeId", employee.Id},
                        {"newDepartmentId", department.Id}
                    })
                    .ExecuteNonQuery();
            }
        }
        
        [TestMethod]
        public void ExecuteSpWithOutParametersAndReadAll() {
            var employeesCount = 20;
            using (var conn = GetConnection()) {
                for (var i = 0; i < employeesCount; i++) {
                    var employeeName = StringUtility.RandomString(_random, 50);
                    conn.Sp("CreateEmployee")
                        .WithParameter("name", employeeName)
                        .ExecuteAndReadFirstOrDefault(MapEmployee);
                }
                var totalCount = -1;
                conn.Sp("GetEmployees")
                    .WithParameters(new {
                        page = 0,
                        pageSize = 10
                    })
                    .WithOutputParameter<int>("totalCount", count => totalCount = count)
                    .ExecuteAndReadAll(MapEmployee);
                Assert.AreEqual(employeesCount, totalCount);
                conn.Sp("GetEmployees")
                    .WithParameters(new {
                        page = 0,
                        pageSize = 10
                    })
                    .WithOutputParameter<int>("totalCount", DbType.Int32, count => totalCount = count)
                    .ExecuteAndReadAll(MapEmployee);
                Assert.AreEqual(employeesCount, totalCount);
                conn.Sp("GetEmployees")
                    .WithParameters(new {
                        page = 0,
                        pageSize = 10
                    })
                    .WithOutputParameter("totalCount", DbType.Int32, count => totalCount = (int) count)
                    .ExecuteAndReadAll(MapEmployee);
                Assert.AreEqual(employeesCount, totalCount);
            }
            

        }

        private dynamic MapEmployee(IDataReader reader) {
            return new {
                Id = reader.Value<int>("Id"),
                Name = reader.Value<string>("Name"),
                Department = new {
                    Id = reader.ValueOrDefault<int>("DepartmentId"),
                    Name = reader.ValueOrDefault<string>("DepartmentName")
                }
            };
        }

        private dynamic MapDepartment(IDataReader reader) {
            return new {
                Id = reader.Value<int>("Id"),
                Name = reader.Value<string>("Name")
            };
        }

        [TestCleanup]
        public void Cleanup() {
            using (var conn = GetConnection()) {
                conn.Sp("ClearDatabase")
                    .ExecuteNonQuery();
            }
        }

        private IDbConnection GetConnection(string name = "Default") {
            var provider = new ConfigDbConnectionProvider(name);
            return provider.GetConnection();
        }

    }
}