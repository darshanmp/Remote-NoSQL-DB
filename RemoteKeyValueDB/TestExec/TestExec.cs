/////////////////////////////////////////////////////////////////////////
// TestExec.cs - CommService server                                     //
// ver 1.1                                                             //
// Modified by : Darshan                                               //
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
* Main test exec package starting project where writers and readers are started based on input
 */
/*
 *   Public Interface
 *   ----------------
 *   TestExec clnt = new TestExec();
 *   clnt.ProcessReadMessage();
 * Maintenance History:
 * --------------------
* ver 1.0 :21 Nov : Test Executive package
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestExec
{
   public class TestExec
    {
        ProcessStarter proc = null;
        public TestExec()
        {
            proc = new ProcessStarter();
        }
        public void RequirementScreen()
        {
            Console.WriteLine("For checking the requirements of Project #4 please check other windows for the reader/writer/Project 2 requirements\n");
            Console.WriteLine("Req#2 : I have used my own NoSQL Db implemented in Project2\n");
            Console.WriteLine("Req#3 : WCF has been implemented as can be seen from code base\n");
            Console.WriteLine("Req#4 : This can be verfied from the Project 2 Requirements Testing window\n");
            Console.WriteLine("Req#5 : Write CLient has been implmented that can be seen from Writer Client window and WriterClient.cs\n");
            Console.WriteLine("Req#6 : The command line parameters are handled can be seen from run.bat & TestExec.cs\n");
            Console.WriteLine("Req#7 : Read CLient has been implmented that can be seen from Reader Client window and ReadClient.cs\n");
            Console.WriteLine("\n\nPlease verify other windows for detailed analysis");
        }
        static void Main(string[] args)
        {
            Console.Title = "TestExec-Process Window";
            TestExec tc = new TestExec();
            tc.RequirementScreen();
            TestExec te = new TestExec();
            string intFace = ""; int writerCount = 0, readerCount = 0, remoteCount = 0;
            string remoteAddr = "",logging="",wpf="";
            List<int> lstPort = new List<int>();
            for (int i = 0; i < args.Length; ++i)
                DecideProcessExce(args, ref intFace, ref writerCount, ref readerCount, ref remoteCount, ref remoteAddr, ref logging, ref wpf, lstPort, ref i);
            string perfClient = "/Performance " + wpf;
            te.proc.startProcess("RemoteKeyValueDB/WPFWriterClient/bin/Debug/WpfApplication1.exe", perfClient);
            string serverCmd = remoteCount + " " + remoteAddr+" "+ wpf;
            te.proc.startProcess("RemoteKeyValueDB/Server/bin/debug/Server.exe", serverCmd);
            string bReqCmd = "/L " + lstPort[0].ToString() + " /R " + remoteCount + " /A " + remoteAddr + logging;
            te.proc.startProcess("RemoteKeyValueDB/BasicRequirementTest/bin/debug/TestExec.exe", bReqCmd);
            lstPort.RemoveAt(0);
            Thread.Sleep(3000);
            int h = 0;
            for (int k = 0; k < writerCount; k++)
            {
                string writerCmd = "/L "+ lstPort[h].ToString() +" /R "+remoteCount + " /A " + remoteAddr + logging +" /Performance "+wpf;
                h++;
                if (intFace == "GUI")
                {
                    te.proc.startProcess("RemoteKeyValueDB/WriterClient/bin/debug/Client.exe", writerCmd);
                }
                else if(intFace=="Cons")
                {
                    
                    te.proc.startProcess("RemoteKeyValueDB/WriterClient/bin/debug/Client.exe", writerCmd);
                }
            }
            for (int j = 0; j < readerCount; j++)
            {
                string readerCmd = "/L " + lstPort[h].ToString() + " /R " + remoteCount +" /A "+ remoteAddr + logging + " /Performance " + wpf;
                h++;
                ProcessStarter proc = new ProcessStarter();
                //Start Reader client
                te.proc.startProcess("RemoteKeyValueDB/ReadClient/bin/debug/Client2.exe",readerCmd);
            }
        }
        /// <summary>
        /// Decide which process needs to be executed
        /// </summary>
        /// <param name="args"></param>
        /// <param name="intFace"></param>
        /// <param name="writerCount"></param>
        /// <param name="readerCount"></param>
        /// <param name="remoteCount"></param>
        /// <param name="remoteAddr"></param>
        /// <param name="logging"></param>
        /// <param name="wpf"></param>
        /// <param name="lstPort"></param>
        /// <param name="i"></param>
        private static void DecideProcessExce(string[] args, ref string intFace, ref int writerCount, ref int readerCount, ref int remoteCount, ref string remoteAddr, ref string logging, ref string wpf, List<int> lstPort, ref int i)
        {
            if ((args.Length > i + 1) && (args[i] == "/WriteInterface"))
            {
                intFace = args[i + 1];
            }
            else if ((args.Length > i + 1) && (args[i] == "/Write"))
            {
                writerCount = int.Parse(args[i + 1]);
            }
            else if ((args.Length > i + 1) && (args[i] == "/Read"))
            {
                readerCount = int.Parse(args[i + 1]);
            }
            else if ((args.Length > i + 1) && (args[i] == "/Local"))
            {
                for (int j = 0; j <= writerCount + readerCount; j++)
                {
                    lstPort.Add(int.Parse(args[i + 1]));
                    i++;
                }
                i--;
            }
            else if ((args.Length > i + 1) && (args[i] == "/Remote"))
            {
                remoteCount = int.Parse(args[i + 1]);
            }
            else if ((args.Length > i + 1) && (args[i] == "/Address"))
            {
                remoteAddr = args[i + 1].ToString();
            }
            else if ((args.Length > i + 1) && (args[i] == "/Logging"))
            {
                if (args[i + 1].ToString() == "Y" || args[i + 1].ToString() == "y")
                    logging = " /Logging " + "true";
                else
                    logging = " /Logging " + "false";
            }
            else if ((args.Length > i + 1) && (args[i] == "/Performance"))
            {
                wpf = args[i + 1].ToString().Trim();

            }
        }
    }

    public class ProcessStarter
    {
        public bool startProcess(string process,string commandLine )
        {
            process = Path.GetFullPath(process);
            Console.Write("\n  FileCall - \"{0}\"", process);
            Console.WriteLine("\n  CommandLine parameters - \"{0}\"", commandLine);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = process,
                Arguments = commandLine,
                // set UseShellExecute to true to see child console, false hides console
                UseShellExecute = true
            };
            try
            {
                Process p = Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}", ex.Message);
                return false;
            }
        }
    }

#if (TEST_TestExec)

    /// <summary>
    /// This is to test the DBElement functionality.But here only the main functionality 
    /// is written as the testing code is moved to TestDBElementTest layer
    /// </summary>
    class TestDBElement
    {
        static void Main(string[] args)
        {
      Console.Title = "Started Process";
      StartedProcess sp = new StartedProcess();
      sp.showCommandLine(args);
      sp.makeSound();
          
      for(int i=0; i<10; ++i)
      {
        Console.Write("\n  running");
        Thread.Sleep(500);
      }
      Console.Write("\n  -- press key to exit: ");
      Console.ReadKey();
        }
    }
#endif
}
