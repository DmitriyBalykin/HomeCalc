using HomeCalc.Core.LogService;
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
    public class SearchComboViewModel : ViewModel
    {
        public SearchComboViewModel()
        {
            logger = LogService.GetLogger();

            List<PurchaseType> types = new List<PurchaseType>();
            types.Add(new PurchaseType { Id = 0, Name = "Еда" });
            types.Add(new PurchaseType { Id = 1, Name = "Хозяйственные товары" });
            types.Add(new PurchaseType { Id = 2, Name = "Автомобиль" });
            types.Add(new PurchaseType { Id = 3, Name = "Квартира" });
            types.Add(new PurchaseType { Id = 4, Name = "Снаряжение" });

            searchResultList = new List<Purchase>();
           // searchResultList.Add(new Purchase { Id = 0, Ty });
            
        }

        private List<Purchase> searchResultList;
        public ObservableCollection<Purchase> SearchResultList
        {
            get
            {
                return new ObservableCollection<Purchase>(searchResultList);
            }
        }
    }
}
