///////////////////////////////////////////////////////////////
// TestExec.cs - Project #4 Remote Key/Value Database        //
// Ver 1.2                                                   //
// Application: Demonstration for Project#2                  //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash
// Source:      Jim Fawcett, CST 4-187, Syracuse University  //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package begins the demonstration of RequestHandler.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   TestExec.cs,  DBElement.cs, DBEngine, Display, 
 *   DBExtensions.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.2 : 04 Oct 15  : Test Exec package where we show to the user various functionalities of the Key/Value DB
 * ver 1.1 : 24 Sep 15
 * ver 1.0 : 18 Sep 15
 * - first release
 *
 */
using DBElementSpace;
using DBEngineSpace;
using DBFactoryClassSpace;
using DisplaySpace;
using ExceptionLayerSpace;
using ItemEditorSpace;
using ItemFactorySpace;
using Project4Starter;
using QueryEngineSpace;
using Schedule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using XMLUtility;
using static System.Console;


namespace RequestHandlerSpace
{
    public interface IClone
    {
        IClone Clone();
    }

    /// <summary>
    /// Test Executive class for displaying all the requirements
    /// </summary>
    public class RequestHandler
    {
        public string DisplayDBStatus { get; set; }
        private DBEngine<int, DBElement<int, string>> db = null;
        private DBEngine<int, DBElement<int, string>> logDb = null;
        private QueryEngine<int, DBElement<int, string>> query = null;
        private DBEngine<string, DBElement<string, List<string>>> dbString = null;
        DBElement<string, List<string>> strElem = null;
        ItemGenerator itemGen = new ItemGenerator();
        private System.Text.StringBuilder sb = null;
        public RequestHandler()
        {
            sb = new StringBuilder();
            db = new DBEngine<int, DBElement<int, string>>();
            query = new QueryEngine<int, DBElement<int, string>>();
            dbString = new DBEngine<string, DBElement<string, List<string>>>();
            strElem = new DBElement<string, List<string>>();
        }
        /// <summary>
        /// Insert tested for addition and deletion of key/value pairs.
        /// </summary>
        public bool Insert(string msg, ref string msgid)
        {
            XElement c;
            DBElement<int, string> elem;
            string key;
            InsertCreator(msg, out msgid, out c, out elem, out key);
            foreach (var i in c.Elements("Value").Single().Elements("Children"))
            {
                elem.children.Add(int.Parse(i.Element("Child").Value));
            }
            WriteLine("Inserting Element with Key {0}\n", key);
            db.insert(int.Parse(key), elem);
            WriteLine("\n\nElement Inserted successfully");
            //db.showDB();
            return true;
        }
        public bool LogCreator(int key,string logMessage, ref string msgid)
        {
            DBElement<int, string> elem =new DBElement<int, string>();
            elem.name = "LogMes" + key.ToString();
            elem.payload = logMessage;
            elem.timeStamp = DateTime.Now;
            elem.descr = "LogMessage";
            WriteLine("Logging Element with Key {0}\n", key);
            logDb.insert(key, elem);
            WriteLine("\n\nElement logged successfully into Event Logging DB\n");
            logDb.showDB();
            //Log into text file
            string error = "",path="",xml="";
            logDb.PersistToXML(out error, out path, out xml);      // PersistXMLFile.xml
           // File.AppendAllText("log.txt", logMessage);
            return true;
        }

        private static void InsertCreator(string msg, out string msgid, out XElement c, out DBElement<int, string> elem, out string key)
        {
            XDocument doc = XDocument.Load(new StringReader(msg));
            c = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Insert").Select(i => i).Single();
            elem = new DBElement<int, string>();
            key = doc.Descendants("Key").Select(i => i).Single().Value;
            msgid = doc.Descendants("MessageID").Select(i => i).Single().Value;
            elem.name = c.Elements("Value").Single().Element("Name").Value;
            elem.descr = c.Elements("Value").Single().Element("Description").Value;
            elem.payload = c.Elements("Value").Single().Element("Payload").Value;
            elem.timeStamp = DateTime.Parse(c.Elements("Value").Single().Element("TimeStamp").Value);
        }

        /// <summary>
        /// Show DB results at the client side
        /// </summary>
        /// <param name="status"></param>
        /// <param name="typeOfDB"></param>
        public void ShowDB(bool status,string typeOfDB)
        {
            if (status == true)
            {
                if (typeOfDB == "int")
                    db.showDB();
                else
                    dbString.showStringDB();
                }
        }
        /// <summary>
        /// handle string db requests at the client side
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        /// <returns></returns>
        public bool stringDBInsert(string msg, ref string msgid)
        {
            XDocument doc = XDocument.Load(new StringReader(msg));
            WriteLine("...We show below a Insert operation for String and a List<string> Database...\n");
            XElement c = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "StringDBInsert").Select(i => i).Single();
            string key = doc.Descendants("Key").Select(i => i).Single().Value;
            msgid = doc.Descendants("MessageID").Select(i => i).Single().Value;
            strElem.name = c.Elements("Value").Single().Element("Name").Value;
            strElem.descr = c.Elements("Value").Single().Element("Description").Value;
            strElem.payload = new List<string>();
            foreach (var i in c.Elements("Value").Single().Element("Payload").Elements("Data"))
            {
                strElem.payload.Add(i.Value);
            }
            strElem.timeStamp = DateTime.Parse(c.Elements("Value").Single().Element("TimeStamp").Value);
            foreach (var i in c.Elements("Value").Single().Elements("Children"))
            {
                strElem.children.Add(i.Element("Child").Value);
            }
            WriteLine("...Inserting Element with Key {0}...\n", key);
            dbString.insert(key, strElem);
            dbString.showStringDB();
            return true;
        }

        /// <summary>
        ///Deletion of Key/Value pairs
        /// </summary>
        public bool Delete(string msg, ref string msgid)
        {
            XDocument doc = XDocument.Load(new StringReader(msg));
            XElement c = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Delete").Select(i => i).Single();
            int key = int.Parse(doc.Descendants("Key").Select(i => i).Single().Value);
            string error;
            bool status = false;
            WriteLine("Showing the database before deletion\n");
           // db.showDB();    //Show the DB contents
            WriteLine("..Deleting the entry from the DB {0} ..\n", key);
            status = db.Delete(key, out error);      //Delete the DB
            WriteLine("Value with Key {0} has been deleted successfully\n", key);
            //db.showDB();                        //Show the DB contents
            WriteLine("\n");
            return status;
        }

        /// <summary>
        /// Update entries for Key/Value Pairs.
        /// </summary>
        public bool Update(string msg, ref string msgid)
        {
            string error, key;
            bool status;
            DBElement<int, string> elem;
            UpdateCreator(msg, out msgid, out error, out status, out elem, out key);
            status = db.Edit(int.Parse(key), elem, out error);
            WriteLine("\n\n...Element updated successfully...");
            //db.showDB();
            return true;
        }

        private static void UpdateCreator(string msg, out string msgid, out string error, out bool status, out DBElement<int, string> elem, out string key)
        {
            error = "";
            status = false;
            XDocument doc = XDocument.Load(new StringReader(msg));
            XElement c = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Update").Select(i => i).Single();
            elem = new DBElement<int, string>();
            key = doc.Descendants("Key").Select(i => i).Single().Value;
            msgid = doc.Descendants("MessageID").Select(i => i).Single().Value;
            elem.name = c.Elements("Value").Single().Element("Name").Value;
            elem.descr = c.Elements("Value").Single().Element("Description").Value;
            elem.payload = c.Elements("Value").Single().Element("Payload").Value;
            elem.timeStamp = DateTime.Parse(c.Elements("Value").Single().Element("TimeStamp").Value);
            foreach (var i in c.Elements("Value").Single().Elements("Children"))
                elem.children.Add(int.Parse(i.Element("Child").Value));
            WriteLine("...Updating Element with Key {0}...\n", key);
        }

        /// <summary>
        /// Manual Persist
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        /// <returns></returns>
        public bool ManualPersist(string msg, ref string msgid)
        {
            bool status = false; string error = "", path = ""; string xml = "";
            WriteLine("\n..Manual persisting of XML file under progress..Contents of DB will not be deleted after persisting..\n");
            status = db.PersistToXML(out error, out path,out xml);      // PersistXMLFile.xml
            if (status == true)
                WriteLine("..Persisted Elements successfully to location ' {0} '..\n", path);
            return status;
        }
        /// <summary>
        /// Restore Augment DB
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        /// <returns></returns>
        public bool RestoreAugmentDB(string msg, ref string msgid)
        {
            bool status = false; string error = "";
            WriteLine("\n..Restoring DB and Augmenting the DB from xml..\n");
            DBEngine<int, DBElement<int, string>> dbNew = new DBEngine<int, DBElement<int, string>>();
            dbNew.RestoreDB(out error, out status);      //Persist to new DB from xml 
            if (status == true)
            {
                WriteLine("\n..New DB is restored & augmented successfully from PersistXMLFile.xml..\n");
                dbNew.showDB();
            }
            return status;
        }

        /// <summary>
        /// To accept a positive time interval or number of writes after which the database contents 
        /// are persisted. This scheduled "save" process shall executed once.
        /// </summary>
        public bool ScheduledPersistance(string msg, ref string msgid)
        {
            string path = "", error = "";bool status = false;
            double time = 0.0; 
            XDocument doc = XDocument.Load(new StringReader(msg));
            XElement c = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "ScheduledPersist").Select(i => i).Single();
            time = double.Parse(c.Elements("TimeInterval").Single().Value);
            WriteLine("\n..Showing the Scheduled Persistance Process..\n");
            Console.Write("..Scheduling two persists 1 seconds apart..\n\n");
            System.Timers.Timer schedular = new System.Timers.Timer();
            schedular.Interval = time;
            schedular.AutoReset = true;
            int count = 0;
            string xml = "";
            schedular.Elapsed += (object source, ElapsedEventArgs e) =>
            {
                ++count;
                status = db.PersistToXML(out error, out path,out xml);
                Write("\n  Persisting Database to {0}\n" , path);      
                WriteLine(xml);
                if (count == 2)
                {
                    schedular.Enabled = false;
                }
            };
            schedular.Enabled = true;
            Thread.Sleep(3000);
            return true;
        }
        /// <summary>
        /// R#7 Queries for the value of a specified key,The children of a specified key.
        /// The set of all keys matching a specified pattern which defaults to all keys.
        /// </summary>
        public void ReadQueryHandle(ref Message msg, ref string msgid)
        {
            bool status = false;
            string pattern = "";
            XDocument doc = XDocument.Load(new StringReader(msg.content));            
            string op = doc.Descendants("OperationMessage").Select(i => i.Element("QueryType")).Single().Value;
            msgid = doc.Descendants("MessageID").Select(i => i).Single().Value;
            int key = 0;
            switch(op)
            {
                case "GetByKey":
                    DBElement<int, string> elem;
                    GetByKey(out status, ref doc, out key, out elem);
                    msg.content = doc.ToString();
                    break;
                case "GetChildren":
                    GetChildren(out status, ref doc, out key, out elem);
                    msg.content = doc.ToString();
                    break;
                case "GetKeyCriteria":
                    pattern = GetKeyCriteria(ref doc);
                    msg.content = doc.ToString();
                    break;
                case "GetByMetadataCriteria":
                    pattern = GetByMetaData(ref doc);
                    msg.content = doc.ToString();
                    break;
                case "GetByTimeStampCriteria":
                    List<int> keyMet;
                    DateTime FromDate = DateTime.Parse(doc.Descendants("FromDate").Select(i => i).Single().Value);
                    DateTime ToDate;
                    GetByTimeStampCriteria(ref doc, out keyMet, FromDate, out ToDate);
                    msg.content = doc.ToString();
                    break;
            }
        }

        /// <summary>
        /// Get by timestamp results are handled here and query constructed
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="keyMet"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        private void GetByTimeStampCriteria(ref XDocument doc, out List<int> keyMet, DateTime FromDate, out DateTime ToDate)
        {
            
            if (!doc.Descendants("ToDate").Any() || doc.Descendants("ToDate").Select(i => i).Single().Value == "")
            {
                ToDate = DateTime.Now;
            }
            else
                ToDate = DateTime.Parse(doc.Descendants("ToDate").Select(i => i).Single().Value);
            WriteLine("\nGetting the set of all keys that contain value within a specified time-date interval '{0}' to '{1}'..\n", FromDate, ToDate);
            query.getTimeStampCriteria(db, FromDate, ToDate, out keyMet);   //get the criteria based on the date range
            WriteLine("\nKeys that has Date Range between {0} and {1} are : \n", FromDate, ToDate);
            foreach (var i in keyMet)
            {
                DBElement<int, string> strElem = new DBElement<int, string>();
                query.getKey(db, i, out strElem); //get the value based on key              
                sb.Append("Key:" + i);
                sb.AppendLine();
                //WriteLine("Key: " + i);
                sb.Append(strElem.showElement()+"\n");      //show the db
                //WriteLine("\n");
            }
            doc.Descendants("Response").Select(i => i).Single().Value = sb.ToString();
            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        ///  Get by GetKeyCriteria results are handled here and query constructed
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string GetKeyCriteria(ref XDocument doc)
        {
            sb = new StringBuilder();
            string pattern = doc.Descendants("Criteria").Select(i => i).Single().Value;
            WriteLine("\nGetting the set of all keys that contains the pattern {0}..\n", pattern);
            List<string> keys = new List<string>();
            query.getKeyCriteria(dbString, pattern, out keys);
            WriteLine("\nKeys that starts with Pattern:{0} are : \n", pattern);
            foreach (var i in keys)
            {
                sb.Append("Key:\n" + i);
                sb.AppendLine();
                //WriteLine("Key:\n " + i);
            }
            Console.WriteLine(sb.ToString());
            doc.Descendants("Response").Select(i => i).Single().Value = sb.ToString();
            dbString.showStringDB();
            return pattern;
        }

        /// <summary>
        /// Get by GetByMetaData results are handled here and query constructed
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string GetByMetaData(ref XDocument doc)
        {
            sb = new StringBuilder();
            string pattern = doc.Descendants("Criteria").Select(i => i).Single().Value;
            List<int> dbKeys = new List<int>();
            WriteLine("\n..Getting the set of all keys that contain a specified string '{0}' in metadata {0}..\n", pattern);
            List<int> keyMet = new List<int>();
            query.getMetadataCriteria(db, pattern, out keyMet);   //Get the metadata criteria
            WriteLine("\n ..Keys that has metadata with {0} in it are : \n", pattern);
            foreach (var i in keyMet)
            {
                DBElement<int, string> strElem = new DBElement<int, string>();
                query.getKey(db, i, out strElem);
                sb.Append(" Key: " + i);
                //WriteLine("Key: " + i);
                sb.AppendLine();
                sb.Append(strElem.showElement());
                //WriteLine("\n");
            }
            doc.Descendants("Response").Select(i => i).Single().Value = sb.ToString();
            Console.WriteLine(sb.ToString());
            return pattern;
        }
        /// <summary>
        /// Get by GetChildren results are handled here and query constructed
        /// </summary>
        /// <param name="status"></param>
        /// <param name="doc"></param>
        /// <param name="key"></param>
        /// <param name="elem"></param>
        private void GetChildren(out bool status, ref XDocument doc, out int key, out DBElement<int, string> elem)
        {
            sb = new StringBuilder();
            key = int.Parse(doc.Descendants("Key").Select(i => i).Single().Value);
            WriteLine("\n\n..Getting the children of specific key..\n");
            status = query.getChildren(db, key, out elem);
            WriteLine("\nChildren for Key:{0} are\n",key);
            sb.Append(elem.showChildren());
            sb.AppendLine();
            doc.Descendants("Response").Select(i => i).Single().Value = sb.ToString();
            elem.showChildren();
        }
        /// <summary>
        /// Get by GetByKey results are handled here and query constructed
        /// </summary>
        /// <param name="status"></param>
        /// <param name="doc"></param>
        /// <param name="key"></param>
        /// <param name="elem"></param>
        private void GetByKey(out bool status, ref XDocument doc, out int key, out DBElement<int, string> elem)
        {
            sb = new StringBuilder();
            key = int.Parse(doc.Descendants("Key").Select(i => i).Single().Value);
            WriteLine("\n..Getting the value for key {0} ..\n", key);
            elem = new DBElement<int, string>();
            status = query.getKey(db, key, out elem);
            sb.Append(String.Format(" Key:" + key));
            sb.AppendLine();
            sb.Append(String.Format(" Value:"));
            sb.AppendLine();
            WriteLine(sb.ToString());
            sb.Append(elem.showElement());
            doc.Descendants("Response").Select(i => i).Single().Value = sb.ToString();
        }

        /// <summary>
        /// Creation of a new immutable database constructed from the result of any query that returns a collection of keys
        /// </summary>
        public void ImmutableDB(string msg, ref string msgid)
        {
            try
            {
                string value;
                List<int> lstChi;
                ImmutableCloneDb(msg, out msgid, out value, out lstChi);
                DBFactoryGen<int, DBElement<int, string>> clon = new DBFactoryGen<int, DBElement<int, string>>();
                clon.CloneDB(lstChi);       //clone the Db 
                WriteLine("..The results have been stored in a Immutable DB can be verified in Line 20 of DBFactoryClass..\n");
                WriteLine("..The DB cannot be edited or will not change over time.\n");
                WriteLine("DB is immutable since there are no insert or edit methods\n");
                WriteLine("..The results are now being retreived from Immutable DB..\n");
                int j = 0;
                WriteLine("***************************************************************************************************************\n");
                WriteLine("                                             IMMUTABLE DATABASE \n");
                foreach (var i in lstChi)
                {
                    clon.getValue(j, out value);     //get the value from the immutable Db
                    WriteLine("Key : {0} , Value : {1} \n", j, value);
                    j++;
                }
                WriteLine("***************************************************************************************************************\n");
            }
            catch (CustomException)
            {
                WriteLine("Error in immutable db \n");
            }
        }
        /// <summary>
        /// Immutable clone db constructed here 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        /// <param name="value"></param>
        /// <param name="lstChi"></param>
        private void ImmutableCloneDb(string msg, out string msgid, out string value, out List<int> lstChi)
        {
            Write("\n    Creation of immutable database from query");
            WriteLine();
            bool status = false; value = "";
            DBElement<string, string> elem = new DBElement<string, string>();
            XDocument doc = XDocument.Load(new StringReader(msg));
            msgid = doc.Descendants("MessageID").Select(i => i).Single().Value;
            int key = int.Parse(doc.Descendants("Key").Select(i => i).Single().Value);
            WriteLine("..Creation of new immutable Database from compound queries..\n");
            WriteLine("..First we will retrieve the results from the main DB..\n");
            DBElement<int, string> elem1 = new DBElement<int, string>();
            WriteLine("\n\n..Getting the children of specific key..\n");
            status = query.getChildren(db, key, out elem1);      //get the children based on the key provided
            DBFactoryGen<int, string> el = new DBFactoryGen<int, string>();
            WriteLine("\n..Children with Key {0} successfully retrieved..\n", key);
            WriteLine("  Key : {0}", key);
            elem1.showChildren();       //show the child elements
            lstChi = new List<int>();
            WriteLine("\n..We will store the same in a immutable DB..\n");
            if (elem1 != null && elem1.children != null)
            {
                foreach (var i in elem1.children)
                {
                    lstChi.Add(i);
                }
            }
        }

        /// <summary>
        /// #R9 Form of an XML file, that describe your project's package structure and 
        /// dependency relationships that can be loaded when the project is graded.
        /// </summary>
        public void LoadPackageStructure(string msg, ref string msgid)
        {
            try
            {
                string error = ""; bool status = false;
                WriteLine("Showing the Database before loading package structure\n");
                XmlTextReader textReader = new XmlTextReader("PackageStructure.xml");
                string path = textReader.BaseURI;
                WriteLine("Package is at the location '{0}'\n", path);
                dbString.showStringDB();          //Show the string Key/Value DB
                WriteLine("..Loading the Package Structure into DB..\n");
                dbString.RestoreStringDB(out error, out status);    //Restore string Key/value Db
                Console.WriteLine("\n..The package structure has been loaded successfully into DB\n");
                dbString.showStringDB();        //show the string DB
            }
            catch(CustomException)
            {
                Console.WriteLine("Error in LoadPackageStructure");
            }
        }
        static void Main(string[] args)
        {
            try
            {
                RequestHandler exec = new RequestHandler();
                //WriteLine("***************************************************************************************************************");
                //WriteLine("                                             KEY/VALUE DATABASE \n");
                //WriteLine("***************************************************************************************************************");
                //WriteLine();
                //exec.TestR1();
                //WaitForUser();
                //exec.TestR2();
                //WaitForUser();
                //exec.TestR3();
                //WaitForUser();
                //exec.TestR4();
                //WaitForUser();
                //exec.TestR5();
                //WaitForUser();
                //exec.TestR6();
                //WaitForUser();
                //exec.TestR7();
                //WaitForUser();
                //exec.TestR8();
                //WaitForUser();
                //exec.TestR9();
                //WaitForUser();
                //exec.TestR10();
                //WaitForUser();
                //exec.TestR11();
                //Write("\n\n");
                //WriteLine("Press any key to exit..\n");
                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw new CustomException("error at testexec main", ex);
            }
        }
    }
}
