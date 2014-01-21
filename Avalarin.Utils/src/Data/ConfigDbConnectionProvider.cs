using System;
using System.Data;
using System.Data.Common;
using Avalarin.Utils;

namespace Avalarin.Data {
    public class ConfigDbConnectionProvider : IDbConnectionProvider {
        private readonly string _connectionStringName;

        public ConfigDbConnectionProvider(string connectionStringName = "Default") {
            if (connectionStringName == null) throw new ArgumentNullException("connectionStringName");
            _connectionStringName = connectionStringName;
        }

        public IDbConnection GetConnection() {
            var settings = ConfigurationUtility.GetConnectionStringSettings(_connectionStringName);
            return CreateConnection(settings.ConnectionString, settings.ProviderName);
        }

        private static IDbConnection CreateConnection(string connectionString, string providerName = null) {
            if (string.IsNullOrEmpty(providerName)) {
                providerName = "System.Data.SqlClient";
            }
            var factory = DbProviderFactories.GetFactory(providerName);
            var conn = factory.CreateConnection();
            if (conn == null) {
                throw new InvalidOperationException("Provider '" + providerName + "' returned null.");
            }
            conn.ConnectionString = connectionString;
            return conn;
        }
    }
}