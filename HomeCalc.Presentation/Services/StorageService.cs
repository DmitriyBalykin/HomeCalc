using HomeCalc.Core.Helpers;
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

        private ListSafe<Purchase> purchaseHistory;

        private MessageDispatcher MsgDispatcher;

        private StatusService Status;

        private Cache<ProductType> ProductTypesCache = new Cache<ProductType>();
        private Cache<ProductSubType> ProductSubTypesCache = new Cache<ProductSubType>();
        private Cache<SettingsModel> SettingsCache = new Cache<SettingsModel>();

        private Logger logger;

        public StorageService()
        {
            MsgDispatcher = MessageDispatcher.GetInstance();
            logger = LogService.GetLogger(); ;
            DBService = DataBaseService.GetInstance();
            Status = StatusService.GetInstance();

            logger.Debug("Storage service initiated");
            logger.Debug("Starting history update");
            Task.Factory.StartNew(async () => await UpdateHistory());
            purchaseHistory = new ListSafe<Purchase>();
        }

        private async Task UpdateHistory()
        {
            try
            {
                logger.Debug("Purchase history update started");
                purchaseHistory.AddRange(await LoadPurchaseList(SearchRequestModel.Requests.Empty).ConfigureAwait(false));
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
            MsgDispatcher.Post("historyUpdated");
        }

        public static StorageService GetInstance()
        {
            if (instance == null)
            {
                instance = new StorageService();
            }
            return instance;
        }
        #region Settings
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
            return SettingsCache.GetCache();
        }
        #endregion

        #region Purchase
        public async Task<Purchase> AddPurchase(Purchase purchase)
        {
            //Product always has name
            var productId = await DBService.SaveProduct(ProductToModel(purchase)).ConfigureAwait(false);
            if (productId < 1)
            {
                logger.Error("AddPurchase: Error occured during product saving: {0}", purchase.Name);
                return null;
            }
            
            if (!string.IsNullOrEmpty(purchase.StoreName))
            {
                purchase.StoreId = await DBService.SaveStore(
                    new StoreModel 
                    {
                        Id = purchase.StoreId,
                        Name = StringUtilities.EscapeStringForDatabase(purchase.StoreName)
                    }).ConfigureAwait(false);
                if (purchase.StoreId < 1)
                {
                    logger.Error("AddPurchase: Error occured during store saving: {0}", purchase.Name);
                    return null;
                }
            }

            var purchaseId = await DBService.SavePurchase(PurchaseToModel(purchase, productId, purchase.StoreId)).ConfigureAwait(false);
            if (purchaseId > 0)
            {
                purchaseHistory.Add(new Purchase(purchase));
                AnnounceHistoryUpdate();
            }
            else
            {
                return null;
            }
            if (!string.IsNullOrEmpty(purchase.PurchaseComment))
            {
                long commentId = await DBService.SaveComment(
                    new CommentModel
                    {
                        PurchaseId = purchaseId,
                        StoreId = purchase.StoreId,
                        Text = StringUtilities.EscapeStringForDatabase(purchase.PurchaseComment),
                        Rate = purchase.PurchaseRate
                    }).ConfigureAwait(false);
                if (commentId < 1)
                {
                    logger.Error("AddPurchase: Error occured during comment saving: {0}", purchase.Name);
                    return null;
                }
            }
            //Store comment not binded to purchase
            if (!string.IsNullOrEmpty(purchase.StoreComment))
            {
                long storeCommentId = await DBService.SaveComment(
                    new CommentModel
                    {
                        PurchaseId = purchase.Id,
                        StoreId = purchase.StoreId,
                        Text = StringUtilities.EscapeStringForDatabase(purchase.StoreComment),
                        Rate = purchase.StoreRate
                    }).ConfigureAwait(false);
                if (storeCommentId < 1)
                {
                    logger.Error("AddPurchase: Error occured during store comment saving: {0}", purchase.Name);
                    return null;
                }
            }

            purchase.Id = purchaseId;

            return purchase;
        }
        public async Task<bool> UpdatePurchase(Purchase purchase)
        {
            if (await DBService.SaveProduct(ProductToModel(purchase)).ConfigureAwait(false) != -1)
            {
                Status.Post("Запис \"{0}\" оновлено", purchase.Name);
                return true;
            }
            return false;
        }
        public async Task<bool> DeletePurchase(Purchase purchase)
        {
            if (await DBService.DeletePurchase(purchase.Id).ConfigureAwait(false))
            {
                Status.Post("Запис \"{0}\" видалено", purchase.Name);
                return true;
            }
            return false;
        }
        [Obsolete("For Test purposes only!!!")]
        public async Task<bool> DeletePurchase(string purchaseName)
        {
            if (await DBService.DeletePurchase(purchaseName).ConfigureAwait(false))
            {
                Status.Post("Запис \"{0}\" видалено", purchaseName);
                return true;
            }
            return false;
        }
        public async Task<Purchase> LoadPurchase(long id)
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
                    foreach (var model in modelsList)
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
        #endregion
        #region Product
        #endregion
        #region ProductType
        public async Task<long> SaveProductType(ProductType productType)
        {
            ProductTypesCache.SetNeedRefresh();
            var result = await DBService.SaveProductType(TypeToModel(productType)).ConfigureAwait(false);
            if (result > 0)
            {
                TypeUpdated();
            }
            return result;
        }

        public async Task<bool> RemoveProductType(ProductType pType)
        {
            var result = await DBService.DeleteProductType(TypeToModel(pType)).ConfigureAwait(false);
            if (result)
            {
                TypeUpdated();
            }
            return result;
        }

        public async Task<long> RenameProductType(ProductType pType, string newProductTypeName)
        {
            pType.Name = newProductTypeName;
            var result = await DBService.SaveProductType(TypeToModel(pType)).ConfigureAwait(false);
            if (result > 0)
            {
                TypeUpdated();
            }
            return result;
        }
        public async Task<List<ProductType>> LoadProductTypeList()
        {
            if (!ProductTypesCache.IsActual())
            {
                ProductTypesCache.SetCache(
                    (await DBService.LoadProductTypeList().ConfigureAwait(false)).Select(p => ModelToType(p)).ToList()
                    );
            }
            return ProductTypesCache.GetCache();
        }
        public async Task<ProductType> ResolveProductType(long id = -1, string name = null)
        {
            if (id > -1)
            {
                // logger.Debug("StorageService: resolving product type without name");
                return (await LoadProductTypeList()).Where(type => type.Id == id).SingleOrDefault();
            }
            else if (name != null)
            {
                // logger.Debug("StorageService: resolving product type with name");
                var matchedType = ProductTypesCache.GetCache().Where(type => type.Name == name).SingleOrDefault();
                return matchedType;
            }
            else
            {
                logger.Debug("StorageService: type resolving failed, incorrect id: {0}", id);
                return null;
            }
        }
        public async Task<ProductSubType> ResolveProductSubType(long id = -1, string name = null)
        {
            if (id > 0)
            {
                logger.Debug("StorageService: resolving product sub type without name");
                return (await LoadProductSubTypeList()).Where(type => type.Id == id).SingleOrDefault();
            }
            else if (name != null)
            {
                logger.Debug("StorageService: resolving product sub type with name");
                var matchedType = ProductSubTypesCache.GetCache().Where(type => type.Name == name).SingleOrDefault();
                return matchedType;
            }
            else
            {
                logger.Debug("StorageService: sub type resolving failed, incorrect id: {0}", id);
                return null;
            }
        }
        internal async Task<bool> RemoveProduct(long productId)
        {
            bool result = await DBService.DeleteProduct(productId).ConfigureAwait(false);
            if (result)
            {
                purchaseHistory.Remove(new Purchase { Id = productId });
            }
            return result;
        }
        #endregion
        #region ProductSubType
        public async Task<long> SaveProductSubType(ProductSubType productSubType)
        {
            ProductSubTypesCache.SetNeedRefresh();
            var result = await DBService.SaveProductSubType(SubTypeToModel(productSubType)).ConfigureAwait(false);
            if (result > 0)
            {
                SubTypeUpdated();
            }
            return result;
        }

        public async Task<bool> RemoveProductSubType(ProductSubType pSubType)
        {
            ProductSubTypesCache.SetNeedRefresh();
            var result = await DBService.DeleteProductSubType(SubTypeToModel(pSubType)).ConfigureAwait(false);
            if (result)
            {
                SubTypeUpdated();
            }
            return result;
        }

        public async Task<List<ProductSubType>> LoadProductSubTypeList(long typeId = 0)
        {
            if (!ProductSubTypesCache.IsActual())
            {
                ProductSubTypesCache.SetCache(
                    (await DBService.LoadProductSubTypeList().ConfigureAwait(false)).Select(p => ModelToSubType(p)).ToList()
                    );
            }
            var result = ProductSubTypesCache.GetCache();
            if (typeId != 0)
            {
                result = result.Where(sType => sType.TypeId == typeId).ToList();
            }

            return result;
        }

        #endregion
        #region Store
        #endregion
        #region Comment
        #endregion

        private ProductType ModelToType(ProductTypeModel model)
        {
            return new ProductType { Id = (int)model.TypeId, Name = model.Name };
        }

        private ProductTypeModel TypeToModel(ProductType type)
        {
            return new ProductTypeModel { TypeId = type.Id, Name = StringUtilities.EscapeStringForDatabase(type.Name) };
        }
        private ProductSubType ModelToSubType(ProductSubTypeModel model)
        {
            return new ProductSubType { Id = (int)model.Id, Name = StringUtilities.EscapeStringForDatabase(model.Name), TypeId = model.TypeId };
        }

        private ProductSubTypeModel SubTypeToModel(ProductSubType subType)
        {
            if (subType == null)
            {
                return null;
            }
            return new ProductSubTypeModel { Id = subType.Id, Name = StringUtilities.EscapeStringForDatabase(subType.Name), TypeId = subType.TypeId };
        }
        private ProductModel ProductToModel(Purchase purchase)
        {
            return new ProductModel
            {
                Name = StringUtilities.EscapeStringForDatabase(purchase.Name),
                Type = TypeToModel(purchase.Type),
                SubType = SubTypeToModel(purchase.SubType),
                IsMonthly = purchase.IsMonthly
            };
        }
        private async Task<Purchase> ModelToPurchase(PurchaseModel model)
        {
            logger.Debug("StorageService: converting purchase storage model to purchase");
            if (model == null)
            {
                return null;
            }
            return new Purchase
            {
                Id = (int)model.Id,
                Date = new DateTime(model.Timestamp),
                Name = model.ProductName,
                ItemCost = model.ItemCost,
                TotalCost = model.TotalCost,
                ItemsNumber = model.ItemsNumber,
                PurchaseComment = model.Comment,
                PurchaseRate = model.Rate,
                StoreName = model.StoreName,
                IsMonthly = model.IsMonthly,
                Type = await ResolveProductType(model.TypeId),
                SubType = await ResolveProductSubType(model.SubTypeId)
            };
        }
        private PurchaseModel PurchaseToModel(Purchase purchase, long productId, long storeId)
        {
            return new PurchaseModel
            {
                Id = purchase.Id,
                ProductId = productId,
                StoreId = storeId,
                Timestamp = purchase.Date.Ticks,
                ItemsNumber = purchase.ItemsNumber,
                ItemCost = purchase.ItemCost,
                TotalCost = purchase.TotalCost,
                Rate = purchase.PurchaseRate
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
            MsgDispatcher.Post("typesUpdated");
        }

        private void SubTypeUpdated()
        {
            MsgDispatcher.Post("subTypesUpdated");
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
