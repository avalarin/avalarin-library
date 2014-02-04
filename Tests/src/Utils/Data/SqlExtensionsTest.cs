using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
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

        [TestMethod]
        public void ExecuteScalarText() {
            var employeesCount = 25;
            using (var conn = GetConnection()) {
                for (var i = 0; i < employeesCount; i++) {
                    var employeeName = StringUtility.RandomString(_random, 50);
                    conn.Sp("CreateEmployee")
                        .WithParameter("name", employeeName)
                        .ExecuteAndReadFirstOrDefault(MapEmployee);
                }
                var count = conn.Text("SELECT COUNT(*) FROM Employee")
                    .ExecuteScalar();
                Assert.AreEqual(employeesCount, count);
            }
        }

        [TestMethod]
        public void ExecuteWithTimeout() {
            var employeesCount = 25;
            using (var conn = GetConnection()) {
                for (var i = 0; i < employeesCount; i++) {
                    var employeeName = StringUtility.RandomString(_random, 50);
                    conn.Sp("CreateEmployee")
                        .WithParameter("name", employeeName)
                        .ExecuteAndReadFirstOrDefault(MapEmployee);
                }
                var hasException = false;
                try {
                    conn.Sp("LongGetEmployeesCount")
                        .WithTimeout(1)
                        .ExecuteScalar();
                }
                catch (SqlException e) {
                    if (e.Message.StartsWith("Timeout expired.")) {
                        hasException = true;
                    }
                }
                Assert.IsTrue(hasException);
                
                var count = conn.Sp("GetEmployeesCount")
                    .WithTimeout(1)
                    .ExecuteScalar();
                Assert.AreEqual(employeesCount, count);
            }
        }

        [TestMethod]
        public void ExecuteWithTransaction() {
            using (var conn = GetConnection()) {
                var employee = conn.Sp("CreateEmployee")
                    .WithParameter("name", StringUtility.RandomString(_random, 50))
                    .ExecuteAndReadFirstOrDefault(MapEmployee);
                var department1 = conn.Sp("CreateDepartment")
                    .WithParameter("name", StringUtility.RandomString(_random, 50))
                    .ExecuteAndReadFirstOrDefault(MapDepartment);
                var department2 = conn.Sp("CreateDepartment")
                    .WithParameter("name", StringUtility.RandomString(_random, 50))
                    .ExecuteAndReadFirstOrDefault(MapDepartment);

                conn.Sp("ChangeEmployeeDepartment")
                    .WithParameters(new { employeeId = employee.Id, newDepartmentId = department1.Id }).ExecuteNonQuery();
                employee = conn.Sp("GetEmployeeById")
                    .WithParameters(new {id = employee.Id}).ExecuteAndReadFirstOrDefault(MapEmployee);
                Assert.AreEqual(employee.Department.Id, department1.Id);

                using (var tran = conn.BeginTransaction()) {
                    conn.Sp("ChangeEmployeeDepartment")
                        .WithTransaction(tran)
                        .WithParameters(new { employeeId = employee.Id, newDepartmentId = department2.Id }).ExecuteNonQuery();
                    tran.Rollback();
                }
                employee = conn.Sp("GetEmployeeById")
                    .WithParameters(new { id = employee.Id })
                    .ExecuteAndReadFirstOrDefault(MapEmployee);
                Assert.AreEqual(employee.Department.Id, department1.Id);
            }
            
        }

        [TestMethod]
        public void ExecuteWithBeginExecutionAndCustomExecuteHandler() {
            var employeesCount = 20;
            using (var conn = GetConnection()) {
                for (var i = 0; i < employeesCount; i++) {
                    var employeeName = StringUtility.RandomString(_random, 50);
                    conn.Sp("CreateEmployee")
                        .WithParameter("name", employeeName)
                        .ExecuteAndReadFirstOrDefault(MapEmployee);
                }

                var totalCount = -1;
                var employees = conn.Sp("GetEmployees")
                    .BeforeExecution(cmd => {
                        cmd.AddInputParameter("page", 0);
                        cmd.AddInputParameter("pageSize", 10);
                        cmd.AddOutputParameter("totalCount", DbType.Int32, 0);
                    })
                    .Execute(cmd => {
                        IEnumerable<object> empls;
                        using (var reader = cmd.ExecuteReader()) {
                            empls = reader.ReadAll(MapEmployee);
                        }
                        var totalCountParameter = (DbParameter)cmd.Parameters["totalCount"];
                        totalCount = (int)totalCountParameter.Value;
                        return empls;
                    });
                Assert.AreEqual(employeesCount, totalCount);

            }
        }

        [TestMethod]
        public void ExecuteWithBeginExecutionAndCustomExecuteReader() {
            var employeesCount = 20;
            using (var conn = GetConnection()) {
                for (var i = 0; i < employeesCount; i++) {
                    var employeeName = StringUtility.RandomString(_random, 50);
                    conn.Sp("CreateEmployee")
                        .WithParameter("name", employeeName)
                        .ExecuteAndReadFirstOrDefault(MapEmployee);
                }

                IDataParameter totalCountParameter = null;
                var employees = conn.Sp("GetEmployees")
                    .BeforeExecution(cmd => {
                        cmd.AddInputParameter("page", 0);
                        cmd.AddInputParameter("pageSize", 10);
                        totalCountParameter = cmd.AddOutputParameter("totalCount", DbType.Int32, 0);
                    })
                    .ExecuteReader((reader)=> reader.ReadAll(MapEmployee));
                var totalCount = (int)totalCountParameter.Value;
                Assert.AreEqual(employeesCount, totalCount);
            }
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
    }
}