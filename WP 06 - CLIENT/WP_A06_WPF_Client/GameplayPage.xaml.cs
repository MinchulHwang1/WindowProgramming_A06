/**
* FILE				: GameplayPage.xaml.cs
* PROJECT			: PROG 2121 - Windows Programming Assignment 06
* PROGRAMMERS		:
*   Minchul Hwang  ID: 8818858
* FIRST VERSION		: Nov. 19, 2023
* DESCRIPTION		: This program is a client program that is linked to the WP_A05_ServerApp program.
*                     Once connected to the server, it continuously contacts the server 
*                     and the connection is maintained for a certain period of time.
*	                  The logic for the game is written down.
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Configuration;
using System.Windows.Markup;
using System.Threading;

namespace WP_A05_WPF_Client
{
    /**
    * PARTIAL CLASS     : GameplayPage : UserControl
    * DESCRIPTION	    :
    *	This class creates a window that drives user UI actions and game logic.
    */
    public partial class GameplayPage : UserControl
    {
        private int countdownValueInSeconds;
        private DispatcherTimer dispatcherTimer;    // Fake timer appearing in UI
        public event EventHandler Closing;          // Closing event handler taken from MainWindow
        Int32 port;
        IPAddress localAddr;
        TcpClient client;
        NetworkStream stream;
        bool SetScreen;
        bool SetSecondScreen;
        string serverIp;
        string serverPort;
        string userName;
        /**
        *	CONSTRUCTOR     : GameplayPage()
        *	DESCRIPTION		
        *		This constructor makes GameplayPage work.
        *		The time received from the user is calculated in seconds and output.
        *	PARAMETERS		
        *		String          responseData        String received from server
        *		String          timer               Time timer received from the user
        *		String          match               Number of word matches received from the server
        *	RETURNS			
        *		void	    :	 There are no return values for this constructor
        */
        public GameplayPage(String responseData, String timer, String match)
        {
            InitializeComponent();
            Label.Content = responseData;
            Match.Content = match;
            SetScreen = false;
            SetSecondScreen = false;
            client = new TcpClient();
            
            serverIp = ConfigurationManager.AppSettings["ServerIp"];        // Get IP
            serverPort = ConfigurationManager.AppSettings["ServerPort"];    // Get port

            port = Int32.Parse(serverPort);
            localAddr = IPAddress.Parse(serverIp);
            client.Connect(localAddr, port);


            if (int.TryParse(timer, out int timerInMinutesInt))
            {
                countdownValueInSeconds = timerInMinutesInt *60 ; // change min to seconds
                Label1.Content = countdownValueInSeconds;
                StartServerConnection();


                // Reset Timer - this timer is just fake. real timer is working on server side
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += DispatcherTimer_Tick;

                // Unloaded closing event on UI and take from MainWindow.
                this.Unloaded += (s, e) => Closing?.Invoke(this, EventArgs.Empty);

                // Timer Start
                dispatcherTimer.Start();

                // Show the timer
                Label1.Content = countdownValueInSeconds.ToString();


            }
            else
            {
                MessageBox.Show("You have not set an appropriate timer value.");
            }
        }


        /**
        *	CONSTRUCTOR     : DispatcherTimer_Tick()
        *	DESCRIPTION		
        *		This method counts down the time in the UI as a fake component.
        *	PARAMETERS		
        *		object          sender          a object when the event occured
        *		EventArgs       e               a value which the event occured
        *	RETURNS			
        *		void	    :	 There are no return values for this constructor
        */
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            countdownValueInSeconds--;
            Label1.Content = countdownValueInSeconds.ToString();

            if (countdownValueInSeconds == 0)   //
            {   
                dispatcherTimer.Stop();
                
                stream = client.GetStream();
                
                string actionMessage = "TIME_UP:" + countdownValueInSeconds.ToString();
                byte[] time_up = Encoding.ASCII.GetBytes(actionMessage);
                stream.Write(time_up, 0, time_up.Length);



                time_up = new Byte[256];

                String GetTime = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                // String data and word matching numbers are received from the server.
                Int32 bytes = stream.Read(time_up, 0, time_up.Length);
                GetTime = System.Text.Encoding.ASCII.GetString(time_up, 0, bytes);
                if (GetTime == "TIME_OUT")
                {
                    MessageBoxResult result = MessageBox.Show("New Game?", "Time out", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        stream.Close();
                        client.Close();
                        SetScreen = true;
                        Window currentWindow = Window.GetWindow(this);
                        currentWindow.Close();
                    }
                    else
                    {
                        stream.Close();
                        client.Close();
                        SetSecondScreen = true;
                        Window currentWindow = Window.GetWindow(this);
                        currentWindow.Close();
                    }
                }
            }

        }

        /**
        *	CONSTRUCTOR     : Guess_button()
        *	DESCRIPTION		
        *		Method used when a user searches for a word
        *	PARAMETERS		
        *		object          sender          a object when the event occured
        *		EventArgs       e               a value which the event occured
        *	RETURNS			
        *		void	    :	 There are no return values for this constructor
        */
        private void Guess_button(object sender, RoutedEventArgs e)
        {
            string guessWord = GuessWord.Text;
            try
            {
                if (client.Connected)
                {
                    // Send Guess word to use word "ACTION"
                    // Server can notice in this area action
                    stream = client.GetStream();
                    string actionMessage = "ACTION:" + guessWord;
                    byte[] guess = Encoding.ASCII.GetBytes(actionMessage);
                    stream.Write(guess, 0, guess.Length);


                    guess = new Byte[256];
                    String responseData = String.Empty;

                    Int32 bytes = stream.Read(guess, 0, guess.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(guess, 0, bytes);
                    string[] splitData = responseData.Split(';');
                    string wordString = splitData[0];
                    string matchValue = splitData[1];


                    test.Content = wordString;
                    Match.Content = matchValue;

                    if (matchValue == "0")
                    {   
                        string endMessage = "ENDING:" + matchValue;
                        byte[] endMsg = Encoding.ASCII.GetBytes(endMessage);
                        stream.Write(endMsg, 0, endMsg.Length);
                        MessageBoxResult result = MessageBox.Show("New Game?", "Exit", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            stream.Close();
                            client.Close();
                            SetScreen = true;
                            Window currentWindow = Window.GetWindow(this);
                            currentWindow.Close();
                        }
                        else
                        {
                            stream.Close();
                            client.Close();
                            SetSecondScreen = true;
                            Window currentWindow = Window.GetWindow(this);
                            currentWindow.Close();
                        }
                    }
                    
                }
                
            }
            catch (Exception)
            {
            }
            

        }

        /**
        *	CONSTRUCTOR     : GetSetScreen()
        *	DESCRIPTION		
        *		This method is used to check whether MainWindowClosing is used or not when the closing message is used.
        *	PARAMETERS		
        *		NONE
        *	RETURNS			
        *		bool        SetScreen       Called in MainWindow.xaml.cs
        */
        public bool GetSetScreen()
        {
            return SetScreen;
        }

        /**
        *	CONSTRUCTOR     : GetSecondScreen()
        *	DESCRIPTION		
        *		This method is used to check whether MainWindowClosing is used or not when the closing message is used.
        *	PARAMETERS		
        *		NONE
        *	RETURNS			
        *		bool        SetSecondScreen       Called in MainWindow.xaml.cs
        */
        public bool GetSecondScreen()
        {
            return SetSecondScreen;
        }

        /**
        *	CONSTRUCTOR     : StartServerConnection()
        *	DESCRIPTION		
        *		This method is used to asynchronously connect to the server
        *	PARAMETERS		
        *		NONE
        *	RETURNS			
        *		None
        */
        private async void StartServerConnection()
        {
            try
            {
                await ConnectToServerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error");
            }
        }

        /**
        *	CONSTRUCTOR     : ConnectToServerAsync()
        *	DESCRIPTION		
        *		This method serves to initialize when connecting to the server asynchronously.
        *	PARAMETERS		
        *		NONE
        *	RETURNS			
        *		None
        */
        private async Task ConnectToServerAsync()
        {
            serverIp = ConfigurationManager.AppSettings["ServerIp"];
            serverPort = ConfigurationManager.AppSettings["ServerPort"];

            port = Int32.Parse(serverPort);
            localAddr = IPAddress.Parse(serverIp);

            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(localAddr, port);
            }
        }

        /**
        *	CONSTRUCTOR     : ShutDownServer()
        *	DESCRIPTION		
        *		This method allows the disconnection required by the GameplayPage to be performed in another environment.
        *	PARAMETERS		
        *		NONE
        *	RETURNS			
        *		None
        */
        public void ShutDownServer()
        {
            if (stream != null)
            {
                try
                {
                    stream.Close();
                }
                catch (Exception ex)
                {
                }
            }
            if (client != null)
            {
                try
                {
                    client.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
