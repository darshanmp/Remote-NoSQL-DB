/////////////////////////////////////////////////////////////////////////
// MainWindows.xaml.cs - CommService GUI Client                        //
// ver 1.1                                                             //
// Darshan    , CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# WPF Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, MakeMessage, Utilities
 * - Added using Project4Starter
 *
 * Note:
 * - This client receives and sends messages.
 */
/*
 *   Public Interface
 *   ----------------
 *   MainWindow clnt = new MainWindow();
 *   clnt.GetURL();
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 1.1 : 29 Oct 2015
 * - Modified to display perfromance results
 * ver 1.0 : 25 Oct 2015
 * changed Xaml to achieve more fluid design
 *   by embedding controls in grid columns as well as rows
 * - added derived sender, overridding notification methods
 *   to put notifications in status textbox
 * - added use of MessageMaker in send_click
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Project4Starter;
using System.Collections;
using ExceptionLayerSpace;
using System.IO;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        static bool firstConnect = true;
        static Receiver rcvr = null;
        static wpfSender sndr = null;
        string localAddress = "localhost";
        string localPort = "8089";
        Hashtable table = new Hashtable();
        /////////////////////////////////////////////////////////////////////
        // nested class wpfSender used to override Sender message handling
        // - routes messages to status textbox
        public class wpfSender : Sender
        {
            TextBox lStat_ = null;  // reference to UIs local status textbox
            System.Windows.Threading.Dispatcher dispatcher_ = null;

            public wpfSender(TextBox lStat, System.Windows.Threading.Dispatcher dispatcher)
            {
                dispatcher_ = dispatcher;  // use to send results action to main UI thread
                lStat_ = lStat;
            }
            public override void sendMsgNotify(string msg)
            {
                Action act = () => { lStat_.Text = msg; };
                dispatcher_.Invoke(act);

            }
            public override void sendExceptionNotify(Exception ex, string msg = "")
            {
                Action act = () => { lStat_.Text = ex.Message; };
                dispatcher_.Invoke(act);
            }
            public override void sendAttemptNotify(int attemptNumber)
            {
                Action act = null;
                act = () => { lStat_.Text = String.Format("attempt to send #{0}", attemptNumber); };
                dispatcher_.Invoke(act);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Title = " WPF Performance Analyzer";
        }
        /// <summary>
        /// Client start event called from App
        /// </summary>
        /// <param name="client"></param>
        /// <param name="port"></param>
        public void WPFClientStart(string client, string port)
        {
            if (client == "/Performance")
            {
                localPort = port;
                lblPort.Content = "Local Port Address: " + localPort;
                if (firstConnect)
                {
                    firstConnect = false;
                    if (rcvr != null)
                        rcvr.shutDown();
                    setupChannel();
                }
            }

        }
        //----< trim off leading and trailing white space >------------------
        string trim(string msg)
        {
            StringBuilder sb = new StringBuilder(msg);
            for (int i = 0; i < sb.Length; ++i)
                if (sb[i] == '\n')
                    sb.Remove(i, 1);
            return sb.ToString().Trim();
        }
        //----< indirectly used by child receive thread to post results >----

        /// <summary>
        /// indirectly used by child receive thread to post results
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fromURL"></param>
        public void postRcvMsg(string content, string fromURL)
        {
            try
            {
                TextBlock item = new TextBlock();
                string displayMessage = "", url = "";
                ulong x = 0; int count = 0;
                if (content != "connection start message")
                {
                    string[] dispMesg = content.Split(';');
                    if (content.Contains("ServerTime"))
                    {
                        for (int i = 0; i < dispMesg.Count(); i++)
                        {
                            if (dispMesg[i].Contains("ClientURL"))
                                table[dispMesg[i + 1]] = dispMesg[i - 1];
                        }
                    }
                    else if (content.Contains("LookupTime"))
                    {
                        displayMessage = LookupTimeProcess(fromURL, item, ref x, ref count, dispMesg);
                    }
                }
            }
            catch (CustomException wx)
            {
                Console.Write("Error in WPF client : {0}\n", wx.Message);
            }
        }
        void setupChannel()
        {
            try
            {
                rcvr = new Receiver(localPort, localAddress);
                Action serviceAction = () =>
                {
                    try
                    {
                        Message rmsg = null;
                        while (true)
                        {
                            rmsg = rcvr.getMessage();
                            Action act = () => { postRcvMsg(rmsg.content, rmsg.fromUrl); };
                            Dispatcher.Invoke(act, System.Windows.Threading.DispatcherPriority.Background);
                        }
                    }
                    catch (Exception ex)
                    {
                        Action act = () =>
                        {
                            lblError.Content = ex.Message;
                        };
                        Dispatcher.Invoke(act);
                    }
                };
                if (rcvr.StartService())
                {
                    rcvr.doService(serviceAction);
                }
                TextBox lStat = new TextBox();
                sndr = new wpfSender(lStat, this.Dispatcher);
            }
            catch (CustomException wx)
            {
                Console.Write("Error in WPF client : {0}\n", wx.Message);
            }
        }
        /// <summary>
        /// Lookup time processing carried out
        /// </summary>
        /// <param name="fromURL"></param>
        /// <param name="item"></param>
        /// <param name="x"></param>
        /// <param name="count"></param>
        /// <param name="dispMesg"></param>
        /// <returns></returns>

        private string LookupTimeProcess(string fromURL, TextBlock item, ref ulong x, ref int count, string[] dispMesg)
        {
            string displayMessage;
            for (int i = 0; i < dispMesg.Count(); i++)
            {
                if (dispMesg[i].Contains("LookupTime"))
                {
                    x = ulong.Parse(dispMesg[i + 1]);
                }
                if (dispMesg[i].Contains("CntMsg"))
                {
                    count = int.Parse(dispMesg[i + 1]);
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("\n Client url : {0} \n", fromURL));
            sb.Append(String.Format("\n (Latency) Total time taken for (Client-Server-Client) request: {0} microsecs", x.ToString()));
            sb.Append(String.Format("\n (Throughput) Total number of messages processed: {0}", count));
            sb.Append(String.Format("\n Average time taken for each request(Client-Server-Client): {0} microsecs", (x / (ulong)count)).ToString());
            if (table[fromURL] != null)
                sb.Append(String.Format("\n Total time taken by Server to process these messages: {0} microsecs", table[fromURL].ToString()));
            sb.Append(String.Format("\n Average time taken by Server to process each message: {0} microsecs \n\n", (ulong.Parse(table[fromURL].ToString()) / (ulong)count)).ToString());
            displayMessage = sb.ToString();

            //Event logged using task async call

            string xw = "Event 1 : This processing took 16 sec";
            Task t = Task.Factory.StartNew(() => ProcessDataAsync(xw));
            t.Start();

            item.Text = displayMessage;
            //item.Text = trim(content);
            item.FontSize = 16;
            rcvmsgs.Items.Insert(0, item);
            return displayMessage;
        }
        static void ProcessDataAsync(string x)
        {
            string filename = "LogFile.txt";
            // Start the HandleFile method.
            Task<int> task = HandleFileAsync(filename);
        }

        static async Task<int> HandleFileAsync(string file)
        {
            int count = 0;
            // ... Use async StreamReader method.
            using (StreamReader reader = new StreamReader(file))
            {
                string v = await reader.ReadToEndAsync();
                count += v.Length;
                for (int i = 0; i < 10000; i++)
                {
                    int x = v.GetHashCode();
                    if (x == 0)
                    {
                        count--;
                    }
                }
            }
            Console.WriteLine("HandleFile exit");
            return count;
        }

    }
}
//----< get Receiver and Sender running >----------------------------
