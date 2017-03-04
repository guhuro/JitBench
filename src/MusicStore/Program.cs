using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MusicStore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // TODO: This is a workaround for https://github.com/dotnet/corefx/issues/17166
            // SQLClient does not connect to LocalDB without it
            AppContext.SetSwitch("System.Data.SqlClient.UseLegacyNetworkingOnWindows", true);
            
            var totalTime = Stopwatch.StartNew();

            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            var builder = new WebHostBuilder()
                .UseConfiguration(config)
                .UseIISIntegration()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup("MusicStore")
                .UseKestrel();

            var host = builder.Build();

            host.Start();

            totalTime.Stop();
            var serverStartupTime = totalTime.ElapsedMilliseconds;
            Console.WriteLine("Server started in {0}ms", serverStartupTime);
            Console.WriteLine();

            long r;
            using (var client = new HttpClient())
            {
                Console.WriteLine("Starting request to http://localhost:5000");
                var requestTime = Stopwatch.StartNew();
                var response = client.GetAsync("http://localhost:5000").Result;
                requestTime.Stop();
                var firstRequestTime = requestTime.ElapsedMilliseconds;

                Console.WriteLine("Response: {0}", response.StatusCode);
                Console.WriteLine("Request took {0}ms", requestTime.ElapsedMilliseconds);
                r = requestTime.ElapsedMilliseconds;
            }

            Console.WriteLine();
            
            using (StreamWriter file = new StreamWriter(File.Create(@"measures.txt")))
            {
                file.WriteLine(totalTime.ElapsedMilliseconds + " " + r);
                Console.WriteLine("Startup time and request time writen to measures.txt.");
            }
        }
    }
}
