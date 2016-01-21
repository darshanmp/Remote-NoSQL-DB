///////////////////////////////////////////////////////////////
// ItemFactory.cs -  ItemFactory element for noSQL database  //
// Ver 1.0                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements Item Factory which is used for generating item and creating item through factory methods
 *
 * Note: This package defines the generation of item using factory method   
 */
/*
 *  Public Interface
 *  ----------------
 *  ItemFactory edit =new ItemFactory();
 *  edit.Create(item);
 * Maintenance:
 * ------------
 * Required Files: DBElement.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15 : This package implements Item Factory which is used for generating item and creating item through factory methods
 */
using DBElementSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemFactorySpace
{
    /// <summary>
    /// Static Class for Item Factory generation
    /// </summary>
    public static class ItemFactory
    {
        /// <summary>
        /// Generate item of DBElement class
        /// </summary>
        /// <returns></returns>
        public static DBElement<int, string> GenerateItem()
        {
            DBElement<int, string> elem = new DBElement<int, string>();
            return elem;
        }
    }
    /// <summary>
    /// Item Generator class for generating of Items for int and string Key/Value DB
    /// </summary>
    public class ItemGenerator
    {

        private static int key = 0;
        Func<int> keyGen = () => { ++key; return key; };
        private static int seed = 0;
        private static string skey = "";
        Func<string> skeyGen = () =>
        {
            ++seed;
            skey = seed.ToString();
            skey = "string" + seed.ToString();
            skey = skey.GetHashCode().ToString();
            return skey;
        };

        //Start loading of  Integer and String Database
        /// <summary>
        /// Generate Item1 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item1(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2014 Mercedes";
            elem.descr = "2014 Mercedes new model";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { keyGen(), 15, 13 });
            elem.payload = "Make:Mercedes;Model:Mercedes AA;Color:Red;Year:2014;";
            key = keyGen();
            return elem;
        }

        /// <summary>
        /// Generate Item2 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item2(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2015 Ferrari FF";
            elem.descr = "Ferrari FF new model 2015";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1 });
            elem.payload = "Make:Ferrari;Model:Ferrari FF;Color:Blue;Year:2015;";
            key = keyGen();
            return elem;
        }

        /// <summary>
        /// Generate Item3 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item3(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2015 Chevrolet";
            elem.descr = "2015 Chevrolet is the recent model";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2 });
            elem.payload = "Make:Chevrolet;Model:Chevrolet 2015;Color:Yellow;Year:2015;";
            key = keyGen();
            return elem;
        }

        /// <summary>
        /// Generate Item4 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item4(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2015 Jaguar XK";
            elem.descr = "2015 Jaguar XK New Model";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3 });
            elem.payload = "Make:Jaguar;Model:Jaguar XK;Color:Brown;Year:2015;";
            key = keyGen();
            return elem;
        }

        /// <summary>
        /// Generate Item5 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item5(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2015 Maserati GranTurismo";
            elem.descr = "Ferrari FF new model 2015";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3, 4 });
            elem.payload = "Make:Maserati;Model:Maserati GranTurismo;Color:Green;Year:2015;";
            key = 54;
            return elem;
        }

        /// <summary>
        /// Generate Item6 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item6(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2014 Mercedes";
            elem.descr = "2014 Mercedes new model";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3, 4, 5 });
            elem.payload = "Make:Mercedes;Model:Mercedes AA;Color:Red;Year:2014;";
            key = keyGen();
            return elem;
        }

        /// <summary>
        /// Generate Item7 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item7(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2013 Mercedes";
            elem.descr = "2013 Mercedes old model";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3, 4, 5, 6 });
            elem.payload = "Make:Mercedes12;Model:Mercedes AAB;Color:Red;Year:2013;";
            key = 56;
            return elem;
        }

        /// <summary>
        /// Generate Item8 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item8(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2012 Mercedes";
            elem.descr = "2012 Mercedes new model 2012";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3 });
            elem.payload = "Make:Mercedes13;Model:Mercedes A;Color:Red;Year:2012;";
            key = keyGen();
            return elem;
        }

        /// <summary>
        /// Generate Item9 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item9(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2011 Mercedes";
            elem.descr = "2011 Mercedes new model";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 6, 7, 8 });
            elem.payload = "Make:Mercedes14;Model:Mercedes BB;Color:Green;Year:2011;";
            key = keyGen();
            return elem;
        }

        /// <summary>
        /// Generate Item10 for int Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<int, string> Item10(out int key)
        {
            DBElement<int, string> elem = ItemFactory.GenerateItem();
            elem.name = "2009 Jaguar CD";
            elem.descr = "2009 Jaguar CD New Model";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 9 });
            elem.payload = "Make:Jaguar56;Model:Jaguar CD;Color:Gray;Year:2009;";
            key = keyGen();
            return elem;
        }

        /// <summary>
        /// Generate Item20 for string Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<string, string> Item20(out string key)
        {
            DBElement<string, string> elem = new DBElement<string, string>();
            elem.name = "Ducati";
            elem.descr = "Newest Bike in the market";
            elem.payload = "version one";
            key = "Bike_01";
            return elem;
        }

        /// <summary>
        /// Generate Item21 for string Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<string, string> Item21(out string key)
        {
            DBElement<string, string> elem = new DBElement<string, string>();
            elem.name = "Hayabusa";
            elem.descr = "Fastest Bike";
            elem.children = new List<string> { "a", "b" };
            elem.payload = "2011";
            key = "Bike_02";
            return elem;
        }

        /// <summary>
        /// Generate Item22 for string Key/Value DB
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DBElement<string, string> Item22(out string key)
        {
            DBElement<string, string> elem = new DBElement<string, string>();
            elem.name = "BMW Bike";
            elem.descr = "Classy bike";
            elem.children.AddRange(new List<string> { "a", "b", "c" });
            elem.payload = "2014";
            key = "Bike_03";
            return elem;
        }
    }

#if (TEST_ITEMFACTORY)

    class TestItemFactory
    {
        static void Main(string[] args)
        {
            Console.Write("\n  All testing of ItemFactory class ");
            Console.Write("\n\n");
            DBElement<int, string> elem = new DBElement<int, string>();
            ItemGenerator itemGen = new ItemGenerator();
            int key = 0;
            elem = itemGen.Item2(out key);  //Generate item2
            Console.WriteLine("Element inserted successfully\n");
            Console.WriteLine("Children is {0} ,{1} ,{2}, {3} ,{4} \n",elem.children[0],elem.descr,elem.name,elem.payload,elem.timeStamp);
        }
    }
#endif
}
