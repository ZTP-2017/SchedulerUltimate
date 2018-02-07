using Akka.Actor;
using Scheduler.Mailer;
using System;
using Scheduler.Logger;
using Scheduler.Messages.Request;

namespace Scheduler.Actors
{
    public class SendActor : ReceiveActor
    {
        private readonly IMailService _mailService;
        private readonly IActorRef _logActor;

        public SendActor(IMailService mailService, ILoggerService loggerService)
        {
            _mailService = mailService;

            var logActorProps = Props.Create<LogActor>(loggerService);
            _logActor = Context.ActorOf(logActorProps);

            Receive<EmailRequestMessage>(x => Handle(x));
        }

        public void Handle(EmailRequestMessage message)
        {
            try
            {
                _mailService.SendEmail(message.Email, message.Body, message.Subject);

                _logActor.Tell(new LogRequestMessage(LoggerService.LogType.Info,
                    "Message was sent", null));
            }
            catch (Exception ex)
            {
                _logActor.Tell(new LogRequestMessage(LoggerService.LogType.Warning,
                    "Message not sent", ex));
            }
        }
    }
}
