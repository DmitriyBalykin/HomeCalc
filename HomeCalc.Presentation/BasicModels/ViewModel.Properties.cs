using HomeCalc.Core;
using HomeCalc.Core.Events;
using HomeCalc.Core.Helpers;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Core.Services.Messages;
using HomeCalc.Model.DbService;
using HomeCalc.Presentation.Models;
using HomeCalc.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HomeCalc.Presentation.BasicModels
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        private void InitializeProperties()
        {
            MsgDispatcher.AddHandler(HandleMessage);

            LoadTypes().ContinueWith(task => Initialize());
        }

        private void HandleMessage(Message message)
        {
            switch (message.MessageType)
            {
                case MessageType.RATING_PANEL_SHOW:
                    OnPropertyChanged(() => ShowRatingPanel);
                    break;
                case MessageType.TYPES_UPDATED:
                    LoadTypes();
                    break;
                case MessageType.SUBTYPES_UPDATED:
                    LoadSubTypes((long)message.Data);
                    break;
                default:
                    break;
            }
        }
        
        private Task LoadTypes()
        {
            return Task.Factory.StartNew(async () => 
            {
                TypeSelectorItems = new ObservableCollection<ProductType>(await StoreService.LoadProductTypeList().ConfigureAwait(false));
            });
        }

        public Task LoadSubTypes(long typeId)
        {
            return Task.Factory.StartNew(async () =>
            {
                var list = new List<ProductSubType>();
                list.Add(new ProductSubType());
                list.AddRange(await StoreService.LoadProductSubTypeList(typeId).ConfigureAwait(false));

                ProductSubTypes = new ObservableCollection<ProductSubType>(list);
            });
        }
        
        #region Properties Core
        protected void Settings_SettingsChanged(object sender, SettingChangedEventArgs e)
        {
            Console.WriteLine("settings changed, thread {0}", Thread.CurrentThread.ManagedThreadId);
            OnPropertyChanged(e.SettingName);
        }
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void OnPropertyChanged<T>(Expression<Func<T>> property, object value = null)
        {
            if (property == null)
            {
                return;
            }
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
            {
                return;
            }
            if (PropertyChanged != null)
            {
                if (value != null)
                {
                    Settings.SaveSetting(property, value);
                }
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
            }
        }
        #endregion
        #region Helpers
        
        #endregion
        #region Common Properties
        private ObservableCollection<ProductType> typeSelectorItems;
        public ObservableCollection<ProductType> TypeSelectorItems
        {
            get
            {
                return typeSelectorItems;
            }
            set
            {
                if (typeSelectorItems != value)
                {
                    typeSelectorItems = value;
                    OnPropertyChanged(() => TypeSelectorItems);
                }
            }
        }

        private ObservableCollection<ProductSubType> productSubTypes;
        public ObservableCollection<ProductSubType> ProductSubTypes
        {
            get
            {
                return productSubTypes;
            }
            set
            {
                if (productSubTypes != value)
                {
                    productSubTypes = value;
                    OnPropertyChanged(() => ProductSubTypes);
                    if (ProductSubTypes.Count > 0)
                    {
                        MsgDispatcher.Post(MessageType.SUBTYPES_LOADED);
                    }
                    else
                    {
                        MsgDispatcher.Post(MessageType.SUBTYPES_UNLOADED);
                    }
                }
            }
        }

        [SettingProperty]
        public bool ShowProductSubType
        {
            get { return Settings.GetSetting(SettingsService.SHOW_PRODUCT_SUBTYPE_KEY).SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting(SettingsService.SHOW_PRODUCT_SUBTYPE_KEY).SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowProductSubType, value);
                    MsgDispatcher.Post(MessageType.RATING_PANEL_SHOW);
                }
            }
        }
        [SettingProperty]
        public bool ShowPurchaseComment
        {
            get { return Settings.GetSetting(SettingsService.SHOW_PURCHASE_COMMENT_KEY).SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting(SettingsService.SHOW_PURCHASE_COMMENT_KEY).SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowPurchaseComment, value);
                    MsgDispatcher.Post(MessageType.RATING_PANEL_SHOW);
                }
            }
        }
        [SettingProperty]
        public bool ShowPurchaseRate
        {
            get { return Settings.GetSetting(SettingsService.SHOW_PURCHASE_RATE_KEY).SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting(SettingsService.SHOW_PURCHASE_RATE_KEY).SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowPurchaseRate, value);
                    MsgDispatcher.Post(MessageType.RATING_PANEL_SHOW);
                }
            }
        }
        [SettingProperty]
        public bool ShowMonthlyPurchase
        {
            get { return Settings.GetSetting(SettingsService.SHOW_MONTHLY_PURCHASE).SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting(SettingsService.SHOW_MONTHLY_PURCHASE).SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowMonthlyPurchase, value);
                }
            }
        }
        [SettingProperty]
        public bool ShowStoreName
        {
            get { return Settings.GetSetting(SettingsService.SHOW_STORE_NAME_KEY).SettingBoolValue; }
            set
            {
                Console.WriteLine("show store name, thread {0}", Thread.CurrentThread.ManagedThreadId);
                if (value != Settings.GetSetting(SettingsService.SHOW_STORE_NAME_KEY).SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowStoreName, value);
                    MsgDispatcher.Post(MessageType.RATING_PANEL_SHOW);
                }
                if (!value)
                {
                    OnPropertyChanged(() => ShowStoreComment, false);
                    OnPropertyChanged(() => ShowStoreRate, false);
                }
            }
        }
        [SettingProperty]
        public bool ShowStoreRate
        {
            get { return Settings.GetSetting(SettingsService.SHOW_STORE_RATE_KEY).SettingBoolValue; }
            set
            {
                Console.WriteLine("show store rate, thread {0}", Thread.CurrentThread.ManagedThreadId);
                if (value != Settings.GetSetting(SettingsService.SHOW_STORE_RATE_KEY).SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowStoreRate, value);
                    MsgDispatcher.Post(MessageType.RATING_PANEL_SHOW);
                }
                if (value)
                {
                    //Show storage name if storage rate selected
                    OnPropertyChanged(() => ShowStoreName, value);
                }
            }
        }
        [SettingProperty]
        public bool ShowStoreComment
        {
            get { return Settings.GetSetting(SettingsService.SHOW_STORE_COMMENT_KEY).SettingBoolValue; }
            set
            {
                Console.WriteLine("show store comment, thread {0}", Thread.CurrentThread.ManagedThreadId);
                if (value != Settings.GetSetting(SettingsService.SHOW_STORE_COMMENT_KEY).SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowStoreComment, value);
                    MsgDispatcher.Post(MessageType.RATING_PANEL_SHOW);
                    
                }
                if (value)
                {
                    //Show storage name if storage comment selected
                    OnPropertyChanged(() => ShowStoreName, value);
                }
            }
        }

        public bool ShowRatingPanel
        {
            get
            {
                var result = Settings.GetSetting(SettingsService.SHOW_PRODUCT_SUBTYPE_KEY).SettingBoolValue ||
                        Settings.GetSetting(SettingsService.SHOW_PURCHASE_RATE_KEY).SettingBoolValue ||
                        Settings.GetSetting(SettingsService.SHOW_PURCHASE_COMMENT_KEY).SettingBoolValue ||
                        Settings.GetSetting(SettingsService.SHOW_STORE_RATE_KEY).SettingBoolValue ||
                        Settings.GetSetting(SettingsService.SHOW_STORE_NAME_KEY).SettingBoolValue;

                return result;
            }
        }
        #endregion
        
    }
}
