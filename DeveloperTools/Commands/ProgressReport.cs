using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperTools.Commands
{
    public class ProgressReport
    {
        public int CurrentProgressAmount { get; set; }
        public int TotalProgressAmount { get; set; }

        public string CurrentProgressMessage { get; set; }
    }
}
