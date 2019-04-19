using HomeCalc.Presentation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Services
{
    interface IStorageService
    {
        Task<Purchase> AddPurchase(Purchase purchase);
        Task<Purchase> LoadPurchase(long id);
        Task<IList<Purchase>> LoadPurchaseList(SearchRequestModel filter);
        Task<IList<Purchase>> LoadPurchaseList(SearchRequestModel.Requests enumFilter);
        Task<IList<ProductType>> LoadProductTypeList();
        Task<IList<SettingsModel>> LoadSettings();
        Task<ProductType> ResolveProductType(long id, string name);
        Task<long> SaveProductType(ProductType purchaseType);
        Task<bool> SaveSettings(SettingsModel settings);
    }
}
