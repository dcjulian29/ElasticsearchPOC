using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace mvc_serilog
{
    public static class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .CaptureStartupErrors(true)
                        .UseSerilog((hostingContext, loggerConfiguration) =>
                        {
                            loggerConfiguration
                                .ReadFrom.Configuration(hostingContext.Configuration)
                                .Enrich.FromLogContext()
                                .Enrich.WithProperty("ApplicationName",
                                    typeof(Program).Assembly.GetName().Name)
                                .Enrich.WithProperty("Environment",
                                    hostingContext.HostingEnvironment)
                                .Enrich.WithMachineName()
                                .WriteTo.Elasticsearch(
                                    ConfigureElasticSink(hostingContext.Configuration));
                        });
                });

        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat =
                    $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                CustomFormatter = new ExceptionAsObjectJsonFormatter()
            };
        }
    }
}
