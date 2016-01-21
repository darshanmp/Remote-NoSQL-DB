///////////////////////////////////////////////////////////////
// DisplayTest.cs -  DisplayTest element for noSQL database  //
// Ver 1.0                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements testing for the formatting functions and tests them
 * along with DBExtensions.
 *
 * Note: This package defines testing for the formatting functions.
 * It simply tests use of Display.
 */
/*
 *  Public Interface
 *  ----------------
 *  Display  x = new Display();
 *  x.Test();
 * Maintenance:
 * ------------
 * Required Files: Display.cs, DBEngine.cs, DBElement.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15 : This package implements testing for the formatting functions and tests them
 * along with DBExtensions.
 */
using DBElementSpace;
using DBEngineSpace;
using DisplaySpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace DisplayTest
{
    /// <summary>
    /// Class for testing of Display Package.Display package is called by other packages to display elements
    /// </summary>
    class DisplayTest
    {
        static bool verbose = false;

        static void Main(string[] args)
        {
            Test1();
            //Load the first element
            DBElement<string, string> newelem1, newerelem1, newerelem2;
            Test2(out newelem1, out newerelem1, out newerelem2);
            Write("\n --- Test DBEngine<string,DBElement<string,List<string>>> ---");
            //Anonymous function to get the string key
            int seed = 0;
            string skey = seed.ToString();
            Func<string> skeyGen = () =>
            {
                ++seed;
                skey = "string" + seed.ToString();
                skey = skey.GetHashCode().ToString();
                return skey;
            };
            //Insert into DB and show the DB
            DBEngine<string, DBElement<string, string>> newdb =
              new DBEngine<string, DBElement<string, string>>();
            newdb.insert(skeyGen(), newelem1);
            newdb.insert(skeyGen(), newerelem1);
            newdb.insert(skeyGen(), newerelem2);
            newdb.showEnumerableDB();
            Write("\n\n");
        }

        private static void Test2(out DBElement<string, string> newelem1, out DBElement<string, string> newerelem1, out DBElement<string, string> newerelem2)
        {
            newelem1 = new DBElement<string, string>();
            newelem1.name = "newelem1";
            newelem1.descr = "test new type";
            newelem1.payload = "one";
            //Load the second element
            newerelem1 = new DBElement<string, string>();
            newerelem1.name = "newerelem1";
            newerelem1.descr = "better formatting";
            newerelem1.payload = "alpha";
            //Load the third element
            newerelem2 = new DBElement<string, string>();
            newerelem2.name = "newerelem2";
            newerelem2.descr = "better formatting";
            newerelem2.children.AddRange(new List<string> { "first", "second" });
            newerelem2.payload = "a";
            if (verbose)
            {
                Write("\n --- Test DBElement<string,List<string>> ---");
                WriteLine();
                newelem1.showEnumerableElement();       //Display the element
                WriteLine();
                newerelem1.showEnumerableElement();      //Display the element
                WriteLine();
                newerelem2.showEnumerableElement();
                WriteLine();
            }
        }

        private static void Test1()
        {
            WriteLine("Testing DBEngine Package");
            WriteLine();
            WriteLine("Test db of scalar elements");
            WriteLine();
            //Payload load
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.payload = "a payload";
            DBElement<int, string> elem2 = new DBElement<int, string>("Darth Vader", "Evil Overlord");
            elem2.payload = "The Empire strikes back!";
            var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
            elem3.payload = "X-Wing fighter in swamp - Oh oh!";
            if (verbose)
            {
                Write("\n --- Test DBElement<int,string> ---");
                WriteLine();
                //Display first element
                elem1.showElement();
                WriteLine();
                //Display second element
                elem2.showElement();
                WriteLine();
                //Display third element
                elem3.showElement();
                WriteLine();
                /* ElementFormatter is not ready for prime time yet */
                //Write(ElementFormatter.formatElement(elem1.showElement<int, string>(), false));
            }
            Write("\n --- Test DBEngine<int,DBElement<int,string>> ---");
            WriteLine();
            int key = 0;
            Func<int> keyGen = () => { ++key; return key; };
            //Insert the db elements into DB
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            bool p1 = db.insert(keyGen(), elem1);
            bool p2 = db.insert(keyGen(), elem2);
            bool p3 = db.insert(keyGen(), elem3);
            if (p1 && p2 && p3)
                Write("\n  all inserts succeeded");
            else
                Write("\n  at least one insert failed");
            db.showDB();        //Display the DB
            WriteLine();
            WriteLine("Test db of enumerable elements\n");
            WriteLine();
        }
    }
}
