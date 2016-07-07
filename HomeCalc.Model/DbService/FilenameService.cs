using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    public class FilenameService
    {
#if DEBUG
        public const string DB_FILE_NAME = "DataStorage_debug.sqlite";
#else
        public const string DB_FILE_NAME = "DataStorage.sqlite";
#endif
        public const string DB_FOLDER = "HomeCalc";

        public static string GetDBPath()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var containerFolderPath = Path.Combine(appDataFolder, FilenameService.DB_FOLDER);

            var fullDbFilePath = Path.Combine(containerFolderPath, FilenameService.DB_FILE_NAME);
            return fullDbFilePath;
        }
    }
}
