///////////////////////////////////////////////////////////////
// GenerateException.cs-Generate exception for noSQL database //
// Ver 1.0                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements exception handling to be used by all other 
 * packages for throwing CustomExcecption
 *
 * Note: This package defines exception handling to be used by all other 
 * packages for throwing CustomExcecption
 */
/*
 *  Public Interface
 *  ----------------
 *  CustomException  x = new CustomException();
 *  x.Message();
 * Maintenance:
 * ------------
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15 : This package implements exception handling to be used by all other 
 * packages for throwing CustomExcecption
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionLayerSpace
{
    /// <summary>
    /// Custom exception class to be implemented by inheriting Exception class
    /// </summary>
    public class CustomException : Exception
    {
        /// <summary>
        /// Default Exception raised
        /// </summary>
        public CustomException()
        {

        }
        /// <summary>
        /// Custom message raised
        /// </summary>
        /// <param name="message"></param>
        public CustomException(string message) : base(message)
        {

        }
        /// <summary>
        /// Custom message raised and the base exception message raised
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public CustomException(string message, Exception ex) : base(message, ex)
        {

        }
    }
#if (TEST_EXCEPTION)
    public class GenerateException
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Showing the exception raised\n");
                throw new CustomException("Test exception");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
#endif
}
