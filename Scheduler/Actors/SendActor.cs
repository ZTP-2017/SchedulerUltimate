using Akka.Actor;
using Scheduler.Mailer;
using Scheduler.Messages;
using System;

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
            var response = $"Message {message.Subject} to {message.Email} was sent";
            try
            {
                _mailService.SendEmail(message.Email, message.Body, message.Subject);
            }
            catch (Exception ex)
            {
                response = $"Message not sent - {ex.Message}";
            }
           

            Sender.Tell(new EmailResponseMessage(response), Self);
        }
    }
}
