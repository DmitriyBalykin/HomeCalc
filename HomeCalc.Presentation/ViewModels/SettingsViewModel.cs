using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Model.DataModels;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        public SettingsViewModel()
        {
            AddCommand("Save", new DelegateCommand(SaveCommandExecute));
        }

        private void SaveCommandExecute(object obj)
        {
            StoreService.SaveSettings(new SettingsModel
            {
                AutoWindowPosition = AutoWindowPosition,
                AutoWindowSize = AutoWindowSize,
                ClearFieldsOnSave = ClearFieldsOnSave,
                ResetCalculation = ResetCalculation
            });
        }

        private bool autoUpdateCheck;
        public bool AutoUpdateCheck
        {
            get { return autoUpdateCheck; }
            set
            {
                if (value != autoUpdateCheck)
                {
                    autoUpdateCheck = value;
                    OnPropertyChanged(() => AutoUpdateCheck);
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
                    OnPropertyChanged(() => AutoUpdate);
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
                    OnPropertyChanged(() => BackupPath);
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
                    OnPropertyChanged(() => ShowPurchaseSubType);
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
                    OnPropertyChanged(() => ShowPurchaseComment);
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
                    OnPropertyChanged(() => ShowPurchaseRate);
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
                    OnPropertyChanged(() => ShowStoreName);
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
                    OnPropertyChanged(() => ShowStoreRate);
                }
            }
        }
    }
}
