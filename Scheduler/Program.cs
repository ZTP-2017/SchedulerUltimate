﻿using Autofac;
using Scheduler.Mailer;
using Scheduler.Data;
using System;
using System.Configuration;
using Hangfire;
using Scheduler.Logger;
using Topshelf;
using Topshelf.Autofac;

namespace Scheduler
{
    public class Program
    {
        public static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            ConfigureAutofac();
            ConfigureHangfire();

            ConfigureTopshelf();

            Console.ReadKey();
        }

        private static void ConfigureAutofac()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Scheduler>();

            builder.RegisterModule<MailerModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<LoggerModule>();
            
            builder.RegisterInstance(GetAppSettings());

            Container = builder.Build();
        }

        private static void ConfigureHangfire()
        {
            GlobalConfiguration.Configuration.UseAutofacActivator(Container);
        }

        private static void ConfigureTopshelf()
        {
            HostFactory.Run(configure =>
            {
                configure.UseAutofacContainer(Container);
                configure.Service<Scheduler>(service =>
                {
                    service.ConstructUsingAutofacContainer();
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                
                configure.SetServiceName("MailerService");
                configure.SetDisplayName("Mailer Service");
                configure.SetDescription("Mailer service ZTP");
                configure.RunAsLocalService();
            });
        }

        private static Settings GetAppSettings()
        {
            return new Settings
            {
                HostingUrl = ConfigurationManager.AppSettings["hostingUrl"],
                DataFilePath = ConfigurationManager.AppSettings["messagesFilePath"]
            };
        }
    }
}
