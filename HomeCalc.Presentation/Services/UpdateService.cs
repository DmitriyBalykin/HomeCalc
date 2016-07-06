﻿using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Updater;

namespace HomeCalc.Core.Services
{
    public class UpdateService
    {
        private static UpdateService instance;
        private Logger logger;
        public event EventHandler UpdatesAvailableEvent;
        private UpdateService()
        {
            logger = LogService.LogService.GetLogger();
        }
        public static UpdateService GetInstance()
        {
            if (instance == null)
            {
                instance = new UpdateService();
            }
            return instance;
        }

        public string GetUpdatesInformation()
        {
            string result = "";
            try
            {
                VersionChecker.GetUpdatesInformation(true).ContinueWith(task =>
                {
                    var updatesInfo = task.Result;
                    if (!updatesInfo.HasNewVersion)
                    {
                        result = "Версія програми є найновішою.";
                    }
                    else
                    {
                        var sb = new StringBuilder();

                        foreach (var updateVersion in updatesInfo.ChangesByVersions.Keys)
                        {
                            sb.AppendLine(updateVersion.ToString());
                            sb.AppendLine(updatesInfo.ChangesByVersions[updateVersion]);
                            sb.AppendLine();
                        }
                        result = sb.ToString();

                        if (UpdatesAvailableEvent != null)
                        {
                            UpdatesAvailableEvent(null, EventArgs.Empty);
                        }
                    }
                    
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (WebException ex)
            {
                logger.Error("Error occured during update download");
                logger.Error(ex.Message);
            }
            return result;
        }

        public string GetUpdatesHistory()
        {
            string result = "";
            try
            {
                VersionChecker.GetUpdatesInformation(false).ContinueWith(task =>
                {
                    var updatesInfo = task.Result;
                    if (updatesInfo.ChangesByVersions.Keys.Count() == 0)
                    {
                        result = "Історія оновлень не знайдена.";
                    }
                    else
                    {
                        var sb = new StringBuilder();

                        foreach (var updateVersion in updatesInfo.ChangesByVersions.Keys)
                        {
                            sb.AppendLine(updateVersion.ToString());
                            sb.AppendLine(updatesInfo.ChangesByVersions[updateVersion]);
                            sb.AppendLine();
                        }
                        result = sb.ToString();
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (WebException ex)
            {
                logger.Error("Error occured during update download");
                logger.Error(ex.Message);
            }
            return result;
        }
    }
}