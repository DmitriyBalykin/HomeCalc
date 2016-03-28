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
            else if(args[0] == "start")
            {
                RunUpdate();
            }
        }

        private static void RunUpdate()
        {
            //wait until main application exits
            //clear main application folder
            //copy files to main application folder
            //start main application
            //exit
        }
    }
}
