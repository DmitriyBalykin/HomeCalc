﻿using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Updater;

namespace HomeCalc.Presentation.ViewModels
{
    public class UpdateViewModel : ViewModel
    {
        Logger logger = LogService.GetLogger();

        public event EventHandler CloseApplicationEventHandler;
        public UpdateViewModel()
        {
            logger = LogService.GetLogger();
            AddCommand("CheckUpdates", new DelegateCommand(CheckUpdatesCommandExecute));
            AddCommand("Update", new DelegateCommand(UpdateCommandExecute));
        }

        private void CheckUpdatesCommandExecute(object obj)
        {
            try
            {
                VersionChecker.GetUpdatesInformation(true).ContinueWith(task => 
                {
                    var updatesInfo = task.Result;
                    if (!updatesInfo.HasNewVersion)
                    {
                        VersionChanges = "Версія програми є найновішою.";
                        return;
                    }

                    var sb = new StringBuilder();

                    foreach (var updateVersion in updatesInfo.ChangesByVersions.Keys)
                    {
                        sb.AppendLine(updateVersion.ToString());
                        sb.AppendLine(updatesInfo.ChangesByVersions[updateVersion]);
                        sb.AppendLine();
                    }
                    VersionChanges = sb.ToString();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (WebException ex)
            {
                logger.Error("Error occured during update download");
                logger.Error(ex.Message);
            }
            
        }

        private void LoadUpdatesHistory()
        {
            try
            {
                VersionChecker.GetUpdatesInformation(false).ContinueWith(task =>
                {
                    var updatesInfo = task.Result;
                    if (updatesInfo.ChangesByVersions.Keys.Count() == 0)
                    {
                        VersionChanges = "Історія оновлень не знайдена.";
                        return;
                    }

                    var sb = new StringBuilder();

                    foreach (var updateVersion in updatesInfo.ChangesByVersions.Keys)
                    {
                        sb.AppendLine(updateVersion.ToString());
                        sb.AppendLine(updatesInfo.ChangesByVersions[updateVersion]);
                        sb.AppendLine();
                    }
                    VersionChanges = sb.ToString();
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (WebException ex)
            {
                logger.Error("Error occured during update download");
                logger.Error(ex.Message);
            }

        }

        private async void UpdateCommandExecute(object obj)
        {
            await VersionChecker.GetUpdatesInformation(true).ContinueWith(async task =>
            {
                var updatesInfo = task.Result;
                if (!updatesInfo.HasNewVersion)
                {
                    VersionChanges = "Версія програми є найновішою, оновлення не виконано.";
                    return;
                }

                await VersionUpdater.StartUpdate(() =>
                {
                    if (CloseApplicationEventHandler != null)
                    {
                        CloseApplicationEventHandler(this, EventArgs.Empty);
                    }
                });

            }, TaskContinuationOptions.OnlyOnRanToCompletion);

        }

        #region Helpers
        

        #endregion

        #region Properties



        private string versionChanges;
        public string VersionChanges
        {
            get { return versionChanges; }
            set
            {
                if (value != versionChanges)
                {
                    versionChanges = value;
                    OnPropertyChanged(() => VersionChanges);
                }
            }
        }
        public string CurrentVersion
        {
            get { return string.Format("Поточна версія: {0}", Assembly.GetExecutingAssembly().GetName().Version); }
        }

        private bool showUpdatesHistory;
        public bool ShowUpdatesHistory
        {
            get { return showUpdatesHistory; }
            set
            {
                if (value != showUpdatesHistory)
                {
                    showUpdatesHistory = value;
                    OnPropertyChanged(() => ShowUpdatesHistory);

                    if (value)
                    {
                        LoadUpdatesHistory();
                    }
                    else
                    {
                        VersionChanges = string.Empty;
                    }
                }
            }
        }
        #endregion
    }
}
