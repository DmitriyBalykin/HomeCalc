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
    public partial class ViewModel
    {
        private void InitializeProperties()
        {
            StoreService.TypesUpdated += StoreService_TypesUpdated;

            typeSelectorItems = new ObservableCollection<PurchaseType>(StoreService.LoadPurchaseTypeList());
        }
        void StoreService_TypesUpdated(object sender, EventArgs e)
        {
            TypeSelectorItems = new ObservableCollection<PurchaseType>(StoreService.LoadPurchaseTypeList());
        }

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
    }
}
