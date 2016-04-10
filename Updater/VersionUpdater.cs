using HomeCalc.Core;
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

        private static string versionBinaryFileName = @"HomeCalc.zip";


        private static Logger logger = LogService.GetLogger();
        private static StatusService status = StatusService.GetInstance();

        public static void StartUpdate(Action successAction)
        {
            logger.Info("Starting update");

            var updateDirectoryPath = GetUpdateDirectory();
            var sourcePath = Path.Combine(versionBinaryPath, versionBinaryFileName);
            var destPath = Path.Combine(updateDirectoryPath, versionBinaryFileName);

            var taskCancellationTokenSource = new ReportingCancellationTokenSource("Update");
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            
            Task.Factory
                .StartNew(() => Helpers.CreateDirectory(updateDirectoryPath, taskCancellationTokenSource), taskCancellationTokenSource.Token)
                .ContinueWith(task =>
                    Helpers.CleanDirectory(updateDirectoryPath, null, taskCancellationTokenSource), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler)
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
            logger.Info("Updater: Starting application update");

            var retriesCount = 10;
            while (retriesCount > 0 && MainAppRunning())
            {
                retriesCount--;
                var message = "HomeCalc application still running, waiting 5 seconds";
                logger.Info(message);
                Thread.Sleep(5000);
            }
            if (MainAppRunning())
            {
                logger.Info("Cannot stop HomeCalc application, exiting...");
                return;
            }
            var updateFolder = AppDomain.CurrentDomain.BaseDirectory.Trim(Path.DirectorySeparatorChar);
            var appFolder = Directory.GetParent(updateFolder).FullName;
            logger.Info("Current updater application running directory: {0}", updateFolder);
            logger.Info("Current updater parent directory: {0}", appFolder);
            try
            {
                logger.Info("Starting application folder cleanup");
                Helpers.CleanDirectory(appFolder, new []{"Update", "Debug", "Release"});
            }
            catch (IOException ex)
            {
                var message = "Application folder cleanup failed. Exiting...";
                logger.Error(message);
                logger.Error(ex.Message);
                return;
            }

            try
            {
                logger.Info("Starting application files copying");
                Helpers.CopyAllFiles(updateFolder, appFolder);
            }
            catch (IOException ex)
            {
                logger.Error("Application files copying failed. Exiting...");
                logger.Error(ex.Message);
                return;
            }

            if (startApp)
            {                
                var appExePath = Path.Combine(appFolder, "HomeCalc.View.exe");
                logger.Info("Starting application: {0}", appExePath);
                try
                {
                    Process.Start(appExePath);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }

        private static bool MainAppRunning()
        {
            var processes = Process.GetProcesses();

            return processes.Any(process => process.ProcessName.Equals("HomeCalc.View")) || processes.Any(process => process.ProcessName.Equals("HomeCalc.View.vshost"));
        }

        private static void RunUpdater(CancellationTokenSource tokenSource)
        {
            var updateDirectoryPath = GetUpdateDirectory();
            var updateExe = Path.Combine(updateDirectoryPath, "Updater.exe");
            logger.Info("Starting Updater application");
            if (!File.Exists(updateExe))
            {
                //throw new Exception("Updater executive file not found");
                tokenSource.Cancel();
                status.Post("Помилка запуску оновлення");
                return;
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
