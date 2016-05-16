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
using HomeCalc.Core.Helpers;

namespace HomeCalc.Presentation.ViewModels
{
    public class AddDataViewModel : ViewModel
    {
        private const int MINIMAL_SEARCH_LENGTH = 2;

        public AddDataViewModel()
        {
            purchase = new Purchase();

            AddCommand("Save", new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute));

            
            StoreService.HistoryUpdated += UpdatePurchaseHistory;

            PurchaseType = TypeSelectorItems.FirstOrDefault();

            actualCalculationTarget = Services.DataService.CalculationTargetProperty.TotalCost;

            Status.Post("Все готово для роботи!");
        }

        void UpdatePurchaseHistory(object sender, EventArgs e)
        {
            PurchaseHistoryItems = new ObservableCollection<Purchase>(StoreService.PurchaseHistory);
        }

        private void SaveCommandExecute(object obj)
        {
            purchase.Name = purchase.Name.Trim();
            if (StoreService.AddPurchase(purchase))
            {
                logger.Info("Purchase saved");
                Status.Post("Покупка \"{0}\" збережена", PurchaseName);
                CleanInputFields();
            }
            else
            {
                logger.Warn("Purchase not saved");
                Status.Post("Помилка: покупка \"{0}\" не збережена", PurchaseName);
            }
        }
        private bool SaveCommandCanExecute(object obj)
        {
            return
                !string.IsNullOrWhiteSpace(purchase.Name) &&
                !string.IsNullOrWhiteSpace(Count) &&
                !string.IsNullOrWhiteSpace(ItemCost) &&
                !string.IsNullOrWhiteSpace(TotalCost);
        }
        private void CleanInputFields()
        {
            PurchaseName = string.Empty;
            Count = string.Empty;
            ItemCost = string.Empty;
            TotalCost = string.Empty;
        }
        private void SearchPurchase()
        {
            if (purchase.Name.Length < MINIMAL_SEARCH_LENGTH)
            {
                return;
            }
            var exactPurchases = StoreService.PurchaseHistory.Where(p => p.Name.Equals(purchase.Name, StringComparison.InvariantCultureIgnoreCase));
            IEnumerable<Purchase> resultList;
            if (exactPurchases.Count() > 0)
            {
                resultList = exactPurchases;

                PurchaseType = resultList.FirstOrDefault().Type;
            }
            else
            {
                resultList = StoreService.PurchaseHistory.Where(p => p.Name.StartsWith(PurchaseName, true, CultureInfo.InvariantCulture));
            }
            PurchaseHistoryItemsWrapper = resultList.OrderByDescending(p => p.Date).Take(10);
        }
        private void DoCalculations()
        {
            try
            {
                fieldCalculationInProgress = true;

                if (purchase != null)
                {
                    purchase.ItemsNumber = StringHelper.ToNumber(Count);
                    purchase.ItemCost = StringHelper.ToNumber(ItemCost);
                    purchase.TotalCost = StringHelper.ToNumber(TotalCost);

                    DataService.PerformCalculation(purchase, actualCalculationTarget);

                    if (actualCalculationTarget == Services.DataService.CalculationTargetProperty.ItemCost)
                    {
                        ItemCost = purchase.ItemCost != 0 ? purchase.ItemCost.ToString() : null;
                    }
                    if (actualCalculationTarget == Services.DataService.CalculationTargetProperty.TotalCost)
                    {
                        TotalCost = purchase.TotalCost != 0 ? purchase.TotalCost.ToString() : null;
                    }
                    if (actualCalculationTarget == Services.DataService.CalculationTargetProperty.ItemsNumber)
                    {
                        Count = purchase.ItemsNumber != 0 ? purchase.ItemsNumber.ToString() : null;
                    }

                    OnPropertyChanged(() => Count);
                    OnPropertyChanged(() => ItemCost);
                    OnPropertyChanged(() => TotalCost);
                }
            }
            finally
            {
                fieldCalculationInProgress = false;
            }
            
        }

        public DateTime DateToStore
        {
            get
            {
                return purchase.Date;
            }
            set
            {
                if (value != purchase.Date)
                {
                    purchase.Date = value;
                    OnPropertyChanged(() => DateToStore);
                }
            }
        }
        private IEnumerable<Purchase> PurchaseHistoryItemsWrapper
        {
            set
            {
                PurchaseHistoryItems = new ObservableCollection<Purchase>(value);
                ShowPurchaseHistory = PurchaseHistoryItems.Count() > 0;
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

        private bool fieldCalculationInProgress = false;
        private string count;
        public string Count
        {
            get { return count; }
            set
            {
                if ((count != value && StringHelper.IsNumber(value)) || value == string.Empty)
                {
                    count = StringHelper.GetCorrected(value);
                    if (!fieldCalculationInProgress)
                    {
                        DoCalculations();
                    }
                }
            }
        }

        string itemCost;
        public string ItemCost
        {
            get { return itemCost; }
            set
            {
                if ((itemCost != value && StringHelper.IsNumber(value)) || value == string.Empty)
                {
                    itemCost = StringHelper.GetCorrected(value, 2);
                    if (!fieldCalculationInProgress)
                    {
                        DoCalculations();
                    }
                }
            }
        }

        string totalCost;
        public string TotalCost
        {
            get { return totalCost; }
            set
            {
                if ((totalCost != value && StringHelper.IsNumber(value)) || value == string.Empty)
                {
                    totalCost = StringHelper.GetCorrected(value, 2);
                    if (!fieldCalculationInProgress)
                    {
                        DoCalculations();
                    }
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
                var type = TypeSelectorItems.Where(e => e.Name == value.Name).FirstOrDefault();
                if (type != purchase.Type)
                {
                    purchase.Type = type;
                    OnPropertyChanged(() => PurchaseType);
                }
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
