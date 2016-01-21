///////////////////////////////////////////////////////////////
// DBExtensionsTest.cs -  DBExtensions for noSQL database    //
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
 * This package implements extensions methods to support 
 * DBElement.cs, DBextensionsTest.cs,DBExtensions.cs
 */
/*
 *   Public Interface
 *   ----------------
 *  DBExtensionTest  x = new DBExtensionTest();
 *  x.GetValue(5);
 * Maintenance:
 * ------------
 * Required Files: 
 *    DBElement.cs, DBextensionsTest.cs,DBExtensions.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.3 : 7 Oct 15 : Modified the methods to incorporate the testing for DBExtensions
 * ver 1.2 : 24 Sep 15
 * - reduced the number of methods and simplified
 * ver 1.1 : 15 Sep 15
 * - added a few comments
 * ver 1.0 : 13 Sep 15
 * - first release
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
namespace DBExtensionsTestSpace
{
    /// <summary>  
    /// - Extension methods are static methods of a static class
    ///   that extend an existing class by adding functionality
    ///   not part of the original class.
    ///   These methods are used to test the Extension methods class  
    /// </summary>
    class DBExtensions
    {
        static void Main(string[] args)
        {
            WriteLine("Testing DBExtensions Package\n");
            WriteLine();

            //Insert first element
            Write("\n --- Test DBElement<int,string> ---");
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.payload = "a payload";
            Write(elem1.showElement<int, string>());

            //Insert first element into DB
            DBEngine<int, DBElement<int, string>> dbs = new DBEngine<int, DBElement<int, string>>();
            dbs.insert(1, elem1);
            dbs.show<int, DBElement<int, string>, string>();
            WriteLine();

            //Insert first element into String Key/Value DB
            Write("\n --- Test DBElement<string,List<string>> ---");
            DBElement<string, List<string>> newelem1 = new DBElement<string, List<string>>();
            newelem1.name = "newelem1";
            newelem1.descr = "test new type";
            newelem1.children = new List<string> { "Key1", "Key2" };
            newelem1.payload = new List<string> { "one", "two", "three" };
            Write(newelem1.showElement<string, List<string>, string>());
            DBEngine<string, DBElement<string, List<string>>> dbe = new DBEngine<string, DBElement<string, List<string>>>();
            dbe.insert("key1", newelem1);
            dbe.show<string, DBElement<string, List<string>>, List<string>, string>();
            Write("\n\n");
        }
    }
}
