using HomeCalc.Core.LogService;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Model.DataModels;
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

            List<PurchaseTypeModel> types = new List<PurchaseTypeModel>();
            types.Add(new PurchaseTypeModel { Id = 0, Name = "Еда" });
            types.Add(new PurchaseTypeModel { Id = 1, Name = "Хозяйственные товары" });
            types.Add(new PurchaseTypeModel { Id = 2, Name = "Автомобиль" });
            types.Add(new PurchaseTypeModel { Id = 3, Name = "Квартира" });
            types.Add(new PurchaseTypeModel { Id = 4, Name = "Снаряжение" });

            searchResultList = new List<PurchaseModel>();
           // searchResultList.Add(new Purchase { Id = 0, Ty });
            
        }

        private List<PurchaseModel> searchResultList;
        public ObservableCollection<PurchaseModel> SearchResultList
        {
            get
            {
                return new ObservableCollection<PurchaseModel>(searchResultList);
            }
        }
    }
}
