using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace YodiSoft.Log.Core
{
    /// <summary>YodiConsoleLoggerProvider</summary>
    public class YodiConsoleLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _minLevel;

        private readonly ConcurrentDictionary<string, YodiConsoleLogger> _loggers = new ConcurrentDictionary<string, YodiConsoleLogger>();

        /// <summary>YodiConsoleLoggerProvider</summary>
        public YodiConsoleLoggerProvider(LogLevel minLevel = LogLevel.Debug)
        {
            _minLevel = minLevel;
        }

        /// <summary>Dispose</summary>
        public void Dispose()
        {
            _loggers.Clear();
        }

        /// <summary>CreateLogger</summary>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, (name) => new YodiConsoleLogger(name, _minLevel));
        }
    }
}