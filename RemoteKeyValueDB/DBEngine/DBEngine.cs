///////////////////////////////////////////////////////////////
// DBEngine.cs - DBEngine for noSQL database                 //
// Ver 1.3                                                  //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
// Source:      Jim Fawcett, CST 4-187, Syracuse University  //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements DBEngine<Key, Value> where Value
 * is the DBElement<key, Data> type.
 *
 * This class is for the DBEngine package.
 * It implements many of the requirements for the db, e.g.,
 * It adds and edit elements,remove elements, gets value for the specific key
 * to retreive keys,values
 *   Public Interface
 *   ----------------
 *  DBEngine <int,string> x = new DBEngine <int,string>();
 *  x.GetValue(5);
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs,Exception Layer.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.3 : 7 Oct 15  : Added few methods for Editing/Cloning/Deleting key/value pairs
 * ver 1.2 : 24 Sep 15
 * - removed extensions methods and tests in test stub
 * - testing is now done in DBEngineTest.cs to avoid circular references
 * ver 1.1 : 15 Sep 15
 * - fixed a casting bug in one of the extension methods
 * ver 1.0 : 08 Sep 15
 */

using DBElementSpace;
using ExceptionLayerSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Console;

namespace DBEngineSpace
{
    /// <summary>
    /// Class for DBEngine which exposes various methods for 
    /// adding and editing elements,removing elements, geting value for the specific key,
    /// to retreive all keys and all values
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class DBEngine<Key, Value> : ICloneable
    {
        /// <summary>
        /// Dictionary object which holds our main database
        /// </summary>
        private Dictionary<Key, Value> dbStore;
        public DBEngine()
        {
            dbStore = new Dictionary<Key, Value>();
        }

        /// <summary>
        /// Clone method which returns the cloned object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        /// <summary>
        /// Insert keys/values into Database
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool insert(Key key, Value val)
        {
            if (dbStore.Keys.Contains(key))
                return false;
            dbStore[key] = val;
            return true;

        }
        /// <summary>
        /// Retrieves values from database
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool getValue(Key key, out Value val)
        {
            try
            {
                bool result = false;
                val = default(Value);
                if (dbStore.Keys.Contains(key))
                {
                    val = dbStore[key];
                    result = true;
                }
                else
                {
                    result = false;
                    val = default(Value);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new CustomException("error at dbengine getvalue", ex);
            }
        }
        /// <summary>
        /// Gets all the keys from DB
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Key> Keys()
        {
            return dbStore.Keys;
        }
        /// <summary>
        /// Retreives the DB object 
        /// </summary>
        /// <returns></returns>
        public Dictionary<Key, Value> GetDBStore()
        {
            return dbStore;
        }
        /// <summary>
        /// Edits the Key/Value pair based on the key provided and returns the success/error details
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Edit(Key key, Value value, out string error)
        {
            try
            {
                error = "Success";
                if (!dbStore.Keys.Contains(key))
                {
                    error = "Key not found in database";
                    return false;
                }
                else
                {
                    dbStore[key] = value;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("error at dbengine edit", ex);
            }
        }

        /// <summary>
        /// Deletes the Key/Value pair based on the key provided and returns the success/error details
        /// </summary>
        /// <param name="key"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Delete(Key key, out string error)
        {
            try
            {
                error = "Success";
                if (!dbStore.Keys.Contains(key))
                {
                    error = "Key not found in database";
                    return false;
                }
                else
                {
                    dbStore.Remove(key);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occurred while persisting XML in DBEngine", ex);
            }
        }

        /// <summary>
        /// Clears the contents of the DB
        /// </summary>
        /// <returns></returns>
        public bool ClearDB()
        {
            dbStore.Clear();
            return true;
        }
     

    }

#if (TEST_DBENGINE)

    class TestDBEngine
    {
        static void Main(string[] args)
        {
            Write("\n  All testing of DBEngine class moved to DBEngineTest package.");
            Write("\n  This allow use of Utilty package without circular dependencies.");
            Write("\n\n");

        }
    }
#endif
}
