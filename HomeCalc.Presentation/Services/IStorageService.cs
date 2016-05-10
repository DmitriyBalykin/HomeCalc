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
        Task<List<PurchaseType>> LoadPurchaseTypeList();
        Task<IEnumerable<SettingsStorageModel>> LoadSettings();
        Task<PurchaseType> ResolvePurchaseType(int id = -1, string name = null);
        Task<bool> SavePurchaseBulk(List<Purchase> purchases);
        Task<bool> SavePurchaseType(PurchaseType purchaseType);
        Task<bool> SaveSettings(SettingsStorageModel settings);
        event EventHandler TypesUpdated;
        Task<bool> UpdatePurchase(Purchase purchase);
    }
}
