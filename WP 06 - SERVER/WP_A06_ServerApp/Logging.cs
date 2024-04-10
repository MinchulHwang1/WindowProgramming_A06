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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace WP_A06_ServerApp
{
    /**
    * PARTIAL CLASS     : Logging
    * DESCRIPTION	    : This class holds the Logging activity.
    *   
    */
    internal class Logging
    {

        Mutex mutex = null;

        /**
        *	CONSTRUCTOR     : Logging()
        *	DESCRIPTION		
        *		This constructor creates a Mutex object and prevents various processes from accessing it.
        *	PARAMETERS		
        *		None
        *	RETURNS			
        *		None
        */
        internal Logging()
        {
            if (!Mutex.TryOpenExisting("MyMutex", out mutex))
            {
                mutex = new Mutex(true, "MyMutex");
                mutex.ReleaseMutex();
            }
        }

        /**
        *	CONSTRUCTOR     : Log()
        *	DESCRIPTION		
        *		This method is responsible for entering values ​​operating on the server into the log file. 
        *		Additionally, it serves to add content rather than create new content.
        *	PARAMETERS		
        *		string           msg          a message which server try to input
        *	RETURNS			
        *		None
        */
        internal void Log(string msg)
        {
            string logFilePath = ConfigurationManager.AppSettings["logPath"];
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
