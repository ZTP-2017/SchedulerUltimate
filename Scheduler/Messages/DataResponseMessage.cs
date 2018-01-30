using System.Collections.Generic;
using Scheduler.Models;

namespace Scheduler.Messages
{
    public class DataResponseMessage
    {
        public IEnumerable<Message> Messages { get; }

        public DataResponseMessage(IEnumerable<Message> messages)
        {
            Messages = messages;
        }
    }
}
