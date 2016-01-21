///////////////////////////////////////////////////////////////
// QueryEngine.cs -  QueryEngine for noSQL database          //
// Ver 1.0                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements Query Engine 
 * which is used for querying engine processing and querying the DB based on the user inputs
 *
 * Note: This package defines querying engine
 */
/*
 *  Public Interface
 *  ----------------
 *  QueryEngine<Key, Value> query =new QueryEngine<Key, Value>();
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
 * ver 1.0 : 7 Oct 15 : This package implements Query Engine 
 * which is used for querying engine processing and querying the DB based on the user inputs
 */
using DBElementSpace;
using DBEngineSpace;
using ExceptionLayerSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Runtime.Caching;


namespace QueryEngineSpace
{

    /// <summary>
    /// Class for Query Engine used to GetKey,GetChildren,GetMetadata and GetTimeStampCriteria
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class QueryEngine<Key, Value>
    {
        public QueryEngine()
        {

        }
        /// <summary>
        /// Get the Value based on the Key on the Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        /// 
        public bool getKey(DBEngine<int, DBElement<int, string>> db, int key, out DBElement<int, string> elem)
        {
            bool status = false;
            status = db.getValue(key, out elem);//Get value for the key
            return status;
        }
        /// <summary>
        /// Get the children of the parent key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool getChildren(DBEngine<int, DBElement<int, string>> db, int key, out DBElement<int, string> elem)
        {
            try
            {
                bool result = false;
                if (key < 0)
                {
                    elem = new DBElement<int, string>();
                    return false;
                }
                else
                {
                    result = db.getValue(key, out elem);//Get the children of the parent key
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occurred while getting children", ex);
            }
        }

        /// <summary>
        /// Get the criteria based on the key on a search pattern
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool getKeyCriteria(DBEngine<string, DBElement<string, List<string>>> db, string parameter, out List<string> elem)
        {
            try
            {
                bool result = false;
                elem = new List<string>();
                List<string> lstKeys = db.Keys().ToList();     //Get the keys of the db
                foreach (var i in lstKeys)
                {
                    if (i.Contains(parameter))      //Search pattern
                        elem.Add(i);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occurred while getting Key criteria", ex);
            }
        }
        /// <summary>
        /// Get the criteria based on the metadata for a search pattern  
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool getMetadataCriteria(DBEngine<int, DBElement<int, string>> db, string parameter, out List<int> elem)
        {
            try
            {
                bool result = false;
                elem = new List<int>();
                Dictionary<int, DBElement<int, string>> lstKeys = db.GetDBStore();
                var dictElem = lstKeys.Select(n => new { n.Key, n.Value }); //Get the list of keys and values   
                foreach (var i in dictElem)
                {
                    DBElement<int, string> strelm = i.Value as DBElement<int, string>;
                    if (strelm.name.Contains(parameter))    //Search for the name pattern
                    {
                        elem.Add(i.Key);
                        result = true;
                    }
                    else if (strelm.descr.Contains(parameter))  //Search for the description pattern
                    {
                        elem.Add(i.Key);
                        result = true;
                    }
                    else if (strelm.payload.Contains(parameter))    //Search for the payload pattern
                    {
                        elem.Add(i.Key);
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occurred while getting metadata criteria", ex);
            }
        }
     

/// <summary>
///  Get the criteria based on the timestamp criteria for a search pattern  
/// </summary>
/// <param name="db"></param>
/// <param name="FromDate"></param>
/// <param name="ToDate"></param>
/// <param name="elem"></param>
/// <returns></returns>
public bool getTimeStampCriteria(DBEngine<int, DBElement<int, string>> db, DateTime FromDate, DateTime ToDate, out List<int> elem)
        {
            try
            {
                bool result = false;
                elem = new List<int>();
                Dictionary<int, DBElement<int, string>> lstKeys = db.GetDBStore();
                var dictElem = lstKeys.Select(n => new { n.Key, n.Value });
                foreach (var i in dictElem)
                {
                    DBElement<int, string> strelm = i.Value as DBElement<int, string>;
                    if (strelm.timeStamp >= FromDate && strelm.timeStamp <= ToDate) //Based on From Date and To Date
                    {
                        elem.Add(i.Key);
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occurred while persisting XML in DBEngine", ex);
            }
        }
    }
#if (TEST_QUERYENGINE)
    public class QueryEngineMain
    {
        static void Main(string[] args)
        {
            Write("\n  All testing of QueryEngine class moved to QueryEngineTest package.");
            Write("\n  This allow use of QueryEngine package without circular dependencies.");
            Write("\n\n");
        }
    }
#endif
}
