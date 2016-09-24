using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using HomeCalc.Presentation.Models;
using System.Linq;

namespace FunctionalTests
{
    [TestClass]
    public class FunctionalTest
    {
        private static StorageService StoreService;
        [TestMethod]
        public void DatabaseUpgradeTest()
        {
        }
        [TestMethod]
        public void DatabaseStateTest()
        {
            StoreService = StorageService.GetInstance();

            var purchase = new Purchase
            {
                Date = DateTime.Now,
                Id = 0,
                ItemCost = 10.4,
                ItemsNumber = 135.2,
                TotalCost = 1406.08,
                IsMonthly = true,
                Name = "Cote d'Azure apple",
                PurchaseComment = "Воно чудо'ве",
                PurchaseRate = 10,
                StoreComment = "Цікави'й магазин",
                StoreName = "Нову'с магазин",
                StoreRate = 5,
                Type = new ProductType { Name = "Їжа", TypeId = 1 },
                SubType = new ProductSubType { Name = "Фрукт", Id = 1 }
            };
            //Save test
            if (!StoreService.AddPurchase(purchase).Result)
            {
                throw new Exception("Purchase saving error");
            }
            //read test

            var searchRequest = new SearchRequestModel
            {
                Name = purchase.Name,
                SearchByName = true
            };
            var controlPurchase = (StoreService.LoadPurchaseList(searchRequest).Result).FirstOrDefault();
            if (!controlPurchase.DeepEquals(purchase))
            {
                throw new Exception("Purchase storage error: not consistent");
            }
            //update test
            purchase.IsMonthly = false;
            purchase.Name = "Звичайне яблуко";

            if (!StoreService.AddPurchase(purchase).Result)
            {
                throw new Exception("Purchase saving error");
            }
            //repeated read test
            var controlPurchase2 = StoreService.LoadPurchase(controlPurchase.Id).Result;
            if (!controlPurchase2.DeepEquals(purchase))
            {
                throw new Exception("Purchase storage error: not consistent");
            }

            //delete test
            if (!StoreService.DeletePurchase(purchase).Result)
            {
                throw new Exception("Purchase deletion error");
            }
            var deletedPurchase = StoreService.LoadPurchase(purchase.Id);
            if (deletedPurchase != null)
            {
                throw new Exception("Purchase deletion error: not deleted");
            }

        }
    }
}
