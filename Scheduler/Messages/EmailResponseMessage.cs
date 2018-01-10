namespace Scheduler.Messages
{
    public class EmailResponseMessage
    {
        public string Message { get; }

        public EmailResponseMessage(string message)
        {
            Message = message;
        }
    }
}
