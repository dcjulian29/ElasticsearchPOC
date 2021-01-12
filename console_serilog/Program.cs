using System;
using System.Reflection;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace console_serilog
{
    internal class Program
    {
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

        private static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Elasticsearch(ConfigureElasticSink())
                .CreateLogger();

            logger.Information("Hello Elastic! (Serilog)");
            logger.Warning("Warning!!! The issue may get worse... (Serilog)");

            var ex = new InvalidCastException();

            logger.Error(ex, "Error Exception. (Serilog)");

            var zero = 0;

            try
            {
                _ = 42 / zero;
            }
            catch (Exception ex1)
            {
                logger.Error(ex1, ex1.Message);
            }

            logger.Dispose();
        }
    }
}
