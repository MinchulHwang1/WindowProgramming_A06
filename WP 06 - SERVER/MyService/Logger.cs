using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Net.NetworkInformation;


namespace MyService
{
    public static class Logger
    {


        public static void Log(string msg)
        {
            EventLog serviceEventLog = new EventLog();
            if (!EventLog.SourceExists("MyEventSource"))
            {
                EventLog.CreateEventSource("MyEventSource", "MyEventLog");
            }
            serviceEventLog.Source = "MyEventSource";
            serviceEventLog.Log = "MyEventLog";
            serviceEventLog.WriteEntry(msg);
            /*
            EventLog serviceEventLog = new EventLog();
            Mutex mutex = new Mutex(); 
            string logFilePath = ConfigurationManager.AppSettings["logPath"];


            if (!EventLog.SourceExists("MyEventSource"))
            {
                EventLog.CreateEventSource("MyEventSource", "MyEventLog");
            }
            serviceEventLog.Source = "MyEventSource";
            serviceEventLog.Log = "MyEventLog";
            serviceEventLog.WriteEntry(msg);

            if (string.IsNullOrEmpty(logFilePath))
            {
                Console.WriteLine("There is wrong logfile path.");
            }
            else
            {
                try
                {
                    mutex.WaitOne();
                    using (StreamWriter sw = File.AppendText(logFilePath))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + ": " + msg);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Log File exception : " + ex.Message);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }*/
        }
    }
}



