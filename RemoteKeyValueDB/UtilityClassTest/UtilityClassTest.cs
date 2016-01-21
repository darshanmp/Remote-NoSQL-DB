///////////////////////////////////////////////////////////////
// UtilityClassTest.cs - Project #2 Key/Value Database       //
// Ver 1.2                                                   //
// Application: Demonstration for Project#2                  //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package begins the demonstration of Utility class.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   TestExec.cs,  DBElement.cs, DBEngine, Excpetion
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 *   Public Interface
 *   ----------------
 *   Utility clnt = new Utility();
 *   clnt.GetURL();
 * Maintenance History:
 * --------------------
 * ver 1.2 : 04 Oct 15  : Utility class generation for persisting and restoring the DB
 * ver 1.1 : 24 Sep 15
 * ver 1.0 : 18 Sep 15
 * - first release
 *
 */
using DBElementSpace;
using DBEngineSpace;
using Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLUtility
{
    class UtilityClassTest
    {
        private static DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
        static void Main(string[] args)
        {
            DBElement<int, string> elem = new DBElement<int, string>();
            elem.name = "2015 Maserati GranTurismo";
            elem.descr = "Ferrari FF new model 2015";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3, 4 });
            elem.payload = "Make:Maserati;Model:Maserati GranTurismo;Color:Green;Year:2015;";
            db.insert(54, elem);
            string path = "";
            "Demonstrating Requirement #6".title();
            Console.WriteLine();
            Console.WriteLine("..Showing the Scheduled Persistance Process..\n");
            Console.Write("..Wait for 3 seconds..\n\n");
            Scheduler sch = new Scheduler();
            sch.PersistXML(db);         //Persist to XML 
            XmlTextReader textReader = new XmlTextReader("PersistXMLFile.xml");
            path = textReader.BaseURI;
            textReader.Close();
            Console.WriteLine("..Persisted Elements successfully to location '{0}'..\n", path);
        }
    }
}
