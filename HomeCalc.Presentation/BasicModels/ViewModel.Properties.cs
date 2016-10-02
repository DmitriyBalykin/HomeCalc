using HomeCalc.Core;
using HomeCalc.Core.Events;
using HomeCalc.Core.Helpers;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
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

        private void HandleMessage(string message)
        {
            switch (message)
            {
                case "showRatingPanel":
                    OnPropertyChanged(() => ShowRatingPanel);
                    break;
                case "typesUpdated":
                    LoadTypes();
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
                ProductSubTypes = new ObservableCollection<ProductSubType>(await StoreService.LoadProductSubTypeList(typeId).ConfigureAwait(false));
            });
        }
        
        #region Properties Core
        protected void Settings_SettingsChanged(object sender, SettingChangedEventArgs e)
        {
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
                        MsgDispatcher.Post("productSubTypesLoaded");
                    }
                    else
                    {
                        MsgDispatcher.Post("productSubTypesUnLoaded");
                    }
                }
            }
        }

        [SettingProperty]
        public bool ShowProductSubType
        {
            get { return Settings.GetSetting("ShowProductSubType").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("ShowProductSubType").SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowProductSubType, value);
                    MsgDispatcher.Post("showRatingPanel");
                }
            }
        }
        [SettingProperty]
        public bool ShowPurchaseComment
        {
            get { return Settings.GetSetting("ShowPurchaseComment").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("ShowPurchaseComment").SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowPurchaseComment, value);
                    MsgDispatcher.Post("showRatingPanel");
                }
            }
        }
        [SettingProperty]
        public bool ShowPurchaseRate
        {
            get { return Settings.GetSetting("ShowPurchaseRate").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("ShowPurchaseRate").SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowPurchaseRate, value);
                    MsgDispatcher.Post("showRatingPanel");
                }
            }
        }
        [SettingProperty]
        public bool ShowMonthlyPurchase
        {
            get { return Settings.GetSetting("ShowMonthlyPurchase").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("ShowMonthlyPurchase").SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowMonthlyPurchase, value);
                }
            }
        }
        [SettingProperty]
        public bool ShowStoreName
        {
            get { return Settings.GetSetting("ShowStoreName").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("ShowStoreName").SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowStoreName, value);
                    MsgDispatcher.Post("showRatingPanel");
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
            get { return Settings.GetSetting("ShowStoreRate").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("ShowStoreRate").SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowStoreRate, value);
                    MsgDispatcher.Post("showRatingPanel");
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
            get { return Settings.GetSetting("ShowStoreComment").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("ShowStoreComment").SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowStoreComment, value);
                    MsgDispatcher.Post("showRatingPanel");
                    
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
                var result = Settings.GetSetting("ShowProductSubType").SettingBoolValue ||
                        Settings.GetSetting("ShowPurchaseRate").SettingBoolValue ||
                        Settings.GetSetting("ShowPurchaseComment").SettingBoolValue ||
                        Settings.GetSetting("ShowStoreRate").SettingBoolValue ||
                        Settings.GetSetting("ShowStoreName").SettingBoolValue;

                return result;
            }
        }
        #endregion
        
    }
}
