/**
* FILE				: LandingPage.xaml.cs
* PROJECT			: PROG 2121 - Windows Programming Assignment 06
* PROGRAMMERS		:
*   Minchul Hwang  ID: 8818858
* FIRST VERSION		: Nov. 19, 2023
* DESCRIPTION		: This program is a client program that is linked to the WP_A05_ServerApp program.
*                     Once connected to the server, it continuously contacts the server 
*                     and the connection is maintained for a certain period of time.
*	                  This page is a script that communicates with the server when the user enters IP, Port, name, and time.
*/

using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WP_A05_WPF_Client
{
    /**
    * PARTIAL CLASS     : LandingPage : UserControl
    * DESCRIPTION	    :
    *	This class creates a window that drives user UI actions.
    */
    public partial class LandingPage : UserControl
    {
        private string address;
        private string portString;
        private string userName;
        private string timer;

        /**
        *	CONSTRUCTOR     : LandingPage()
        *	DESCRIPTION		
        *		This constructor makes LandingPage work.
        *	PARAMETERS		
        *		void        :    Void is used as there are no paramters for this constructor
        *	RETURNS			
        *		void	    :	 There are no return values for this constructor
        */
        public LandingPage()
        {
            InitializeComponent();
        }

        /**
        *	METHOD    : SubmissionClick()
        *	DESCRIPTION		
        *		When user press the submit button after entering certain information, 
        *		communication with the server begins.
        *	PARAMETERS		
        *		object          sender          a object when the event occured
        *		EventArgs       e               a value which the event occured
        *	RETURNS			
        *		void	    :	 There are no return values for this method
        */
        private void SubmissionClick(object sender, RoutedEventArgs e)
        {
            // Save data from UI component
            address = IPTextbox.Text;
            portString = PortTextbox.Text;
            userName = UserNameTextbox.Text;
            timer = TimeLimitTextbox.Text;

            // Instantiate a new client
            TcpClient client = new TcpClient();

            try
            {
                // Send time to server.
                int port = int.Parse(portString);
                client.Connect(address, port);
                NetworkStream stream = client.GetStream();
                string timerMessage = "Timer:" + timer;
                byte[] data = Encoding.ASCII.GetBytes(timerMessage);
                stream.Write(data, 0, data.Length);

                data = new Byte[256];

                String responseData = String.Empty;
                String MatchData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                // String data and word matching numbers are received from the server.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                string[] splitData = responseData.Split(';');

                string stringData = splitData[0];
                string matchData = splitData[1];


                // Open new window which is GameplayPage
                MainWindow changeWindow = new MainWindow();
                changeWindow.Show();
                changeWindow.NavigateToGame(stringData, timer, matchData);

            }
            catch (Exception)
            {
                ErrorMessage.Visibility = Visibility.Visible;

            }
            finally
            {
                client.Close();
            }
        }

        public string GetName()
        {
            return userName;
        }
    }
}
