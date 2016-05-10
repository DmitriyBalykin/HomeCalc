using HomeCalc.Core;
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
            Task.Factory.StartNew(async () => await UpdateHistory());
        }

        private async Task UpdateHistory()
        {
            purchaseHistory = await LoadPurchaseList(SearchRequestModel.Requests.Empty).ConfigureAwait(false);
            if (purchaseHistory != null && purchaseHistory.Count() > 0)
            {
                AnnounceHistoryUpdate();
            }
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

        public async Task<bool> SaveSettings(SettingsStorageModel settings)
        {
            return await DBService.SaveSettings(settings).ConfigureAwait(false);
        }
        public async Task<IEnumerable<SettingsStorageModel>> LoadSettings()
        {
            return await DBService.LoadSettings().ConfigureAwait(false);
        }
        public async Task<bool> AddPurchase(Purchase purchase)
        {
            var result = await DBService.SavePurchase(PurchaseToModel(purchase)).ConfigureAwait(false);
            if (result)
            {
                purchaseHistory.Add(new Purchase(purchase));
                AnnounceHistoryUpdate();
            }
            return result;
        }
        public async Task<bool> UpdatePurchase(Purchase purchase)
        {
            if (await DBService.SavePurchase(PurchaseToModel(purchase)).ConfigureAwait(false))
            {
                Status.Post("Запис \"{0}\" оновлено", purchase.Name);
                return true;
            }
            return false;
        }
        public async Task<bool> SavePurchaseBulk(List<Purchase> purchases)
        {
            var result = await DBService.SavePurchaseBulk(purchases.Select(p => PurchaseToModel(p))).ConfigureAwait(false);
            if (result)
            {
                purchaseHistory.AddRange(purchases);
                AnnounceHistoryUpdate();
            }
            return result;
        }
        public async Task<bool> SavePurchaseType(PurchaseType purchaseType)
        {
            PurchaseTypesCache.IsActual = false;
            var result = await DBService.SavePurchaseType(TypeToModel(purchaseType)).ConfigureAwait(false);
            if (result)
            {
                TypeUpdated();
            }
            return result;
        }
        public async Task<Purchase> LoadPurchase(int id)
        {
            return ModelToPurchase(await DBService.LoadPurchase(id).ConfigureAwait(false));
        }
        public async Task<List<Purchase>> LoadPurchaseList(SearchRequestModel.Requests enumFilter)
        {
            var list = new List<Purchase>();
            switch (enumFilter)
            {
                case SearchRequestModel.Requests.Empty:
                    list = (await DBService.LoadCompletePurchaseList().ConfigureAwait(false)).Select(p => ModelToPurchase(p)).ToList();
                    break;
            }
            return list;
        }
        public async Task<List<Purchase>> LoadPurchaseList(SearchRequestModel filter)
        {
            return (await DBService.LoadPurchaseList(filter).ConfigureAwait(false)).Select(p => ModelToPurchase(p)).ToList();
        }
        public async Task<List<PurchaseType>> LoadPurchaseTypeList()
        {
            if (!PurchaseTypesCache.IsActual)
            {
                PurchaseTypesCache.Cache = (await DBService.LoadPurchaseTypeList().ConfigureAwait(false)).Select(p => ModelToType(p)).ToList();
            }
            return PurchaseTypesCache.Cache;
        }
        public async Task<PurchaseType> ResolvePurchaseType(int id = -1, string name = null)
        {
            if (id > -1)
            {
                return (await LoadPurchaseTypeList().ConfigureAwait(false)).Where(type => type.TypeId == id).SingleOrDefault();
            }
            else if (name != null)
            {
                var matchedType = (await LoadPurchaseTypeList().ConfigureAwait(false)).Where(type => type.Name == name).SingleOrDefault();
                if (matchedType == null)
                {
                    await SavePurchaseType(new PurchaseType {
                        Name = name
                    });
                    //type id generated by DBMS as Primary Key
                    matchedType = (await LoadPurchaseTypeList().ConfigureAwait(false)).Where(type => type.Name == name).SingleOrDefault();
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

        internal async Task<bool> RemovePurchase(int purchaseId)
        {
            bool result = await DBService.RemovePurchase(purchaseId).ConfigureAwait(false);
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
