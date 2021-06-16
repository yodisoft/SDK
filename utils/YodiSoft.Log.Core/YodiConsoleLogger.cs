using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace YodiSoft.Log.Core
{
    internal class YodiConsoleLogger : ILogger
    {
        private readonly LogLevel _minLevel;

        public string Name { get; }

        private static readonly IYodiLog YodiLog = new YodiLog();

        public YodiConsoleLogger(string name, LogLevel minLevel = LogLevel.Information)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
            _minLevel = minLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
//            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

//            var message = formatter(state, exception);
//            if (!string.IsNullOrEmpty(message) || exception != null)
//            {
//                WriteMessage(logLevel, Name, eventId.Id, message, exception);
//            }
            if (exception != null)
            {
                YodiLog.Add(logLevel, ToLog(exception, state.ToString()), Name);
            }
            else
            {
                YodiLog.Add(logLevel, state.ToString(), Name);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
//            if (state == null) throw new ArgumentNullException(nameof(state));
//
//            return ConsoleLogScope.Push(Name, state);
            return null;
        }

//        public virtual ConsoleColor GetColor(LogLevel logLevel)
//        {
//            switch (logLevel)
//            {
//                case LogLevel.Critical:
//                    return ConsoleColor.Red;
//                case LogLevel.Error:
//                    return ConsoleColor.Magenta;
//                case LogLevel.Warning:
//                    return ConsoleColor.DarkYellow;
//                case LogLevel.Information:
//                    return ConsoleColor.DarkGreen;
//                case LogLevel.Debug:
//                    return ConsoleColor.Gray;
//                case LogLevel.Trace:
//                    return ConsoleColor.DarkGray;
//                default:
//                    return ConsoleColor.White;
//            }
//        }

//        public virtual string GetMessage(string msg, Exception exception)
//        {
//            return $"{DateTime.Now:HH:mm:ss}> {msg}";
//        }

//        public virtual bool IsEnabled(string msg, LogLevel level)
//        {
//            return !string.IsNullOrEmpty(msg) && IsEnabled(level);
//        }

//        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
//        {
//            //if (!IsEnabled(message, logLevel)) return;
//
//            var logLevelColor = GetColor(logLevel);
//            lock (_syncObj)
//            {
//                var color = Console.ForegroundColor;
//                Console.ForegroundColor = logLevelColor;
//                Console.WriteLine(GetMessage(message, exception));
//                Console.ForegroundColor = color;
//            }
//        }

        private static string ToLog(Exception ex, string errorCode)
        {
            var sb = new StringBuilder();
            sb.Append("#");
            sb.Append(errorCode);
            sb.Append(" ");
            sb.Append(ex.Message);
            var inner = ex.InnerException;
            while (inner != null)
            {
                if (!string.IsNullOrEmpty(inner.Message))
                {
                    sb.Append("\r\n" + inner.Message);
                }
                inner = inner.InnerException;
            }
            sb.Append("\r\n" + ex.StackTrace);
            return sb.ToString();
        }
    }
}