namespace Scheduler.Messages.Request
{
    public class DataRequestMessage
    {
        public string Path { get; }

        public DataRequestMessage(string path)
        {
            Path = path;
        }
    }
}
