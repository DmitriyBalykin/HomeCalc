using System;
using System.IO;

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
            var containerFolderPath = Path.Combine(appDataFolder, DB_FOLDER);

            var fullDbFilePath = Path.Combine(containerFolderPath, DB_FILE_NAME);
            return fullDbFilePath;
        }
    }
}
