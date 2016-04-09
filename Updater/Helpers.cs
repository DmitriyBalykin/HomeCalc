using HomeCalc.Core;
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
        private static StatusService status = StatusService.GetInstance();
        public static void CopyAllFiles(string source, string destination, ReportingCancellationTokenSource tokenSource = null)
        {
            try
            {
                logger.Info(string.Format("Starting copying files from {0} to {1}", source, destination));

                var itemsToCopy = Directory.EnumerateFileSystemEntries(source);
                foreach (var itemPath in itemsToCopy)
                {
                    var itemName = Path.GetFileName(itemPath);
                    var destinationItemPath = Path.Combine(destination, itemName);
                    if (File.Exists(destinationItemPath))
                    {
                        File.SetAttributes(destinationItemPath, FileAttributes.Normal);
                    }
                    var attributes = File.GetAttributes(itemName);
                    if (attributes.HasFlag(FileAttributes.Directory))
                    {
                        Directory.CreateDirectory(destinationItemPath);
                        CopyAllFiles(itemName, destinationItemPath, tokenSource);
                    }
                    else
                    {
                        File.Copy(itemPath, destinationItemPath, true);
                    }
                    //throw new IOException("test io exception copy files");
                }
            }
            catch (IOException ex)
            {
                var message = "File copying failed";
                logger.Error(message);
                logger.Error(ex.Message);
                if (tokenSource != null)
                {
                    tokenSource.Cancel(message);
                }
                return;
            }
            logger.Info(string.Format("File copying finished succesfully"));
        }

        public static void CleanDirectory(string path, string[] exceptions = null, ReportingCancellationTokenSource tokenSource = null)
        {
            try
            {
                logger.Info(string.Format("Starting folder {0} cleanup", path));
                Directory.EnumerateFileSystemEntries(path).ToList().ForEach(item => 
                {
                    var fileName = Path.GetFileName(item);
                    if (exceptions != null && exceptions.Contains(fileName))
                    {
                        return;
                    }
                    var fileAttributes = File.GetAttributes(item);
                    File.SetAttributes(item, FileAttributes.Normal);
                    if (fileAttributes.HasFlag(FileAttributes.Directory))
                    {
                        Directory.Delete(item, true);
                    }
                    else
                    {
                        File.Delete(item); 
                    }
                });
            }
            catch (IOException ex)
            {
                var message = "Directory cleanup failed";
                logger.Error(message);
                logger.Error(ex.Message);
                tokenSource.Cancel(message);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                var message = "Directory cleanup failed: you have no access permission";
                logger.Error(message);
                logger.Error(ex.Message);
                if (tokenSource != null)
                {
                    tokenSource.Cancel(message);
                }
                return;
            }
            logger.Info(string.Format("Directory cleanup finished succesfully"));
        }

        public static void DownloadFile(string source, string destination, ReportingCancellationTokenSource tokenSource = null)
        {
            try
            {
                logger.Info(string.Format("Starting file download from URL: {0} to directory: {1}", source, destination));

                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }
                var webClient = new WebClient();
                status.Post("Завантаження оновлення триває");
                //status.StartProgress();
                //webClient.DownloadProgressChanged += (o, e) => 
                //{
                //    status.UpdateProgress(e.ProgressPercentage); 
                //};
                webClient.DownloadFile(source, destination);
            }
            catch (IOException ex)
            {
                //status.StopProgress();
                var message = string.Format("Cannot download file to path {0} : file exists and cannot be deleted", destination);
                logger.Error(message);
                logger.Error(ex.Message);
                tokenSource.Cancel(message);
                return;
            }
            catch (WebException ex)
            {
                //status.StopProgress();
                var message = "File download failed";
                logger.Error(message);
                logger.Error(ex.Message);
                if (tokenSource != null)
                {
                    tokenSource.Cancel(message);
                }
                return;
            }
            status.StopProgress();
            logger.Info(string.Format("File download finished succesfully"));
        }
        public static void UnpackFile(string filePath, ReportingCancellationTokenSource tokenSource = null, string destination = null)
        {
            try
            {
                if (destination == null)
                {
                    destination = Directory.GetParent(filePath).FullName;
                }
                logger.Info(string.Format("Starting file {0} unpack to path {1}", filePath, destination));

                ZipFile.ExtractToDirectory(filePath, destination);
            }
            catch (IOException ex)
            {
                var message = string.Format("Cannot unpack file on a path {0}", filePath);
                logger.Error(message);
                logger.Error(ex.Message);
                if (tokenSource != null)
                {
                    tokenSource.Cancel(message);
                }
                return;
            }
            logger.Info(string.Format("File unpack finished succesfully"));
        }
    }
}
