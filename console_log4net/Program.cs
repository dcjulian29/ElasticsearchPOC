using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using log4net;

namespace console_log4net
{
    static internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            _log.Info("Hello Elasticsearch! (log4net)");

            _log.Warn("Something bad is about to happen... (log4net)");

            _log.Error("kaboom!", new ApplicationException("The application exploded"));

            Exception newException = new Exception("There was a system error");
            newException.Data.Add("CustomProperty", "CustomPropertyValue");
            newException.Data.Add("SystemUserID", "User43");

            _log.Error("Something broke.", newException);

            var zero = 0;

            try
            {
                _ = 42 / zero;
            }
            catch (Exception ex1)
            {
                _log.Error(ex1.Message, ex1);
            }

            Console.WriteLine("Done.");

            _log.Debug("Done");

            // Give Background threads time to send log4net logs
            var time = 5;
            do
            {
                Thread.Yield();
                Thread.Sleep(1000);
                time--;
            } while (time != 0);

            Console.WriteLine("Exiting.");
        }
    }
}
