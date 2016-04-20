using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public static class VersionChecker
    {
        private static string versionCheckPath = @"http://www.homecalc.com.ua/distributives/";
        private static string versionCheckFilename = @"versionInfo.txt";

        public static Task<VersionsInformation> GetUpdatesInformation()
        {
            var webclient = new WebClient();

            var source = Path.Combine(versionCheckPath, versionCheckFilename);
            var destination = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, versionCheckFilename);

            try
            {
                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }
                return webclient.DownloadFileTaskAsync(source, destination).ContinueWith(result => ReadInformation(destination), TaskContinuationOptions.OnlyOnRanToCompletion);
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

        private static VersionsInformation ReadInformation(string filePath)
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
