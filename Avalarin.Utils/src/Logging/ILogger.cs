using System;

namespace Avalarin.Logging {
    public interface ILogger {

        bool IsDebugEnabled { get; }
        
        bool IsInfoEnabled { get; }
        
        bool IsWarnEnabled { get; }
        
        bool IsErrorEnabled { get; }
        
        bool IsFatalEnabled { get; }

        void Debug(object message);

        void DebugException(string message, Exception exception);

        void Debug(string format, params object[] args);

        void Debug(string format, object arg0);

        void Debug(string format, object arg0, object arg1);

        void Debug(string format, object arg0, object arg1, object arg2);

        void Debug(IFormatProvider provider, string format, params object[] args);

        void Info(object message);

        void InfoException(string message, Exception exception);

        void Info(string format, params object[] args);

        void Info(string format, object arg0);

        void Info(string format, object arg0, object arg1);

        void Info(string format, object arg0, object arg1, object arg2);

        void Info(IFormatProvider provider, string format, params object[] args);

        void Warn(object message);

        void WarnException(string message, Exception exception);

        void Warn(string format, params object[] args);

        void Warn(string format, object arg0);

        void Warn(string format, object arg0, object arg1);

        void Warn(string format, object arg0, object arg1, object arg2);

        void Warn(IFormatProvider provider, string format, params object[] args);

        void Error(object message);

        void ErrorException(string message, Exception exception);

        void Error(string format, params object[] args);

        void Error(string format, object arg0);

        void Error(string format, object arg0, object arg1);

        void Error(string format, object arg0, object arg1, object arg2);

        void Error(IFormatProvider provider, string format, params object[] args);

        void Fatal(object message);

        void FatalException(string message, Exception exception);

        void Fatal(string format, params object[] args);

        void Fatal(string format, object arg0);

        void Fatal(string format, object arg0, object arg1);

        void Fatal(string format, object arg0, object arg1, object arg2);

        void Fatal(IFormatProvider provider, string format, params object[] args);
    }
}
