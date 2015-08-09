using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.VendorManagement.Services.Implementation
{
    public abstract class BaseCsvWriter
    {

        protected TextWriter GetTextWriter(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            return File.CreateText(filename);
        }
    }
}
