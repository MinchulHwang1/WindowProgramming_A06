/**
* FILE				: Server.cs
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
using System.Configuration;
using System.Collections;

namespace WP_A06_ServerApp
{
    /**
    * PARTIAL CLASS     : Server
    * DESCRIPTION	    : This class holds the server activity.
    *   
    */
    internal class Server
    {
        private int _matchValue;
        Dictionary<string, bool> wordDictionary;
        Int32 port;
        IPAddress localAddr;
        string serverIp;
        string serverPort;
        TcpListener server = null;
        string stringData;
        int matchData;
        string matchDataString;
        string path;
        string gamePath;
        Logging logging;

        /**
        *	CONSTRUCTOR     : Server()
        *	DESCRIPTION		
        *		The creator of this server defines the server's IP 
        *		and port and prepares to accept data from the client.
        *	PARAMETERS		
        *		None
        *	RETURNS			
        *		None
        */
        public Server()
        {
            serverIp = ConfigurationManager.AppSettings["ServerIp"];
            serverPort = ConfigurationManager.AppSettings["ServerPort"];
            port = Int32.Parse(serverPort);
            localAddr = IPAddress.Parse(serverIp);
            server = new TcpListener(localAddr, port);
            
            logging = new Logging();
        }

        /**
        *	CONSTRUCTOR     : Task StartListenerAsync()
        *	DESCRIPTION		
        *		This asynchronous listener runs when a client first connects.
        *		It operates multithreaded.
        *	PARAMETERS		
        *		None
        *	RETURNS			
        *		None
        */
        internal async Task StartListenerAsync()
        {
            try
            {
                server.Start();
                Console.WriteLine("Server is listening...");
                logging.Log("Connected!");
                while (true)
                {
                    Console.WriteLine("Please Enter a message to send, or \"Shutdown\" to end:");
                    String message = Console.ReadLine();
                    TcpClient client = await server.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClientAsync(client));
                }
            }
            finally
            {
                server?.Stop();
            }
        }

        /**
        *	CONSTRUCTOR     : Task HandleClientAsync()
        *	DESCRIPTION		
        *		This method processes the data received from the client as a TASK.
        *		Different actions are taken depending on the message sent by the client.
        *	PARAMETERS		
        *		TcpClient           client          socket connected to the client
        *	RETURNS			
        *		None
        */
        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                Byte[] bytes = new Byte[256];
                String data = null;

                while (true)
                {
                    int i = await stream.ReadAsync(bytes, 0, bytes.Length);

                    if (i == 0) break;


                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    // When the start of received data is Timer.
                    if (data.StartsWith("Timer:"))
                    {
                        string time = data.Substring(6);
                        // Translate data bytes to a ASCII string.

                        // Load New text file
                        path = ConfigurationManager.AppSettings["path"];
                        Random random = new Random();
                        int randomFileToPick = random.Next(1, 7);   // PICKS NUMBER BETWEEN 1-4
                        gamePath = path + randomFileToPick + ".txt";
                        logging.Log("Get Text String");

                        // get string and other data from object GameObject(it is taken from text file)
                        GameObject game1 = new GameObject(gamePath);
                        stringData = game1.GetStringData();
                        matchData = game1.GetMatchValue();
                        matchDataString = matchData.ToString();
                        List<string> wordDataList = game1.GetWordDataList();
                       
                        // Insert the list of words received from the object into a new dictionary.
                        wordDictionary = new Dictionary<string, bool>();

                        foreach (string word in wordDataList)
                        {
                            // Add each word to Dictionary and set true as default
                            wordDictionary[word] = true;
                        }

                        _matchValue = game1.GetMatchValue();
                        string combineData = stringData + ";" + matchDataString;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(combineData);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        // Console.WriteLine("Sent: {0}", combineData);

                        // Count down in server side
                        int timeValue = int.Parse(time);
                        if (timeValue > 0)
                        {
                            // Timer start.
                            Timer timer = new Timer(timeValue * 60 * 1000);
                            timer.AutoReset = false;
                            timer.Elapsed += (sender, e) => TimerElapsed(sender, e, stream, client); // Call event handler
                            timer.Start();
                        }
                    }

                    // When the start of received data is ACTION.
                    else if (data.StartsWith("ACTION:"))
                    {
                        // Words received from users.
                        string action = data.Substring(7);
                        


                        if (wordDictionary.ContainsKey(action))
                        {
                            if (wordDictionary[action] == true)     // When there is a word in dictionary
                            {
                                _matchValue--;

                                string combineData = action + ";" + _matchValue;
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(combineData);
                                byte[] responseMsg1 = Encoding.ASCII.GetBytes("Correct!");
                                stream.Write(msg, 0, msg.Length);
                                wordDictionary[action] = false;
                                logging.Log("Correct word : " + action);
                            }
                            else                                    // When words entered by the user are duplicated
                            {
                                string combineData = "Duplicated" + ";" + _matchValue;
                                byte[] responseMsg2 = System.Text.Encoding.ASCII.GetBytes(combineData);
                                stream.Write(responseMsg2, 0, responseMsg2.Length);
                                logging.Log("Duplicated word : " + action);
                            }
                        }
                        else                                        // When there is no word
                        {
                            string combineData = "No Match" + ";" + _matchValue;
                            byte[] responseMsg3 = System.Text.Encoding.ASCII.GetBytes(combineData);
                            stream.Write(responseMsg3, 0, responseMsg3.Length);
                            logging.Log("Wrong word");
                        }
                    }
                    else if (data.StartsWith("TIME_UP:"))           // When the time is up
                    {
                        byte[] responseMsgTIME = Encoding.ASCII.GetBytes("TIME_OUT");
                        stream.Write(responseMsgTIME, 0, responseMsgTIME.Length);
                        stream.Close();
                        client.Close();
                    }

                    else if (data.StartsWith("ENDING:"))            // When user want to quit
                    {
                        byte[] responseMsg4 = Encoding.ASCII.GetBytes("Connection out");
                        stream.Write(responseMsg4, 0, responseMsg4.Length);
                        logging.Log("Game Ending");
                        stream.Close();
                        client.Close();
                    }
                }
            }
            catch (IOException ex){
                if (ex != null)
                {
                    logging.Log("IOException: " + ex.Message);
                }
                else
                {
                    logging.Log("IOException is null");
                }
            }
            finally{
                client.Close();
                logging.Log("You Stopped Server..");
            }
        }



/** Class comment
* Name     : TimerElapsed
* Purpose  : When the time input by user is expires, the connection between the server and the client is disconnected
* Input    : EventHandler(object sender, ElapsedEventArgs e)
*            NetworkStream     stream 
*            TcpClient         client
* Output   : Notification that connection has been lost.
* Return   : None
                         */
    private void TimerElapsed(object sender, ElapsedEventArgs e, NetworkStream stream, TcpClient client)
        {
            Console.WriteLine("Timer elapsed. Closing connection.");

            byte[] responseMsgTIME = Encoding.ASCII.GetBytes("TIME_OUT");
            try
            {
                stream.Write(responseMsgTIME, 0, responseMsgTIME.Length);
            }
            catch (Exception ex)
            {
                logging.Log("Time OUT");
            }
        }
    }
    
}

