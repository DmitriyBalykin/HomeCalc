using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Model.DataModels;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            Task.Factory.StartNew(async () => 
            {
                var settings = await StoreService.LoadSettings();
                AutoUpdateCheck = bool.Parse(settings.Single(setting => setting.SettingName == "AutoUpdateCheck").SettingValue);
                AutoUpdate = bool.Parse(settings.Single(setting => setting.SettingName == "AutoUpdate").SettingValue);
                BackupPath = settings.Single(setting => setting.SettingName == "BackupPath").SettingValue;
                ShowPurchaseSubType = bool.Parse(settings.Single(setting => setting.SettingName == "ShowPurchaseSubType").SettingValue);
                ShowPurchaseComment = bool.Parse(settings.Single(setting => setting.SettingName == "ShowPurchaseComment").SettingValue);
                ShowPurchaseRate = bool.Parse(settings.Single(setting => setting.SettingName == "ShowPurchaseRate").SettingValue);
                ShowStoreName = bool.Parse(settings.Single(setting => setting.SettingName == "ShowStoreName").SettingValue);
                ShowStoreRate = bool.Parse(settings.Single(setting => setting.SettingName == "ShowStoreRate").SettingValue);
            });
            
        }

        private void SelectBackupPathCommandExecute(object obj)
        {
            //throw new NotImplementedException();
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
