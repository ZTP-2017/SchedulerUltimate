using System;
using Scheduler.Logger;

namespace Scheduler.Messages.Request
{
    public class LogRequestMessage
    {
        public LoggerService.LogType Type { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public LogRequestMessage(LoggerService.LogType logType, string message, Exception exception)
        {
            Type = logType;
            Message = message;
            Exception = exception;
        }
    }
}
