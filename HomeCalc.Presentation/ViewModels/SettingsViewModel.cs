using HomeCalc.Core.Helpers;
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

            //InitiateSettings();
        }

        //private void InitiateSettings()
        //{
        //    Task.Factory.StartNew(async () =>
        //    {
        //        var settings = await StoreService.LoadSettings();
        //        var properties = this.GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(SettingProperty)).Count() > 0);
        //        foreach (var property in properties)
        //        {
        //            if (property.PropertyType == typeof(bool))
        //            {
        //                property.SetValue(this, SettingsService.GetBooleanValue(property.Name));
        //            }
        //            else
        //            {
        //                property.SetValue(this, SettingsService.GetStringValue(property.Name));
        //            }
        //        }
        //    });
        //}

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
        [SettingProperty]
        public bool AutoUpdateCheck
        {
            get { return Settings.GetSetting("AutoUpdateCheck").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("AutoUpdateCheck").SettingBoolValue)
                {
                    OnPropertyChanged(() => AutoUpdateCheck, value);
                }
            }
        }
        [SettingProperty]
        public bool AutoUpdate
        {
            get { return Settings.GetSetting("AutoUpdate").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("AutoUpdate").SettingBoolValue)
                {
                    OnPropertyChanged(() => AutoUpdate, value);
                }
            }
        }
        [SettingProperty]
        public string BackupPath
        {
            get { return Settings.GetSetting("BackupPath").SettingStringValue; }
            set
            {
                if (value != Settings.GetSetting("BackupPath").SettingStringValue)
                {
                    OnPropertyChanged(() => BackupPath, value);
                }
            }
        }
        
        [SettingProperty]
        public bool DoDatabaseBackup
        {
            get { return Settings.GetSetting("DoDatabaseBackup").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("DoDatabaseBackup").SettingBoolValue)
                {
                    OnPropertyChanged(() => DoDatabaseBackup, value);
                }
            }
        }
        #endregion

        #region Helpers
        
        #endregion
    }
}
