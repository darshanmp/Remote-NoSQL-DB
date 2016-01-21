/////////////////////////////////////////////////////////////////////////
// CommService.cs - Implementation of WCF message-passing service      //
// ver 1.1                                                             //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
// Modified BY  : Darshan                                             //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to the C# Console Wizard code:
 * - added reference to System.ServiceModel
 * - added using System.ServiceModel
 * - added reference to Project4Starter.ICommService
 * - copied BlockingQueue.cs into project folder
 * - Added BlockingQueue.cs to project
 */
/*
 * Maintenance History:
 * --------------------
 * ver 1.1 : 21 Oct 2015
 * - added verbose mode to support debugging and learning
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Project4Starter
{
  using SWTools;
  using Util = XMLUtility.Utilities ;

  [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
  public class CommService : ICommService
  {
    // static rcvrQueue is shared by all instances of this class

    private static SWTools.BlockingQueue<Message> rcvrQueue = 
      new SWTools.BlockingQueue<Message>();

    //----< called by clients, will only block briefly >-----------------

    public void sendMessage(Message msg)
    {
      if(Util.verbose)
        Console.Write("\n  this is CommService.sendMessage");
      rcvrQueue.enQ(msg);
    }
    //----< called by server, blocks caller while empty >----------------
    /*
     * Note: this is NOT a service method - see interface definition
     */
    public Message getMessage()
    {
      if(Util.verbose)
        Console.Write("\n  this is CommService.getMessage");
      return rcvrQueue.deQ();
    }
        public bool LogEvent()
        {
            return true;
        }
  }
}
