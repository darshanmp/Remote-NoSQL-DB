///////////////////////////////////////////////////////////////
// Display.cs - Display methods to simplify display actions   //
// Ver 1.3                                                  //
// Application: Project#2                                    //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Source:      Darshan Masti Prakash                        //
// Author:      Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements formatting functions and tests them
 * along with DBExtensions.
 *
 * Note: This package defines formatting functions.
 * It simply tests use of DBExtensions.
 */
/*
 *  Public Interface
 *  ----------------
 *  Display  x = new Display();
 *  x.Insert(5);
 * Maintenance:
 * ------------
 * Required Files: Display.cs, DBEngine.cs, DBElement.cs, 
 *                 DBExtensions.cs,Utility.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.3 : 7 Oct 15 : Modified display functionality to include the int,string Key/Value DB
 * ver 1.2 : 24 Sep 15
 * - minor tweeks to extension methods to use names from
 *   DBExtensions
 * ver 1.1 : 15 Sep 15
 * - fixed a couple of minor bugs and added more comments
 * ver 1.0 : 13 Sep 15
 * - first release
 * It's purpose is to compress
 * displays for TestExec.cs
 */
using DBElementSpace;
using DBEngineSpace;
using DBExtensionsSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace DisplaySpace
{
    /// <summary>
    /// Element formatted class to format class
    /// </summary>
    public class ElementFormatter
    {
        /// <summary>
        /// Margin is created to display 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string makeMargin(string src)
        {
            StringBuilder temp = new StringBuilder("  ");
            foreach (char ch in src)
            {
                if (ch != '\n')
                    temp.Append(ch);
                else
                {
                    temp.Append(ch).Append("  ");
                }
            }
            return temp.ToString();
        }


        /// <summary>
        /// replaces newline with ", "
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string makeLinear(string src)
        {
            StringBuilder temp = new StringBuilder();
            foreach (char ch in src)
            {
                if (ch != '\n')
                    temp.Append(ch);
                else
                    temp.Append(", ");
            }
            return temp.ToString();
        }

        /// <summary>
        /// use this method to format db elements
        /// </summary>
        /// <param name="src"></param>
        /// <param name="showMargin"></param>
        /// <returns></returns>
        public static string formatElement(string src, bool showMargin = true)
        {
            if (showMargin)
                return makeMargin(src.ToString());
            return makeLinear(src.ToString());
        }
        /// <summary>
        /// This method is used to display db elements 
        /// </summary>
        /// <param name="src"></param>

        public static void showElement(string src)
        {
            Write("\n{0}", makeMargin(src.ToString()));
        }
    }

    /// <summary>
    /// Extensions for displaying the element
    /// </summary>
    public static class DisplayExtensions
    {
        /// <summary>
        /// Show the element and display
        /// </summary>
        /// <param name="element"></param>
        public static string showElement(this DBElement<int, string> element)
        {
            StringBuilder accum = new StringBuilder();
            accum.Append(element.showElement<int, string>());
            Console.Write(accum.ToString());
            return accum.ToString();
        }
        /// <summary>
        /// Display the children
        /// </summary>
        /// <param name="element"></param>
        public static string showChildren(this DBElement<int, string> element)
        {
            StringBuilder append = new StringBuilder();
            append.Append(element.showChildren<int, string>());            
            Console.Write(append.ToString());
            return append.ToString();
        }
        /// <summary>
        /// Iterate through the element to display the elements for string DB
        /// </summary>
        /// <param name="enumElement"></param>
        public static void showEnumerableElement(this DBElement<string, string> enumElement)
        {
            Console.Write(enumElement.showElement<string, string>());
        }
        /// <summary>
        /// Iterate through the element to display the elements for show DB
        /// </summary>
        /// <param name="db"></param>
        public static void showDB(this DBEngine<int, DBElement<int, string>> db)
        {
            db.show<int, DBElement<int, string>, string>();
        }
        /// <summary>
        /// Iterate through the element to display the elements for showEnumerableDB
        /// </summary>
        /// <param name="db"></param>
        public static void showEnumerableDB(this DBEngine<string, DBElement<string, string>> db)
        {
            db.show<string, DBElement<string, string>, string>();
        }

        /// <summary>
        /// Iterate through the element to display the elements for showEnumerableDB
        /// </summary>
        /// <param name="db"></param>
        public static void showStringDB(this DBEngine<string, DBElement<string, List<string>>> db)
        {
            db.showStringDB<string, DBElement<string, List<string>>, string>();
           // db.showStringDB<int, DBElement<string, List<string>>,string>();
        }
    }

#if (TEST_DISPLAY)

    class TestDisplay
    {
        static void Main(string[] args)
        {
            Write("\n  All testing of Display class moved to DisplayTest package.");
            Write("\n  This allow use of Display package without circular dependencies.");
            Write("\n\n");
        }
    }
#endif
}

