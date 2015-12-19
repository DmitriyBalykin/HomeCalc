﻿using HomeCalc.Core;
using HomeCalc.Model.DataModels;
using HomeCalc.Model.DbService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class StorageService : HomeCalc.Presentation.Services.IStorageService
    {
        DataBaseService DBService;
        private static StorageService instance;

        private List<Purchase> purchaseHistory;

        public event EventHandler TypesUpdated;
        public event EventHandler HistoryUpdated;

        private StatusService Status;

        public StorageService()
        {
            DBService = DataBaseService.GetInstance();
            Status = StatusService.GetInstance();
            UpdateHistory();
        }

        private void UpdateHistory()
        {
            if (purchaseHistory == null)
            {
                purchaseHistory = new List<Purchase>();
            }
            Task.Factory.StartNew(() =>
            {
                purchaseHistory = LoadPurchaseList(SearchRequest.Requests.Empty);
            }).ContinueWith( t => AnnounceHistoryUpdate());
        }

        private void AnnounceHistoryUpdate()
        {
            if (HistoryUpdated != null)
            {
                HistoryUpdated(null, EventArgs.Empty);
            }
        }

        internal static StorageService GetInstance()
        {
            if (instance == null)
            {
                instance = new StorageService();
            }
            return instance;
        }

        public bool SaveSettings(SettingsModel settings)
        {
            return DBService.SaveSettings(settings);
        }
        public SettingsModel LoadSettings()
        {
            return DBService.LoadSettings();
        }
        public bool AddPurchase(Purchase purchase)
        {
            var result = DBService.AddPurchase(PurchaseToModel(purchase));
            if (result)
            {
                purchaseHistory.Add(new Purchase(purchase));
                AnnounceHistoryUpdate();
            }
            return result;
        }
        public bool UpdatePurchase(Purchase purchase)
        {
            if (DBService.UpdatePurchase(PurchaseToModel(purchase)))
            {
                Status.Post("Запис \"{0}\" оновлено", purchase.Name);
                return true;
            }
            return false;
        }
        public bool SavePurchaseBulk(List<Purchase> purchases)
        {
            var result = DBService.SavePurchaseBulk(purchases.Select(p => PurchaseToModel(p)));
            if (result)
            {
                purchaseHistory.AddRange(purchases);
                AnnounceHistoryUpdate();
            }
            return result;
        }
        public bool SavePurchaseType(PurchaseType purchaseType)
        {
            PurchaseTypesCache.IsActual = false;
            var result = DBService.SavePurchaseType(TypeToModel(purchaseType));
            if (result)
            {
                TypeUpdated();
            }
            return result;
        }
        public Purchase LoadPurchase(int id)
        {
            return ModelToPurchase(DBService.LoadPurchase(id));
        }
        public List<Purchase> LoadPurchaseList(SearchRequest.Requests enumFilter)
        {
            var list = new List<Purchase>();
            switch (enumFilter)
            {
                case SearchRequest.Requests.Empty:
                    list = DBService.LoadCompletePurchaseList().Select(p => ModelToPurchase(p)).ToList();
                    break;
            }
            return list;
        }
        public List<Purchase> LoadPurchaseList(SearchRequest filter)
        {
            return DBService.LoadPurchaseList(
                p => (!filter.SearchByName || p.Name.Contains(filter.NameFilter)) &&
                     (!filter.SearchByType || p.TypeId == filter.Type.TypeId) &&
                     (!filter.SearchByDate || (p.Timestamp > filter.DateStart.Ticks) && (p.Timestamp <= filter.DateEnd.Ticks)) &&
                     (!filter.SearchByCost || (p.TotalCost >= filter.CostStart) && (p.TotalCost <= filter.CostEnd))
                ).Select(p => ModelToPurchase(p)).ToList();
        }
        public List<PurchaseType> LoadPurchaseTypeList()
        {
            if (!PurchaseTypesCache.IsActual)
            {
                PurchaseTypesCache.Cache = DBService.LoadPurchaseTypeList().Select(p => ModelToType(p)).ToList();
            }
            return PurchaseTypesCache.Cache;
        }
        public PurchaseType ResolvePurchaseType(int id = -1, string name = null)
        {
            if (id > -1)
            {
                return LoadPurchaseTypeList().Where(type => type.TypeId == id).SingleOrDefault();
            }
            else if (name != null)
            {
                var matchedType = LoadPurchaseTypeList().Where(type => type.Name == name).SingleOrDefault();
                if (matchedType == null)
                {
                    SavePurchaseType(new PurchaseType {
                        Name = name
                    });
                    //type id generated by DBMS as Primary Key
                    matchedType = LoadPurchaseTypeList().Where(type => type.Name == name).SingleOrDefault();
                }
                return matchedType;
            }
            else
            {
                return null;
            }
        }
        private PurchaseType ModelToType(PurchaseTypeModel model)
        {
            return new PurchaseType { TypeId = (int)model.TypeId, Name = model.Name };
        }
        private PurchaseTypeModel TypeToModel(PurchaseType type)
        {
            return new PurchaseTypeModel { TypeId = type.TypeId, Name = type.Name };
        }
        private Purchase ModelToPurchase(PurchaseModel model)
        {
            return new Purchase {
                Id = (int)model.PurchaseId,
                Date = new DateTime(model.Timestamp),
                ItemCost = model.ItemCost,
                ItemsNumber = model.ItemsNumber,
                Name = model.Name,
                TotalCost = model.TotalCost,
                Type = ModelToType(model.Type)
            };
        }
        private PurchaseModel PurchaseToModel(Purchase purchase)
        {
            return new PurchaseModel
            {
                PurchaseId = purchase.Id,
                Timestamp = purchase.Date.Ticks,
                Name = purchase.Name,
                ItemsNumber = purchase.ItemsNumber,
                ItemCost = purchase.ItemCost,
                TotalCost = purchase.TotalCost,
                TypeId = purchase.Type.TypeId
            };
        }

        private void TypeUpdated()
        {
            if (TypesUpdated != null)
            {
                TypesUpdated.Invoke(null, EventArgs.Empty);
            }
        }

        internal bool RemovePurchase(int purchaseId)
        {
            bool result = DBService.RemovePurchase(purchaseId);
            if (result)
            {
                purchaseHistory.Remove(new Purchase { Id = purchaseId });
            }
            return result;
        }

        public List<Purchase> PurchaseHistory
        {
            get
            {
                return purchaseHistory;
            }
        }
    }

    class PurchaseTypesCache
    {
        public static List<PurchaseType> Cache { get; set; }
        private static bool isActual;
        public static bool IsActual
        {
            get
            {
                return isActual && Cache.Count > 0;
            }
            set
            {
                isActual = value;
            }
        }
    }
}
