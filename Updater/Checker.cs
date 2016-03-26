using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public static class Checker
    {
        private static string versionCheckPath = @"H:\workspace\HomeCalc\versionInfo.txt";
        public static VersionsInformation GetUpdatesInformation()
        {
            var fileContent = File.ReadAllText(versionCheckPath);
            var versionBlocks = fileContent.Split(new []{'{','}'}).Where(block => !string.IsNullOrEmpty(block)).ToList();

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var versionInfo = new VersionsInformation();
            foreach (var block in versionBlocks)
            {
                var blockParts = block.Split(',');
                if (blockParts.Length == 2)
                {
                    var version = parseVersion(blockParts[0]);
                    if (version > currentVersion)
                    {
                        var info = blockParts[1];
                        versionInfo.SetLatestVersionIfEmpty(version);
                        versionInfo.Add(version, info);
                    }
                }
            }
            return versionInfo;
        }
        private static Version parseVersion(string versionStr)
        {
            try
            {
                return new Version(versionStr);
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
