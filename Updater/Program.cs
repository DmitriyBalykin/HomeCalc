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
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                try
                {
                    VersionChecker.GetUpdatesInformation().ContinueWith(task => 
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
                        Console.WriteLine(result);
                        Console.WriteLine("Press Enter to exit.");
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                    
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Error occured during update download.");
                    Console.WriteLine(ex.Message);
                }

                Console.ReadLine();
            }
            else
            {
                switch (args[0])
                {
                    case "update-start":
                        VersionUpdater.StartUpdate(() => { });
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
