﻿using HomeCalc.Core.LogService;
using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Services
{
    public class SettingsService
    {
        public const string AUTO_UPDATE_KEY = "AutoUpdate";
        public const string AUTO_UPDATE_CHECK_KEY = "AutoUpdateCheck";
        public const string BACKUP_PATH_KEY = "BackupPath";
        public const string DO_DATABASE_BACKUP = "DoDatabaseBackup";
        public const string SHOW_PURCHASE_SUBTYPE_KEY = "ShowPurchaseSubType";
        public const string SHOW_PURCHASE_COMMENT_KEY = "ShowPurchaseComment";
        public const string SHOW_PURCHASE_RATE_KEY = "ShowPurchaseRate";
        public const string SHOW_STORE_NAME_KEY = "ShowStoreName";
        public const string SHOW_STORE_RATE_KEY = "ShowStoreRate";
        public const string SHOW_STORE_COMMENT = "ShowStoreComment";

        Logger logger;
        static SettingsService instance;

        IStorageService storageService;
        private SettingsService()
        {
            logger = LogService.GetLogger();
            storageService = StorageService.GetInstance();
        }

        public static SettingsService GetInstance()
        {
            if (instance == null)
            {
                instance = new SettingsService();
            }
            return instance;
        }

        public bool GetBooleanValue(string settingKey)
        {
            var filtered = storageService.LoadSettings().Result
                .Where(setting => setting.SettingName.Equals(settingKey, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            return filtered == null ? false : filtered.SettingBoolValue;
        }

        public string GetStringValue(string settingKey)
        {
            var filtered = storageService.LoadSettings().Result
                .Where(setting => setting.SettingName.Equals(settingKey, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            return filtered == null ? string.Empty : filtered.SettingStringValue;
        }

        public void SaveSetting<T>(Expression<Func<T>> setting, object value)
        {
            var expression = setting.Body as MemberExpression;
            Task.Factory.StartNew(async () =>
            {
                if (expression != null)
                {
                    string settingName = expression.Member.Name;
                    var boolValue = value as bool?;
                    if (boolValue.HasValue)
                    {
                        await storageService.SaveSettings(new SettingsModel
                        {
                            SettingName = settingName,
                            SettingBoolValue = boolValue.Value
                        }).ConfigureAwait(false);
                    }
                    else
                    {
                        await storageService.SaveSettings(new SettingsModel
                        {
                            SettingName = settingName,
                            SettingStringValue = value.ToString()
                        }).ConfigureAwait(false);
                    }
                }
            });

        }
    }
}
