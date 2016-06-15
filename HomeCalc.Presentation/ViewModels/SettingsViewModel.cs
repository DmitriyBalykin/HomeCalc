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

            AssignSettings();
        }

        private void AssignSettings()
        {
            Task.Factory.StartNew(async () =>
            {
                var settings = await StoreService.LoadSettings();
                var properties = this.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var settingModel = settings.Where(setting => setting.SettingName.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    var settingValue = settingModel != null ? settingModel.SettingValue : String.Empty;
                    if (!string.IsNullOrEmpty(settingValue))
                    {
                        bool boolValue;
                        if (property.PropertyType == typeof(bool) && bool.TryParse(settingValue, out boolValue))
                        {
                            property.SetValue(this, boolValue);
                        }
                        else
                        {
                            property.SetValue(this, settingValue);
                        }
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
        private bool showPurchaseSubType;
        public bool ShowPurchaseSubType
        {
            get { return showPurchaseSubType; }
            set
            {
                if (value != showPurchaseSubType)
                {
                    showPurchaseSubType = value;
                    OnPropertyChanged(() => ShowPurchaseSubType, showPurchaseSubType);
                }
            }
        }
        private bool showPurchaseComment;
        public bool ShowPurchaseComment
        {
            get { return showPurchaseComment; }
            set
            {
                if (value != showPurchaseComment)
                {
                    showPurchaseComment = value;
                    OnPropertyChanged(() => ShowPurchaseComment, showPurchaseComment);
                }
            }
        }
        private bool showPurchaseRate;
        public bool ShowPurchaseRate
        {
            get { return showPurchaseRate; }
            set
            {
                if (value != showPurchaseRate)
                {
                    showPurchaseRate = value;
                    OnPropertyChanged(() => ShowPurchaseRate, showPurchaseRate);
                }
            }
        }
        private bool showStoreName;
        public bool ShowStoreName
        {
            get { return showStoreName; }
            set
            {
                if (value != showStoreName)
                {
                    showStoreName = value;
                    OnPropertyChanged(() => ShowStoreName, showStoreName);
                }
            }
        }
        private bool showStoreRate;
        public bool ShowStoreRate
        {
            get { return showStoreRate; }
            set
            {
                if (value != showStoreRate)
                {
                    showStoreRate = value;
                    OnPropertyChanged(() => ShowStoreRate, showStoreRate);
                }
            }
        }
        #endregion

        #region Helpers
        
        #endregion
    }
}
