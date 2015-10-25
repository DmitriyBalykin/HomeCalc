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

namespace HomeCalc.Presentation.ViewModels
{
    public class AddDataViewModel : ViewModel
    {
        public AddDataViewModel()
        {
        //    logger = LogService.GetLogger();
            AddCommand("Save", new DelegateCommand(SaveCommandExecute));

            typeSelectorItems = StoreService.LoadPurchaseTypeList();

            actualCalculation = CalcTotalCost;

            Status.Post("Завантажено");
        }

        private void SaveCommandExecute(object obj)
        {
            if (StoreService.SavePurchase(
                new Purchase {
                    Date = DateTime.Now,
                    ItemCost = double.Parse(ItemCost),
                    ItemsNumber = double.Parse(Count),
                    TotalCost = double.Parse(TotalCost),
                    Name = PurchaseName,
                    Type = PurchaseType
                }))
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

        private IList<PurchaseType> typeSelectorItems;
        public ObservableCollection<PurchaseType> TypeSelectorItems
        { 
            get
            {
                return new ObservableCollection<PurchaseType>(typeSelectorItems); 
            }
        }

        public string PurchaseName { get; set; }
        private string count;
        private string itemCost;
        private string totalCost;
        public string Count
        {
            get { return count; }
            set
            {
                if (value != count)
                {
                    count = value;
                    OnPropertyChanged(() => Count);
                    actualCalculation.Invoke();
                }
            }
        }
        public string ItemCost
        {
            get { return itemCost; }
            set
            {
                if (value != itemCost)
                {
                    itemCost = value;
                    OnPropertyChanged(() => ItemCost);
                    actualCalculation.Invoke();
                }
            }
        }
        public string TotalCost
        {
            get { return totalCost; }
            set
            {
                if (value != totalCost)
                {
                    totalCost = value;
                    OnPropertyChanged(() => TotalCost);
                    actualCalculation.Invoke();
                }
            }
        }
        private PurchaseType purchaseType;
        public PurchaseType PurchaseType
        {
            get
            {
                return purchaseType; 
            }
            set
            {
                purchaseType = value;
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
                    actualCalculation = CalcItemCost;
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
                    actualCalculation = CalcItemCount;
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
                }
                if (value)
                {
                    actualCalculation = CalcTotalCost;
                }
                OnPropertyChanged(() => IsCalcByTotal);
            }
        }

        private void CalcItemCount() {
            if (!calcInProgress && !(string.IsNullOrEmpty(TotalCost) || string.IsNullOrEmpty(ItemCost)))
            {
                calcInProgress = true;
                Count = (double.Parse(TotalCost) / double.Parse(ItemCost)).ToString();
                calcInProgress = false;
            }
        }
        private void CalcItemCost()
        {
            if (!calcInProgress && !(string.IsNullOrEmpty(Count) || string.IsNullOrEmpty(TotalCost)))
            {
                calcInProgress = true;
                ItemCost = (double.Parse(TotalCost) / double.Parse(Count)).ToString();
                calcInProgress = false;
            }
        }
        private void CalcTotalCost()
        {
            if (!calcInProgress && !(string.IsNullOrEmpty(Count) || string.IsNullOrEmpty(ItemCost)))
            {
                calcInProgress = true;
                TotalCost = (double.Parse(Count) * double.Parse(ItemCost)).ToString();
                calcInProgress = false;
            }
        }

        private Action actualCalculation;
        private bool calcInProgress = false;
    }
}
