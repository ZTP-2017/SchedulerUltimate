using Akka.Actor;
using Scheduler.Data;
using Scheduler.Messages.Request;
using Scheduler.Messages.Response;
using Scheduler.Models;

namespace Scheduler.Actors
{
    public class DataActor : ReceiveActor
    {
        private readonly IDataService _dataService;

        public DataActor(IDataService dataService)
        {
            _dataService = dataService;

            Receive<DataRequestMessage>(x => Handle(x));
        }

        public void Handle(DataRequestMessage message)
        {
            var result = _dataService.GetAllMessages<Message>(message.Path);

            Sender.Tell(new DataResponseMessage(result));
        }
    }
}
