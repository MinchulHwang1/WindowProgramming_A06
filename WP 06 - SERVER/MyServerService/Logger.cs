/**
* FILE				: Logging.cs
* PROJECT			: PROG 2121 - Windows Programming Assignment 06
* PROGRAMMERS		:
*   Minchul Hwang  ID: 8818858
* FIRST VERSION		: Nov. 26, 2023
* DESCRIPTION		: This program is a server program that is linked to the WP_A05_ServerApp program.
*                     All server operations are recorded in the log file.
*                     
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace MyServerService
{
    public static class Logger
    {
        /**
        *	CONSTRUCTOR     : Log()
        *	DESCRIPTION		
        *		This method creates a Mutex object and prevents various processes from accessing it.
        *		and also make log file
        *	PARAMETERS		
        *		string      msg         the message what the server want to write
        *	RETURNS			
        *		None
        */

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
            }
        }
    }
}
