using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public class VersionUpdater
    {
        private static string versionBinaryPath = @"H:\HomeCalcUpdate\";

        private static object monitor = new object();

        public static string Update()
        {
            string result = null;

            result = CopyFiles();

            result += RunUpdater();

            return result;
        }

        private static string RunUpdater()
        {
            var updateDirectoryPath = GetUpdateDirectory();
            var updateExe = Path.Combine(updateDirectoryPath, "Updater.exe");

            if (!File.Exists(updateExe))
            {
                return "Updater executive file not found";
            }

            Process.Start(updateExe);

            return null;
        }

        private static string CopyFiles()
        {
            var updateDirectoryPath = GetUpdateDirectory();

            lock (monitor)
            {
                if (!Directory.Exists(versionBinaryPath) || Directory.EnumerateFiles(versionBinaryPath).Count() == 0)
                {
                    return "Update sourcedirectory is empty";
                }
                if (!Directory.Exists(updateDirectoryPath))
                {
                    try
                    {
                        Directory.CreateDirectory(updateDirectoryPath);
                    }
                    catch (IOException ex)
                    {
                        return ex.Message;
                    }
                }
                else
                {
                    try
                    {
                        Directory.EnumerateFiles(updateDirectoryPath).ToList().ForEach(filePath => File.Delete(filePath));
                    }
                    catch (IOException ex)
                    {
                        return ex.Message;
                    }
                }

                try
                {
                    var itemsToCopy = Directory.EnumerateFileSystemEntries(versionBinaryPath);
                    foreach (var itemPath in itemsToCopy)
                    {
                        var itemName = Path.GetFileName(itemPath);
                        var destinationPath = Path.Combine(updateDirectoryPath, itemName);
                        File.Copy(itemPath, destinationPath);

                    }
                }
                catch (IOException ex)
                {
                    return ex.Message;
                }
            }

            return null;
        }

        private static string GetUpdateDirectory()
        {
            var currentDirPath = AppDomain.CurrentDomain.BaseDirectory;

            return Path.Combine(currentDirPath, "Update");
        }
    }
}
