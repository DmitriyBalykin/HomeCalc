using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCalc.Presentation.Models;
using System.Globalization;
using HomeCalc.Presentation.Services;

namespace HomeCalc.Presentation.ViewModels
{
    public class AddDataViewModel : ViewModel
    {
        public AddDataViewModel()
        {
            purchase = new Purchase();

            AddCommand("Save", new DelegateCommand(SaveCommandExecute));

            StoreService.TypesUpdated += StoreService_TypesUpdated;
            StoreService.HistoryUpdated += UpdatePurchaseHistory;

            typeSelectorItems = new ObservableCollection<PurchaseType>( StoreService.LoadPurchaseTypeList());

            PurchaseType = TypeSelectorItems.FirstOrDefault();

            

            actualCalculationTarget = Services.DataService.CalculationTargetProperty.TotalCost;

            Status.Post("Завантажено");
        }

        void UpdatePurchaseHistory(object sender, EventArgs e)
        {
            PurchaseHistoryItems = new ObservableCollection<Purchase>(StoreService.PurchaseHistory);
        }
        void StoreService_TypesUpdated(object sender, EventArgs e)
        {
            TypeSelectorItems = new ObservableCollection<PurchaseType>(StoreService.LoadPurchaseTypeList());
        }

        private void SaveCommandExecute(object obj)
        {
            if (StoreService.AddPurchase(purchase))
            {
                logger.Info("Purchase saved");
                Status.Post("Покупка \"{0}\" збережена", PurchaseName);
            }
            else
            {
                logger.Warn("Purchase not saved");
                Status.Post("Помилка: покупка \"{0}\" не збережена", PurchaseName);
            }
        }

        private void SearchPurchase()
        {
            var exactPurchases = StoreService.PurchaseHistory.Where(p => p.Name == purchase.Name);
            IEnumerable<Purchase> resultList;
            if (exactPurchases.Count() > 0)
            {
                resultList = exactPurchases;
            }
            else
            {
                resultList = StoreService.PurchaseHistory.Where(p => p.Name.StartsWith(PurchaseName, true, CultureInfo.InvariantCulture));
            }
            PurchaseHistoryItemsWrapper = resultList.OrderByDescending(p => p.Date).Take(10);
        }
        private void DoCalculations()
        {
            if (purchase != null)
            {
                DataService.PerformCalculation(purchase, actualCalculationTarget);

                OnPropertyChanged(() => Count);
                OnPropertyChanged(() => ItemCost);
                OnPropertyChanged(() => TotalCost);
            }
        }
        private DateTime dateToStore = DateTime.Now;
        public DateTime DateToStore {
            get
            {
                return dateToStore;
            }
            set
            {
                if (value != dateToStore)
                {
                    dateToStore = value;
                }
            }
        }
        private IEnumerable<Purchase> PurchaseHistoryItemsWrapper
        {
            set
            {
                PurchaseHistoryItems = new ObservableCollection<Purchase>(value);
                ShowPurchaseHistory = true;
            }
        }
        private ObservableCollection<Purchase> purchaseHistoryItems;
        public ObservableCollection<Purchase> PurchaseHistoryItems
        {
            get
            {
                return purchaseHistoryItems;
            }
            set
            {
                if (purchaseHistoryItems != value)
                {
                    purchaseHistoryItems = value;
                    OnPropertyChanged(() => PurchaseHistoryItems);
                }
            }
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

        private DataService.CalculationTargetProperty actualCalculationTarget;

        private Purchase purchase;
        public string PurchaseName
        {
            get
            {
                return purchase.Name;
            }
            set
            {
                if (value != purchase.Name)
                {
                    purchase.Name = value;
                    OnPropertyChanged(() => PurchaseName);
                    SearchPurchase();
                }
            }
        }

        public string Count
        {
            get { return purchase.ItemsNumber.ToString(); }
            set
            {
                double result;
                if (Double.TryParse(value, out result))
                {
                    purchase.ItemsNumber = result;

                    DoCalculations();
                }
            }
        }

        public string ItemCost
        {
            get { return purchase.ItemCost.ToString(); }
            set
            {
                double result;
                if (double.TryParse(value, out result))
                {
                    purchase.ItemCost = result;

                    DoCalculations();
                }
            }
        }
        public string TotalCost
        {
            get { return purchase.TotalCost.ToString(); }
            set
            {
                double result;
                if (double.TryParse(value, out result))
                {
                    purchase.TotalCost = result;

                    DoCalculations();
                }
            }
        }
        public PurchaseType PurchaseType
        {
            get
            {
                return purchase.Type; 
            }
            set
            {
                purchase.Type = value;
            }
        }
        
        private bool calcItemCost = false;
        public bool IsCalcItemCost
        {
            get
            {
                return calcItemCost;
            }
            set
            {
                if (calcItemCost != value)
                {
                    calcItemCost = value;
                    OnPropertyChanged(() => IsCalcItemCost);
                }
                if (value)
                {
                    IsCalcItemsCount = false;
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.ItemCost;
                }
                setTotalCalc();
            }
        }
        private bool calcItemsCount = false;
        public bool IsCalcItemsCount
        {
            get
            {
                return calcItemsCount;
            }
            set
            {
                if (calcItemsCount != value)
                {
                    calcItemsCount = value;
                    OnPropertyChanged(() => IsCalcItemsCount);
                }
                if (value)
                {
                    IsCalcItemCost = false;
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.ItemsNumber;
                }
                setTotalCalc();
            }
        }
        private void setTotalCalc()
        {
            IsCalcByTotal = !(calcItemsCount || calcItemCost);
        }
        private bool calcTotalCost = true;
        public bool IsCalcByTotal
        {
            get
            {
                return !calcTotalCost;
            }
            set
            {
                if (value != calcTotalCost)
                {
                    calcTotalCost = value;
                    OnPropertyChanged(() => IsCalcByTotal);
                }
                if (value)
                {
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.TotalCost;
                }
            }
        }

        private bool showPurchaseHistory;
        public bool ShowPurchaseHistory
        {
            get
            {
                return showPurchaseHistory;
            }
            set
            {
                if (value != showPurchaseHistory)
                {
                    showPurchaseHistory = value;
                    OnPropertyChanged(() => ShowPurchaseHistory);
                }
            }
        }

        
    }
}
