/////////////////////////////////////////////////////////////////////////
// ReadClient.cs - Read Client Package to perform performance of queries//                                    
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
    using System.Xml;
    using System.Xml.Linq;
    using XMLUtility;
    using Util = XMLUtility.Utilities;

    ///////////////////////////////////////////////////////////////////////
    // Client class sends and receives messages in this version
    // - commandline format: /L http://localhost:8085/CommService 
    //                       /R http://localhost:8080/CommService
    //   Either one or both may be ommitted

   public class ReadClient
    {
        Hashtable table = null;
        Hashtable execTime = null;
        Random stringRand = null;
        public int numMsgs { get; set; }
        public int intRand { get; set; } = 1;
        public int childRand { get; set; } = 0;
        public int msgRand { get; set; } = 1;
        public int total { get; set; }
        public ulong ReadClientTime { get; set; }
        public ReadClient()
        {
            stringRand = new Random();
            table = new Hashtable();
            execTime = new Hashtable();
        }
        /// <summary>
        /// RemoveOtherReads method to remove other entries from input file xml
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
            catch (Exception ex)
            {
                throw new CustomException("Error in Remove other reads", ex);
            }
        }

        /// <summary>
        /// RemoveOtherWrites method to remove other entries from input file xml
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
        string localUrl { get; set; } = "http://localhost:8082/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";
        bool loggingCheck { get; set; } = true;
        string wpfClientURL = "http://localhost:8089/CommService";

        /// <summary>
        /// processCommandLine from batch file
        /// </summary>
        /// <param name="args"></param>
        public void processCommandLine(string[] args)
        {
            if (args.Length == 0)
                return;
            bool logging = true;
            localUrl = Util.processCommandLineForLocal(args, localUrl,ref logging,ref wpfClientURL);
            loggingCheck = logging;
            remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
        }
        static void Main(string[] args)
        {
            HiResTimer hres = new HiResTimer(); //High Resolution Timer
            Console.Write("\nStarting CommService Read client");
            Console.Write("\n =============================\n");
            Console.Title = "Read Client";
            Message msg = new Message();
            ReadClient clnt = new ReadClient();
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
            msg = PerformanceTest(hres, msg, clnt, sndr);
            // Wait for user to press a key to quit.
            Util.waitForUser();
            // shut down this client's Receiver and Sender by sending close messages
            rcvr.shutDown();
            sndr.shutdown();
            Console.Write("\n\n");
        }

        /// <summary>
        /// Performance testing by inserting queries
        /// </summary>
        /// <param name="hres"></param>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <returns></returns>
        private static Message PerformanceTest(HiResTimer hres, Message msg, ReadClient clnt, Sender sndr)
        {
            XmlTextReader textReader = new XmlTextReader("Input.xml");
            HiResTimer timerH = new HiResTimer();
            if (File.Exists("Input.xml"))
            {
                XDocument docTemp;
                string msgid;
                //GetKeyCriteria
                GetByKeyCriteria(hres, msg, clnt, sndr, timerH, out docTemp, out msgid);
                //GetByMetadataCriteria
                docTemp = GetByMetadataCriteria(msg, clnt, sndr, ref msgid);
                //GetByTimeStampCriteria
                docTemp = GetByTimeStampCriteria(msg, clnt, sndr, ref msgid);
                docTemp = GetByKey(msg, clnt, sndr, ref msgid);                //GetByKey
                docTemp = GetChildren(msg, clnt, sndr, ref msgid);                //GetChildren
            }
            timerH.Stop();
            clnt.ReadClientTime = timerH.ElapsedMicroseconds;
            msg = new Message();
            msg.fromUrl = clnt.localUrl;
            msg.toUrl = clnt.remoteUrl;
            msg.content = "done";
            sndr.sendMessage(msg);
            return msg;
        }

        /// <summary>
        /// Get children from specific message
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <param name="msgid"></param>
        /// <returns></returns>
        private static XDocument GetChildren(Message msg, ReadClient clnt, Sender sndr, ref string msgid)
        {
            XDocument docTemp = XDocument.Load("Input.xml");
            clnt.RemoveOtherReads(ref docTemp, "GetChildren");
            clnt.RemoveOtherWrites(ref docTemp, "GetChildren");
            XElement w = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetChildren").Select(j => j).Single();
            clnt.numMsgs = int.Parse(w.Elements("Count").Single().Value);
            clnt.total += clnt.numMsgs;
            for (int i = 0; i < clnt.numMsgs; i++)
            {
                docTemp = XDocument.Load("Input.xml");
                clnt.RemoveOtherReads(ref docTemp, "GetChildren");
                clnt.RemoveOtherWrites(ref docTemp, "GetChildren");
                msgid = clnt.msgRand++.ToString();
                XElement y = null;
                y = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetChildren").Select(j => j).Single();
                y.Elements("MessageID").Single().Value = msgid;
                y.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                msg.content = docTemp.ToString();
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                if (sndr.sendMessage(msg))
                {
                    Console.Write("Sent message(Message ID {0}) to GetChildren from key\n", msgid);
                    Thread.Sleep(50);
                }
            }
            return docTemp;
        }

        /// <summary>
        /// GetByKey method to get the Keys from the Messaage
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <param name="msgid"></param>
        /// <returns></returns>
        private static XDocument GetByKey(Message msg, ReadClient clnt, Sender sndr, ref string msgid)
        {
            XDocument docTemp = XDocument.Load("Input.xml");
            clnt.RemoveOtherReads(ref docTemp, "GetByKey");
            clnt.RemoveOtherWrites(ref docTemp, "GetByKey");
            XElement q = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByKey").Select(j => j).Single();
            clnt.numMsgs = int.Parse(q.Elements("Count").Single().Value);
            clnt.total += clnt.numMsgs;
            for (int i = 0; i < clnt.numMsgs; i++)
            {
                docTemp = XDocument.Load("Input.xml");
                clnt.RemoveOtherReads(ref docTemp, "GetByKey");
                clnt.RemoveOtherWrites(ref docTemp, "GetByKey");
                msgid = clnt.msgRand++.ToString();
                XElement y = null;       
                y = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByKey").Select(j => j).Single();
                y.Elements("MessageID").Single().Value = msgid;
                y.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                msg.content = docTemp.ToString();
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                if (sndr.sendMessage(msg))
                {
                    Console.Write("Sent message(Message ID {0}) to GetByKey from key\n", msgid);
                    Thread.Sleep(50);
                }
            }
            return docTemp;
        }

        /// <summary>
        /// GetByTimeStampCriteria for querying based on the criteria
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <param name="msgid"></param>
        /// <returns></returns>
        private static XDocument GetByTimeStampCriteria(Message msg, ReadClient clnt, Sender sndr, ref string msgid)
        {
            XDocument docTemp = XDocument.Load("Input.xml");
            clnt.RemoveOtherReads(ref docTemp, "GetByTimeStampCriteria");
            clnt.RemoveOtherWrites(ref docTemp, "GetByTimeStampCriteria");
            XElement r = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByTimeStampCriteria").Select(j => j).Single();
            clnt.numMsgs = int.Parse(r.Elements("Count").Single().Value);
            clnt.total += clnt.numMsgs;
            for (int i = 0; i < clnt.numMsgs; i++)
            {
                docTemp = XDocument.Load("Input.xml");
                clnt.RemoveOtherReads(ref docTemp, "GetByTimeStampCriteria");
                clnt.RemoveOtherWrites(ref docTemp, "GetByTimeStampCriteria");
                msgid = clnt.msgRand++.ToString();
                XElement y = null;
                y = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByTimeStampCriteria").Select(j => j).Single();
                y.Elements("MessageID").Single().Value = msgid;
                y.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                msg.content = docTemp.ToString();
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                if (sndr.sendMessage(msg))
                {
                    Console.Write("Sent message(Message ID {0}) to search fromdate & to date criteria timpestamp\n", msgid);
                    Thread.Sleep(50);
                }
            }
            return docTemp;
        }

        /// <summary>
        /// GetByMetadataCriteria criteria for the query type
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <param name="msgid"></param>
        /// <returns></returns>
        private static XDocument GetByMetadataCriteria(Message msg, ReadClient clnt, Sender sndr, ref string msgid)
        {
            XDocument docTemp = XDocument.Load("Input.xml");
            clnt.RemoveOtherReads(ref docTemp, "GetByMetadataCriteria");
            clnt.RemoveOtherWrites(ref docTemp, "GetByMetadataCriteria");
            XElement l = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByMetadataCriteria").Select(j => j).Single();
            clnt.numMsgs = int.Parse(l.Elements("Count").Single().Value);
            clnt.total += clnt.numMsgs;
            for (int i = 0; i < clnt.numMsgs; i++)
            {
                docTemp = XDocument.Load("Input.xml");
                clnt.RemoveOtherReads(ref docTemp, "GetByMetadataCriteria");
                clnt.RemoveOtherWrites(ref docTemp, "GetByMetadataCriteria");
                msgid = clnt.msgRand++.ToString();
                XElement y = null;//GetByMetadataCriteria query carried out     
                y = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetByMetadataCriteria").Select(j => j).Single();
                y.Elements("MessageID").Single().Value = msgid;
                y.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                msg.content = docTemp.ToString();
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                if (sndr.sendMessage(msg))
                {
                    Console.Write("Sent message(Message ID {0}) to GetByMetadataCriteria from key\n", msgid);
                    Thread.Sleep(50);
                }
            }
            return docTemp;
        }

        /// <summary>
        ///  GetByKeyCriteria criteria for the query type 
        /// </summary>
        /// <param name="hres"></param>
        /// <param name="msg"></param>
        /// <param name="clnt"></param>
        /// <param name="sndr"></param>
        /// <param name="timerH"></param>
        /// <param name="docTemp"></param>
        /// <param name="msgid"></param>
        private static void GetByKeyCriteria(HiResTimer hres, Message msg, ReadClient clnt, Sender sndr, HiResTimer timerH, out XDocument docTemp, out string msgid)
        {
            docTemp = XDocument.Load("Input.xml");
            clnt.RemoveOtherReads(ref docTemp, "GetKeyCriteria");
            clnt.RemoveOtherWrites(ref docTemp, "GetKeyCriteria");
            XElement k = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetKeyCriteria").Select(j => j).Single();
            clnt.numMsgs = int.Parse(k.Elements("Count").Single().Value);
            clnt.total += clnt.numMsgs;
            hres.Start();
            timerH.Start();//High resolution timer started       
            msgid = "";
            for (int i = 0; i < clnt.numMsgs; i++)
            {
                docTemp = XDocument.Load("Input.xml");
                clnt.RemoveOtherReads(ref docTemp, "GetKeyCriteria");
                clnt.RemoveOtherWrites(ref docTemp, "GetKeyCriteria");
                msgid = clnt.msgRand++.ToString();
                XElement y = null;
                y = docTemp.Descendants("OperationMessage").Where(d => d.Element("QueryType").Value == "GetKeyCriteria").Select(j => j).Single();
                y.Elements("MessageID").Single().Value = msgid;
                y.Elements("InsertTime").Single().Value = DateTime.Now.Ticks.ToString();
                msg.content = docTemp.ToString();
                msg.fromUrl = clnt.localUrl;
                msg.toUrl = clnt.remoteUrl;
                if (sndr.sendMessage(msg))
                {
                    Console.Write("Sent message(Message ID {0}) to GetCriteria from key\n", msgid);
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Define service action that needs to be handled for receiver requests at the client
        /// </summary>
        /// <param name="hres"></param>
        /// <param name="clnt"></param>
        /// <param name="rcvr"></param>
        /// <returns></returns>
        private static Action DefineServiceAction(HiResTimer hres, ReadClient clnt, Receiver rcvr)
        {
            return () =>
            {
                Message msg1 = null;
                while (true)
                {
                    msg1 = rcvr.getMessage();
                    if (msg1.content == "done")
                    {
                        hres.Stop();
                        ulong lookUpTime = hres.ElapsedMicroseconds;
                        Message msg2 = new Message();
                        msg2.fromUrl = clnt.localUrl;
                        msg2.toUrl = clnt.wpfClientURL;
                        msg2.content = "LookupTime;" + lookUpTime + ";" + "CntMsg;" + (ulong)clnt.total + ";";
                        Sender wpfSender = new Sender(clnt.localUrl);
                        wpfSender.sendMessage(msg2);
                        Console.WriteLine("\n----------------------Overall Performance Statistics for Read Client-----------------------------\n");
                        Console.WriteLine("Number of messages processed is {0}", clnt.total);
                        Console.WriteLine("\nTotal Execution time for the messages to be processed at Client Side is {0} microsecs", clnt.ReadClientTime);
                        Console.WriteLine("\nAverage Execution time for the messages to be processed at Client Side is {0} microsecs", clnt.ReadClientTime / (ulong)clnt.total);
                        Console.WriteLine("\nTotal Execution time for the messages from Client-Server-Client {0} microsecs", lookUpTime);
                        Console.WriteLine("\nAverage Execution time for the messages from Client-Server-Client is {0} microsecs", lookUpTime / (ulong)clnt.total);
                        Console.WriteLine("\n----------------------Overall Performance Statistics for Read Client-----------------------------\n");
                        break;
                    }
                    else if (msg1.content == "connection start message")
                        Console.WriteLine("Connection start message receieved at client side");
                    else
                    {
                        XDocument docTemp = XDocument.Load(new StringReader(msg1.content));
                        string mid = docTemp.Descendants("OperationMessage").Elements("MessageID").Single().Value;
                        string op = docTemp.Descendants("OperationMessage").Elements("Operation").Single().Value;
                        string qt = docTemp.Descendants("OperationMessage").Elements("QueryType").Single().Value;
                        string resp = docTemp.Descendants("OperationMessage").Elements("Response").Single().Value;
                        Console.Write("\nMessage - MessageID:{0} received at the client", mid);
                        Console.Write("\nSender:{0}", msg1.fromUrl);
                        Console.Write("\nOperation :{0}", op);
                        Console.Write("\nQueryType :{0}", qt);
                        Console.Write("\nResponse :{0}", resp);
                        string oldDt = docTemp.Descendants("InsertTime").Select(i => i).Single().Value.ToString();
                        long microseconds = (DateTime.Now.Ticks - long.Parse(oldDt)) / 10;
                        Console.Write("\nExecution time for message(MessageID:{1}) is {0} microseconds\n", microseconds, mid);
                    }
                }
                Util.waitForUser();
            };
        }
    }


#if (TEST_ReaderClient)

    /// <summary>
    /// This is to test the DBElement functionality.But here only the main functionality 
    /// is written as the testing code is moved to TestDBElementTest layer
    /// </summary>
    class TestDBElement
    {
        static void Main(string[] args)
        {
               HiResTimer hres = new HiResTimer(); //High Resolution Timer
            Console.Write("\nStarting CommService Read client");
            Console.Write("\n =============================\n");
            Console.Title = "Read Client";
            Message msg = new Message();
            ReadClient clnt = new ReadClient();
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
            msg = PerformanceTest(hres, msg, clnt, sndr);
            // Wait for user to press a key to quit.
            Util.waitForUser();
            // shut down this client's Receiver and Sender by sending close messages
            rcvr.shutDown();
            sndr.shutdown();
            Console.Write("\n\n");
        }
    }
#endif
}

