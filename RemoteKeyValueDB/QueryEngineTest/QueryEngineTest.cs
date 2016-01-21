///////////////////////////////////////////////////////////////
// QueryEngine.cs -  QueryEngineTest for noSQL database          //
// Ver 1.0                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements Query Engine Test
 * which is used for querying engine processing and querying the DB based on the user inputs
 *
 * Note: This package defines querying engine
 */
/*
 *  Public Interface
 *  ----------------
 *  QueryEngineTest<Key, Value> query =new QueryEngineTest<Key, Value>();
 *  query.GetValue(key);
 * Maintenance:
 * ------------
 * Required Files: DBElement.cs,DBEngine.cs,ExceptionLayer.cs   
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15 : This package implements Query Engine Test layer
 * which is used for querying engine processing and querying the DB based on the user inputs
 */
using DBElementSpace;
using DBEngineSpace;
using QueryEngineSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEngineTest
{
    class QueryEngineTestClass
    {
        private static DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
        private static QueryEngine<int, DBElement<int, string>> query = new QueryEngine<int, DBElement<int, string>>();
        static void Main(string[] args)
        {
            Console.WriteLine("Started testing of QueryEngineTest\n");
            DBElement<int, string> elem = new DBElement<int, string>();
            int key = 54;
            elem.name = "2015 Maserati GranTurismo";
            elem.descr = "Ferrari FF new model 2015";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3, 4 });
            elem.payload = "Make:Maserati;Model:Maserati GranTurismo;Color:Green;Year:2015;";
            key = 54;
            db.insert(54, elem);
            bool status = query.getKey(db, key, out elem);
            Console.WriteLine("Obtained the key successfully\n {0} ,{1}, {2} ,{3} \n", elem.name, elem.payload, elem.timeStamp, elem.descr);
        }
    }
}
