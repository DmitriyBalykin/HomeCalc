using HomeCalc.Core.LogService;
using HomeCalc.Core.Services;
using HomeCalc.Core.Utilities;
using HomeCalc.Model.DataModels;
using HomeCalc.Model.DbService;
using HomeCalc.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class StorageService : IStorageService
    {
        DataBaseService DBService;
        private static StorageService instance;

        private List<Purchase> purchaseHistory;

        public event EventHandler TypesUpdated;
        public event EventHandler HistoryUpdated;

        private StatusService Status;

        private Cache<PurchaseType> PurchaseTypesCache = new Cache<PurchaseType>();
        private Cache<SettingsModel> SettingsCache = new Cache<SettingsModel>();

        private Logger logger;

        public StorageService()
        {
            logger = LogService.GetLogger(); ;
            DBService = DataBaseService.GetInstance();
            Status = StatusService.GetInstance();

            logger.Debug("Storage service initiated");
            logger.Debug("Starting history update");
            Task.Factory.StartNew(async () => await UpdateHistory());
        }

        private async Task UpdateHistory()
        {
            try
            {
                logger.Debug("Purchase history update started");
                purchaseHistory = await LoadPurchaseList(SearchRequestModel.Requests.Empty).ConfigureAwait(false);
                logger.Debug("Purchase data loaded");
                if (purchaseHistory != null && purchaseHistory.Count() > 0)
                {
                    AnnounceHistoryUpdate();
                    logger.Debug("Purchase history updated");
                }
                else
                {
                    logger.Debug("No purchase history updates found");
                }
            }
            catch (Exception ex)
            {
                logger.Error("History update failed");
                logger.Error(ex.Message);
            }
            
        }

        private void AnnounceHistoryUpdate()
        {
            logger.Debug("Announce history update");
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

        public async Task<bool> SaveSettings(SettingsModel settings)
        {
            SettingsCache.SetNeedRefresh();
            return await DBService.SaveSettings(SettingToStorage(settings)).ConfigureAwait(false);
        }
        public async Task<List<SettingsModel>> LoadSettings()
        {
            var settings = (await DBService.LoadSettings().ConfigureAwait(false)).Select(settingModel => StorageToSetting(settingModel)).ToList();
            if (!SettingsCache.IsActual())
            {
                SettingsCache.SetCache(settings);
            }
            return settings;
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
            PurchaseTypesCache.SetNeedRefresh();
            var result = await DBService.SavePurchaseType(TypeToModel(purchaseType)).ConfigureAwait(false);
            if (result)
            {
                TypeUpdated();
            }
            return result;
        }

        public async Task<bool> RemovePurchaseType(PurchaseType pType)
        {
            var result = await DBService.DeletePurchaseType(TypeToModel(pType)).ConfigureAwait(false);
            if (result)
            {
                TypeUpdated();
            }
            return result;
        }

        public async Task<bool> RenamePurchaseType(PurchaseType pType, string newPurchaseTypeName)
        {
            pType.Name = newPurchaseTypeName;
            var result = await DBService.SavePurchaseType(TypeToModel(pType)).ConfigureAwait(false);
            if (result)
            {
                TypeUpdated();
            }
            return result;
        }

        public async Task<Purchase> LoadPurchase(int id)
        {
            return await ModelToPurchase(await DBService.LoadPurchase(id).ConfigureAwait(false));
        }
        public async Task<List<Purchase>> LoadPurchaseList(SearchRequestModel.Requests enumFilter)
        {
            var list = new List<Purchase>();
            switch (enumFilter)
            {
                case SearchRequestModel.Requests.Empty:
                    logger.Debug("Selected load purchase with no filter");
                    var modelsList = await DBService.LoadCompletePurchaseList().ConfigureAwait(false);
                    foreach(var model in modelsList)
                    {
                        list.Add(await ModelToPurchase(model));
                    }
                    break;
            }
            logger.Debug("StorageService: purchase list loaded");
            return list;
        }
        public async Task<List<Purchase>> LoadPurchaseList(SearchRequestModel filter)
        {
            var convertedList = new List<Purchase>();

            var modelsList = await DBService.LoadPurchaseList(filter).ConfigureAwait(false);
            foreach (var model in modelsList)
            {
                convertedList.Add(await ModelToPurchase(model));
            }

            return convertedList;
        }
        public async Task<List<PurchaseType>> LoadPurchaseTypeList()
        {
            if (!PurchaseTypesCache.IsActual())
            {
                PurchaseTypesCache.SetCache(
                    (await DBService.LoadPurchaseTypeList().ConfigureAwait(false)).Select(p => ModelToType(p)).ToList()
                    );
            }
            return PurchaseTypesCache.GetCache();
        }
        public async Task<PurchaseType> ResolvePurchaseType(long id = -1, string name = null)
        {
            if (id > -1)
            {
                logger.Debug("StorageService: resolving purchase type without name");
                return (await LoadPurchaseTypeList()).Where(type => type.TypeId == id).SingleOrDefault();
            }
            else if (name != null)
            {
                logger.Debug("StorageService: resolving purchase type with name");
                var matchedType = PurchaseTypesCache.GetCache().Where(type => type.Name == name).SingleOrDefault();
                return matchedType;
            }
            else
            {
                logger.Debug("StorageService: resolving failed, incorrect id: {0}", id);
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
        private async Task<Purchase> ModelToPurchase(PurchaseModel model)
        {
            logger.Debug("StorageService: converting purchase storage model to purchase");
            return new Purchase {
                Id = (int)model.PurchaseId,
                Date = new DateTime(model.Timestamp),
                ItemCost = model.ItemCost,
                ItemsNumber = model.ItemsNumber,
                Name = model.Name,
                TotalCost = model.TotalCost,
                Type = await ResolvePurchaseType(model.TypeId)
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
        private SettingsStorageModel SettingToStorage(SettingsModel settings)
        {
            return new SettingsStorageModel
            {
                ProfileId = 0,
                SettingId = settings.SettingId,
                SettingName = settings.SettingName,
                SettingValue = string.IsNullOrEmpty(settings.SettingStringValue) ? settings.SettingBoolValue.ToString() : settings.SettingStringValue
            };
        }
        private SettingsModel StorageToSetting(SettingsStorageModel model)
        {
            bool value;
            bool isBool = bool.TryParse(model.SettingValue, out value);
            return new SettingsModel
            {
                SettingId = model.SettingId,
                SettingName = model.SettingName,
                SettingBoolValue = value,
                SettingStringValue = isBool ? string.Empty : model.SettingValue
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

}
