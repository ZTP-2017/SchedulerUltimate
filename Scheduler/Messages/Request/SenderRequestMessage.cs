using Scheduler.Data;
using Scheduler.Mailer;

namespace Scheduler.Messages.Request
{
    public class SenderRequestMessage
    {
        public IDataService DataService { get; }
        public IMailService MailService { get; }
        public string FilePath { get; }

        public SenderRequestMessage(IDataService dataService, IMailService mailService, string filePath)
        {
            DataService = dataService;
            MailService = mailService;
            FilePath = filePath;
        }
    }
}
