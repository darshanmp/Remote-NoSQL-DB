///////////////////////////////////////////////////////////////
// DBEngineTest.cs - DB EngineTest for noSQL database       //
// Ver 1.1                                                  //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
// Source:      Jim Fawcett, CST 4-187, Syracuse University  //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package replaces DBEngine test stub to remove
 * circular package references.
 *
 * Now this testing depends on the class definitions in DBElement,
 * DBEngine, and the extension methods defined in DBExtensions.
 * We no longer need to define extension methods in DBEngine.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBEngine.cs, DBElement.cs, DBEngine.cs,Utility.cs  
 *   DBExtensions.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 *   Public Interface
 *   ----------------
 *  DBEngineTest <int,string> x = new DBEngineTest <int,string>();
 *  x.GetValue(5);
 * Maintenance History:
 * --------------------
 * ver 1.1 : 7 Oct 15 : Added the Testing for the DBEngineTest functionality
 * ver 1.0 : 24 Sep 15
 *
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

namespace DBEngineTestSpace
{
    /// <summary>
    /// DBEngineTestClass for testing the DBEngine class
    /// </summary>
    class DBEngineTestClass
    {
        static void Main(string[] args)
        {
            DBEngine<int, DBElement<int, string>> db = Test1();
            //Insert the Second element
            Write("\n --- Test DBElement<string,List<string>> ---");
            DBElement<string, List<string>> newerelem1 = new DBElement<string, List<string>>();
            newerelem1.name = "newerelem1";
            newerelem1.descr = "better formatting";
            newerelem1.payload = new List<string> { "alpha", "beta", "gamma" };
            newerelem1.payload.Add("delta");
            newerelem1.payload.Add("epsilon");
            Write(newerelem1.showElement<string, List<string>, string>());
            WriteLine();
            //Insert the Third element
            DBElement<string, List<string>> newerelem2 = new DBElement<string, List<string>>();
            newerelem2.name = "newerelem2";
            newerelem2.descr = "better formatting";
            newerelem1.children.AddRange(new[] { "first", "second" });
            newerelem2.payload = new List<string> { "a", "b", "c" };
            newerelem2.payload.Add("d");
            newerelem2.payload.Add("e");
            Write(newerelem2.showElement<string, List<string>, string>());
            WriteLine();
            Write("\n --- Test DBEngine<string,DBElement<string,List<string>>> ---");
            //Anonymous functionality for generating string key
            int seed = 0;
            string skey = seed.ToString();
            Func<string> skeyGen = () =>
            {
                ++seed;
                skey = "string" + seed.ToString();
                skey = skey.GetHashCode().ToString();
                return skey;
            };
            Test2(db, newerelem1, newerelem2, skeyGen);
        }

        private static void Test2(DBEngine<int, DBElement<int, string>> db, DBElement<string, List<string>> newerelem1, DBElement<string, List<string>> newerelem2, Func<string> skeyGen)
        {
            //Insert entries into DB
            DBEngine<string, DBElement<string, List<string>>> newdb =
              new DBEngine<string, DBElement<string, List<string>>>();
            newdb.insert(skeyGen(), newerelem1);
            newdb.insert(skeyGen(), newerelem2);
            newdb.show<string, DBElement<string, List<string>>, List<string>, string>();
            WriteLine();
            //Testing the Edit functionality
            WriteLine("Testing edits\n");
            db.show<int, DBElement<int, string>, string>();
            DBElement<int, string> editElement = new DBElement<int, string>();
            db.getValue(1, out editElement);
            editElement.showElement<int, string>();
            editElement.name = "editedName";
            editElement.descr = "editedDescription";
            db.show<int, DBElement<int, string>, string>();
            Write("\n\n");
        }

        private static DBEngine<int, DBElement<int, string>> Test1()
        {
            WriteLine("Testing DBEngine Package\n");
            WriteLine();
            Write("\n --- Test DBElement<int,string> ---");
            //Int&String Key/Value DB
            //Insert the First element
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.payload = "a payload";
            Write(elem1.showElement<int, string>());
            WriteLine();
            //Insert the Second element
            DBElement<int, string> elem2 = new DBElement<int, string>("Darth Vader", "Evil Overlord");
            elem2.payload = "The Empire strikes back!";
            Write(elem2.showElement<int, string>());
            WriteLine();
            //Insert the Third element
            var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
            elem3.children.AddRange(new List<int> { 1, 5, 23 });
            elem3.payload = "X-Wing fighter in swamp - Oh oh!";
            Write(elem3.showElement<int, string>());
            WriteLine();
            Write("\n --- Test DBEngine<int,DBElement<int,string>> ---");
            int key = 0;
            // anonymous function to generate keys
            Func<int> keyGen = () => { ++key; return key; };
            //Insert all the elements into the Database
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            bool p1 = db.insert(keyGen(), elem1);
            bool p2 = db.insert(keyGen(), elem2);
            bool p3 = db.insert(keyGen(), elem3);
            if (p1 && p2 && p3)
                Write("\n  all inserts succeeded");
            else
                Write("\n  at least one insert failed");
            //Show the DB 
            db.show<int, DBElement<int, string>, string>();
            WriteLine();
            //String & String Key/Value DB
            //Insert the First element
            Write("\n --- Test DBElement<string,List<string>> ---");
            DBElement<string, List<string>> newelem1 = new DBElement<string, List<string>>();
            newelem1.name = "newelem1";
            newelem1.descr = "test new type";
            newelem1.payload = new List<string> { "one", "two", "three" };
            Write(newelem1.showElement<string, List<string>>());
            WriteLine();
            return db;
        }
    }
}
