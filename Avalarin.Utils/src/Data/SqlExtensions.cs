using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Avalarin.Utils;

namespace Avalarin.Data {
    public static class SqlExtensions {
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
            private readonly IDictionary<string, IOutputParameter> _outputParameters = new Dictionary<string, IOutputParameter>();

            private IDbConnection Connection { get; set; }
            public CommandType CommandType { get; private set; }
            public string Text { get; private set; }

            private IDictionary<string, object> Parameters { get { return _parameters; } }
            private IDictionary<string, IOutputParameter> OutputParameters { get { return _outputParameters; } }
            private IDbTransaction Transaction { get; set; }
            private int? Timeout { get; set; }

            private Action<IDbCommand> BeforeExecutionHandler { get; set; }
            private Action<IDbCommand, object> OnCompletedHandler { get; set; }
            private Action<Exception, IDbCommand> OnExceptionHandler { get; set; }

            private DbCommandWrapper(IDbConnection connection, CommandType commandType, string text) {
                if (connection == null) throw new ArgumentNullException("connection");
                if (text == null) throw new ArgumentNullException("text");
                Connection = connection;
                CommandType = commandType;
                Text = text;

                Timeout = null;
            }

            #region Modification
            public DbCommandWrapper WithParameters(object parameters) {
                if (parameters == null) throw new ArgumentNullException("parameters");
                parameters.EachProperties((name, value) => {
                    CheckParameterForDuplication(name);
                    Parameters[name] = value;
                });
                return this;
            }

            public DbCommandWrapper WithParameters(IDictionary<string, object> parameters) {
                if (parameters == null) throw new ArgumentNullException("parameters");
                foreach (var key in parameters.Keys) {
                    var value = parameters[key];
                    CheckParameterForDuplication(key);
                    Parameters[key] = value;
                }
                return this;
            }

            public DbCommandWrapper WithParameter(string name, object value) {
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name", "Name cannot be null or empty.");
                if (value == null) throw new ArgumentNullException("value");
                CheckParameterForDuplication(name);
                Parameters[name] = value;
                return this;
            }

            public DbCommandWrapper WithOutputParameter<T>(string name, Action<T> setter) {
                return WithOutputParameter<T>(name, 0, setter);
            }

            public DbCommandWrapper WithOutputParameter<T>(string name, int size, Action<T> setter) {
                CheckParameterForDuplication(name);
                OutputParameters.Add(name, new OutputParameter<T>(name, size, setter));
                return this;
            }

            public DbCommandWrapper WithOutputParameter<T>(string name, DbType type, Action<T> setter) {
                return WithOutputParameter<T>(name, type, 0, setter);
            }

            public DbCommandWrapper WithOutputParameter<T>(string name, DbType type, int size, Action<T> setter) {
                CheckParameterForDuplication(name);
                OutputParameters.Add(name, new OutputParameter<T>(name, type, size, setter));
                return this;
            }

            public DbCommandWrapper WithOutputParameter(string name, DbType type, Action<object> setter) {
                return WithOutputParameter(name, type, 0, setter);
            }

            public DbCommandWrapper WithOutputParameter(string name, DbType type, int size, Action<object> setter) {
                CheckParameterForDuplication(name);
                OutputParameters.Add(name, new OutputParameter(name, type, size, setter));
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

            public DbCommandWrapper OnException(Action<Exception, IDbCommand> handler) {
                if (handler == null) throw new ArgumentNullException("handler");
                OnExceptionHandler += handler;
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
                    if (Timeout.HasValue) {
                        cmd.CommandTimeout = Timeout.Value;
                    }
                    cmd.AddInputParameters(Parameters);
                    foreach (var outputParameter in OutputParameters.Values) {
                        cmd.AddOutputParameter(outputParameter.Name, outputParameter.DbType, outputParameter.Size);
                    }
                    if (BeforeExecutionHandler != null) {
                        BeforeExecutionHandler(cmd);
                    }
                    if (Connection.State == ConnectionState.Closed) {
                        Connection.Open();
                    }
                    try {
                        var result = executeHandler(cmd);
                        foreach (var outputParameter in OutputParameters.Values) {
                            var parameter = (DbParameter)cmd.Parameters[outputParameter.Name];
                            outputParameter.SetValue(parameter.Value);
                        }
                        if (OnCompletedHandler != null) {
                            OnCompletedHandler(cmd, result);
                        }
                        return result;
                    }
                    catch (Exception e) {
                        if (OnExceptionHandler != null) {
                            OnExceptionHandler(e, cmd);
                        }
                        throw;
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

            private void CheckParameterForDuplication(string name) {
                if (Parameters.ContainsKey(name) || OutputParameters.ContainsKey(name)) {
                    throw new DuplicateNameException("Duplicate parameter '" + name + "'.");
                }
            }
        }
    }
}