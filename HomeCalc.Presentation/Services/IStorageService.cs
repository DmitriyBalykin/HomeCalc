using HomeCalc.Model.DataModels;
using HomeCalc.Model.DbConnectionWrappers;
using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace HomeCalc.Presentation.Services
{
    interface IStorageService
    {
        Task<bool> AddPurchase(Purchase purchase);
        event EventHandler HistoryUpdated;
        Task<Purchase> LoadPurchase(int id);
        Task<List<Purchase>> LoadPurchaseList(SearchRequestModel filter);
        Task<List<Purchase>> LoadPurchaseList(SearchRequestModel.Requests enumFilter);
        Task<List<ProductType>> LoadProductTypeList();
        Task<List<SettingsModel>> LoadSettings();
        Task<ProductType> ResolveProductType(long id, string name);
        //Task<bool> SavePurchaseBulk(List<Purchase> purchases);
        Task<bool> SaveProductType(ProductType purchaseType);
        Task<bool> SaveSettings(SettingsModel settings);
        event EventHandler TypesUpdated;
        Task<bool> UpdatePurchase(Purchase purchase);
    }
}
