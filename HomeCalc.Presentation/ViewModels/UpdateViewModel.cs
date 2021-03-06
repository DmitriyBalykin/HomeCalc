﻿using HomeCalc.Core.Helpers;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.Services;
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
        public event EventHandler CloseApplicationEventHandler;
        public UpdateViewModel()
        {
            AddCommand("CheckUpdates", new DelegateCommand(CheckUpdatesCommandExecute));
            AddCommand("Update", new DelegateCommand(UpdateCommandExecute));

            if (Settings.GetSetting(SettingsService.AUTO_UPDATE_CHECK_KEY).SettingBoolValue)
            {
                RefreshUpdatesInformation();
            }
        }

        private void CheckUpdatesCommandExecute(object obj)
        {
            RefreshUpdatesInformation();
        }

        private void LoadUpdatesHistory()
        {
            RefreshHistoryInformation();
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
        private void RefreshUpdatesInformation()
        {
            Task.Factory.StartNew(async () =>
            {
                var changes = await UpdateService.GetUpdatesInformation();
                UIDispatcherHelper.CallOnUIThread(() => { VersionChanges = changes; });
            });
        }

        private void RefreshHistoryInformation()
        {
            Task.Factory.StartNew(async () =>
            {
                var history = await UpdateService.GetUpdatesHistory();
                UIDispatcherHelper.CallOnUIThread(() => { VersionChanges = history; });
            });
        }

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
