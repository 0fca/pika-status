using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
    }
}
