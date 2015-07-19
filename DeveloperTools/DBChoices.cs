using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeveloperTools
{
    class DBChoices
    {
        public static Dictionary<string, string> DBNamesAndLocations
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Dev", "http://miseinventorydevbasic:4msw4Yy1TKRMObPudg2n@miseinventorydevbasic.sb05.stations.graphenedb.com:24789/db/data/"},
                    {"QA", "http://miseinventoryqa:yGudYbqPNn0t5fqO41Ns@miseinventoryqa.sb07.stations.graphenedb.com:24789/db/data/"} ,
                    {"Production", "http://miseinventoryprod:x3MPvwDUdKpd9aPU1EfN@miseinventoryprod.sb07.stations.graphenedb.com:24789/db/data/"}
                };
            }
        } 
        public static void LoadComboBox(ComboBox box)
        {
            box.Items.Clear();
            foreach (var kv in DBNamesAndLocations)
            {
                box.Items.Add(kv);
            }
            box.SelectedIndex = 0;
        }
    }
}
