///////////////////////////////////////////////////////////////
// DBExtensions.cs - DBExtensions for noSQL database         //
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
 * displaying DBElements and DBEngine instances.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   Utility.cs, DBEngine.cs, DBElement.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *   Public Interface
 *   ----------------
 *  DBExtension  x = new DBExtension();
 *  x.GetValue(5);
 * Maintenance History:
 * --------------------
 * ver 1.3 : 7 Oct 15 : Modified the methods to incorporate the new structure of Key(int)& Value(String) 
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
using ExceptionLayerSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace DBExtensionsSpace
{
    /////////////////////////////////////////////////////////////////////////
    // Extension methods class 
    // - Extension methods are static methods of a static class
    //   that extend an existing class by adding functionality
    //   not part of the original class.
    // - These methods are all extending the DBElement<Key, Data> class.
    //
    public static class DBElementExtensions
    {

        /// <summary>
        ///  ----< write metadata to string >-------------------------------------
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static string showMetaData<Key, Data>(this DBElement<Key, Data> elem)
        {
            try
            {
                StringBuilder accum = new StringBuilder();
                if (elem != null)
                {
                    accum.Append(String.Format("\n Name: {0}", elem.name));
                    accum.Append(String.Format("\n Text Description: {0}", elem.descr));
                    accum.Append(String.Format("\n Time: {0}", elem.timeStamp));
                    if (elem.children.Count() > 0)
                    {
                        accum.Append(String.Format("\n Children: "));
                        bool first = true;
                        foreach (Key key in elem.children)
                        {
                            if (first)
                            {
                                accum.Append(String.Format("{0}", key.ToString()));
                                first = false;
                            }
                            else
                                accum.Append(String.Format(", {0}", key.ToString()));
                        }
                    }
                }
                return accum.ToString();
            }
            catch (Exception ex)
            {
                throw new CustomException("Error at dbextension showmetadata", ex);
            }
        }

        /// <summary>
        /// write details of element with simple Data to string to show the children
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static string showChildren<Key, Data>(this DBElement<Key, Data> elem)
        {
            try
            {
                StringBuilder accum = new StringBuilder();
                if (elem != null)
                {
                    if (elem.children.Count() > 0)
                    {
                        accum.Append(String.Format("\n Children: "));
                        bool first = true;
                        foreach (Key key in elem.children)
                        {
                            if (first)
                            {
                                accum.Append(String.Format("{0}", key.ToString()));
                                first = false;
                            }
                            else
                                accum.Append(String.Format(", {0}", key.ToString()));
                        }
                    }
                }
                    return accum.ToString();                
            }
            catch (Exception ex)
            {
                throw new CustomException("Error at dbextension showchildren", ex);
            }
        }

        /// <summary>
        ///  write details of element with simple Data to string to show the element
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static string showElement<Key, Data>(this DBElement<Key, Data> elem)
        {
            try
            {
                if (elem != null)
                {
                    StringBuilder accum = new StringBuilder();
                    accum.Append(elem.showMetaData());
                    if (elem.payload != null)
                    {
                        accum.Append(String.Format("\n Payload:{0}", elem.payload.ToString()));
                    }
                    return accum.ToString();
                }
                return "";
            }
            catch (Exception ex)
            {
                throw new CustomException("Error at dbextension showelement", ex);
            }
        }

        /// <summary>
        ///  write details of element with simple Data to string to show the element
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static string showStringElement(this DBElement<string, List<string>> elem)
        {        
            try
            {
                if (elem != null)
                {
                    StringBuilder accum = new StringBuilder();
                    accum.Append(elem.showMetaData());
                    if (elem.payload != null)
                    {
                        accum.Append(String.Format("\n Payload: "));
                        foreach (var i in elem.payload.ToList())
                        {
                            accum.Append(String.Format("{0}", i));
                        }
                    }
                    return accum.ToString();
                }
                return "";
            }
            catch (Exception ex)
            {
                throw new CustomException("Error at dbextension showelement", ex);
            }
        }
        /// <summary>
        ///  write details of element with simple enumerable Data to string to show the element
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static string showElement<Key, Data, T>(this DBElement<Key, Data> elem)
          where Data : IEnumerable<T>  // constraint clause
        {
            try
            {
                StringBuilder accum = new StringBuilder();
                accum.Append(elem.showMetaData());
                if (elem != null)
                {
                    if (elem.payload != null)
                    {
                        IEnumerable<object> d = elem.payload as IEnumerable<object>;
                        if (d == null)
                            accum.Append(String.Format("\n Payload: {0}", elem.payload.ToString()));
                        else
                        {
                            bool first = true;
                            accum.Append(String.Format("\n Payload:\n  "));
                            foreach (var item in elem.payload)
                            {
                                if (first)
                                {
                                    accum.Append(String.Format("{0}", item));
                                    first = false;
                                }
                                else
                                    accum.Append(String.Format(", {0}", item));
                            }
                        }
                    }
                }
                return accum.ToString();
            }
            catch (Exception ex)
            {
                throw new CustomException("Error at dbextension showelement", ex);
            }
        }
    }
    public static class DBEngineExtensions
    {
        /// <summary>
        ///  write simple db elements out to Console
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Value"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <param name="db"></param>
        public static void show<Key, Value, Data>(this DBEngine<Key, Value> db)
        {
            try
            {
                Write("-------------------Database --------------------------\n");
                foreach (Key key in db.Keys())
                {
                    Value value;
                    db.getValue(key, out value);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                    Write("\n\n  -- Key = {0} --\n", key);
                    Write(elem.showElement());
                }
                WriteLine();
                Write("\n----------------------------------------------------\n");
            }
            catch (Exception ex)
            {
                throw new CustomException("Error at dbextension show", ex);
            }
        }

        /// <summary>
        ///  write simple db elements out to Console
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Value"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <param name="db"></param>
        public static void showStringDB<Key, Value, Data>(this DBEngine<Key, Value> db)
        {
            try
            {
                Write("-------------------Database --------------------------\n");
                foreach (Key key in db.Keys())
                {
                    Value value;
                    db.getValue(key, out value);
                  //  DBElement <Key, Data> elem = value as DBElement<Key,Data>;
                    DBElement<string, System.Collections.Generic.List < string >> elem = value as DBElement<string, System.Collections.Generic.List<string>>;
                    Write("\n\n  -- Key = {0} --", key);
                    Write(elem.showStringElement());               
                }
                WriteLine();
                Write("\n----------------------------------------------------\n");
            }
            catch (Exception ex)
            {
                throw new CustomException("Error at dbextension show", ex);
            }
        }

        /// <summary>
        /// write enumerable db elements out to Console
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Value"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        public static void show<Key, Value, Data, T>(this DBEngine<Key, Value> db)
          where Data : IEnumerable<T>
        {
            try
            {
                Write("\n-------------------Database --------------------------\n");
                foreach (Key key in db.Keys())
                {
                    Value value;
                    db.getValue(key, out value);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                    Write("\n\n  -- Key = {0} --", key);
                    Write(elem.showElement<Key, Data, T>());
                }
                WriteLine();
                Write("--------------------------------------------------------\n");
            }
            catch (Exception ex)
            {
                throw new CustomException("Error at dbextension show", ex);
            }
        }
    }

#if (TEST_DBEXTENSIONS)

    class TestDBExtensions
    {
        static void Main(string[] args)
        {
            Write("\n  Please check the DBExtensionsTest layer for Test Layer for this Package.");
            Write("\n  This allow use of DBExtensions package without circular dependencies.");
            Write("\n\n");

        }
    }
#endif
}
