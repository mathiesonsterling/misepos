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
                task.Wait();
            }
            catch (Exception e)
            {
                var msg = e.Message;
                throw;
            }
        }
    }
}
