///////////////////////////////////////////////////////////////
// DBElement.cs - Define element for noSQL database          //
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
 * This package implements the DBElement<Key, Data> type, used by 
 * DBEngine<key, Value> where Value is DBElement<Key, Data>.
 *
 * The DBElement<Key, Data> state consists of metadata and an
 * instance of the Data type.
 *
 *   ItemFactory - used to ensure that all db elements have the
 *                 same structure even if built by different
 *                 software parts.
 *   ItemEditor  - used to ensure that db elements are edited
 *                 correctly and maintain the intended structure.
 *   Public Interface
 *   ----------------
 *  DBElement <int,string> x = new DBElement <int,string>();
 *  x.GetValue(5);
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBElement.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.1 : 7  Oct 15
 * ver 1.0 : 24 Sep 15
 * - removed extension methods, removed tests from test stub
 * - Testing now  uses DBEngineTest.cs
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

/// <summary>
/// Namespace for DBElement
/// </summary>
namespace DBElementSpace
{
    /////////////////////////////////////////////////////////////////////
    // DBElement<Key, Data> class
    // - Instances of this class are the "values" in our key/value 
    //   noSQL database.
    // - Key and Data are unspecified classes, to be supplied by the
    //   application that uses the noSQL database.

    /// <summary>
    /// Class for the DBElement which will hold the Key/Value Dictionary
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Data"></typeparam>
    public class DBElement<Key, Data>
    {
        //The Element is comprised of Name,Description,Timestamp,Children elements pointing to the child records
        //Payload which contains the actual data
        public string name { get; set; }          // metadata    |
        public string descr { get; set; }         // metadata    |
        public DateTime timeStamp { get; set; }   // metadata   value
        public List<Key> children { get; set; }   // metadata    |
        public Data payload { get; set; }         // data        |

        /// <summary>
        /// Constructor of DBELement class where initializations of values are done
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Descr"></param>
        public DBElement(string Name = "unnamed", string Descr = "undescribed")
        {
            name = Name;
            descr = Descr;
            timeStamp = DateTime.Now;
            children = new List<Key>();
        }
    }

#if (TEST_DBELEMENT)

    /// <summary>
    /// This is to test the DBElement functionality.But here only the main functionality 
    /// is written as the testing code is moved to TestDBElementTest layer
    /// </summary>
    class TestDBElement
    {
        static void Main(string[] args)
        {
            Write("\n  All testing of DBElement class moved to DBElementTest package.");
            Write("\n  This allow use of DBExtensions package without circular dependencies.");
            Write("\n\n");
        }
    }
#endif
}
