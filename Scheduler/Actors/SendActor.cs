using Akka.Actor;
using Scheduler.Mailer;
using Scheduler.Messages;

namespace Scheduler.Actors
{
    public class SendActor : ReceiveActor
    {
        private readonly IMailService _mailService;

        public SendActor(IMailService mailService)
        {
            _mailService = mailService;
            Receive<EmailRequestMessage>(x => Handle(x));
        }

        public void Handle(EmailRequestMessage message)
        {
            _mailService.SendEmail(message.Email, message.Body, message.Subject);

            Sender.Tell(new EmailResponseMessage($"Message {message.Subject} to {message.Email} was sent"), Self);
        }
    }
}
