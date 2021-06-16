using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace YodiSoft.Log.Core
{
    internal class YodiLog : IYodiLog
    {
        private const int BaseInterval = 250;

        private readonly AutoResetEvent _stopEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent _commandStopEvent = new AutoResetEvent(false);

        private StringBuilder _tecBuffer = new StringBuilder();

        private readonly object _lockObj = new object();

        public YodiLog()
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorkerDoWork;
            backgroundWorker.RunWorkerAsync();
        }

        public void Add(LogLevel levelId, string msg, string category = "")
        {
            var a = BuildLine(DateTime.Now, levelId, msg, Thread.CurrentThread.ManagedThreadId, category);
            lock (_lockObj)
            {
                _tecBuffer.AppendLine(a);
            }
        }

        public void Stop()
        {
            Console.WriteLine("Log off");
            _commandStopEvent.Set();
            _stopEvent.WaitOne(500);
        }

        private void Exec()
        {
            StringBuilder tmp;
            lock (_lockObj)
            {
                if (_tecBuffer.Length == 0)
                {
                    return;
                }
                tmp = _tecBuffer;
                _tecBuffer = new StringBuilder();
            }
            Console.Write(tmp.ToString());
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var interval = BaseInterval;
                while (true)
                {
                    if (_commandStopEvent.WaitOne(interval))
                    {
                        Exec();
                        break;
                    }
                    Exec();
                }
            }
            catch
            {
                //
            }
            _stopEvent.Set();
        }

        private static string BuildLine(DateTime dt, LogLevel levelId, string msg, int taskId, string category)
        {
            var h = new StringBuilder();
            if (dt.Day < 10)
            {
                h.Append("0");
            }
            h.Append(dt.Day);
            h.Append(".");
            if (dt.Month < 10)
            {
                h.Append("0");
            }
            h.Append(dt.Month);
            h.Append(".");
            h.Append(dt.Year);
            h.Append(" ");
            if (dt.Hour < 10)
            {
                h.Append("0");
            }
            h.Append(dt.Hour);
            h.Append(":");
            if (dt.Minute < 10)
            {
                h.Append("0");
            }
            h.Append(dt.Minute);
            h.Append(":");
            if (dt.Second < 10)
            {
                h.Append("0");
            }
            h.Append(dt.Second);
            h.Append(".");
            if (dt.Millisecond < 10)
            {
                h.Append("00");
            }
            else
            {
                if (dt.Millisecond < 100)
                {
                    h.Append("0");
                }
            }
            h.Append(dt.Millisecond);
            h.Append(" [");
            if (taskId < 10)
            {
                h.Append(" ");
            }
            h.Append(taskId);
            h.Append("] [");
            h.Append(levelId.ToString()[0]);
            if (!string.IsNullOrEmpty(category))
            {
                h.Append("] [");
                h.Append(category);
            }
            h.Append("] ");
            h.Append(msg);
            return h.ToString();
        }
    }
}