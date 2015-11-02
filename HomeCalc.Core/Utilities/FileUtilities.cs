using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Utilities
{
    public class FileUtilities
    {
        private static Logger logger = LogService.LogService.GetLogger();
        public static string HOMEEX_DATA_FILE = "homeexData.csv";
        public static bool FileExists(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
        }
    }
}
