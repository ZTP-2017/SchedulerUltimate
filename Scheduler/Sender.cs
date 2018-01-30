using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Scheduler.Actors;
using Scheduler.Data;
using Scheduler.Interfaces;
using Scheduler.Mailer;
using Scheduler.Messages;
using Scheduler.Models;
using Serilog;

namespace Scheduler
{
    public class Sender : ISender
    {
        private static int _sentMessagesCount;
        private static IEnumerable<Message> _messages;

        private readonly IActorRef _loadDataActor;
        private readonly IActorRef _messageSendActor;

        public Sender(IMailService mailService, IDataService dataService)
        {
            var dataLoaderProps = Props.Create<DataActor>(dataService);
            var messagesSenderProps = Props.Create<SendActor>(mailService);

            var actorSystem = ActorSystem.Create("Scheduler");

            _loadDataActor = actorSystem.ActorOf(dataLoaderProps);
            _messageSendActor = actorSystem.ActorOf(messagesSenderProps);
        }

        public async void LoadAllMessagesFromFile(string path)
        {
            var result = await _loadDataActor.Ask<DataResponseMessage>(new DataRequestMessage(path));
            _messages = result.Messages;
        }

        public void SendEmails()
        {
            try
            {
                var messages = GetMessages(100);

                messages.ForEach(async x =>
                {
                    var response = (await _messageSendActor
                        .Ask<EmailResponseMessage>(new EmailRequestMessage(
                            x.Email, x.Body, x.Subject))).Message;

                    Log.Information(response);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Messages send error: ");
            }
        }

        public void SetSkipValue(int value)
        {
            _sentMessagesCount = value;
        }

        private List<Message> GetMessages(int count)
        {
            var messages = _messages.Skip(_sentMessagesCount).Take(count).ToList();

            _sentMessagesCount += messages.Count;

            return messages;
        }
    }
}
