using System;
using System.IO;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;
using Newtonsoft.Json;

namespace DeveloperTools
{
    class JSONExampleGenerator
    {
        public static void DoIt(string[] args)
        {
            try
            {
                if (Directory.Exists("output") == false)
                {
                    Directory.CreateDirectory("output");
                }
                WriteMenu();
                WriteOrderItem();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }

        private static void WriteMenu()
        {
            var menu = new FakeDomineesRestaurantServiceClient();
            var json = JsonConvert.SerializeObject(menu);

            File.WriteAllText("output/menu.json", json);
        }

        private static void WriteOrderItem()
        {
            var oi = new OrderItem
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                Revision = new EventID{AppInstanceCode = MiseAppTypes.DummyData, OrderingID = 100},
                MenuItem = new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "miTest",
                    DisplayWeight = 100,
                    Price = new Money(1.99M),
                }
            };

            var json = JsonConvert.SerializeObject(oi);
            File.WriteAllText("output/oi.json", json);
        }
    }
}
