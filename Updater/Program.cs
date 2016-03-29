using System;
using System.Collections.Generic;
using System.Linq;
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
                var info = VersionChecker.GetUpdatesInformation();
                var result = string.Empty;
                if (info.LatestVersion == null)
                {
                    result = "No updates found";
                }
                else
                {
                    result = "Found newer version: " + info.LatestVersion;
                }
                Console.WriteLine(result);
                Console.WriteLine("Press Enter to exit.");


                Console.ReadLine();
            }
            else
            {
                switch (args[0])
                {
                    case "update-start":
                        VersionUpdater.StartUpdate();
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
