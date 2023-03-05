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

            var port = ReadPortFromStdIn(args);

            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(l =>
                {
                    l.AddSerilog();
                })
                .UseStartup<Startup>()
                .UseSockets()
                .UseConfiguration(configuration)
                .UseUrls($"http://status.cloud.localhost:{port}", $"https://status.cloud.localhost:{port+1}")
                .Build();
            host.Run();
        }

        private static int ReadPortFromStdIn(IReadOnlyList<string> args)
        {
            var port = 8001;
            try
            {
                port = int.Parse(args[0]);
            }
            catch
            {
                // ignored
            }
            return port;
        }
    }
}