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
            //using (var db = dbManager.GetContext())
            //{

            //}
            return false;
        }
        public SettingsModel LoadSettings()
        {
            SettingsModel settings = null;
            //using (var db = dbManager.GetContext())
            //{

            //}
            return settings;
        }
        public bool SavePurchase(Purchase purchase)
        {
            try
            {
                DBService.SavePurchase(PurchaseToModel(purchase));
                return true;
            }
            catch (Exception)
            {
                
                return false;
            }
        }
        public bool SavePurchaseType(PurchaseType purchaseType)
        {
            try
            {
                DBService.SavePurchaseType(new PurchaseTypeModel 
                {
                    Name = purchaseType.Name
                });
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public Purchase LoadPurchase(int id)
        {
            Purchase purchase = null;

            return purchase;
        }
        public IEnumerable<PurchaseModel> LoadPurchaseList(object filter)
        {
            IEnumerable<PurchaseModel> list = null;

            return list;
        }
        public IList<PurchaseType> LoadPurchaseTypeList()
        {
            IList<PurchaseType> list = new List<PurchaseType>();
            try
            {
                return list = DBService.LoadPurchaseTypeList().Select(p => ModelToType(p)).ToList();
            }
            catch (Exception)
            {
                //Logger.Warn();
            }
            return list;
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
            return new Purchase {  };
        }
        private PurchaseModel PurchaseToModel(Purchase purchase)
        {
            return new PurchaseModel
            {
                Timestamp = (purchase.Date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds.ToString(),
                Name = purchase.Name,
                ItemsNumber = purchase.ItemsNumber,
                ItemCost = purchase.ItemCost,
                TotalCost = purchase.TotalCost,
                TypeId = purchase.Type.TypeId
            };
        }
    }
}
