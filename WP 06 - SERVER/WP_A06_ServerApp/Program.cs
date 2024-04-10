/**
* FILE				: Program.cs
* PROJECT			: PROG 2121 - Windows Programming Assignment 06
* PROGRAMMERS		:
*   Minchul Hwang  ID: 8818858
* FIRST VERSION		: Nov. 19, 2023
* DESCRIPTION		: This program is a server program that is linked to the WP_A05_ServerApp program.
*                     The server connected to the client is a single server and uses multithreading.
*                     
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.ServiceProcess;
using System.Security.Authentication.ExtendedProtection;

namespace WP_A06_ServerApp
{
    internal class Program
    {
        private ServiceController serviceController;

        public Program()
        {
            serviceController = new ServiceController("MyServerService");
        }
        static async Task Main(string[] args)
        {
            Server server = new Server();
            await server.StartListenerAsync();
        }

    }
    

}
