using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PregnancyLibrary;

namespace PersistentMenuHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            // Add
            PregnancyLibrary.PersistentMenuHandler.AddPersistentMenuAsync().Wait();
            // Delete
            Console.ReadKey();
            PregnancyLibrary.PersistentMenuHandler.DeletePersistentMenuAsync().Wait();
        }
    }
}
