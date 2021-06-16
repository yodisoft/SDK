using Microsoft.Extensions.Logging;

namespace YodiSoft.Log.Core
{
    /// <summary>Log</summary>
    internal interface IYodiLog
    {
        /// <summary>Add message to log</summary>
        /// <param name="levelId">Message level log</param>
        /// <param name="msg">Message</param>
        /// <param name="category">Message category</param>
        void Add(LogLevel levelId, string msg, string category);

        /// <summary>Stop system log</summary>
        void Stop();
    }
}