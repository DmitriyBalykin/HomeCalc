using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    public class VersionUpdater
    {
        private static string versionBinaryPath = @"http://www.homecalc.com.ua/distributives/";

        private static string versionBinaryFileName = @"HomeCalcUpdate.zip";


        private static Logger logger = LogService.GetLogger();

        public static void StartUpdate(Action successAction)
        {
            logger.Info("Starting update");

            var updateDirectoryPath = GetUpdateDirectory();
            var sourcePath = Path.Combine(versionBinaryPath, versionBinaryFileName);
            var destPath = Path.Combine(updateDirectoryPath, versionBinaryFileName);

            var taskCancellationTokenSource = new CancellationTokenSource();
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory
                .StartNew(() => Helpers.CleanDirectory(updateDirectoryPath, taskCancellationTokenSource), taskCancellationTokenSource.Token)
                .ContinueWith(task =>
                    Helpers.DownloadFile(sourcePath, destPath, taskCancellationTokenSource), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler)
                .ContinueWith(task =>
                    Helpers.UnpackFile(destPath, taskCancellationTokenSource), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler)
                .ContinueWith(task =>
                    RunUpdater(taskCancellationTokenSource), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler)
                .ContinueWith(task => successAction(), TaskContinuationOptions.OnlyOnRanToCompletion);
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
            if (MainAppRunning())
            {
                logger.Info("Cannot stop HomeCalc application, exiting...");
                return;
            }
            var updateFolder = AppDomain.CurrentDomain.BaseDirectory;
            var appFolder = Directory.GetDirectoryRoot(updateFolder);

            try
            {
                logger.Info("Starting application folder cleanup");
                Helpers.CleanDirectory(appFolder).Wait();
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
                Helpers.CopyAllFiles(updateFolder, appFolder).Wait();
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

        private static void RunUpdater(CancellationTokenSource tokenSource)
        {
            var updateDirectoryPath = GetUpdateDirectory();
            var updateExe = Path.Combine(updateDirectoryPath, "Updater.exe");

            if (!File.Exists(updateExe))
            {
                //throw new Exception("Updater executive file not found");
                tokenSource.Cancel();
            }

            Process.Start(updateExe, "update-app");
        }
        private static string GetUpdateDirectory()
        {
            var currentDirPath = AppDomain.CurrentDomain.BaseDirectory;

            return Path.Combine(currentDirPath, "Update");
        }
    }
}
