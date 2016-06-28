using HomeCalc.Core;
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
using System.Threading.Tasks;
using System.Windows.Input;

namespace HomeCalc.Presentation.BasicModels
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        private void InitializeProperties()
        {
            StoreService.TypesUpdated += StoreService_TypesUpdated;

            LoadData().ContinueWith(task => Initialize());
        }
        private Task LoadData()
        {
            return Task.Factory.StartNew(async () => 
            {
                TypeSelectorItems = new ObservableCollection<PurchaseType>(await StoreService.LoadPurchaseTypeList().ConfigureAwait(false));
            });
        }
        void StoreService_TypesUpdated(object sender, EventArgs e)
        {
            LoadData();
        }
        #region Properties Core
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
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(memberExpression.Member.Name));

                if (value != null)
                {
                    SettingsService.SaveSetting(property, value);
                }
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
        #endregion
        
    }
}
