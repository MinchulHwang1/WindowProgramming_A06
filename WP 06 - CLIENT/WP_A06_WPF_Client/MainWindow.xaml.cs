/**
* FILE				: MainWindow.xaml.cs
* PROJECT			: PROG 2121 - Windows Programming Assignment 06
* PROGRAMMERS		:
*   Minchul Hwang  ID: 8818858
* FIRST VERSION		: Nov. 19, 2023
* DESCRIPTION		: This program is a client program that is linked to the WP_A05_ServerApp program.
*                     Once connected to the server, it continuously contacts the server 
*                     and the connection is maintained for a certain period of time.
*	                  It plays the role of a game where you match words from a certain string of characters coming from the server.
*/

using System;
using System.Windows;
using System.ComponentModel;

namespace WP_A05_WPF_Client
{
    /**
    * PARTIAL CLASS     : MainWindow
    * DESCRIPTION	    :
    *	This class creates a window that drives user UI actions.
    */
    public partial class MainWindow : Window
    {
        /**
        *	CONSTRUCTOR     : MainWindow()
        *	DESCRIPTION		
        *		This constructor instantiates an object of class type MainWindow which will be used as a template in creating our
        *		The close button only works when the game is started.
        *	PARAMETERS		
        *		void        :    Void is used as there are no paramters for this constructor
        *	RETURNS			
        *		void	    :	 There are no return values for this constructor
        */
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new LandingPage();
        }

        /**
        *	METHOD    : NavigateToGame()
        *	DESCRIPTION		
        *		This method is executed when the user enters information on the first main page and clicks submit.
        *		When executed, a GameplayPage object is created and preparations for gameplay are completed.
        *	PARAMETERS		
        *		String          responseData            Data containing a string received from the server.
        *		String          timer                   a time input by user
        *		String          match                   Number of words in the string received from the server
        *	RETURNS			
        *		void	    :	 There are no return values for this method
        */
        public void NavigateToGame(String responseData, String timer, String match)
        {
            GameplayPage gameplayPage = new GameplayPage(responseData, timer, match);
            gameplayPage.Closing += GameplayPage_Closing; // Register a handler for the UserControl's Closing event
            MainFrame.Content = gameplayPage;
        }

        /**
        *	METHOD     : GameplayPage_Closing()
        *	DESCRIPTION		
        *		Method to use Closing in GameplayPage
        *	PARAMETERS		
        *		object          sender          a object when the event occured
        *		EventArgs       e               a value which the event occured
        *	RETURNS			
        *		void	    :	 There are no return values for this method
        */
        private void GameplayPage_Closing(object sender, EventArgs e)
        {
        }

        /**
        *	METHOD     : MainWindow_Closing()
        *	DESCRIPTION		
        *		When the GameplayPage ends, a method to ask the user whether to continue or not.
        *	PARAMETERS		
        *		object               sender          a object when the event occured
        *		CancelEventArgs      e               An event that turns off the UI when closing occurs in the gameplayPage
        *	RETURNS			
        *		void	    :	 There are no return values for this method
        */
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // it works just on GameplayPage
            if (MainFrame.Content is GameplayPage)
            {
                GameplayPage gameplayPage = (GameplayPage)MainFrame.Content;

                if (gameplayPage.GetSetScreen() == false)
                {
                    if (gameplayPage.GetSecondScreen() == true)
                    {
                        Application.Current.Shutdown();     // When user select No on New game selection message box
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Do you want to exit GameplayPage?", "Exit", MessageBoxButton.YesNo);

                        // If user said No
                        if (result != MessageBoxResult.Yes)
                        {
                            e.Cancel = true; // Disable MainWindow from closing
                        }
                        else
                        {
                            // Shut down Server.
                            gameplayPage.ShutDownServer();
                        }
                    }
                }
                else
                {
                    // Shut down Server.
                    gameplayPage.ShutDownServer();
                }
            }
            else
            {
                Application.Current.Shutdown();
            }

        }
    }
}