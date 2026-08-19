using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace PikaStatus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/pika-status-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                var port = 12000;

                var host = WebHost.CreateDefaultBuilder(args)
                    .ConfigureLogging(l =>
                    {
                        l.AddSerilog();
                    })
                    .UseStartup<Startup>()
                    .UseSockets()
                    .UseConfiguration(configuration)
                    .UseUrls($"http://status.cloud.localhost:{port}")
                    .Build();

                host.Run();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
