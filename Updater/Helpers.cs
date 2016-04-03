using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    public class Helpers
    {
        private static Logger logger = LogService.GetLogger();
        public static Task CopyAllFiles(string source, string destination, CancellationTokenSource tokenSource = null)
        {
            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();
            }
            return Task.Factory.StartNew(() => 
            {
                try
                {
                    logger.Info(string.Format("Starting copying files from {0} to {1}", source, destination));

                    var itemsToCopy = Directory.EnumerateFileSystemEntries(source);
                    foreach (var itemPath in itemsToCopy)
                    {
                        var itemName = Path.GetFileName(itemPath);
                        var destinationPath = Path.Combine(destination, itemName);
                        File.Copy(itemPath, destinationPath);
                    }
                }
                catch (IOException ex)
                {
                    logger.Error("File copying failed");
                    logger.Error(ex.Message);
                    tokenSource.Cancel();
                    return;
                }
                logger.Info(string.Format("File copying finished succesfully"));
            }, tokenSource.Token);
        }

        public static Task CleanDirectory(string path, CancellationTokenSource tokenSource = null)
        {
            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();
            }
            return Task.Factory.StartNew(() => 
            {
                try
                {
                    logger.Info(string.Format("Starting folder {0} cleanup", path));
                    Directory.EnumerateFileSystemEntries(path).ToList().ForEach(item => File.Delete(item));
                }
                catch (IOException ex)
                {
                    logger.Error("Directory cleanup failed");
                    logger.Error(ex.Message);
                    tokenSource.Cancel();
                    return;
                }
                logger.Info(string.Format("Directory cleanup finished succesfully"));
            }, tokenSource.Token);
        }

        public static Task DownloadFile(string source, string destination, CancellationTokenSource tokenSource = null)
        {
            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();
            }
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    logger.Info(string.Format("Starting file download from URL: {0} to directory: {1}", source, destination));

                    if (File.Exists(destination))
                    {
                        File.Delete(destination);
                    }
                    var webClient = new WebClient();
                    webClient.DownloadFile(source, destination);
                }
                catch (IOException ex)
                {
                    logger.Error(string.Format("Cannot download file to path {0} : file exists and cannot be deleted", destination));
                    logger.Error(ex.Message);
                    tokenSource.Cancel();
                    return;
                }
                catch(WebException ex)
                {
                    logger.Error("File download failed");
                    logger.Error(ex.Message);
                    tokenSource.Cancel();
                    return;
                }
                logger.Info(string.Format("File download finished succesfully"));
            }, tokenSource.Token);
        }
        public static Task UnpackFile(string filePath, CancellationTokenSource tokenSource = null, string destination = null)
        {
            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();
            }
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (destination == null)
                    {
                        destination = Directory.GetDirectoryRoot(filePath);
                    }
                    logger.Info(string.Format("Starting file {0} unpack to path {1}", filePath, destination));

                    ZipFile.ExtractToDirectory(filePath, destination);
                }
                catch (IOException ex)
                {
                    logger.Error(string.Format("Cannot unpack file on a path {0}", filePath));
                    logger.Error(ex.Message);
                    tokenSource.Cancel();
                    return;
                }
                logger.Info(string.Format("File unpack finished succesfully"));
            }, tokenSource.Token);
        }
    }
}
