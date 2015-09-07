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
        static string fileName = "Data.csv";
        public static bool ImportDataFromFile(string folderPath)
        {
            string filePath = Path.Combine(folderPath, fileName);
            if (!FileExists(folderPath))
            {
                logger.Warn("File with data absent at path: {0}", filePath);
                return false;
            }
            var content = File.ReadAllLines(filePath);



            return true;
        }
        public static bool FileExists(string folderPath)
        {
            return !string.IsNullOrEmpty(folderPath) && File.Exists(Path.Combine(folderPath, fileName));
        }
    }
}
