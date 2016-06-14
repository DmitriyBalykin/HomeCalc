using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    class Program
    {
        private static Logger logger = LogService.GetLogger();

        static void Main(string[] args)
        {
            logger.Info("start arguments: " + string.Join(" ", args));
            if (args.Length == 0)
            {
                try
                {
                    VersionChecker.GetUpdatesInformation(true).ContinueWith(task => 
                    {
                        var result = string.Empty;
                        var verInfo = task.Result;
                        if (verInfo.LatestVersion == null)
                        {
                            result = "No updates found";
                        }
                        else
                        {
                            result = "Found newer version: " + verInfo.LatestVersion;
                        }
                        logger.Info(result);
                        logger.Info("Press Enter to exit.");
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                    
                }
                catch (WebException ex)
                {
                    logger.Info("Error occured during update download.");
                    logger.Info(ex.Message);
                }

                Console.ReadLine();
            }
            else
            {
                switch (args[0])
                {
                    case "update-start":
                        Task.Factory.StartNew(async () => await VersionUpdater.StartUpdate(() => { }));
                        break;
                    case "update-app":
                        VersionUpdater.UpdateApplication();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
