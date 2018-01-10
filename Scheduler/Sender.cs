using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Scheduler.Actors;
using Scheduler.Data.Interfaces;
using Scheduler.Interfaces;
using Scheduler.Mailer.Interfaces;
using Scheduler.Messages;
using Scheduler.Models;
using Serilog;

namespace Scheduler
{
    public class Sender : ISender
    {
        private static int skipMessagesCount;
        private List<Message> _messages;
        private readonly ActorSystem _actorSystem;
        private readonly IMailService _mailService;

        public Sender(IMailService mailService, IDataService dataService)
        {
            _mailService = mailService;
            _actorSystem =  ActorSystem.Create("MailActor");
            _messages = dataService.GetAllMessages<Message>(Settings.DataFilePath);
        }

        public void SendEmails()
        {
            try
            {
                Log.Information("Get data from file");
                var messages = GetMessages();

                var actor = _actorSystem.ActorOf(Props.Create<EmailActor>(_mailService), "EmailActor");

                messages.ForEach(async x => Log.Information(
                    (await actor.Ask<EmailResponseMessage>(new EmailRequestMessage(x.Email, x.Body, x.Subject))).Message)
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Messages send error: ");
            }
        }

        public void SetSkipValue(int value)
        {
            skipMessagesCount = value;
        }

        private List<Message> GetMessages()
        {
            var messages = _messages.Skip(skipMessagesCount).Take(100).ToList();

            skipMessagesCount += messages.Count;

            return messages;
        }
    }
}
