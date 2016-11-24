using HomeCalc.Core.Events;
using HomeCalc.Core.LogService;
using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Services
{
    public class SettingsService : IDisposable
    {
        public const string AUTO_UPDATE_KEY = "AutoUpdate";
        public const string AUTO_UPDATE_CHECK_KEY = "AutoUpdateCheck";
        public const string BACKUP_PATH_KEY = "BackupPath";
        public const string DO_DATABASE_BACKUP = "DoDatabaseBackup";
        public const string SHOW_PURCHASE_SUBTYPE_KEY = "ShowProductSubType";
        public const string SHOW_PURCHASE_COMMENT_KEY = "ShowPurchaseComment";
        public const string SHOW_PURCHASE_RATE_KEY = "ShowPurchaseRate";
        public const string SHOW_MONTHLY_PURCHASE = "ShowMonthlyPurchase";
        public const string SHOW_STORE_NAME_KEY = "ShowStoreName";
        public const string SHOW_STORE_RATE_KEY = "ShowStoreRate";
        public const string SHOW_STORE_COMMENT_KEY = "ShowStoreComment";
        public const string SHOW_PURCHASE_DETAILS_KEY = "ShowPurchaseDetails";
        public const string SEND_EMAIL_AUTO_KEY = "SendEmailAuto";

        private Dictionary<string, SettingsModel> HighLevelCache;

        public event EventHandler<SettingChangedEventArgs> SettingsChanged;

        Logger logger;
        static SettingsService instance;

        private static object monitor = new object();

        IStorageService storageService;
        private SettingsService()
        {
            logger = LogService.GetLogger();

            storageService = StorageService.GetInstance();

            HighLevelCache = storageService.LoadSettings().Result.ToDictionary(item => item.SettingName);

            logger.SendEmail = GetSetting(SEND_EMAIL_AUTO_KEY).SettingBoolValue;
        }

        public static SettingsService GetInstance()
        {
            if (instance == null)
            {
                lock (monitor)
                {
                    if (instance == null)
                    {
                        instance = new SettingsService();
                    }
                }
            }
            return instance;
        }

        public SettingsModel GetSetting(string settingKey)
        {
            Console.WriteLine("Get settings, thread {0}", Thread.CurrentThread.ManagedThreadId);
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
