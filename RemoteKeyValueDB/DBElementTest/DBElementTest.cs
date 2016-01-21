///////////////////////////////////////////////////////////////
// DBElementTest.cs - Define test element for noSQL database //    
// Ver 1.1                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
// Source:      Jim Fawcett, CST 4-187, Syracuse University  //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package replaces DBElement test stub to remove
 * circular package references.
 *
 * Now this testing depends on the class definitions in DBElement
 * and the extension methods defined in DBExtensions.  We no longer
 * need to define extension methods in DBElement.
 *   Public Interface
 *   ----------------
 *  DBElementTest <int,string> x = new DBElementTest <int,string>();
 *  x.GetValue(5);
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBElementTest.cs,  DBElement.cs, DBExtensions.cs,XMLUtility.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.1 : 7 Oct 2015 :Testing have been carried out on this package
 * ver 1.0 : 24 Sep 15
 */
using DBElementSpace;
using DBExtensionsSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

/// <summary>
/// Namespace for DBElementTestSpace which contains the DBElementClass
/// </summary>
namespace DBElementTestSpace
{
    /// <summary>
    /// Class to test DBElement functionality
    /// </summary>
    class DBElementTest
    {
        /// <summary>
        /// Testing whether DBElement functionality is working correctly as expected 
        /// by passing in different parameters.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            WriteLine("Testing DBElement Package");
            WriteLine();
            Write("\n --- Test DBElement<int,string> ---");
            WriteLine();

            //Inserting the first element
            DBElement<int, string> elem2 = new DBElement<int, string>("Darth Vader", "Evil Overlord");
            elem2.payload = "The Empire strikes back!";
            Write(elem2.showElement<int, string>());
            WriteLine();

            //Inserting the second element
            var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
            elem3.children = new List<int> { 1, 2, 7 };
            elem3.payload = "X-Wing fighter in swamp - Oh oh!";
            Write(elem3.showElement<int, string>());
            WriteLine();

            Write("\n --- Test DBElement<string,List<string>> ---");
            WriteLine();
            //Inserting the first element to the String Value Database
            DBElement<string, List<string>> newelem1 = new DBElement<string, List<string>>();
            newelem1.name = "newelem1";
            newelem1.descr = "test new type";
            newelem1.payload = new List<string> { "one", "two", "three" };
            Write(newelem1.showElement<string, List<string>, string>());
            WriteLine();

            //Inserting the second element to the String Value Database
            DBElement<string, List<string>> newerelem1 = new DBElement<string, List<string>>();
            newerelem1.name = "newerelem1";
            newerelem1.descr = "same stuff";
            newerelem1.children.Add("first_key");
            newerelem1.children.Add("second_key");
            newerelem1.payload = new List<string> { "alpha", "beta", "gamma" };
            newerelem1.payload.AddRange(new[] { "delta", "epsilon" });
            Write(newerelem1.showElement<string, List<string>, string>());
            WriteLine();
            Write("\n\n");
        }
    }
}
