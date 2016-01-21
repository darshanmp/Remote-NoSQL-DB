
/////////////////////////////////////////////////////////////////////////
// Server.cs - CommService server                                      //
// ver 1.1                                                             //
// Modified by : Darshan                                               //
// Prototype: Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - This server now receives and then sends back received messages.
 */
/*
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 *   Public Interface
 *   ----------------
 *   Server clnt = new Server();
 *   clnt.ProcessReadMessage();
 * Maintenance History:
 * --------------------
 * ver 3.0 :21 Nov : Server modified to handle  requests 
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 * ver 2.1 : 24 Oct 2015
 * - added Sender so Server can echo back messages it receives
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project4Starter
{
    using DBElementSpace;
    using DisplaySpace;
    using System.Collections;
    using System.IO;
    using System.Xml.Linq;
    using XMLUtility;
    using Util = XMLUtility.Utilities;
    using RequestHandlerSpace;
    using System.Threading;
    using ExceptionLayerSpace;

    /// <summary>
    /// 
    /// </summary>
    public class Server
    {
        RequestHandler rh = null;
        DBElement<int, string> elem = null;
        DBElement<string, List<string>> strElem = null;
        Hashtable tot = null;
        public Server()
        {
            elem = new DBElement<int, string>();
            strElem = new DBElement<string, List<string>>();
            rh = new RequestHandler();
            tot = new Hashtable();
        }
        string address { get; set; } = "localhost";
        string port { get; set; } = "8080";
        string Wpfport { get; set; } = "8089";

        //----< quick way to grab ports and addresses from commandline >-----

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void ProcessCommandLine(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    Console.WriteLine("Server Port {0}", args[0]);
                    Console.WriteLine("Server Address {0}", args[1]);
                    Console.WriteLine("WPF Performance Client Address {0}", args[2]);
                    port = args[0];
                }
                if (args.Length > 1)
                {
                    address = args[1];
                }
                if (args.Length > 1)
                {
                    Wpfport = args[2];
                }
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error occurred in ProcessCommandLine ", ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="typeOfDB"></param>
        public void showDB(bool status, string typeOfDB)
        {
            rh.ShowDB(status, typeOfDB);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        public void ProcessMessage(ref Message msg, ref string msgid)
        {
            try
            {
                string key = ""; bool status = false;
                XDocument doc = XDocument.Load(new StringReader(msg.content));
                string op = doc.Descendants("Operation").Select(i => i).Single().Value;
                if (doc.Descendants("Key").Count() > 0)
                    key = doc.Descendants("Key").Select(i => i).Single().Value;
                msgid = doc.Descendants("MessageID").Select(i => i).Single().Value;
                HandleSwitchProcess(msg, ref msgid, ref status, doc, op);
                msg.content = doc.ToString();
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error occurred in ProcessMessage ", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        /// <param name="status"></param>
        /// <param name="doc"></param>
        /// <param name="op"></param>
        private void HandleSwitchProcess(Message msg, ref string msgid, ref bool status, XDocument doc, string op)
        {
            try
            {
                switch (op)
                {
                    case "Insert":
                        status = InsertHandler(msg, ref msgid, doc);
                        LogMessage("LogInsertOp");
                        break;
                    case "StringDBInsert":
                        status = StringDbHandler(msg, ref msgid, doc);
                        break;
                    case "Delete":
                        status = DeleteHandler(msg, ref msgid, doc);
                        break;
                    case "Update":
                        status = rh.Update(msg.content, ref msgid);
                        if (status == true)
                            doc.Descendants("Response").Select(i => i).Single().Value = "Update is successful";
                        break;
                    case "ManualPersist":
                        status = ManualPersistHandler(msg, ref msgid, doc);
                        break;
                    case "RestoreDB":
                        status = rh.RestoreAugmentDB(msg.content, ref msgid);
                        if (status == true)
                            doc.Descendants("Response").Select(i => i).Single().Value = "RestoreDB is successful";
                        break;
                    case "ScheduledPersist":
                        status = rh.ScheduledPersistance(msg.content, ref msgid);
                        if (status == true)
                            doc.Descendants("Response").Select(i => i).Single().Value = "Scheduled Persisting is successful";
                        break;
                    case "LoadPackageStructure":
                        rh.LoadPackageStructure(msg.content, ref msgid);
                        doc.Descendants("Response").Select(i => i).Single().Value = "Package structure is loaded successfully";
                        break;
                    case "ImmutableDB":
                        rh.ImmutableDB(msg.content, ref msgid);
                        doc.Descendants("Response").Select(i => i).Single().Value = "Immutable DB created";
                        break;
                }
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error occurred in HandleSwitchProcess ", ex);
            }
        }
        private string LogMessage(string dispMesg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("\n (Latency) Total time taken for (Client-Server-Client) request: {0} microsecs",23)); 
            dispMesg = sb.ToString();
            //Event logged using task async call
            string xw = "Event 1 : This processing took 16 sec";
            Task t = Task.Factory.StartNew(() => ProcessDataAsync(dispMesg+xw)); //ASync call
            //ProcessDatasync(dispMesg + xw); //Sync call            
            return dispMesg;
        }
        static void  ProcessDatasync(string x)
        {
            string msgid = "";
            RequestHandler rh = new RequestHandler();
            Random ran = new Random();
            bool status = rh.LogCreator(ran.Next(), x, ref msgid);     
        }
        static async Task ProcessDataAsync(string x)
        {
            await Task.Run(() =>
            {
                string msgid = "";
                RequestHandler rh = new RequestHandler();
                Random ran = new Random();
                bool status = rh.LogCreator(ran.Next(), x, ref msgid);
                return status;
            });
        }
        private bool ManualPersistHandler(Message msg, ref string msgid, XDocument doc)
        {
            bool status = rh.ManualPersist(msg.content, ref msgid);
            if (status == true)
                doc.Descendants("Response").Select(i => i).Single().Value = "Manual Persist is successful";
            return status;
        }

        /// <summary>
        /// Delete handler
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private bool DeleteHandler(Message msg, ref string msgid, XDocument doc)
        {
            bool status = rh.Delete(msg.content, ref msgid);
            if (status == true)
                doc.Descendants("Response").Select(i => i).Single().Value = "Delete is successful";
            return status;
        }

        /// <summary>
        /// string db handler
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private bool StringDbHandler(Message msg, ref string msgid, XDocument doc)
        {
            bool status = rh.stringDBInsert(msg.content, ref msgid);
            doc.Descendants("Response").Select(i => i).Single().Value = "Insert is successful";
            return status;
        }

        //insert db handler
        private bool InsertHandler(Message msg, ref string msgid, XDocument doc)
        {
            bool status = rh.Insert(msg.content, ref msgid);
            if (status == true)
                doc.Descendants("Response").Select(i => i).Single().Value = "Insert is successful";
            return status;
        }

        /// <summary>
        /// Process read messsage at the server
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        public void ProcessReadMessage(ref Message msg, ref string msgid)
        {
            try
            {
                XDocument doc = XDocument.Load(new StringReader(msg.content));
                string op = doc.Descendants("QueryType").Select(i => i).Single().Value;
                msgid = doc.Descendants("MessageID").Select(i => i).Single().Value;
                rh.ReadQueryHandle(ref msg, ref msgid);
            }
            catch (CustomException ex)
            {
                throw new CustomException("Error occurred in HandleSwitchProcess ", ex);
            }
        }
        /// <summary>
        /// Main entry into Server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Util.verbose = false;
                Server srvr = new Server();
                srvr.ProcessCommandLine(args);
                Console.Title = "Server";
                Console.Write(String.Format("\nStarting CommService server listening on port {0}", srvr.port));
                Console.WriteLine("\nServer Address is {0}", Util.makeUrl(srvr.address, srvr.port));
                Console.Write("\n ====================================================\n");
                Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
                Receiver rcvr = new Receiver(srvr.port, srvr.address);
                Action serviceAction = DefineServiceAction(srvr, sndr, rcvr);
                if (rcvr.StartService())
                {
                    rcvr.doService(serviceAction); // This serviceAction is asynchronous, // so the call doesn't block.
                }
                Util.waitForUser();
            }
            catch (CustomException ex)
            {
                Console.WriteLine("Error occured in Server.cs{0}", ex);
            }
        }

        private static Action DefineServiceAction(Server srvr, Sender sndr, Receiver rcvr)
        {
            Action serviceAction = () =>
            {
                Message msg = null; string msgid = "";
                while (true)
                {
                    msg = rcvr.getMessage();   // note use of non-service method to deQ messages
                    Console.Write("\nReceived message:");
                    Console.Write("\nSender is {0}", msg.fromUrl);
                    if (msg.content == "connection start message")
                    {
                        Console.Write("\nConnection start message recieved\n");
                        continue; //Don't Send back Start message
                    }
                    else if (msg.content == "done")
                    {
                        msg = FinalMessageServe(srvr, sndr, msg);
                        continue;
                    }
                    else if (msg.content == "closeServer")
                    {
                        Console.Write("Received closeServer");
                        break;
                    }
                    else
                    {
                        ProcessResponse(srvr, sndr, ref msg, ref msgid);
                    }
                }
            };
            return serviceAction;
        }

        private static Message FinalMessageServe(Server srvr, Sender sndr, Message msg)
        {
            Console.WriteLine("Displaying DB entries");
            srvr.showDB(true, "int");
            Console.Write("\nClient has finished\n");
            Util.swapUrls(ref msg);
            string totalTime = srvr.tot[msg.toUrl].ToString();
            //call the wpf to send message 
            Sender wpfSender = new Sender(Util.makeUrl(srvr.address, srvr.port));
            Message wpfMsg = new Message(); //WPF sender 
            wpfMsg.fromUrl = msg.fromUrl;
            wpfMsg.toUrl = Util.makeUrl(srvr.address, srvr.Wpfport);
            wpfMsg.content = "ServerTime;" + totalTime + ";" + "ClientURL;" + msg.toUrl;
            srvr.tot.Remove(msg.toUrl);
            wpfSender.sendMessage(wpfMsg);
            sndr.sendMessage(msg);       //call the wpf to send message 
            return msg;
        }

        /// <summary>
        /// Proces wheh the response has been received
        /// </summary>
        /// <param name="srvr"></param>
        /// <param name="sndr"></param>
        /// <param name="msg"></param>
        /// <param name="msgid"></param>
        private static void ProcessResponse(Server srvr, Sender sndr, ref Message msg, ref string msgid)
        {
            HiResTimer hres = new HiResTimer(); //High Resolution Timer
            hres.Start();
            XDocument doc = XDocument.Load(new StringReader(msg.content));
            string op = doc.Descendants("Operation").Select(i => i).Single().Value;
            if (op == "Read")
                srvr.ProcessReadMessage(ref msg, ref msgid);
            else
                srvr.ProcessMessage(ref msg, ref msgid);
            hres.Stop();
            Console.WriteLine("Server execution time for processing message(MsgID:{0}) is {1} microseconds \n", msgid, hres.ElapsedMicroseconds);
            Util.swapUrls(ref msg);  // swap urls for outgoing message
            if (srvr.tot.ContainsKey(msg.toUrl))
            {
                ulong x = ulong.Parse(srvr.tot[msg.toUrl].ToString());
                x += hres.ElapsedMicroseconds;
                srvr.tot[msg.toUrl] = x.ToString();
            }
            else
                srvr.tot[msg.toUrl] = hres.ElapsedMicroseconds;
            sndr.sendMessage(msg);
        }
    }


#if (TEST_DBServer)

    /// <summary>
    /// This is to test the DBElement functionality.But here only the main functionality 
    /// is written as the testing code is moved to TestDBElementTest layer
    /// </summary>
    class TestServer
    {
        static void Main(string[] args)
        {
         try
            {
                Util.verbose = false;
                Server srvr = new Server();
                srvr.ProcessCommandLine(args);
                Console.Title = "Server";
                Console.Write(String.Format("\nStarting CommService server listening on port {0}", srvr.port));
                Console.WriteLine("\nServer Address is {0}", Util.makeUrl(srvr.address, srvr.port));
                Console.Write("\n ====================================================\n");
                Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
                Receiver rcvr = new Receiver(srvr.port, srvr.address);
                Action serviceAction = DefineServiceAction(srvr, sndr, rcvr);
                if (rcvr.StartService())
                {
                    rcvr.doService(serviceAction); // This serviceAction is asynchronous, // so the call doesn't block.
                }
                Util.waitForUser();
            }
            catch (CustomException ex)
            {
                Console.WriteLine("Error occured in Server.cs{0}", ex);
            }
        }
    }
#endif
}
