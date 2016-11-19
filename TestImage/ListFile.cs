using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestImage
{
    class ListFile
    {
        public static IEnumerable<string> List(string parentFolder)
        {
            return Directory.GetFiles(parentFolder);
        }
    }
}
