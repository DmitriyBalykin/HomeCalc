using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class AddDataViewModel : ViewModel
    {
        public AddDataViewModel()
        {
            logger = LogService.GetLogger();
            AddCommand("Save", new DelegateCommand(SaveCommandExecute));

            typeSelectorItems = new List<PurchaseType>();
            typeSelectorItems.Add(new PurchaseType { Id = 0, Name = "Еда" });
            typeSelectorItems.Add(new PurchaseType { Id = 1, Name = "Хозяйственные товары" });
            typeSelectorItems.Add(new PurchaseType { Id = 2, Name = "Автомобиль" });
            typeSelectorItems.Add(new PurchaseType { Id = 3, Name = "Квартира" });
            typeSelectorItems.Add(new PurchaseType { Id = 4, Name = "Снаряжение" });
        }

        private void SaveCommandExecute(object obj)
        {
            //throw new NotImplementedException();
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
        public string Count { get; set; }
        public string ItemCount { get; set; }
        public string TotalCost { get; set; }

        private bool calcTotalCost = true;
        private bool calcItemCost = false;
        public bool CalcItemCost
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
                    OnPropertyChanged(() => CalcItemCost);
                }
                if (value)
                {
                    CalcItemsCount = false;
                    calcTotalCost = false;
                }
                else
                {
                    setTotalCalc();
                }
            }
        }
        private bool calcItemsCount = false;
        public bool CalcItemsCount
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
                    OnPropertyChanged(() => CalcItemsCount);
                }
                if (value)
                {
                    CalcItemCost = false;
                    calcTotalCost = false;
                }
                else
                {
                    setTotalCalc();
                }
            }
        }
        private void setTotalCalc()
        {
            if (!calcItemsCount && !calcItemCost)
            {
                calcTotalCost = true;
            }
        }
    }
}
