/////////////////////////////////////////////////////////////////////////
// WriteClient.cs - Write Client Package to perform performance of queries//                                    
// ver 1.1                                                             //
//Darshan Masti Prakash                                                //              
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   remotePort, remoteAddress, localPort, localAddress
 *
 *   Public Interface
 *   ----------------
 *   ReadClient clnt = new ReadClient();
 *   clnt.processCommandLine(args);
 */
/*
 * Maintenance History:
 * --------------------
 * ver 1.1 :21 Nov 2015 -Modified the code to handle queries performance testing
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Project4Starter
{
    using ExceptionLayerSpace;
    using System.Collections;
    using System.IO;
    using System.Timers;
    using System.Xml;
    using System.Xml.Linq;
    using XMLUtility;
    using Util = XMLUtility.Utilities;

    ///////////////////////////////////////////////////////////////////////
    // Client class sends and receives messages in this version
    // - commandline format: /L http://localhost:8085/CommService 
    //                       /R http://localhost:8080/CommService
    //   Either one or both may be ommitted
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
        public static int LastElement()
        {
            if (keyList.Count == 0)
                return 2;
            return keyList[keyList.Count - 1];
        }
    }
    public class WriterClient
    {
        Hashtable table = null;
        Hashtable execTime = null;
        Random stringRand = null;
        public int numMsgs { get; set; }
        public int intRand { get; set; } = 1;
        public int childRand { get; set; } = 0;
        public int msgRand { get; set; } = 1;
        public int total { get; set; }
        public ulong WriteClientTime { get; set; }
        public WriterClient()
        {
            stringRand = new Random();
            table = new Hashtable();
            execTime = new Hashtable();
        }
        /// <summary>
        /// Randomstring generator for the project
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RandomStringGenerator(int length)
        {
            const string chars = "ABCDEFGHIJ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(y => y[stringRand.Next(length)]).ToArray());
        }
        /// <summary>
        /// UpdateGenerator generator for the project
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
                msgid = msgRand++.ToString();
                XElement k = null;
                k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Update").Select(i => i).Single();
                RemoveOtherWrites(ref doc, "Update");
                RemoveOtherReads(ref doc, "Update");
                k.Elements("Value").Single().Element("Payload").Value = "Updated " + RandomStringGenerator(10);
                k.Elements("MessageID").Single().Value = msgid;
                k.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                k.Elements("Key").Single().Value = key;
                k.Elements("Value").Single().Element("Name").Value = "Updated " + RandomStringGenerator(5);
                k.Elements("Value").Single().Element("Description").Value = "Updated " + RandomStringGenerator(6);
                k.Elements("Value").Single().Element("TimeStamp").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (var i in k.Elements("Value").Single().Elements("Children"))
                    i.Element("Child").Value = childRand++.ToString();
                return doc.ToString();
            }
            catch (Exception ex)
            {
                throw new CustomException("Error in Update generator ", ex);
            }

        }
        /// <summary>
        /// WriteDataForDelete generator for the project
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
                msgid = msgRand++.ToString();
                k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Delete").Select(i => i).Single();
                k.Elements("MessageID").Single().Value = msgid;
                k.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                k.Elements("Key").Single().Value = key;
                KeyLs.Remove(int.Parse(key));
                RemoveOtherWrites(ref doc, "Delete");
                RemoveOtherReads(ref doc, "Delete");
                return doc.ToString();
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error in WriteDataForDelete generator ", ex);
            }
        }

        /// <summary>
        /// RemoveOtherReads generator for the project
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="valid"></param>
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
            catch (CustomException ex)
            {
                throw new CustomException("Error in RemoveOtherReads generator ", ex);
            }
        }

        /// <summary>
        /// RemoveOtherWrites generator for the project
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="valid"></param>
        void RemoveOtherWrites(ref XDocument doc, string valid)
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

        /// <summary>
        /// InsertGenerator for generating items
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="msgid"></param>
        /// <param name="key"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public string InsertGenerator(XDocument doc, ref string msgid, ref string key, string op)
        {
            msgid = msgRand++.ToString();
            XElement k = null;
            k = doc.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Insert").Select(i => i).Single();
            RemoveOtherWrites(ref doc, "Insert");
            RemoveOtherReads(ref doc, "Insert");
            key = intRand++.ToString();
            KeyLs.Insert(int.Parse(key));
            k.Elements("Value").Single().Element("Payload").Value = "Pay" + RandomStringGenerator(10);
            k.Elements("MessageID").Single().Value = msgid;
            k.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
            k.Elements("Key").Single().Value = key;
            k.Elements("Value").Single().Element("Name").Value = "N" + RandomStringGenerator(5);
            k.Elements("Value").Single().Element("Description").Value = "D" + RandomStringGenerator(6);
            k.Elements("Value").Single().Element("TimeStamp").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            foreach (var i in k.Elements("Value").Single().Elements("Children"))
                i.Element("Child").Value = childRand++.ToString();
            return doc.ToString();
        }
        string localUrl { get; set; } = "http://localhost:8081/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";
        bool loggingCheck { get; set; } = true;
        string wpfClientURL = "http://localhost:8089/CommService";

        /// <summary>
        /// processCommandLine arguments for the writer client
        /// </summary>
        /// <param name="args"></param>
        public void processCommandLine(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    return;
                bool logging = true;
                localUrl = Util.processCommandLineForLocal(args, localUrl, ref logging, ref wpfClientURL);
                loggingCheck = logging;
                remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error in processCommandLine", ex);
            }
        }
        static void Main(string[] args)
        {
            try
            {
                HiResTimer hres = new HiResTimer(); //High Resolution Timer
                Console.Write("\nStarting CommService write client");
                Console.Write("\n =============================\n");
                Console.Title = "Write Client";
                Message msg = new Message();
                WriterClient clnt = new WriterClient();
                clnt.processCommandLine(args);
                string localPort = Util.urlPort(clnt.localUrl);
                string localAddr = Util.urlAddress(clnt.localUrl);
                Receiver rcvr = new Receiver(localPort, localAddr);
                Action serviceAction = DefineServiceAction(hres, clnt, rcvr);
                if (rcvr.StartService())
                    rcvr.doService(serviceAction);
                Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message         
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                Console.Write("Sender's url is {0}", msg.fromUrl);
                Console.Write("Attempting to connect to {0}\n", msg.toUrl);
                if (!sndr.Connect(msg.toUrl))
                {
                    Console.Write("Could not connect in {0} attempts\n", sndr.MaxConnectAttempts);
                    sndr.shutdown();
                    rcvr.shutDown();
                    return;
                }
                PerformanceTesting(hres, msg, clnt, rcvr, sndr);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error in main of writer client", ex);
            }

        }
        /// <summary>
        ///  Performance testing by inserting items
        /// </summary>
        /// <param name="hres"></param>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="rcvr"></param>
        /// <param name="sndr"></param>
        private static void PerformanceTesting(HiResTimer hres, Message msg, WriterClient clnt, Receiver rcvr, Sender sndr)
        {
            try
            {
                XmlTextReader textReader = new XmlTextReader("Input.xml");
                string xmlStr = "";
                HiResTimer t = new HiResTimer();
                t.Start();
                if (File.Exists("Input.xml"))
                {
                    XDocument docTemp;
                    string key, msgid;
                    xmlStr = InsertPerformance(hres, msg, clnt, sndr, xmlStr, out docTemp, out key, out msgid);
                    string keyUp;
                    XElement l;
                    UpdatePerformance(msg, clnt, sndr, ref xmlStr, out docTemp, key, ref msgid, out keyUp, out l);
                    //Delete entries
                    docTemp = DeletePerformance(msg, clnt, sndr, ref xmlStr, key, ref msgid, ref keyUp, l);
                }
                t.Stop();
                clnt.WriteClientTime = t.ElapsedMicroseconds;
                msg = new Message();
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                msg.content = "done";
                sndr.sendMessage(msg);
                // Wait for user to press a key to quit.
                Util.waitForUser();
                // shut down this client's Receiver and Sender by sending close messages
                rcvr.shutDown();
                sndr.shutdown();
                Console.Write("\n\n");
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error in main of peformance testing", ex);
            }
        }

        /// <summary>
        /// Insert performance results writer client
        /// </summary>
        /// <param name="hres"></param>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <param name="xmlStr"></param>
        /// <param name="docTemp"></param>
        /// <param name="key"></param>
        /// <param name="msgid"></param>
        /// <returns></returns>
        private static string InsertPerformance(HiResTimer hres, Message msg, WriterClient clnt, Sender sndr, string xmlStr, out XDocument docTemp, out string key, out string msgid)
        {
            docTemp = XDocument.Load("Input.xml");
            XElement k = docTemp.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Insert").Select(i => i).Single();
            if (k.Elements("Count").Single().Value != "")
                clnt.numMsgs = int.Parse(k.Elements("Count").Single().Value);
            clnt.total += clnt.numMsgs;
            hres.Start();     //High resolution timer started     
            key = "";
            msgid = "";
            for (int i = 0; i < clnt.numMsgs; i++)
            {
                docTemp = XDocument.Load("Input.xml");
                xmlStr = clnt.InsertGenerator(docTemp, ref msgid, ref key, "int");    //Generate data using the xml structure
                msg.content = xmlStr;
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                if (sndr.sendMessage(msg))
                {
                    Console.Write("Sent message(Message ID {0}) to insert into DB successfully\n", msgid);
                    Thread.Sleep(50);
                }
            }

            return xmlStr;
        }

        /// <summary>
        /// DeletePerformance for the client
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <param name="xmlStr"></param>
        /// <param name="key"></param>
        /// <param name="msgid"></param>
        /// <param name="keyUp"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        private static XDocument DeletePerformance(Message msg, WriterClient clnt, Sender sndr, ref string xmlStr, string key, ref string msgid, ref string keyUp, XElement l)
        {
            XDocument docTemp = XDocument.Load("Input.xml");
            XElement m = docTemp.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Delete").Select(i => i).Single();
            clnt.numMsgs = int.Parse(l.Elements("Count").Single().Value);
            clnt.total += clnt.numMsgs;
            for (int i = 0; i < clnt.numMsgs; i++)
            {
                if (KeyLs.Get() != null)
                {
                    keyUp = KeyLs.LastElement().ToString();
                    //keyUp = KeyLs.Get()[0].ToString();
                    KeyLs.Remove(int.Parse(keyUp));
                }
                else
                    keyUp = 2.ToString();
                //Construct DB Delete
                xmlStr = clnt.WriteDataForDelete(docTemp, ref msgid, ref keyUp);    //Generate data using the xml structure
                msg.content = xmlStr;
                if (clnt.loggingCheck == true)
                    Console.Write("Sending Message..Message is \n {0} \n", msg.content);
                else
                    Console.Write("Sending Message..Message(Message ID:{0})\n", msgid);
                if (sndr.sendMessage(msg))
                {
                    Console.Write("Message(MessageID {1},Key {0}) sent for deletion successfully\n", key, msgid);
                    Thread.Sleep(50);
                }
            }

            return docTemp;
        }

        /// <summary>
        /// UpdatePerformance for the client
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <param name="xmlStr"></param>
        /// <param name="docTemp"></param>
        /// <param name="key"></param>
        /// <param name="msgid"></param>
        /// <param name="keyUp"></param>
        /// <param name="l"></param>
        private static void UpdatePerformance(Message msg, WriterClient clnt, Sender sndr, ref string xmlStr, out XDocument docTemp, string key, ref string msgid, out string keyUp, out XElement l)
        {
            //Update performance testing
            docTemp = XDocument.Load("Input.xml");
            keyUp = "";
            l = docTemp.Descendants("OperationMessage").Where(d => d.Element("Operation").Value == "Update").Select(i => i).Single();
            clnt.numMsgs = int.Parse(l.Elements("Count").Single().Value);
            clnt.total += clnt.numMsgs;
            for (int i = 0; i < clnt.numMsgs; i++)
            {
                if (key == "")
                    keyUp = "2";
                else
                    keyUp = key;
                xmlStr = clnt.UpdateGenerator(docTemp, ref msgid, ref keyUp, "");    //Generate data using the xml structure
                msg.content = xmlStr;
                if (clnt.loggingCheck == true)
                    Console.Write("Sending Message..Message is \n {0} \n", msg.content);
                else
                    Console.Write("Sending Message..Message(Message ID:{0})\n", msgid);
                if (sndr.sendMessage(msg))
                {
                    Console.Write("\nUpdated entries in DB - Key:{0} ,MessageID: {1} successfully\n", key, msgid);
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        ///  Define service action that needs to be handled for receiver requests at the client
        /// </summary>
        /// <param name="hres"></param>
        /// <param name="clnt"></param>
        /// <param name="rcvr"></param>
        /// <returns></returns>
        private static Action DefineServiceAction(HiResTimer hres, WriterClient clnt, Receiver rcvr)
        {
            try
            {
                Action serviceAction = ServiceActionMake(hres, clnt, rcvr);
                return serviceAction;
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error in define service action of writer client", ex);
            }
        }

        private static Action ServiceActionMake(HiResTimer hres, WriterClient clnt, Receiver rcvr)
        {
            Action serviceAction = () =>
            {
                Message msg1 = null;
                while (true)
                {
                    msg1 = rcvr.getMessage();   // note use of non-service method to deQ messages                           
                    if (msg1.content == "done")
                    {
                        hres.Stop(); ulong lookUpTime = hres.ElapsedMicroseconds;
                        Message msg2 = new Message();
                        msg2.fromUrl = clnt.localUrl;
                        msg2.toUrl = clnt.wpfClientURL;
                        msg2.content = "LookupTime;" + lookUpTime + ";" + "CntMsg;" + (ulong)clnt.total + ";";
                        Sender wpfSender = new Sender(clnt.localUrl);
                        wpfSender.sendMessage(msg2);
                        Console.WriteLine("\n----------------------Overall Performance Statistics for Write Client-----------------------------\n");
                        Console.WriteLine("Number of messages processed is {0}", clnt.total);
                        Console.WriteLine("\nTotal Execution time for the messages to be processed at Client Side is {0} microsecs", clnt.WriteClientTime);
                        Console.WriteLine("\nAverage Execution time for the messages to be processed at Client Side is {0} microsecs", clnt.WriteClientTime / (ulong)clnt.total);
                        Console.WriteLine("\nTotal Execution time for the messages from Client-Server-Client {0} microsecs", lookUpTime);
                        Console.WriteLine("\nAverage Execution time for the messages from Client-Server-Client is {0} microsecs", lookUpTime / (ulong)clnt.total);
                        Console.WriteLine("\n----------------------Overall Performance Statistics for Write Client-----------------------------\n");
                        break;
                    }
                    else if (msg1.content == "connection start message")
                        Console.WriteLine("Connection start message receieved at client side");
                    else
                    {
                        XDocument docTemp = XDocument.Load(new StringReader(msg1.content));
                        string mid = docTemp.Descendants("OperationMessage").Elements("MessageID").Single().Value;
                        string op = docTemp.Descendants("OperationMessage").Elements("Operation").Single().Value;
                        string resp = docTemp.Descendants("OperationMessage").Elements("Response").Single().Value;
                        Console.Write("\nMessage - MessageID:{0} received at the client", mid);
                        Console.Write("\nSender:{0}", msg1.fromUrl); Console.Write("\nOperation :{0}", op);
                        Console.Write("\nResponse :{0}", resp);
                        string oldDt = docTemp.Descendants("InsertTime").Select(i => i).Single().Value.ToString();
                        long microseconds = (DateTime.Now.Ticks - long.Parse(oldDt)) / 10;
                        Console.Write("\nExecution time for message(MessageID:{1}) is {0} microseconds\n", microseconds, mid);
                    }
                }
                Util.waitForUser();
            };
            return serviceAction;
        }
    }

#if (TEST_WriterClient)

    /// <summary>
    /// This is to test the DBElement functionality.But here only the main functionality 
    /// is written as the testing code is moved to TestDBElementTest layer
    /// </summary>
    class TestDBElement
    {
        static void Main(string[] args)
        {
    try
            {
                HiResTimer hres = new HiResTimer(); //High Resolution Timer
                Console.Write("\nStarting CommService write client");
                Console.Write("\n =============================\n");
                Console.Title = "Write Client";
                Message msg = new Message();
                WriterClient clnt = new WriterClient();
                clnt.processCommandLine(args);
                string localPort = Util.urlPort(clnt.localUrl);
                string localAddr = Util.urlAddress(clnt.localUrl);
                Receiver rcvr = new Receiver(localPort, localAddr);
                Action serviceAction = DefineServiceAction(hres, clnt, rcvr);
                if (rcvr.StartService())
                    rcvr.doService(serviceAction);
                Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message         
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                Console.Write("Sender's url is {0}", msg.fromUrl);
                Console.Write("Attempting to connect to {0}\n", msg.toUrl);
                if (!sndr.Connect(msg.toUrl))
                {
                    Console.Write("Could not connect in {0} attempts\n", sndr.MaxConnectAttempts);
                    sndr.shutdown();
                    rcvr.shutDown();
                    return;
                }
                PerformanceTesting(hres, msg, clnt, rcvr, sndr);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error in main of writer client", ex);
            }
        }
    }
#endif
}
