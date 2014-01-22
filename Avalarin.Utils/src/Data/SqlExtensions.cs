using System;
using System.Collections.Generic;
using System.Data;
using Avalarin.Utils;

namespace Avalarin.Data {
    public static class SqlExtensions {
        private const int DefaultCommandTimeout = 30;

        #region Parameters
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
        #endregion

        public static SqlExtensions.DbCommandWrapper Sp(this IDbConnection connection, string text) {
            if (connection == null) throw new ArgumentNullException("connection");
            if (text == null) throw new ArgumentNullException("text");
            return DbCommandWrapper.CreateSp(connection, text);
        }

        public static SqlExtensions.DbCommandWrapper Text(this IDbConnection connection, string text) {
            if (connection == null) throw new ArgumentNullException("connection");
            if (text == null) throw new ArgumentNullException("text");
            return DbCommandWrapper.CreateText(connection, text);
        }

        public sealed class DbCommandWrapper {
            private readonly IDictionary<string, object> _parameters = new Dictionary<string, object>();

            private IDbConnection Connection { get; set; }
            public CommandType CommandType { get; private set; }
            public string Text { get; private set; }

            private IDictionary<string, object> Parameters { get { return _parameters; } }
            private IDbTransaction Transaction { get; set; }
            private int Timeout { get; set; }

            private Action<IDbCommand> BeforeExecutionHandler { get; set; }
            private Action<IDbCommand, object> OnCompletedHandler { get; set; }
            private Func<Exception, IDbCommand, bool> OnExceptionHandler { get; set; }

            private DbCommandWrapper(IDbConnection connection, CommandType commandType, string text) {
                if (connection == null) throw new ArgumentNullException("connection");
                if (text == null) throw new ArgumentNullException("text");
                Connection = connection;
                CommandType = commandType;
                Text = text;

                Timeout = SqlExtensions.DefaultCommandTimeout;
            }

            #region Modification
            public DbCommandWrapper WithParameters(object parameters) {
                if (parameters == null) throw new ArgumentNullException("parameters");
                parameters.EachProperties((name, value) => {
                    if (Parameters.ContainsKey(name)) {
                        throw new DuplicateNameException("Dublicate parameter '" + name + "'.");
                    }
                    Parameters[name] = value;
                });
                return this;
            }

            public DbCommandWrapper WithParameters(IDictionary<string, object> parameters) {
                if (parameters == null) throw new ArgumentNullException("parameters");
                foreach (var key in parameters.Keys) {
                    var value = parameters[key];
                    if (Parameters.ContainsKey(key)) {
                        throw new DuplicateNameException("Dublicate parameter '" + key + "'.");
                    }
                    Parameters[key] = value;
                }
                return this;
            }

            public DbCommandWrapper WithParameter(string name, object value) {
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name", "Name cannot be null or empty.");
                if (value == null) throw new ArgumentNullException("value");
                if (Parameters.ContainsKey(name)) {
                    throw new DuplicateNameException("Dublicate parameter '" + name + "'.");
                }
                Parameters[name] = value;
                return this;
            }

            public DbCommandWrapper WithTransaction(IDbTransaction transaction) {
                if (transaction == null) throw new ArgumentNullException("transaction");
                Transaction = transaction;
                return this;
            }

            public DbCommandWrapper WithTimeout(int timeout) {
                Timeout = timeout;
                return this;
            }

            public DbCommandWrapper BeforeExecution(Action<IDbCommand> handler) {
                if (handler == null) throw new ArgumentNullException("handler");
                BeforeExecutionHandler = handler;
                return this;
            }

            public DbCommandWrapper OnCompleted(Action<IDbCommand, object> handler) {
                if (handler == null) throw new ArgumentNullException("handler");
                OnCompletedHandler = handler;
                return this;
            }

            public DbCommandWrapper OnException(Func<Exception, IDbCommand, bool> handler) {
                if (handler == null) throw new ArgumentNullException("handler");
                OnExceptionHandler += handler;
                return this;
            }

            public DbCommandWrapper OnException(Action<Exception, IDbCommand> handler) {
                if (handler == null) throw new ArgumentNullException("handler");
                OnException((e, cmd) => {
                    handler(e, cmd);
                    return false;
                });
                return this;
            }

            public DbCommandWrapper OnException(Func<Exception, bool> handler) {
                if (handler == null) throw new ArgumentNullException("handler");
                OnException((e, cmd) => handler(e));
                return this;
            }

            public DbCommandWrapper OnException(Action<Exception> handler) {
                if (handler == null) throw new ArgumentNullException("handler");
                OnException((e, cmd) => {
                    handler(e);
                    return false;
                });
                return this;
            } 
            #endregion

            #region Execution

            public T Execute<T>(Func<IDbCommand, T> executeHandler) {
                if (executeHandler == null) throw new ArgumentNullException("executeHandler");
                using (var cmd = Connection.CreateCommand()) {
                    cmd.Transaction = Transaction;
                    cmd.CommandType = CommandType;
                    cmd.CommandText = Text;
                    cmd.CommandTimeout = Timeout;
                    cmd.AddInputParameters(Parameters);
                    if (BeforeExecutionHandler != null) {
                        BeforeExecutionHandler(cmd);
                    }
                    if (Connection.State == ConnectionState.Closed) {
                        Connection.Open();
                    }
                    try {
                        var result = executeHandler(cmd);
                        if (OnCompletedHandler != null) {
                            OnCompletedHandler(cmd, result);
                        }
                        return result;
                    }
                    catch (Exception e) {
                        if (OnExceptionHandler == null) {
                            throw;
                        }
                        if (!OnExceptionHandler(e, cmd)) {
                            throw;
                        }
                        return default(T);
                    }
                }
            }

            public void ExecuteNonQuery() {
                Execute<object>(cmd => {
                    cmd.ExecuteNonQuery();
                    return null;
                });
            }

            public object ExecuteScalar() {
                return Execute(cmd => cmd.ExecuteScalar());
            }

            public void ExecuteReader(Action<IDataReader, IDataParameterCollection> callback) {
                if (callback == null) throw new ArgumentNullException("callback");
                Execute<object>(cmd => {
                    using (var reader = cmd.ExecuteReader()) {
                        callback(reader, cmd.Parameters);
                    }
                    return null;
                });
            }

            public void ExecuteReader(Action<IDataReader> callback) {
                ExecuteReader((reader, parameters) => callback(reader));
            }

            public IEnumerable<T> ExecuteAndReadAll<T>(MapperWithIndex<T> mapper) {
                if (mapper == null) throw new ArgumentNullException("mapper");
                return Execute(cmd => {
                    using (var reader = cmd.ExecuteReader()) {
                        return reader.ReadAll(mapper);
                    }
                });
            }

            public IEnumerable<T> ExecuteAndReadAll<T>(Mapper<T> mapper) {
                return ExecuteAndReadAll((reader, index) => mapper(reader));
            }

            public T ExecuteAndReadFirstOrDefault<T>(Mapper<T> mapper) {
                if (mapper == null) throw new ArgumentNullException("mapper");
                return Execute(cmd => {
                    using (var reader = cmd.ExecuteReader()) {
                        return reader.ReadFirstOrDefault(mapper);
                    }
                });
            }

            #endregion

            #region Static
            public static DbCommandWrapper CreateSp(IDbConnection connection, string text) {
                return new DbCommandWrapper(connection, CommandType.StoredProcedure, text);
            }

            public static DbCommandWrapper CreateText(IDbConnection connection, string text) {
                return new DbCommandWrapper(connection, CommandType.Text, text);
            } 
            #endregion
        }

    }
}