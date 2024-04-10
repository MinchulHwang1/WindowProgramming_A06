/**
* FILE				: GameObject.cs
* PROJECT			: PROG 2121 - Windows Programming Assignment 06
* PROGRAMMERS		:
*   Minchul Hwang  ID: 8818858
* FIRST VERSION		: Nov. 19, 2023
* DESCRIPTION		: This program is a game object program that imports data from a text file as a variable.
*                     
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;


namespace WP_A06_ServerApp
{
    /**
    * PARTIAL CLASS     : GameObject
    * DESCRIPTION	    : This class holds the GameObject activity.
    *   
    */
    internal class GameObject
    {
        private string fileContent;
        private string stringData;
        private int matchValue;
        private string wordValue;
        private List<string> wordDataList;

        public GameObject() { }

        /**
        *	CONSTRUCTOR     : GameObject()
        *	DESCRIPTION		
        *		This constructor takes data from a text file and stores it as a program variable.
        *	PARAMETERS		
        *		string      filePath        File path where the text file is located
        *	RETURNS			
        *		None
        */
        public GameObject(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            stringData = lines[0];
            matchValue = int.Parse(lines[1]);
            wordDataList = new List<string>();
            foreach (string line in lines.Skip(2))
            {
                wordDataList.Add((line));
            }
           
        }

        /** 
         * -- Class comment --
         * Name     : GetStringData
         * Purpose  : Return string stringData
         * Input    : None
         * Output   : None
         * Return   : stringData        string      A string contains question which user has to solve
         */
        public string GetStringData()
        {
            return stringData;
        }

        /** 
         * -- Class comment --
         * Name     : GetMatchValue
         * Purpose  : Return matchValue(which the number of matched with words)
         * Input    : None
         * Output   : None
         * Return   : matchValue        int      An integer which is matched with words
         */
        public int GetMatchValue()
        {
            return matchValue;
        }

        /** 
         * -- Class comment --
         * Name     : List<string> GetWordDataList
         * Purpose  : Return wordDataList(which the list with words)
         * Input    : None
         * Output   : None
         * Return   : wordDataList        list      which the list with words
         */
        public List<string> GetWordDataList()
        {
            return wordDataList;
        }
    }
}
