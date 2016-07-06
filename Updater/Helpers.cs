using HomeCalc.Core.LogService;
using HomeCalc.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;

namespace Updater
{
    public class Helpers
    {
        private static Logger logger = LogService.GetLogger();
        private static StatusService status = StatusService.GetInstance();
        public static void CopyAllFiles(string source, string destination, ReportingCancellationTokenSource tokenSource, List<string> exclusions = null)
        {
            if (exclusions == null)
            {
                exclusions = new List<string>();
            }
            try
            {
                logger.Info(string.Format("Starting copying files from {0} to {1}", source, destination));

                var itemsToCopy = Directory.EnumerateFileSystemEntries(source);
                foreach (var itemPath in itemsToCopy)
                {
                    var itemName = Path.GetFileName(itemPath);

                    if (exclusions.Contains(itemName))
                    {
                        logger.Info("File {0} found in exclusion list, skipping", itemName);
                        continue;
                    }

                    var destinationItemPath = Path.Combine(destination, itemName);
                    logger.Info("Preparing to copy file {0} to {1}", itemPath, destinationItemPath);
                    if (File.Exists(destinationItemPath))
                    {
                        File.SetAttributes(destinationItemPath, FileAttributes.Normal);
                    }
                    var attributes = File.GetAttributes(itemPath);
                    if (attributes.HasFlag(FileAttributes.Directory))
                    {
                        logger.Info("Copying directory {0}", itemName);
                        Directory.CreateDirectory(destinationItemPath);

                        CopyAllFiles(itemPath, destinationItemPath, tokenSource);
                    }
                    else
                    {
                        logger.Info("Copying file {0}", itemName);
                        File.Copy(itemPath, destinationItemPath, true);
                    }
                }
            }
            catch (IOException ex)
            {
                var message = "File copying failed";
                logger.Error(message);
                logger.Error(ex.Message);
                CancelTask(tokenSource);
            }
            logger.Info(string.Format("File copying finished succesfully"));
        }

        public static void CleanDirectory(string path, ReportingCancellationTokenSource tokenSource, string[] exceptions = null)
        {
            try
            {
                var message = "";
                if (exceptions == null)
                {
                    message = string.Format("Starting folder {0} cleanup", path);
                }
                else
                {
                    message = string.Format("Starting folder {0} cleanup, exceptions: {1}", path, string.Join("; ", exceptions));
                }
                logger.Info(message);
                if (!Directory.Exists(path))
                {
                    logger.Info("Cleanup folder {0} missing, exiting cleanup", path);
                }
                Directory.EnumerateFileSystemEntries(path).ToList().ForEach(item => 
                {
                    var fileName = Path.GetFileName(item);
                    logger.Info("***********************");
                    logger.Info("Preparing to delete {0}", fileName);
                    if (exceptions != null)
                    {
                        if (exceptions.Contains(fileName))
                        {
                            logger.Info("Skipping {0} as exception", fileName);
                            return;
                        }
                        else
                        {
                            logger.Info("File {0} not found in exceptions \"{1}\", deleting", fileName, string.Join("; ", exceptions));
                        }
                    }
                    
                    var fileAttributes = File.GetAttributes(item);
                    File.SetAttributes(item, FileAttributes.Normal);
                    if (fileAttributes.HasFlag(FileAttributes.Directory))
                    {
                        logger.Info("Deleting folder {0}", item);
                        Directory.Delete(item, true);
                    }
                    else
                    {
                        logger.Info("Deleting file {0}", item);
                        File.Delete(item); 
                    }
                });
            }
            catch (IOException ex)
            {
                var message = "Directory cleanup failed";
                logger.Error(message);
                logger.Error(ex.Message);
                CancelTask(tokenSource);
            }
            catch (UnauthorizedAccessException ex)
            {
                var message = "Directory cleanup failed: you have no access permission";
                logger.Error(message);
                logger.Error(ex.Message);
                CancelTask(tokenSource);
            }
            logger.Info(string.Format("Directory cleanup finished succesfully"));
        }

        public static void DownloadFile(string source, string destination, ReportingCancellationTokenSource tokenSource)
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
                CancelTask(tokenSource);
            }
            catch (WebException ex)
            {
                //status.StopProgress();
                var message = "File download failed";
                logger.Error(message);
                logger.Error(ex.Message);
                CancelTask(tokenSource);
            }
            finally
            {
                status.StopProgress();
            }
            logger.Info(string.Format("File download finished succesfully"));
        }
        public static void UnpackFile(string filePath, ReportingCancellationTokenSource tokenSource, string destination = null)
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
                CancelTask(tokenSource);
            }
            logger.Info(string.Format("File unpack finished succesfully"));
        }

        internal static void CreateDirectory(string destination, ReportingCancellationTokenSource taskCancellationTokenSource)
        {
            try
            {
                if (destination == null)
                {
                    throw new IOException("Directory path cannot be null");
                }
                if (Directory.Exists(destination))
                {
                    logger.Info(string.Format("Update directory with path {0} exists", destination));
                    return;
                }
                logger.Info(string.Format("Creating directory with path {0}", destination));

                Directory.CreateDirectory(destination);
            }
            catch (IOException ex)
            {
                var message = string.Format("Cannot create directory on a path {0}", destination);
                logger.Error(message);
                logger.Error(ex.Message);
                CancelTask(taskCancellationTokenSource);
            }
            logger.Info(string.Format("Directory creation finished succesfully"));
        }

        private static void CancelTask(CancellationTokenSource cts)
        {
            try
            {
                logger.Info("OperationCanceledException is initiated");
                if (cts != null)
                {
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
            catch (OperationCanceledException)
            {
                logger.Info("Handling OperationCanceledException");
            }
        }
    }
}
