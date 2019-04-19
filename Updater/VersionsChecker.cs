using HomeCalc.Core.Settings;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Updater
{
    public static class VersionChecker
    {
        // private static string versionCheckPath = @"http://www.homecalc.com.ua/distributives/";
        // private static string versionCheckFilename = @"versionInfo.txt";

        public static Task<VersionsInformation> GetUpdatesInformation(bool compareVersion)
        {
            var settings = ConfigurationManager.GetSection(HomecalcApplicationSettingsSection.SectionName) as HomecalcApplicationSettingsSection;

            var webclient = new WebClient();

            var source = Path.Combine(settings.Update.Folder, settings.Update.VersionFileName);
            var destination = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settings.Update.VersionFileName);

            try
            {
                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }
                return webclient.DownloadFileTaskAsync(source, destination).ContinueWith(result => ReadInformation(destination, compareVersion), TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch(IOException exIo)
            {
                throw exIo;
            }
            catch (WebException exWeb)
            {
                throw exWeb;
            }
        }

        private static VersionsInformation ReadInformation(string filePath, bool compareVersion)
        {
            var fileContent = File.ReadAllText(filePath);
            var versionBlocks = fileContent.Split(new []{'{','}'}).Where(block => !string.IsNullOrEmpty(block)).ToList();

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var versionInfo = new VersionsInformation();
            foreach (var block in versionBlocks)
            {
                var blockParts = block.Split(new []{"::"}, StringSplitOptions.RemoveEmptyEntries);
                if (blockParts.Length == 2)
                {
                    var version = parseVersion(blockParts[0]);
                    if (compareVersion && version <= currentVersion)
                    {
                        continue;
                    }

                    var info = blockParts[1];
                    versionInfo.SetLatestVersionIfEmpty(version);
                    versionInfo.Add(version, info);
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
