///////////////////////////////////////////////////////////////
// Scheduler.cs - Scheduler for noSQL database               //
// Ver 1.1                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
// Source: Jim Fawcett                                       //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements Scheduler
 * which is used for scheduling the persisting of DB periodically
 *
 * Note: This package defines Scheduler
 */
/*
 *   Public Interface
 *   ----------------
 *   Scheduler clnt = new Scheduler();
 *   clnt.ScheduleXML();

 * Maintenance:
 * ------------
 * Required Files: DBElement.cs,DBEngine.cs,ExceptionLayer.cs,Utility.cs   
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15 : This package implements Scheduler which is used for scheduling 
 * the persisting of DB periodically
 */
using DBElementSpace;
using DBEngineSpace;
using ExceptionLayerSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Schedule
{
    /// <summary>
    /// Scheduler Class for 
    /// </summary>
    public class Scheduler
    {
        //Declare a timer to hold the timer schedule
        public System.Timers.Timer schedular { get; set; } = new System.Timers.Timer();
        public Scheduler()
        {
        }
        /// <summary>
        /// Persisting XML after a specified time interval
        /// </summary>
        public void PersistXML(DBEngine<int, DBElement<int, string>> db)
        {
            try
            {
                schedular.Interval = 3000;  //Set timer to 3 seconds
                schedular.AutoReset = false;
                schedular.Enabled = true; // Anonymous function called after 3 sec
                schedular.Elapsed += (object source, ElapsedEventArgs e) => ScheduleXML(source, e, db);
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                throw new CustomException("error at scheduler persistxml", ex);
            }
        }
        /// <summary>
        /// Schedule XML persisting by calling the utlity function
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        static bool ScheduleXML(object source, ElapsedEventArgs e, DBEngine<int, DBElement<int, string>> db)
        {
            try
            {
                string error = "";
                string path = "";string xml = "";
                XMLUtility.XMLUtilitySpace.PersistToXML(db, out error, out path,out xml);
                error = "Success";
                Scheduler s = new Scheduler();
                s.SetResetOff(); //Disable the scheduler
                return true;
            }
            catch (Exception ex)
            {
                throw new CustomException("error at scheduler persistxml", ex);
            }
        }
        /// <summary>
        /// Scheduler is disabled
        /// </summary>
        /// <returns></returns>
        public bool SetResetOff()
        {
            schedular.Enabled = false;
            return true;
        }
#if (TEST_SCHEDULER)
        static void Main(string[] args)
        {
            Console.WriteLine("Demonstrate Timer - needed for scheduled persistance in Project #2");
            Console.Write("\n\n  press any key to exit\n");
            Console.WriteLine("..Showing the Scheduled Persistance Process..\n");
            Console.Write("..Wait for for 3 seconds..\n\n");
            Scheduler sch = new Scheduler();
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            sch.PersistXML(db);
        }
    }
#endif
}
