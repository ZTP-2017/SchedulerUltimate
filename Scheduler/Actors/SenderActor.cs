using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Scheduler.Data;
using Scheduler.Logger;
using Scheduler.Mailer;
using Scheduler.Messages.Request;
using Scheduler.Messages.Response;
using Scheduler.Models;

namespace Scheduler.Actors
{
    public class SenderActor : ReceiveActor
    {
        private readonly IActorRef _dataActor;
        private readonly IActorRef _sendActor;
        private readonly string _filePath;
        

        public SenderActor(IDataService dataService, IMailService mailService, ILoggerService loggerService, string filePath)
        {
            _filePath = filePath;

            var dataActorProps = Props.Create<DataActor>(dataService);
            _dataActor = Context.ActorOf(dataActorProps);

            var sendActorProps = Props.Create<SendActor>(mailService, loggerService);
            _sendActor = Context.ActorOf(sendActorProps);

            Receive<SenderRequestMessage>(x => Handle(x));
        }

        public async void Handle(SenderRequestMessage message)
        {
            var messages = await GetMessagesFromFile();

            messages.ForEach(x =>
            {
                _sendActor.Tell(new EmailRequestMessage(x.Email, x.Body, x.Subject));
            });
        }

        private async Task<List<Message>> GetMessagesFromFile()
        {
            var result = await _dataActor.Ask<DataResponseMessage>(new DataRequestMessage(_filePath));

            return result.Messages.ToList();
        }
    }
}
