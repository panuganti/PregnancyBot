using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorage;

namespace TestGoogleDatastore
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = GoogleCloud.GetInstance("ElectionsSPA", "TestCloudStorage");
            bool yesno = store.FileExistsAsync("allACs.json", "json").Result;
            Console.WriteLine(yesno);
            Console.ReadKey();
        }
    }
}
