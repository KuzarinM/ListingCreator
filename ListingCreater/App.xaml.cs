using ListingCreater.Storages.Implementations;
using ListingCreater.Storages.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using ServiceStationBusinessLogic.OfficePackage;
using ServiceStationBusinessLogic.OfficePackage.Implements;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LogLevel = NLog.LogLevel;

namespace ListingCreater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = new ConfigurationBuilder();

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            ConfigureLogger(services);
            services.AddTransient(typeof(MainWindow));
            services.AddScoped<AbstractSaveToWord, SaveToWord>();
            services.AddSingleton<IConfigurationStorage, ConfigurationStorage>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }

        private static void ConfigureLogger(IServiceCollection services)
        {
            var nLogConfig = new LoggingConfiguration();
            var logConsole = new ConsoleTarget();
            var blackhole = new NullTarget();

            var logFile = new FileTarget()
            {
                FileName = "${basedir}/logs/${shortdate}_logs.log"
            };

            nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, blackhole, "Microsoft.AspNetCore.*", true);
            nLogConfig.AddRule(LogLevel.Info, LogLevel.Warn, logFile, "Microsoft.EntityFrameworkCore.*", true);
            nLogConfig.AddRule(LogLevel.Info, LogLevel.Warn, logFile, "Microsoft.AspNetCore.*", true);
            nLogConfig.AddRule(LogLevel.Info, LogLevel.Warn, logFile, "System.Net.Http.HttpClient.Refit.*", true);
            nLogConfig.AddRule(LogLevel.Info, LogLevel.Error, logConsole);
            nLogConfig.AddRule(LogLevel.Debug, LogLevel.Error, logFile);

            services.AddLogging(m =>
            {
                m.AddNLog(nLogConfig);
            });
        }
    }
}
