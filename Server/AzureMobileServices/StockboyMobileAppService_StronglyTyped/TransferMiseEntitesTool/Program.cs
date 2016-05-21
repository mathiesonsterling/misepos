using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferMiseEntitesTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var tranfer = new EntityDBTransferer();
            var task = tranfer.TransferRecords();

            try
            {
                task.Wait(new TimeSpan(0, 1, 0, 0));
                Console.WriteLine("All entities should now be completed!");
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Console.WriteLine("Error while writing items - " + e.Message + "::" + e.StackTrace);
            }

            Console.ReadLine();
        }
    }
}
