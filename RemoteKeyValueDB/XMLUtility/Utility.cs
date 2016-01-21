///////////////////////////////////////////////////////////////
// Utility.cs - Project #2 Key/Value Database               //
// Ver 1.2                                                   //
// Application: Demonstration for Project#2                  //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package begins the demonstration of Utility class.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   TestExec.cs,  DBElement.cs, DBEngine, Excpetion
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 *   Public Interface
 *   ----------------
 *   Utility clnt = new Utility();
 *   clnt.GetURL();
 * Maintenance History:
 * --------------------
 * ver 1.2 : 04 Oct 15  : Utility class generation for persisting and restoring the DB
 * ver 1.1 : 24 Sep 15
 * ver 1.0 : 18 Sep 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using DBElementSpace;
using DBEngineSpace;
using ExceptionLayerSpace;
using System.Threading;
using Project4Starter;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace XMLUtility
{
    public static class Utilities
    {

        //----< helper makes it easy to grab endpoints >---------------------
        static public string processCommandLineForLocal(string[] args, string localUrl,ref bool logging,ref string wpfClient)
        {
            bool loggingValid = false;
            string loc = "";string addr = "";
            for (int i = 0; i < args.Length; ++i)
            {
                if ((args.Length > i + 1) && (args[i] == "/l" || args[i] == "/L"))
                {
                    loc = args[i + 1];
                }
                if ((args.Length > i + 1) && (args[i] == "/a" || args[i] == "/A"))
                {
                    addr = args[i + 1];
                }
                if ((args.Length > i + 1) && (args[i] == "/Logging"))
                {
                    loggingValid = true;
                    if (args[i + 1].ToString() == "true")
                        logging = true;
                    else
                        logging = false;
                }
                if ((args.Length > i + 1) && (args[i] == "/Performance"))
                    wpfClient = "http://" + "localhost" + ":" + args[i + 1].ToString() + "/" + "CommService";
            }
            if (loggingValid == true)
            {
                localUrl = "http://" + addr + ":" + loc + "/" + "CommService";
                return localUrl;
            }
            else
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    if ((args.Length > i + 1) && (args[i] == "/l" || args[i] == "/L"))
                    {
                        localUrl = args[i + 1];
                    }
                }
                Console.WriteLine("LocalURL :" + localUrl);
                return localUrl;
            }           
        }

        static public string processCommandLineForRemote(string[] args, string remoteUrl)
        {
            string loc = ""; string addr = ""; bool loggingValid = false;
            for (int i = 0; i < args.Length; ++i)
            {
                if ((args.Length > i + 1) && (args[i] == "/r" || args[i] == "/R"))
                {
                    loc = args[i + 1];
                }
                else if ((args.Length > i + 1) && (args[i] == "/a" || args[i] == "/A"))
                {
                    addr = args[i + 1];
                }
                else if ((args.Length > i + 1) && (args[i] == "/Logging"))
                {
                    loggingValid = true;
                }
            }
            if (loggingValid == true)
            {
                remoteUrl = "http://" + addr + ":" + loc + "/" + "CommService";
                Console.WriteLine("RemoteURl :" + remoteUrl);
                return remoteUrl;
            }
            else
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    if ((args.Length > i + 1) && (args[i] == "/r" || args[i] == "/R"))
                    {
                        remoteUrl = args[i + 1];
                    }
                }
                Console.WriteLine("RemoteURl :" + remoteUrl);
                return remoteUrl;
            }
        }

        //----< helper functions to construct url strings >------------------
        public static string makeUrl(string address, string port)
        {
            return "http://" + address + ":" + port + "/CommService";
        }
        public static string urlPort(string url)
        {
            int posColon = url.LastIndexOf(':');
            int posSlash = url.LastIndexOf('/');
            string port = url.Substring(posColon + 1, posSlash - posColon - 1);
            return port;
        }
        public static string urlAddress(string url)
        {
            int posFirstColon = url.IndexOf(':');
            int posLastColon = url.LastIndexOf(':');
            string port = url.Substring(posFirstColon + 3, posLastColon - posFirstColon - 3);
            return port;
        }

        public static void swapUrls(ref Message msg)
        {
            string temp = msg.fromUrl;
            msg.fromUrl = msg.toUrl;
            msg.toUrl = temp;
        }

        public static bool verbose { get; set; } = false;

        public static void waitForUser()
        {
            Thread.Sleep(200);
            Console.Write("\nPress any key to quit: ");
            Console.ReadKey();
        }

        public static void showMessage(Message msg)
        {
            Console.Write("\n  msg.fromUrl: {0}", msg.fromUrl);
            Console.Write("\n  msg.toUrl:   {0}", msg.toUrl);
            Console.Write("\n  msg.content: {0}", msg.content);
        }
    }
    public static class XMLUtilitySpace
    {
        /// <summary>
        /// Persist to XML the Database contents of Key/Value DB
        /// </summary>
        /// <param name="db"></param>
        /// <param name="error"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool PersistToXML(this DBEngine<int, DBElement<int, string>> db, out string error, out string path,out string sendXML)
        {
            try
            {
                path = ""; sendXML = "";
                if (db.Keys().Count() == 0)
                {
                    error = "DB does not contain any elements";
                    return false;
                }
                XDocument xml = new XDocument();    //declare the xml
                xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
                XComment comment = new XComment("This is the persisted XML file");
                xml.Add(comment);
                XElement displayItems = new XElement("Items");
                XMLCreation(db, displayItems);
                xml.Add(displayItems);
                using (XmlWriter writer = XmlWriter.Create("PersistXMLFile.xml"))   //open the xml writer
                {
                    XmlTextReader textReader = new XmlTextReader("PersistXMLFile.xml"); //read the xml file
                    path = textReader.BaseURI;
                    textReader.Close();
                    xml.Save(writer);       //save the xml
                    sendXML = xml.ToString();
                    if (File.Exists("PersistXMLFile.xml"))
                    {
                        error = "Success";
                        return true;
                    }
                    else
                    {
                        error = "Failure";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occurred while persisting xml", ex);
            }
        }
        /// <summary>
        /// Creation of xml and the dbelement contents into xml
        /// </summary>
        /// <param name="db"></param>
        /// <param name="displayItems"></param>
        private static void XMLCreation(DBEngine<int, DBElement<int, string>> db, XElement displayItems)
        {
            foreach (int key in db.Keys())
            {
                XElement displayitem = new XElement("Item");
                DBElement<int, string> elem = new DBElement<int, string>();
                db.getValue(key, out elem);
                XElement displayKey = new XElement("Key", key); //read the key  
                XElement displayValue = new XElement("Value");  //read the value
                XElement displayName = new XElement("Name", elem.name); //read the name
                XElement displayDescr = new XElement("Description", elem.descr);    //read the desc
                XElement displayTime = new XElement("TimeStamp", elem.timeStamp.ToString("yyyy-MM-dd HH:mm:ss"));
                XElement displayChildren = new XElement("Children");
                displayChildren = new XElement("Children"); //read the children into xml
                foreach (var i in elem.children)
                {
                    XElement displayChild = new XElement("Child", i);
                    displayChildren.Add(displayChild);
                }
                XElement displayPayLoad = new XElement("Payload", elem.payload);    //read the payload
                displayValue.Add(displayName);
                displayValue.Add(displayDescr);
                displayValue.Add(displayTime);
                if (elem.children.Count > 0)
                    displayValue.Add(displayChildren);
                displayValue.Add(displayPayLoad);
                displayitem.Add(displayKey);
                displayitem.Add(displayValue);
                displayItems.Add(displayitem);
            }
        }

        /// <summary>
        /// Restore the Db contents from xml 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="error"></param>
        /// <param name="resultStat"></param>
        public static void RestoreDB(this DBEngine<int, DBElement<int, string>> element, out string error, out bool resultStat)
        {
            try
            {
                int key = 0;
                error = "Success";
                XElement xml;
                if (File.Exists("PersistXMLFile.xml"))      //check if file exists
                {
                    xml = XElement.Load("PersistXMLFile.xml");  //load the file contents
                    foreach (XElement item in xml.Elements())
                    {
                        DBElement<int, string> elem = new DBElement<int, string>();

                        key = int.Parse(item.Elements("Key").First().Value.ToString());     //get the key contents
                        elem.name = item.Elements("Value").Elements("Name").Single().Value.ToString(); //read the value contents
                        elem.descr = item.Elements("Value").Elements("Description").Single().Value.ToString();
                        elem.timeStamp = DateTime.Parse(item.Elements("Value").Elements("TimeStamp").Single().Value.ToString());
                        elem.payload = item.Elements("Value").Elements("Payload").Single().Value.ToString();
                        List<int> childs = new List<int>();
                        if (item.Elements("Value").Elements("Children").Count() > 0)
                        {
                            foreach (var i in item.Elements("Value").Elements("Children").Elements("Child"))
                            {
                                childs.Add(int.Parse(i.Value.ToString()));
                            }
                        }
                        elem.children = childs;
                        element.insert(key, elem);
                    }
                    resultStat = true;
                }
                else
                {
                    error = "Failure";
                    resultStat = false;
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occurred while restoring Database", ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="error"></param>
        /// <param name="resultStat"></param>
        //public static void RestoreStringDB(this DBEngine<string, DBElement<string, List<string>>> element, out string error, out bool resultStat)
        //{
        //    try
        //    {
        //        int key = 0;
        //        error = "Success";
        //        XElement xml;
        //        if (File.Exists("PersistXMLFile.xml"))      //check if file exists
        //        {
        //            xml = XElement.Load("PersistXMLFile.xml");  //load the file contents
        //            foreach (XElement item in xml.Elements())
        //            {
        //                DBElement<string, List<string>> elem = new DBElement<string, List<string>>();

        //                key = int.Parse(item.Elements("Key").First().Value.ToString());     //get the key contents
        //                elem.name = item.Elements("Value").Elements("Name").Single().Value.ToString(); //read the value contents
        //                elem.descr = item.Elements("Value").Elements("Description").Single().Value.ToString();
        //                elem.timeStamp = DateTime.Parse(item.Elements("Value").Elements("TimeStamp").Single().Value.ToString());
        //                elem.payload = item.Elements("Value").Elements("Payload").Single().Value.ToString();
        //                List<int> childs = new List<int>();
        //                if (item.Elements("Value").Elements("Children").Count() > 0)
        //                {
        //                    foreach (var i in item.Elements("Value").Elements("Children").Elements("Child"))
        //                    {
        //                        childs.Add(int.Parse(i.Value.ToString()));
        //                    }
        //                }
        //                elem.children = childs;
        //                element.insert(key, elem);
        //            }
        //            resultStat = true;
        //        }
        //        else
        //        {
        //            error = "Failure";
        //            resultStat = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new CustomException("Error occurred while restoring Database", ex);
        //    }
        //}


        /// <summary>
        /// Title extension  for the display unit
        /// </summary>
        /// <param name="aString"></param>
        /// <param name="underline"></param>
        public static void title(this string aString, char underline = '-')
        {
            Console.Write("\n  {0}", aString);
            Console.Write("\n {0}", new string(underline, aString.Length + 2));
        }

        /// <summary>
        /// Restore the string db from xml
        /// </summary>
        /// <param name="element"></param>
        /// <param name="error"></param>
        /// <param name="resultStat"></param>
        public static void RestoreStringDB(this DBEngine<string, DBElement<string, List<string>>> element, out string error, out bool resultStat)
        {
            try
            {
                string key = "";
                error = "Success";
                XElement xml;
                if (File.Exists("PackageStructure.xml"))
                {
                    xml = XElement.Load("PackageStructure.xml");
                    foreach (XElement item in xml.Elements())
                    {
                        DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
                        key = item.Elements("Key").First().Value.ToString();
                        elem.name = item.Elements("Value").Elements("Name").Single().Value.ToString();
                        elem.descr = item.Elements("Value").Elements("Description").Single().Value.ToString();
                        elem.timeStamp = DateTime.Now;
                        elem.payload = new List<string>();
                        foreach (var i in item.Elements("Value").Elements("Payload"))
                        {
                            elem.payload.Add(i.Value);
                        }
                        //elem.payload = item.Elements("Value").Elements("Payload").Single().Value.ToString();
                        List<string> childs = new List<string>();
                        if (item.Elements("Value").Elements("Children").Count() > 0)
                        {
                            foreach (var i in item.Elements("Value").Elements("Children").Elements("Child"))
                            {
                                childs.Add(i.Value.ToString());
                            }
                        }
                        elem.children = childs;
                        element.insert(key, elem);
                    }
                    resultStat = true;
                }
                else
                {
                    error = "Failure";
                    resultStat = false;
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Error occurred while inputting the Package XML\n", ex);
            }
        }

#if (TEST_UTILITY)
        public class XMLUtilityMain
        {
            static void Main(string[] args)
            {
                XMLUtilitySpace.title("testing utilities");
                Console.WriteLine();
                XMLUtilitySpace.title("testing makeUrl");
                string localUrl = Utilities.makeUrl("localhost", "7070");
                string remoteUrl = Utilities.makeUrl("localhost", "7071");
                Console.Write("\n  localUrl  = {0}", localUrl);
                Console.Write("\n  remoteUrl = {0}", remoteUrl);
                Console.WriteLine();

                XMLUtilitySpace.title("testing url parsing");
                string port = Utilities.urlPort(localUrl);
                string addr = Utilities.urlAddress(localUrl);
                Console.Write("\n  local port = {0}", port);
                Console.Write("\n  local addr = {0}", addr);
                Console.WriteLine();

                XMLUtilitySpace.title("testing processCommandLine");
                bool logging = false;
                string wpfClientURL = "http://localhost:8089/CommService";
                localUrl = Utilities.processCommandLineForLocal(args, localUrl, ref logging,ref wpfClientURL);
                remoteUrl = Utilities.processCommandLineForRemote(args, remoteUrl);
                Console.Write("\n  localUrl  = {0}", localUrl);
                Console.Write("\n  remoteUrl = {0}", remoteUrl);
                Console.WriteLine();

                XMLUtilitySpace.title("testing swapUrls(ref Message msg)");
                Message msg = new Message();
                msg.toUrl = "http://localhost:8080/CommService";
                msg.fromUrl = "http://localhost:8081/CommService";
                msg.content = "swapee";
                Utilities.showMessage(msg);
                Console.WriteLine();
                Utilities.swapUrls(ref msg);
                Utilities.showMessage(msg);
                Console.Write("\n\n");
            }
        }
    }

    public class HiResTimer
    {
        protected ulong a, b, f;
        public HiResTimer()
        {
            a = b = 0UL;
            if (QueryPerformanceFrequency(out f) == 0)
                throw new Win32Exception();
        }
        public ulong ElapsedTicks
        {
            get
            { return (b - a); }
        }

        public ulong ElapsedMicroseconds
        {
            get
            {
                ulong d = (b - a);
                if (d < 0x10c6f7a0b5edUL) // 2^64 / 1e6
                    return (d * 1000000UL) / f;
                else
                    return (d / f) * 1000000UL;
            }
        }

        public TimeSpan ElapsedTimeSpan
        {
            get
            {
                ulong t = 10UL * ElapsedMicroseconds;
                if ((t & 0x8000000000000000UL) == 0UL)
                    return new TimeSpan((long)t);
                else
                    return TimeSpan.MaxValue;
            }
        }
        public ulong Frequency
        {
            get
            { return f; }
        }
        public ulong Start()
        {
            Thread.Sleep(0);
            QueryPerformanceCounter(out a);
            return a;
        }
        public ulong Stop()
        {
            QueryPerformanceCounter(out b);
            return b;
        }

        // Here, C# makes calls into C language functions in Win32 API
        // through the magic of .Net Interop
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern
           int QueryPerformanceFrequency(out ulong x);
        [DllImport("kernel32.dll")]
        protected static extern
           int QueryPerformanceCounter(out ulong x);
    }
#endif

}
