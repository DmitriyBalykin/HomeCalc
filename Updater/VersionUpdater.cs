using HomeCalc.Core.LogService;
using HomeCalc.Core.Services;
using HomeCalc.Core.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    public class VersionUpdater
    {
        //private static string versionBinaryPath = @"https://homecalc.file.core.windows.net/release/";

        //private static string versionBinaryFileName = @"HomeCalc.zip";


        private static Logger logger = LogService.GetLogger();
        private static StatusService status = StatusService.GetInstance();

        public static async Task StartUpdate(Action successAction)
        {
            logger.Info("Starting update");

            var settings = ConfigurationManager.GetSection(HomecalcApplicationSettingsSection.SectionName) as HomecalcApplicationSettingsSection;

            var updateDirectoryPath = GetUpdateDirectory();
            var sourcePath = Path.Combine(settings.Update.Folder, settings.Update.BinaryFileName);
            var destPath = Path.Combine(updateDirectoryPath, settings.Update.BinaryFileName);

            var taskCancellationTokenSource = new ReportingCancellationTokenSource("Update");
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Console.WriteLine("Waiting to enter for debug");
            Console.ReadLine();

            try
            {
                await Task.Factory
                .StartNew(() => Helpers.CreateDirectory(updateDirectoryPath, taskCancellationTokenSource), taskCancellationTokenSource.Token, TaskCreationOptions.None, taskScheduler)
                .ContinueWith(task =>
                    Helpers.CleanDirectory(updateDirectoryPath, taskCancellationTokenSource, null), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler)
                .ContinueWith(task =>
                    Helpers.DownloadFile(sourcePath, destPath, taskCancellationTokenSource), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler)
                .ContinueWith(task =>
                    Helpers.UnpackFile(destPath, taskCancellationTokenSource), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler)
                .ContinueWith(task =>
                    RunUpdater(taskCancellationTokenSource), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler)
                .ContinueWith(task => successAction(), taskCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Update operation was cancelled");
            }
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

            var cancellationTokenSource = new ReportingCancellationTokenSource();
            
            logger.Info("Starting application folder cleanup");
            Helpers.CleanDirectory(appFolder, cancellationTokenSource, new []{"Update", "Debug", "Release"});
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                var message = "Application folder cleanup failed. Exiting...";
                logger.Error(message);
                return;
            }

            logger.Info("Starting application files copying");
            Helpers.CopyAllFiles(updateFolder, appFolder, cancellationTokenSource, new List<string> { "HomeCalc.zip", "HomeCalcUpdate.zip" });
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                var message = "Application files copying failed. Exiting...";
                logger.Error(message);
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
