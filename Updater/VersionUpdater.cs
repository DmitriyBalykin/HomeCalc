using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    public class VersionUpdater
    {
        private static string versionBinaryPath = @"H:\HomeCalcUpdate\";

        private static Logger logger = LogService.GetLogger();

        private static object monitor = new object();

        public static string StartUpdate()
        {
            logger.Info("Starting update");

            string result = null;

            result = CopyFiles();

            result += RunUpdater();

            return result;
        }

        public static void UpdateApplication(bool startApp = true)
        {
            logger.Info("Starting application update");

            var retriesCount = 10;
            while (retriesCount > 0 && MainAppRunning())
            {
                retriesCount--;
                logger.Info("HomeCalc application still running, waiting 5 seconds");
                Thread.Sleep(5000);
            }

            var updateFolder = AppDomain.CurrentDomain.BaseDirectory;
            var appFolder = Directory.GetDirectoryRoot(updateFolder);

            try
            {
                logger.Info("Starting application folder cleanup");
                Directory.EnumerateFileSystemEntries(appFolder).ToList().ForEach(item => File.Delete(item));
            }
            catch (IOException ex)
            {
                logger.Error("Application folder cleanup failed. Exiting...");
                logger.Error(ex.Message);
                return;
            }

            try
            {
                logger.Info("Starting application files copying");
                CopyAllFiles(updateFolder, appFolder);
            }
            catch (IOException ex)
            {
                logger.Error("Application files copying failed. Exiting...");
                logger.Error(ex.Message);
                return;
            }

            if (startApp)
            {
                logger.Info("Starting application.");
                var appExePath = Path.Combine(appFolder, "HomeCalc.View.exe");
                Process.Start(appExePath);
            }
        }

        private static bool MainAppRunning()
        {
            var processes = Process.GetProcesses();

            return processes.Any(process => process.ProcessName.Contains("HomeCalc"));
        }

        private static string RunUpdater()
        {
            var updateDirectoryPath = GetUpdateDirectory();
            var updateExe = Path.Combine(updateDirectoryPath, "Updater.exe");

            if (!File.Exists(updateExe))
            {
                return "Updater executive file not found";
            }

            Process.Start(updateExe, "update-app");

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
                    CopyAllFiles(versionBinaryPath, updateDirectoryPath);
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

        private static void CopyAllFiles(string source, string destination)
        {
            var itemsToCopy = Directory.EnumerateFileSystemEntries(source);
            foreach (var itemPath in itemsToCopy)
            {
                var itemName = Path.GetFileName(itemPath);
                var destinationPath = Path.Combine(destination, itemName);
                File.Copy(itemPath, destinationPath);

            }
        }
    }
}
