using System;
using System.Reflection;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace serilog_example
{
    class Program
    {
        static void Main(string[] args)
        {

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Elasticsearch(ConfigureElasticSink())
                .CreateLogger();

            logger.Information("Hello Elastic!");
            logger.Warning("Warning!!! The issue may get worse...");

            var ex = new InvalidCastException();

            logger.Error(ex, "Error Exception.");


            logger.Dispose();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink()
        {
            return new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
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
