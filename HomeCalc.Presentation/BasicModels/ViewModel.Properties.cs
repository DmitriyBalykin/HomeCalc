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
            StoreService.TypesUpdated += StoreService_TypesUpdated;
            StoreService.SubTypesUpdated += StoreService_SubTypesUpdated;

            LoadTypes()
                .ContinueWith(task => LoadSubTypes())
                .ContinueWith(task => Initialize());
        }
        
        private Task LoadTypes()
        {
            return Task.Factory.StartNew(async () => 
            {
                TypeSelectorItems = new ObservableCollection<PurchaseType>(await StoreService.LoadPurchaseTypeList().ConfigureAwait(false));
            });
        }
        private Task LoadSubTypes()
        {
            return Task.Factory.StartNew(async () =>
            {
                PurchaseSubTypes = new ObservableCollection<PurchaseSubType>(await StoreService.LoadPurchaseSubTypeList().ConfigureAwait(false));
            });
        }
        void StoreService_TypesUpdated(object sender, EventArgs e)
        {
            LoadTypes();
        }
        void StoreService_SubTypesUpdated(object sender, EventArgs e)
        {
            LoadSubTypes();
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
        private ObservableCollection<PurchaseType> typeSelectorItems;
        public ObservableCollection<PurchaseType> TypeSelectorItems
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

        private ObservableCollection<PurchaseSubType> purchaseSubTypes;
        public ObservableCollection<PurchaseSubType> PurchaseSubTypes
        {
            get
            {
                return purchaseSubTypes;
            }
            set
            {
                if (purchaseSubTypes != value)
                {
                    purchaseSubTypes = value;
                    OnPropertyChanged(() => PurchaseSubTypes);
                }
            }
        }

        [SettingProperty]
        public bool ShowPurchaseSubType
        {
            get { return Settings.GetSetting("ShowPurchaseSubType").SettingBoolValue; }
            set
            {
                if (value != Settings.GetSetting("ShowPurchaseSubType").SettingBoolValue)
                {
                    OnPropertyChanged(() => ShowPurchaseSubType, value);
                    OnPropertyChanged(() => ShowRatingPanel);
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
                    OnPropertyChanged(() => ShowRatingPanel);
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
                    OnPropertyChanged(() => ShowRatingPanel);
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
                    OnPropertyChanged(() => ShowRatingPanel);
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
                    OnPropertyChanged(() => ShowRatingPanel);
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
                    OnPropertyChanged(() => ShowRatingPanel);
                    
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
                var result = Settings.GetSetting("ShowPurchaseSubType").SettingBoolValue ||
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
