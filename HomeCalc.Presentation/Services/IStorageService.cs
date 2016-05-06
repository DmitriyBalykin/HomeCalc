﻿using System;
namespace HomeCalc.Presentation.Services
{
    interface IStorageService
    {
        bool AddPurchase(HomeCalc.Presentation.Models.Purchase purchase);
        event EventHandler HistoryUpdated;
        HomeCalc.Presentation.Models.Purchase LoadPurchase(int id);
        System.Collections.Generic.List<HomeCalc.Presentation.Models.Purchase> LoadPurchaseList(HomeCalc.Presentation.Models.SearchRequestModel filter);
        System.Collections.Generic.List<HomeCalc.Presentation.Models.Purchase> LoadPurchaseList(HomeCalc.Presentation.Models.SearchRequestModel.Requests enumFilter);
        System.Collections.Generic.List<HomeCalc.Presentation.Models.PurchaseType> LoadPurchaseTypeList();
        HomeCalc.Model.DataModels.SettingsStorageModel LoadSettings();
        HomeCalc.Presentation.Models.PurchaseType ResolvePurchaseType(int id = -1, string name = null);
        bool SavePurchaseBulk(System.Collections.Generic.List<HomeCalc.Presentation.Models.Purchase> purchases);
        bool SavePurchaseType(HomeCalc.Presentation.Models.PurchaseType purchaseType);
        bool SaveSettings(HomeCalc.Model.DataModels.SettingsStorageModel settings);
        event EventHandler TypesUpdated;
        bool UpdatePurchase(HomeCalc.Presentation.Models.Purchase purchase);
    }
}
