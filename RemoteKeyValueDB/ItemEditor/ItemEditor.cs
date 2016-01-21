///////////////////////////////////////////////////////////////
// ItemEditor.cs -  ItemEditor for noSQL database    //
// Ver 1.0                                                   //
// Application: Project#2 :Key/Value Database                //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Y50, Core-i7, Windows 10              //
// Author:      Darshan Masti Prakash                        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements ItemEditor package used for Editing item and modifying item while updating
 */
/*

 *  Public Interface
 *  ----------------
 *  ItemEditor edit =new ItemEditor();
 *  edit.Create(item);
 * Maintenance:
 * ------------
 * Required Files: ExceptionLayer.cs, DBEngine.cs, DBElement.cs
 *
 * Build Process:  devenv KeyValueDB.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15 : This package implements ItemEditor package used for Editing item 
 */
using DBElementSpace;
using DBEngineSpace;
using ExceptionLayerSpace;
using ItemFactorySpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemEditorSpace
{
    /// <summary>
    /// Class for modification of Item and editing of DB Elements
    /// </summary>
    public class ItemModifier
    {
        private DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();

        /// <summary>
        /// Editing Data while updating.This returns element to be modified
        /// </summary>
        /// <param name="dbStore"></param>
        /// <param name="key"></param>
        /// <param name="element"></param>
        /// <param name="Operation"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool EditData(ref DBEngine<int, DBElement<int, string>> dbStore, int key, out DBElement<int, string> element, string Operation, out string error)
        {
            try
            {
                DBElement<int, string> elem = new DBElement<int, string>();
                error = "success"; bool success = true;
                success = dbStore.getValue(key, out element);
                if (success != true)
                {
                    error = "Key not found in DB";
                    return false;
                }
                elem = element;
                switch (Operation)
                {
                    case "EditName":        //Modify Name 
                        {
                            element.name = elem.name;
                            break;
                        }
                    case "EditDescription":      //Modify Description 
                        {
                            element.descr = elem.descr;
                            break;
                        }
                    case "EditPayload":         //Modify Payload 
                        {
                            element.payload = elem.payload;
                            break;
                        }
                    case "EditChildren":        //Modify Children 
                        {
                            element.children = new List<int>();
                            element.children = elem.children;
                            break;
                        }
                    case "EditKeyNotValid":         //Key editing is not valid
                        {
                            error = "Editing of Key is not allowed";
                            return false;
                        }
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new CustomException("error at itemeditor editdata", ex);
            }
        }

#if (TEST_ITEMEDITOR)
        static void Main(string[] args)
        {
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            DBElement<int, string> elem = new DBElement<int, string>();
            Console.WriteLine("Item editor testing");
            elem.name = "test";
            elem.payload = "sdsad";
            int key = 54;string error = "";
            ItemModifier mod = new ItemModifier();
            mod.EditData(ref db, key, out elem, "EditDescription", out error);
            Console.WriteLine("Item successfully edited for key {0}\n",key);
            
        }
    }
#endif
}
