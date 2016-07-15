using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Model.DataModels;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        public SettingsViewModel()
        {
            AddCommand("SelectBackupPath", new DelegateCommand(SelectBackupPathCommandExecute));

            InitiateSettings();
        }

        private void InitiateSettings()
        {
            Task.Factory.StartNew(async () =>
            {
                var settings = await StoreService.LoadSettings();
                var properties = this.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(bool))
                    {
                        property.SetValue(this, SettingsService.GetBooleanValue(property.Name));
                    }
                    else
                    {
                        property.SetValue(this, SettingsService.GetStringValue(property.Name));
                    }
                }
            });
        }

        private void SelectBackupPathCommandExecute(object obj)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select existing installation path";
            dlg.IsFolderPicker = true;
            dlg.EnsurePathExists = true;
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                BackupPath = dlg.FileName;
            }
        }

        #region Properties
        private bool autoUpdateCheck;
        public bool AutoUpdateCheck
        {
            get { return autoUpdateCheck; }
            set
            {
                if (value != autoUpdateCheck)
                {
                    autoUpdateCheck = value;
                    OnPropertyChanged(() => AutoUpdateCheck, autoUpdateCheck);
                }
            }
        }
        private bool autoUpdate;
        public bool AutoUpdate
        {
            get { return autoUpdate; }
            set
            {
                if (value != autoUpdate)
                {
                    autoUpdate = value;
                    OnPropertyChanged(() => AutoUpdate, autoUpdate);
                }
            }
        }
        private string backupPath;
        public string BackupPath
        {
            get { return backupPath; }
            set
            {
                if (value != backupPath)
                {
                    backupPath = value;
                    OnPropertyChanged(() => BackupPath, backupPath);
                }
            }
        }
        
        private bool doDatabaseBackup;
        public bool DoDatabaseBackup
        {
            get { return doDatabaseBackup; }
            set
            {
                if (value != doDatabaseBackup)
                {
                    doDatabaseBackup = value;
                    OnPropertyChanged(() => DoDatabaseBackup, doDatabaseBackup);
                }
            }
        }
        #endregion

        #region Helpers
        
        #endregion
    }
}
