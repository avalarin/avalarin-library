using System;
using System.Collections.Generic;
using System.Data;
using Avalarin.Utils;

namespace Avalarin.Data {
    public static class SqlParameterExtensions {
        /// <summary>
        /// Adds an input parameter with the specified name and value.
        /// </summary>
        /// <param name="cmd">An <see cref="IDbCommand"/> in which parameter should be added.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>An instance of the added <see cref="IDataParameter"/>.</returns>
        /// <exception cref="ArgumentNullException">cmd is null.</exception>
        public static IDataParameter AddInputParameter(this IDbCommand cmd, string name, object value) {
            if (cmd == null) {
                throw new ArgumentNullException("cmd");
            }
            IDbDataParameter parameter = cmd.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.Direction = ParameterDirection.Input;

            cmd.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Adds an output parameter with the specified name and value.
        /// </summary>
        /// <param name="cmd">An <see cref="IDbCommand"/> in which parameter should be added.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">The type of the parameter.</param>
        /// <param name="size">The size of the parameter.</param>
        /// <returns>An instance of the added <see cref="IDataParameter"/>.</returns>
        /// <exception cref="ArgumentNullException">cmd is null.</exception>
        public static IDataParameter AddOutputParameter(this IDbCommand cmd, string name, DbType dbType, int size) {
            if (cmd == null) {
                throw new ArgumentNullException("cmd");
            }
            IDbDataParameter parameter = cmd.CreateParameter();

            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Size = size;
            parameter.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Adds a return value parameter with the specified name and value.
        /// </summary>
        /// <param name="cmd">An <see cref="IDbCommand"/> in which parameter should be added.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>An instance of the added <see cref="IDataParameter"/>.</returns>
        /// <exception cref="ArgumentNullException">cmd is null.</exception>
        public static IDataParameter AddReturnValueParameter(this IDbCommand cmd, string name) {
            if (cmd == null) {
                throw new ArgumentNullException("cmd");
            }
            IDbDataParameter parameter = cmd.CreateParameter();

            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.ReturnValue;

            cmd.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Adds a few parameters from the anonymous object.
        /// </summary>
        /// <param name="cmd">An <see cref="IDbCommand"/> in which parameter should be added.</param>
        /// <param name="parameters">Anonymous object.</param>
        /// <exception cref="ArgumentNullException">cmd is null.</exception>
        /// <exception cref="ArgumentNullException">parameters is null.</exception>
        public static void AddInputParameters(this IDbCommand cmd, object parameters) {
            if (cmd == null) throw new ArgumentNullException("cmd");
            if (parameters == null) throw new ArgumentNullException("parameters");
            parameters.EachProperties((name, value) => {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "@" + name;
                parameter.Value = value;
                parameter.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(parameter);
            });
        }

        /// <summary>
        /// Adds a few parameters from the dictionary.
        /// </summary>
        /// <param name="cmd">An <see cref="IDbCommand"/> in which parameter should be added.</param>
        /// <param name="parameters">Dictionary.</param>
        /// <exception cref="ArgumentNullException">cmd is null.</exception>
        /// <exception cref="ArgumentNullException">parameters is null.</exception>
        public static void AddInputParameters(this IDbCommand cmd, IDictionary<string, object> parameters) {
            if (cmd == null) throw new ArgumentNullException("cmd");
            if (parameters == null) throw new ArgumentNullException("parameters");
            foreach (var kvp in parameters) {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "@" + kvp.Key;
                parameter.Value = kvp.Value;
                parameter.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(parameter);
            }
        }
    }
}