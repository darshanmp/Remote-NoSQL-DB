/////////////////////////////////////////////////////////////////////////
//BasicRequirementTest.cs - CommService client sends and receives messages//
// ver 1.0                                                               //
// Darshan                                                               //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - in this incantation it is used to handle testing of Project 2 requirements
 * - If you provide command line arguments they should be ordered as:
 *   "/L " + lstPort[0].ToString() + " /R " + remoteCount + " /A " + remoteAddr + loggingFlag
 *
 *   Public Interface
 *   ----------------
 *   BasicRequirementTest bQ = new BasicRequirementTest();
 *   bQ.TestR7E(ref msg, sndr);
 */
/*
 * Maintenance History:
 * --------------------
 * ver 1.0 : 21 Nov 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using XMLUtility;
using System.Xml.Linq;
using ExceptionLayerSpace;
using Util = XMLUtility.Utilities;
using System.Threading;
using System.IO;
using System.Xml;
using Project4Starter;

namespace BasicRequirementTestSpace
{
    /// <summary>
    /// Static key generation
    /// </summary>
    static class KeyLs
    {
        static List<int> keyList; // Static List instance
        static KeyLs()
        {
            keyList = new List<int>();
        }
        public static void Insert(int value)
        {
            keyList.Add(value);
        }
        public static List<int> Get()
        {
            return keyList;
        }
        public static void Remove(int value)
        {
            if (keyList.Contains(value))
                keyList.Remove(value);
        }
    }

    /// <summary>
    /// BasicRequirementTest class test
    /// </summary>
    public class BasicRequirementTest
    {
        Random intRand = null;
        Random stringRand = null;
        Random msgId = null;

        public BasicRequirementTest()
        {
            intRand = new Random();
            stringRand = new Random();
            msgId = new Random();
        }
        //----< retrieve urls from the CommandLine if there are any >--------
        string localUrl { get; set; } = "http://localhost:8081/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";

        //Generate random string for perf test
        public string RandomStringGenerator(int length)
        {
            const string chars = "ABCDEFGHIJ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(y => y[stringRand.Next(length)]).ToArray());
        }
        //Generate random string for perf insert test
        public string InsertGenerator(XDocument doc, ref string msgid, ref string key, string op)
        {
            try
            {
                msgid = intRand.Next(1, 2000).ToString();
                XElement k = null;
                if (op == "StringDBInsert")
                {
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "StringDBInsert").Select(i => i).Single();
                    RemoveOtherWrites(ref doc, "StringDBInsert");
                    RemoveOtherReads(ref doc, "StringDBInsert");
                    key = "DBKey" + RandomStringGenerator(5);
                    foreach (var i in k.Elements("Value").Single().Elements("Payload").Elements("Data"))
                        i.Value = "Pay" + RandomStringGenerator(3);
                }
                else
                {
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Insert").Select(i => i).Single();
                    RemoveOtherWrites(ref doc, "Insert");
                    RemoveOtherReads(ref doc, "Insert");
                    key = intRand.Next(1, 100).ToString();
                    KeyLs.Insert(int.Parse(key));
                    k.Elements("Value").Single().Element("Payload").Value = "Pay" + RandomStringGenerator(10);
                }
                k.Elements("MessageID").Single().Value = msgid;
                k.Elements("Key").Single().Value = key;
                k.Elements("Value").Single().Element("Name").Value = "N" + RandomStringGenerator(5);
                k.Elements("Value").Single().Element("Description").Value = "D" + RandomStringGenerator(6);
                k.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                k.Elements("Value").Single().Element("TimeStamp").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (var i in k.Elements("Value").Single().Elements("Children"))
                    i.Element("Child").Value = intRand.Next(1, 100).ToString();
                return doc.ToString();
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error occured while modifying the item", ex);
            }
        }
        /// <summary>
        /// Generate random string for perf UpdateGenerator test
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="msgid"></param>
        /// <param name="key"></param>
        /// <param name="op"></param>
        /// <returns></returns>

        public string UpdateGenerator(XDocument doc, ref string msgid, ref string key, string op)
        {
            try
            {
                msgid = intRand.Next(1, 2000).ToString();
                XElement k = null;
                k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Update").Select(i => i).Single();
                RemoveOtherWrites(ref doc, "Update");
                RemoveOtherReads(ref doc, "Update");
                if (op == "newValue")
                {
                    k.Elements("Value").Single().Element("Payload").Value = "Pay" + RandomStringGenerator(10);
                    k.Elements("MessageID").Single().Value = msgid;
                    k.Elements("Key").Single().Value = key;
                    k.Elements("Value").Single().Element("Name").Value = "N" + RandomStringGenerator(5);
                    k.Elements("Value").Single().Element("Description").Value = "D" + RandomStringGenerator(6);
                    k.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                    k.Elements("Value").Single().Element("TimeStamp").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    k.Elements("Value").Single().Element("Payload").Value = "Updated " + RandomStringGenerator(10);
                    k.Elements("MessageID").Single().Value = msgid;
                    k.Elements("Key").Single().Value = key;
                    k.Elements("Value").Single().Element("Name").Value = "Updated " + RandomStringGenerator(5);
                    k.Elements("Value").Single().Element("Description").Value = "Updated " + RandomStringGenerator(6);
                    k.Elements("Value").Single().Element("TimeStamp").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                foreach (var i in k.Elements("Value").Single().Elements("Children"))
                    i.Element("Child").Value = intRand.Next(1, 200).ToString();
                return doc.ToString();
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error occured while modifying the item", ex);
            }
        }


        /// <summary>
        /// Generate random string for perf WriteDataForDelete test
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="msgid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string WriteDataForDelete(XDocument doc, ref string msgid, ref string key)
        {
            try
            {
                XElement k = null;
                msgid = intRand.Next(1, 2000).ToString();
                k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Delete").Select(i => i).Single();
                k.Elements("MessageID").Single().Value = msgid;
                k.Elements("Key").Single().Value = key;
                k.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                KeyLs.Remove(int.Parse(key));
                RemoveOtherWrites(ref doc, "Delete");
                RemoveOtherReads(ref doc, "Delete");
                return doc.ToString();
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error occured while deleting the item", ex);
            }
        }
        /// <summary>
        /// Process command line args
        /// </summary>
        /// <param name="args"></param>
        public void processCommandLine(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    return;
                bool logging = false;
                string wpfClientURL = "http://localhost:8089/CommService";
                localUrl = Util.processCommandLineForLocal(args, localUrl, ref logging, ref wpfClientURL);
                remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error in  processCommandLine", ex);
            }
        }
        /// <summary>
        /// Main program entry
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Write("\nStarting CommService write client");
            Console.Write("\n =============================\n");
            Console.Title = "Project 2 Requirements Testing";
            BasicRequirementTest clnt = new BasicRequirementTest();
            clnt.processCommandLine(args);
            string localPort = Util.urlPort(clnt.localUrl);
            string localAddr = Util.urlAddress(clnt.localUrl);
            Receiver rcvr = new Receiver(localPort, localAddr);
            if (rcvr.StartService())
                rcvr.doService(rcvr.defaultServiceAction());
            Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message
            Message msg = new Message();
            msg.fromUrl = clnt.localUrl;
            msg.toUrl = clnt.remoteUrl;
            Console.Write("\nSender's url is {0} \n", msg.fromUrl);
            Console.Write("\nAttempting to connect to {0}\n", msg.toUrl);
            if (!sndr.Connect(msg.toUrl))
            {
                Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
                sndr.shutdown();
                rcvr.shutDown();
                return;
            }
            string key = "";
            clnt.TestR2(ref msg, sndr, ref key);
            clnt.TestR3(ref msg, sndr);
            clnt.TestR4(ref msg, sndr, key);
            clnt.TestR5(ref msg, sndr);
            clnt.TestR6(ref msg, sndr);
            clnt.TestR7A(ref msg, sndr);
            clnt.TestR7B(ref msg, sndr);
            clnt.TestR7C(ref msg, sndr);
            clnt.TestR7D(ref msg, sndr);
            clnt.TestR7E(ref msg, sndr);
            clnt.TestR8(ref msg, sndr);
            clnt.TestR9(ref msg, sndr);
            Util.waitForUser();            // Wait for user to press a key to quit.
            rcvr.shutDown();               // shut down this client's Receiver and Sender by sending close messages
            sndr.shutdown();
            Console.Write("\n\n");
        }

        /// <summary>
        /// Requirement 2& 3 tested 
        /// </summary>
        void TestR2(ref Message msg, Sender sndr, ref string temp)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                "Demonstrating Requirement #2".title();
                Write("\nDBEngine works for two sets of Key/Value pair types");
                WriteLine();
                Write("\nShowing the <int,string> db insert");
                WriteLine();
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest wc = new BasicRequirementTest();
                string xmlStr = "";
                if (File.Exists("Input.xml"))
                {
                    XDocument doc;
                    string msgid, key;
                    TestR21(msg, sndr, out temp, wc, out xmlStr, out doc, out msgid, out key);
                    doc = XDocument.Load("Input.xml");
                    xmlStr = wc.InsertGenerator(doc, ref msgid, ref key, "StringDBInsert");
                    msg.content = xmlStr;
                    Console.Write("\nSending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nInserted in DB with Message ID {0} successfully \n\n", msgid);
                        Thread.Sleep(50);
                    }
                    msg = new Message();
                    msg.fromUrl = localUrl;
                    msg.toUrl = remoteUrl;
                    msg.content = "done";
                    sndr.sendMessage(msg);
                }
                Thread.Sleep(3000);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error occured while modifying the item", ex);
            }
        }

        /// <summary>
        /// TestR2 sub method created
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        /// <param name="temp"></param>
        /// <param name="wc"></param>
        /// <param name="xmlStr"></param>
        /// <param name="doc"></param>
        /// <param name="msgid"></param>
        /// <param name="key"></param>
        private static void TestR21(Message msg, Sender sndr, out string temp, BasicRequirementTest wc, out string xmlStr, out XDocument doc, out string msgid, out string key)
        {
            doc = XDocument.Load("Input.xml");
            msgid = "";
            key = "";
            xmlStr = wc.InsertGenerator(doc, ref msgid, ref key, "int");    //Generate data using the xml structure
            temp = key;
            msg.content = xmlStr;
            Console.Write("\nSending Message..Message is \n {0} \n", msg.content);
            if (sndr.sendMessage(msg))
            {
                Console.Write("\nInserted in DB with Message ID {0} successfully \n\n", msgid);
                Thread.Sleep(50);

            }
            WriteLine();
            Write("\nShowing the <int,string> db insert");
            WriteLine();
            doc = XDocument.Load("Input.xml");
            xmlStr = wc.InsertGenerator(doc, ref msgid, ref key, "Insert");
            msg.content = xmlStr;
            Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
            if (sndr.sendMessage(msg))
            {
                Console.Write("\nInserted in DB with Message ID {0} successfully \n\n", msgid);
                Thread.Sleep(50);
            }
            WriteLine();
            Write("\nShowing the <string,List<string>> db insert");
            WriteLine();
            doc = XDocument.Load("Input.xml");
            xmlStr = wc.InsertGenerator(doc, ref msgid, ref key, "StringDBInsert");
            msg.content = xmlStr;
            Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
            if (sndr.sendMessage(msg))
            {
                Console.Write("\nInserted in DB with Message ID {0} successfully \n\n", msgid);
                Thread.Sleep(50);
            }
            WriteLine();
            Write("\nShowing the <string,List<string>> db insert");
            WriteLine();
        }

        /// <summary>
        /// Requirement 3 tested for Deletion of Key/Value pairs
        /// </summary>
        void TestR3(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                string xmlStr = "";
                Console.ForegroundColor = ConsoleColor.Green;
                "Demonstrating Requirement #3".title('=');
                Write("\nAdding and Deleting key/value pairs\n");
                WriteLine();
                "Adding new element to DBEngine<int, string>".title();
                if (File.Exists("Input.xml"))
                {
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "", key = "";
                    xmlStr = te.InsertGenerator(doc, ref msgid, ref key, "int");    //Generate data using the xml structure
                    msg.content = xmlStr;
                    Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nInserted in DB with Message ID {0} successfully \n\n", msgid);
                        Thread.Sleep(50);
                    }
                    "\nRemoving the new element".title();
                    doc = XDocument.Load("Input.xml");
                    //Construct DB Delete
                    xmlStr = te.WriteDataForDelete(doc, ref msgid, ref key);    //Generate data using the xml structure

                    msg.content = xmlStr;
                    Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage sent for deletion : Key {0}, Message ID {0} successfully \n\n", key, msgid);
                        Thread.Sleep(50);
                    }
                }
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured in TestR4", ex);
            }
        }
        /// <summary>
        /// Requirement 4 for Editing Key/Value pairs tested
        /// </summary>
        void TestR4(ref Message msg, Sender sndr, string key)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                string xmlStr = "";
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #4".title('=');
                    Write("\nEditing of MetaData and Payload");
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "";
                    xmlStr = te.UpdateGenerator(doc, ref msgid, ref key, "");    //Generate data using the xml structure
                    msg.content = xmlStr;
                    Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\n Updated entries in DB - Key:{0} ,MessageID: {1} successfully \n\n", key, msgid);
                        Thread.Sleep(50);
                    }
                    "\nDemonstrating an insert of element which is replaced by a new value instance.. \n".title();
                    doc = XDocument.Load("Input.xml");
                    xmlStr = te.UpdateGenerator(doc, ref msgid, ref key, "newValue");    //Generate data using the xml structure
                    msg.content = xmlStr;
                    Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage sent for element to be replaced by a new value instance");
                        Console.Write("\n Key: {0} ,MessageID: {1} \n", key, msgid);
                        Thread.Sleep(50);
                    }
                }
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while modifying the item", ex);
            }
        }
        /// <summary>
        /// Requirement 5 for Persisting,Restoring,Augmentation of DB
        /// </summary>
        void TestR5(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {
                    "Demonstrating Requirement #5".title('='); Write("Persisting of XML file\n");
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "", status = "No";
                    msgid = intRand.Next(1, 2000).ToString(); XElement k = null;
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "ManualPersist").Select(i => i).Single();
                    RemoveOtherWrites(ref doc, "ManualPersist"); RemoveOtherReads(ref doc, "ManualPersist");
                    k.Elements("MessageID").Single().Value = msgid; status = k.Elements("Status").Single().Value;
                    if (status == "Yes")
                    {
                        msg.content = doc.ToString(); Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                        if (sndr.sendMessage(msg))
                        {
                            Console.Write("\nMessage(MessageID:{0}) to Persist DB successfully sent to Database\n", msgid);
                            Thread.Sleep(50);
                        }
                    }
                    Write("\nRestoring & Augmenting of DB\n");
                    doc = XDocument.Load("Input.xml"); msgid = intRand.Next(1, 2000).ToString();
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "RestoreDB").Select(i => i).Single();
                    RemoveOtherWrites(ref doc, "RestoreDB");
                    RemoveOtherReads(ref doc, "RestoreDB");
                    k.Elements("MessageID").Single().Value = msgid;
                    status = k.Elements("Status").Single().Value;
                    if (status == "Yes")
                    {
                        msg.content = doc.ToString(); Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                        if (sndr.sendMessage(msg))
                        {
                            Console.Write("\nMessage(MessageID:{0}) sent to Restore the DB successfully..\n", msgid);
                            Thread.Sleep(50);
                        }
                    }
                }
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while modifying the item", ex);
            }
        }

        /// <summary>
        /// Requirement 6 for scheduled save process based on time interval provided by the user
        /// </summary>
        void TestR6(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #6".title('=');
                    Write("Scheduling of XML file\n");
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "", status = "No";
                    msgid = intRand.Next(1, 2000).ToString();
                    XElement k = null;
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "ScheduledPersist").Select(i => i).Single();
                    RemoveOtherWrites(ref doc, "ScheduledPersist");
                    RemoveOtherReads(ref doc, "ScheduledPersist");
                    k.Elements("MessageID").Single().Value = msgid;
                    status = k.Elements("Status").Single().Value;
                    if (status == "Yes")
                    {
                        msg.content = doc.ToString();
                        Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                        if (sndr.sendMessage(msg))
                        {
                            Console.Write("\nMessage(MessageID:{0}) Schedule Save Process after every {1} microsecs executed twice\n", msgid, k.Elements("TimeInterval").Single().Value);
                            Thread.Sleep(50);
                        }
                    }
                }
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while modifying the item", ex);
            }
        }

        /// <summary>
        /// Requirement 7 for showing how the query read operation for getting Value for Key
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        void TestR7A(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #7 - Querying details based on Key".title('=');
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "";
                    msgid = intRand.Next(1, 2000).ToString();
                    XElement k = null;
                    RemoveOtherReads(ref doc, "GetByKey");
                    RemoveOtherWrites(ref doc, "GetByKey");
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByKey").Select(i => i).Single();
                    k.Elements("MessageID").Single().Value = msgid;
                    k.Elements("Key").Single().Value = KeyLs.Get()[0].ToString();
                    msg.content = doc.ToString();
                    Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage(MessageID:{0}) to retrieve values for key sent successfully \n", msgid);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while modifying the item", ex);
            }
        }

        /// <summary>
        /// Requirement 7 for showing how the query read operation for getting children for key
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        void TestR7B(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #7 - Querying Children details based on Key".title('=');
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "";
                    msgid = intRand.Next(1, 2000).ToString();
                    XElement k = null;
                    RemoveOtherReads(ref doc, "GetChildren");
                    RemoveOtherWrites(ref doc, "GetChildren");
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetChildren").Select(i => i).Single();
                    k.Elements("MessageID").Single().Value = msgid;
                    k.Elements("Key").Single().Value = KeyLs.Get()[0].ToString();
                    msg.content = doc.ToString();
                    Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage(MessageID:{0}) to retrieve values for children sent successfully \n", msgid);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }

        /// <summary>
        /// Requirement 7 for showing how the query read operation for getting keys matching a specific pattern
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        void TestR7C(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #7 - Querying for getting keys matching a specific pattern".title('=');
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "";
                    msgid = intRand.Next(1, 2000).ToString();
                    XElement k = null;
                    RemoveOtherReads(ref doc, "GetKeyCriteria");
                    RemoveOtherWrites(ref doc, "GetKeyCriteria");
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetKeyCriteria").Select(i => i).Single();
                    k.Elements("MessageID").Single().Value = msgid;
                    msg.content = doc.ToString();
                    Console.Write("\nSending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage(MessageID:{0}) to retrieve values for children sent successfully \n", msgid);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }

        /// <summary>
        /// Requirement 7 for showing how the query read operation for getting elements having specific pattern in metadata
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        void TestR7D(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #7 - Querying for getting elements having specific pattern in metadata".title('=');
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "";
                    msgid = intRand.Next(1, 2000).ToString();
                    XElement k = null;
                    RemoveOtherReads(ref doc, "GetByMetadataCriteria");
                    RemoveOtherWrites(ref doc, "GetByMetadataCriteria");
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByMetadataCriteria").Select(i => i).Single();
                    k.Elements("MessageID").Single().Value = msgid;
                    msg.content = doc.ToString();
                    Console.Write("\nSending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage(MessageID:{0}) to retrieve db elements with metatadata matching pattern sent successfully \n", msgid);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }

        /// <summary>
        /// Requirement 7 for showing how the query read operation for getting elements created within specified time interval
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        void TestR7E(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #7 - Querying for  getting elements created within specified time interval".title('=');
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "";
                    msgid = intRand.Next(1, 2000).ToString();
                    XElement k = null;
                    RemoveOtherReads(ref doc, "GetByTimeStampCriteria");
                    RemoveOtherWrites(ref doc, "GetByTimeStampCriteria");
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByTimeStampCriteria").Select(i => i).Single();
                    k.Elements("MessageID").Single().Value = msgid;
                    msg.content = doc.ToString();
                    Console.Write("\nSending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage(MessageID:{0}) to retrieve db elements created within specified time interval sent successfully \n", msgid);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }

        /// <summary>
        ///  Requirement 8 for showing results have been stored in a Immutable DB
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        void TestR8(ref Message msg, Sender sndr)
        {
            try
            {
                XDocument doc = XDocument.Load("Input.xml");
                string msgid = "";
                XElement k = null;
                Console.ForegroundColor = ConsoleColor.Green;
                "Demonstrating Requirement #8 - Immutable DB".title('=');
                WriteLine("The results have been stored in a Immutable DB can be verified in Line 404 of RequestHandler");
                WriteLine("The DB cannot be edited or will not change over time");
                WriteLine("DB is immutable since there are no insert or edit methods");
                WriteLine("The results are now being retreived from Immutable DB");
                k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "ImmutableDB").Select(i => i).Single();
                RemoveOtherReads(ref doc, "ImmutableDB");
                RemoveOtherWrites(ref doc, "ImmutableDB");
                k.Elements("MessageID").Single().Value = msgid;
                k.Elements("Key").Single().Value = KeyLs.Get()[0].ToString();
                msg.content = doc.ToString();
                Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                if (sndr.sendMessage(msg))
                {
                    Console.Write("\nMessage sent successfully, Message ID: {0} \n\n", msgid);
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }
        /// <summary>
        /// Requirement 9 for loading package structure into Db
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        void TestR9(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #9 - Loading Package structure".title('=');
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "";
                    msgid = intRand.Next(1, 2000).ToString();
                    XElement k = null;
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "LoadPackageStructure").Select(i => i).Single();
                    RemoveOtherReads(ref doc, "LoadPackageStructure");
                    RemoveOtherWrites(ref doc, "LoadPackageStructure");
                    k.Elements("MessageID").Single().Value = msgid;
                    msg.content = doc.ToString();
                    Console.Write("\nSending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage(MessageID:{0}) to retrieve db elements created within specified time interval sent successfully \n", msgid);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }

        /// <summary>
        /// Requirement 7 for showing how the query read operation for getting packages for Repository
        /// <param name="msg"></param>
        /// <param name="sndr"></param>
        void TestR10(ref Message msg, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                BasicRequirementTest te = new BasicRequirementTest();
                if (File.Exists("Input.xml"))
                {

                    "Demonstrating Requirement #7 - Querying Children package details based on Repository Key".title('=');
                    WriteLine();
                    XDocument doc = XDocument.Load("Input.xml");
                    string msgid = "";
                    msgid = intRand.Next(1, 2000).ToString();
                    XElement k = null;
                    RemoveOtherReads(ref doc, "GetChildren");
                    RemoveOtherWrites(ref doc, "GetChildren");
                    k = doc.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetChildren").Select(i => i).Single();
                    k.Elements("MessageID").Single().Value = msgid;
                    k.Elements("Key").Single().Value = KeyLs.Get()[0].ToString();
                    msg.content = doc.ToString();
                    Console.Write("\n Sending Message..Message is \n {0} \n", msg.content);
                    if (sndr.sendMessage(msg))
                    {
                        Console.Write("\nMessage(MessageID:{0}) to retrieve values for children sent successfully \n", msgid);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }

        void RemoveOtherReads(ref XDocument doc, string valid)
        {
            try
            {
                List<string> coll = new List<string>() { "GetByKey", "GetChildren", "GetKeyCriteria", "GetByMetadataCriteria", "GetByTimeStampCriteria" };
                coll.RemoveAll(i => i == valid);
                foreach (var i in coll)
                {
                    doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Read" && d.Element("QueryType").Value == i).Remove();
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }
        void RemoveOtherWrites(ref XDocument doc, string valid)
        {
            try
            {
                List<string> coll = new List<string>() { "Insert", "StringDBInsert", "Update", "Delete", "ManualPersist", "RestoreDB", "ScheduledPersist" };
                coll.RemoveAll(i => i == valid);
                foreach (var i in coll)
                    doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == i).Remove();
                if (valid != "LoadPackageStructure")
                    doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "LoadPackageStructure").Remove();
                if (valid != "ImmutableDB")
                    doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "ImmutableDB").Remove();
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occured while getting key details", ex);
            }
        }
    }
}
