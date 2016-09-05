﻿using HomeCalc.Core.Events;
using HomeCalc.Core.LogService;
using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Services
{
    public class SettingsService : IDisposable
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

        private Dictionary<string, SettingsModel> HighLevelCache;

        public event EventHandler<SettingChangedEventArgs> SettingsChanged;

        Logger logger;
        static SettingsService instance;

        IStorageService storageService;
        private SettingsService()
        {
            logger = LogService.GetLogger();
            storageService = StorageService.GetInstance();

            HighLevelCache = storageService.LoadSettings().Result.ToDictionary(item => item.SettingName);
        }

        public static SettingsService GetInstance()
        {
            if (instance == null)
            {
                instance = new SettingsService();
            }
            return instance;
        }

        public SettingsModel GetSetting(string settingKey)
        {
            //var filtered = storageService.LoadSettings().Result
            //    .Where(setting => setting.SettingName.Equals(settingKey, StringComparison.InvariantCultureIgnoreCase))
            //    .FirstOrDefault();

            //return filtered == null ? false : filtered.SettingBoolValue;
            SettingsModel settingModel;
            if (HighLevelCache.TryGetValue(settingKey, out settingModel))
            {
                return settingModel;
            }
            else
            {
                return new SettingsModel();
            }
        }

        //public string GetStringValue(string settingKey)
        //{
        //    //var filtered = storageService.LoadSettings().Result
        //    //    .Where(setting => setting.SettingName.Equals(settingKey, StringComparison.InvariantCultureIgnoreCase))
        //    //    .FirstOrDefault();

        //    //return filtered == null ? string.Empty : filtered.SettingStringValue;
        //}

        public void SaveSetting<T>(Expression<Func<T>> setting, object value)
        {
            var expression = setting.Body as MemberExpression;
            if (expression != null)
            {
                var settingModel = new SettingsModel();
                settingModel.SettingName = expression.Member.Name;
                if (typeof(T) == typeof(bool))
                {
                    settingModel.SettingBoolValue = (value as bool?) ?? false;
                }
                else if (typeof(T) == typeof(string))
                {
                    settingModel.SettingStringValue = value.ToString();
                }
                else
                {
                    logger.Error("Unsupported type of property");
                    throw new Exception("Unsupported type of property");
                }

                HighLevelCache[settingModel.SettingName] = settingModel;

                if (SettingsChanged != null)
                {
                    SettingsChanged(null, new SettingChangedEventArgs { SettingName = settingModel.SettingName });
                }
                Task.Factory.StartNew(async () =>
                {
                    await storageService.SaveSettings(settingModel).ConfigureAwait(false);
                });
            }
        }

        public void Dispose()
        {
            if (SettingsChanged != null)
            {
                SettingsChanged = null;
            }
        }
    }
}
