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

            List<ProductTypeModel> types = new List<ProductTypeModel>();
            types.Add(new ProductTypeModel { TypeId = 0, Name = "Еда" });
            types.Add(new ProductTypeModel { TypeId = 1, Name = "Хозяйственные товары" });
            types.Add(new ProductTypeModel { TypeId = 2, Name = "Автомобиль" });
            types.Add(new ProductTypeModel { TypeId = 3, Name = "Квартира" });
            types.Add(new ProductTypeModel { TypeId = 4, Name = "Снаряжение" });

            searchResultList = new List<ProductModel>();
           // searchResultList.Add(new Purchase { Id = 0, Ty });
            
        }

        private List<ProductModel> searchResultList;
        public ObservableCollection<ProductModel> SearchResultList
        {
            get
            {
                return new ObservableCollection<ProductModel>(searchResultList);
            }
        }
    }
}
