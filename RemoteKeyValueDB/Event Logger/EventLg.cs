using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_Logger
{
    class EventLogger
    {
        static void Main(string[] args)
        {
        }

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
