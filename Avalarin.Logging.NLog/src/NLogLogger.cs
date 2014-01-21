using System;
using NLog;

namespace Avalarin.Logging.NLog {
    public sealed class NLogLogger : ILogger {
        private readonly Logger _logger;

        public NLogLogger(Logger logger) {
            _logger = logger;
        }

        public void DebugException(string message, Exception exception) {
            _logger.DebugException(message, exception);
        }

        public void Debug(IFormatProvider formatProvider, string message, params object[] args) {
            _logger.Debug(formatProvider, message, args);
        }

        public void Debug(string message, params object[] args) {
            _logger.Debug(message, args);
        }

        public void InfoException(string message, Exception exception) {
            _logger.InfoException(message, exception);
        }

        public void Info(IFormatProvider formatProvider, string message, params object[] args) {
            _logger.Info(formatProvider, message, args);
        }

        public void Info(string message, params object[] args) {
            _logger.Info(message, args);
        }

        public void WarnException(string message, Exception exception) {
            _logger.WarnException(message, exception);
        }

        public void Warn(IFormatProvider formatProvider, string message, params object[] args) {
            _logger.Warn(formatProvider, message, args);
        }

        public void Warn(string message, params object[] args) {
            _logger.Warn(message, args);
        }

        public void ErrorException(string message, Exception exception) {
            _logger.ErrorException(message, exception);
        }

        public void Error(IFormatProvider formatProvider, string message, params object[] args) {
            _logger.Error(formatProvider, message, args);
        }

        public void Error(string message, params object[] args) {
            _logger.Error(message, args);
        }

        public void FatalException(string message, Exception exception) {
            _logger.FatalException(message, exception);
        }

        public void Fatal(IFormatProvider formatProvider, string message, params object[] args) {
            _logger.Fatal(formatProvider, message, args);
        }

        public void Fatal(string message, params object[] args) {
            _logger.Fatal(message, args);
        }

        public void Debug(object value) {
            _logger.Debug(value);
        }

        public void Debug(string message, object arg1, object arg2) {
            _logger.Debug(message, arg1, arg2);
        }

        public void Debug(string message, object arg1, object arg2, object arg3) {
            _logger.Debug(message, arg1, arg2, arg3);
        }

        public void Debug(string message, object argument) {
            _logger.Debug(message, argument);
        }

        public void Info(object value) {
            _logger.Info(value);
        }

        public void Info(string message, object arg1, object arg2) {
            _logger.Info(message, arg1, arg2);
        }

        public void Info(string message, object arg1, object arg2, object arg3) {
            _logger.Info(message, arg1, arg2, arg3);
        }

        public void Info(string message, object argument) {
            _logger.Info(message, argument);
        }

        public void Warn(object value) {
            _logger.Warn(value);
        }

        public void Warn(string message, object arg1, object arg2) {
            _logger.Warn(message, arg1, arg2);
        }

        public void Warn(string message, object arg1, object arg2, object arg3) {
            _logger.Warn(message, arg1, arg2, arg3);
        }

        public void Warn(string message, object argument) {
            _logger.Warn(message, argument);
        }

        public void Error(object value) {
            _logger.Error(value);
        }

        public void Error(string message, object arg1, object arg2) {
            _logger.Error(message, arg1, arg2);
        }

        public void Error(string message, object arg1, object arg2, object arg3) {
            _logger.Error(message, arg1, arg2, arg3);
        }

        public void Error(string message, object argument) {
            _logger.Error(message, argument);
        }

        public void Fatal(object value) {
            _logger.Fatal(value);
        }

        public void Fatal(string message, object arg1, object arg2) {
            _logger.Fatal(message, arg1, arg2);
        }

        public void Fatal(string message, object arg1, object arg2, object arg3) {
            _logger.Fatal(message, arg1, arg2, arg3);
        }

        public void Fatal(string message, object argument) {
            _logger.Fatal(message, argument);
        }

        public bool IsDebugEnabled {
            get { return _logger.IsDebugEnabled; }
        }

        public bool IsInfoEnabled {
            get { return _logger.IsInfoEnabled; }
        }

        public bool IsWarnEnabled {
            get { return _logger.IsWarnEnabled; }
        }

        public bool IsErrorEnabled {
            get { return _logger.IsErrorEnabled; }
        }

        public bool IsFatalEnabled {
            get { return _logger.IsFatalEnabled; }
        }

    }
}