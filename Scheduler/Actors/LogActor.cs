using Akka.Actor;
using Scheduler.Logger;
using Scheduler.Messages.Request;

namespace Scheduler.Actors
{
    public class LogActor : ReceiveActor
    {
        private readonly ILoggerService _loggerService;

        public LogActor(ILoggerService loggerService)
        {
            _loggerService = loggerService;

            Receive<LogRequestMessage>(x => Handle(x));
        }

        public void Handle(LogRequestMessage message)
        {
            _loggerService.CreateLog(message.Type, message.Message, message.Exception);
        }
    }
}
