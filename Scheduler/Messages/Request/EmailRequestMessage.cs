namespace Scheduler.Messages.Request
{
    public class EmailRequestMessage
    {
        public string Email { get; }
        public string Subject { get; }
        public string Body { get; }

        public EmailRequestMessage(string email, string subject, string body)
        {
            Email = email;
            Subject = subject;
            Body = body;
        }
    }
}
