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
                Type = new ProductType { Name = "Їжа", Id = 0 },
                SubType = new ProductSubType { Name = "Фрукт", Id = 0 }
            };
            //Cleanup
            Assert.IsTrue(StoreService.DeletePurchase(purchase.Name).Result, "Cleanup failed");
            
            //Save test
            purchase = StoreService.AddPurchase(purchase).Result;
            Assert.IsTrue(purchase != null, "Purchase saving error");
            //read test

            var searchRequest = new SearchRequestModel
            {
                Name = purchase.Name,
                SearchByName = true
            };

            var foundList = StoreService.LoadPurchaseList(searchRequest).Result;
            Assert.IsTrue(foundList.Count == 1, "Purchase storage error: records duplicating found");

            var controlPurchase = foundList.FirstOrDefault();
            Assert.IsTrue(controlPurchase.DeepEquals(purchase), "Purchase storage error: not consistent");

            //update test
            purchase.IsMonthly = false;
            purchase.Name = "Звичайне яблуко";

            purchase = StoreService.AddPurchase(purchase).Result;
            Assert.IsTrue(purchase != null, "Purchase saving error");
            //repeated read test
            var controlPurchase2 = StoreService.LoadPurchase(controlPurchase.Id).Result;
            Assert.IsTrue(controlPurchase2.DeepEquals(purchase), "Purchase storage error: not consistent");

            //delete test
            Assert.IsTrue(StoreService.DeletePurchase(purchase).Result, "Purchase deletion error");

            var deletedPurchase = StoreService.LoadPurchase(purchase.Id);
            Assert.IsTrue (deletedPurchase == null, "Purchase deletion error: not deleted");

        }
    }
}
