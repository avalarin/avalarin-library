using System;
using System.Configuration;
using System.Globalization;

namespace Avalarin.Utils {

    /// <summary>
    /// The <see cref="ConfigurationUtility"/> class provides a set of utility methods that can be used to work with configuration files.
    /// </summary>
    public static class ConfigurationUtility {
        private const string DefaultConnectionStringName = "Default";

        /// <summary>
        /// Gets the default connection string.
        /// </summary>
        /// <returns>Default connection string.</returns>
        public static string GetConnectionString() {
            return GetConnectionString(null);
        }

        /// <summary>
        /// Gets the connection string associated with specified name.
        /// </summary>
        /// <param name="connectionStringName">The name of connection string to get.</param>
        /// <returns>Connection string associated with the specified name.</returns>
        public static string GetConnectionString(String connectionStringName) {
            return GetConnectionStringSettings(connectionStringName).ConnectionString;
        }

        /// <summary>
        /// Gets the default <see cref="ConnectionStringSettings"/>.
        /// </summary>
        /// <returns>Default <see cref="ConnectionStringSettings"/>.</returns>
        /// <exception cref="InvalidOperationException">Connection strings section is invalid or not found.</exception>
        /// <exception cref="InvalidOperationException">Default connection string not found.</exception>
        public static ConnectionStringSettings GetConnectionStringSettings() {
            return GetConnectionStringSettings(null);
        }

        /// <summary>
        /// Gets the <see cref="ConnectionStringSettings"/> string associated with specified name.
        /// </summary>
        /// <param name="connectionStringName">The name of connection string to get.</param>
        /// <returns><see cref="ConnectionStringSettings"/> associated with the specified name.</returns>
        /// <exception cref="InvalidOperationException">Connection strings section is invalid or not found.</exception>
        /// <exception cref="InvalidOperationException">Connection string not found.</exception>
        public static ConnectionStringSettings GetConnectionStringSettings(String connectionStringName) {
            if (connectionStringName == null) {
                connectionStringName = DefaultConnectionStringName;
            }
            var section = ConfigurationManager.GetSection("connectionStrings") as ConnectionStringsSection;
            if (section == null) {
                throw new InvalidOperationException(
                    "ConnectionStringsSection is invalid or not found.");
            }
            ConnectionStringSettings settings = section.ConnectionStrings[connectionStringName];
            if (settings == null) {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "ConnectionString '{0}' not found.",
                        connectionStringName)
                );
            }
            return settings;
        }

        /// <summary>
        /// Returns an <see cref="ConfigurationSection"/> associated with the specified name. If it does not exist a new instance will be created.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="ConfigurationSection"/>.</typeparam>
        /// <param name="sectionName">The name of <see cref="ConfigurationSection"/>.</param>
        /// <returns>An <see cref="ConfigurationSection"/> associated with specified name.</returns>
        public static T GetSection<T>(String sectionName) where T : ConfigurationSection, new() {
            return (T) ConfigurationManager.GetSection(sectionName) ?? new T();
        }
    }
}