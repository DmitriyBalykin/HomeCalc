using HomeCalc.Model.DataModels;
using HomeCalc.Model.DbService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class StorageService
    {
        DataService DBService;
        private static StorageService instance;
        public StorageService()
        {
            DBService = DataService.GetInstance();
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
        public bool SavePurchase(Purchase purchase)
        {
            return DBService.SavePurchase(PurchaseToModel(purchase));
        }
        public bool SavePurchaseType(PurchaseType purchaseType)
        {
            return DBService.SavePurchaseType(TypeToModel(purchaseType));
        }
        public Purchase LoadPurchase(int id)
        {
            return ModelToPurchase(DBService.LoadPurchase(id));
        }
        public IEnumerable<Purchase> LoadPurchaseList(object filter)
        {
            return DBService.LoadPurchaseList(filter).Select(p => ModelToPurchase(p));
        }
        public IList<PurchaseType> LoadPurchaseTypeList()
        {
            return DBService.LoadPurchaseTypeList().Select(p => ModelToType(p)).ToList();
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
                Timestamp = purchase.Date.Ticks,
                Name = purchase.Name,
                ItemsNumber = purchase.ItemsNumber,
                ItemCost = purchase.ItemCost,
                TotalCost = purchase.TotalCost,
                TypeId = purchase.Type.TypeId
            };
        }
    }
}
