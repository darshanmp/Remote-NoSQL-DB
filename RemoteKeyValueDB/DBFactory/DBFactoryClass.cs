///////////////////////////////////////////////////////////////
// DBFactoryClass.cs - DBFactoryClass for noSQL database     //
// Ver 1.0                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements methods to support the creation of DBFactory 
 * GetValue and Clone functionalities are implemented here 
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 * DBEngine.cs, DBElement.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 *   Public Interface
 *   ----------------
 *  DBFactoryClass  x = new DBFactoryClass();
 *  x.Insert(5);
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15 : Implemneted methods to support the creation of DBFactory 
 * GetValue and Clone functionalities are implemented here 
 */

using DBElementSpace;
using DBEngineSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBFactoryClassSpace
{
    /// <summary>
    /// Class for generating factory class for the DB and also the immutable DB is here
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class DBFactoryGen<Key, Value>
    {

        private Dictionary<int, string> dbImmut = new Dictionary<int, string>();
        public DBFactoryGen()
        {

        }

        /// <summary>
        /// Cloning method to create a immutable DB
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool CloneDB(List<int> child)
        {
            int j = 0;
            foreach (var i in child)
            {
                dbImmut.Add(j, i.ToString());
                j++;
            }
            return true;
        }

        /// <summary>
        /// Get the value for a specific key 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        /// 
        public bool getValue(int key, out string value)
        {
            bool result = false;
            value = "";
            foreach (var i in dbImmut.Keys)
            {
                if (i == key)
                {
                    value = dbImmut[key];
                    result = true;
                }
            }
            return result;
        }
    }
#if (TEST_DBFACTORYCLASS)
    class DBFactoryMain
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing DBEngine Package\n");
            List<int> lstChi = new List<int>();
            Console.WriteLine("Cloning the DB\n");
            DBFactoryGen<int, DBElement<int, string>> clon = new DBFactoryGen<int, DBElement<int, string>>();         
            lstChi.Add(1);
            lstChi.Add(2);
            lstChi.Add(3);
            clon.CloneDB(lstChi);       //clone the Db 
            int j = 0;string value;
            Console.WriteLine("\n..We will store the same in a immutable DB..\n");
            Console.WriteLine("\n..get the value from the immutable Db..\n");
            foreach (var i in lstChi)
            {
                clon.getValue(j, out value);     //get the value from the immutable Db
                Console.WriteLine("Key : {0} , Value : {1} \n", j, value);
                j++;
            }

        }
    }
#endif
}
