using System;
using Akka.Actor;
using Microsoft.Owin.Hosting;
using Scheduler.Actors;
using Scheduler.Data;
using Scheduler.Logger;
using Scheduler.Mailer;
using Scheduler.Messages.Request;

namespace Scheduler
{
    public class Scheduler
    {
        private readonly IMailService _mailService;
        private readonly IDataService _dataService;
        private readonly ILoggerService _loggerService;
        private readonly Settings _settings;

        private IActorRef _logActor;
        private IDisposable _webApp;

        private ActorSystem _actorSystem;

        public Scheduler(IMailService mailService, IDataService dataService, ILoggerService loggerService, Settings settings)
        {
            _settings = settings;
            _dataService = dataService;
            _mailService = mailService;
            _loggerService = loggerService;
        }

        public void Start()
        {
            try
            {
                _webApp = WebApp.Start<Startup>(_settings.HostingUrl);

                _actorSystem = ActorSystem.Create("Scheduler");

                var logActorProps = Props.Create<LogActor>(_loggerService);
                _logActor = _actorSystem.ActorOf(logActorProps);

                var senderActorProps = Props.Create<SenderActor>(_dataService, _mailService, _loggerService, _settings.DataFilePath);
                var senderActor = _actorSystem.ActorOf(senderActorProps);
                senderActor.Tell(new SenderRequestMessage(_dataService, _mailService, _settings.DataFilePath));

                _logActor.Tell(new LogRequestMessage(LoggerService.LogType.Info, "Start service", null));
            }
            catch (Exception ex)
            {
                _logActor.Tell(new LogRequestMessage(LoggerService.LogType.Error, "Start service error", ex));
            }
        }

        public void Stop()
        {
            try
            {
                _webApp.Dispose();
                _actorSystem.Terminate().Wait();
                _actorSystem.Dispose();

                _logActor.Tell(new LogRequestMessage(LoggerService.LogType.Info, "Stop service", null));
            }
            catch (Exception ex)
            {
                _logActor.Tell(new LogRequestMessage(LoggerService.LogType.Error, "Stop service error", ex));
            }
        }
    }
}
