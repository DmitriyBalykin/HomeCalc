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
        public static bool ImportDataFromFile(string folderPath)
        {
            string fileName = "Data.csv";
            string filePath = Path.Combine(folderPath, fileName);
            var content = File.ReadAllLines(filePath);



            return true;
        }
        public static bool FileExists(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
        }
    }
}
