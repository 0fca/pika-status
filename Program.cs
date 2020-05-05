using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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

            var port = ReadPortFromStdIn(args);

            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(l =>
                {
                    l.AddSerilog();
                })
                .UseStartup<Startup>()
                .UseSockets()
                .UseConfiguration(configuration)
                .UseUrls($"http://localhost:{port}")
                .Build();
            host.Run();
        }

        private static int ReadPortFromStdIn(IReadOnlyList<string> args)
        {
            var port = 5001;
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