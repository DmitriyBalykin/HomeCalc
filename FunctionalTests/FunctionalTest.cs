using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var purchase2 = new Purchase
            {
                Date = DateTime.Now,
                Id = 0,
                ItemCost = 17.5,
                ItemsNumber = 544,
                TotalCost = 9520,
                IsMonthly = false,
                Name = "Ozone Mantra M6",
                PurchaseComment = "Це палапрал",
                PurchaseRate = 8,
                StoreComment = "Завжди тут беру",
                StoreName = "Від Перевалова",
                StoreRate = 7,
                Type = new ProductType { Name = "Спорядження", Id = 0 },
                SubType = new ProductSubType { Name = "Льотне спорядження", Id = 0 }
            };
            var purchase3 = new Purchase
            {
                Date = DateTime.Now,
                Id = 0,
                ItemCost = 234,
                ItemsNumber = 53,
                TotalCost = 12402,
                IsMonthly = true,
                Name = "Помаранч",
                PurchaseComment = "Нічотак",
                PurchaseRate = 9,
                StoreComment = "Зайшов після роботи",
                StoreName = "Фора",
                StoreRate = 6,
                Type = new ProductType { Name = "Їжа", Id = 0 },
                SubType = new ProductSubType { Name = "Фрукт", Id = 0 }
            };
            var purchase4 = new Purchase
            {
                Date = DateTime.Now,
                Id = 0,
                ItemCost = 1,
                ItemsNumber = 2,
                TotalCost = 2,
                IsMonthly = false,
                Name = "Груша",
                Type = new ProductType { Name = "Їжа", Id = 0 }
            };
            //Cleanup
            Assert.IsTrue(StoreService.DeletePurchase(purchase.Name).Result, "Cleanup failed");
            Assert.IsTrue(StoreService.DeletePurchase(purchase2.Name).Result, "Cleanup failed");
            Assert.IsTrue(StoreService.DeletePurchase(purchase3.Name).Result, "Cleanup failed");
            Assert.IsTrue(StoreService.DeletePurchase(purchase4.Name).Result, "Cleanup failed");

            var foundListPre = StoreService.LoadPurchaseList(SearchRequestModel.Requests.Empty).Result;

            //Save test
            purchase = StoreService.AddPurchase(purchase).Result;
            Assert.IsNotNull(purchase, "Purchase saving error");
            purchase2 = StoreService.AddPurchase(purchase2).Result;
            Assert.IsNotNull(purchase, "Purchase saving error");
            purchase3 = StoreService.AddPurchase(purchase3).Result;
            Assert.IsNotNull(purchase, "Purchase saving error");
            purchase4 = StoreService.AddPurchase(purchase4).Result;
            Assert.IsNotNull(purchase, "Purchase saving error");
            //read test

            var searchRequest = new SearchRequestModel
            {
                Name = purchase.Name,
                SearchByName = true
            };
            var searchRequest2 = new SearchRequestModel
            {
                Name = purchase2.Name,
                SearchByName = true
            };
            var searchRequest3 = new SearchRequestModel
            {
                Name = purchase3.Name,
                SearchByName = true
            };

            var searchRequest4 = new SearchRequestModel
            {
                Name = purchase4.Name,
                SearchByName = true
            };

            var foundList = StoreService.LoadPurchaseList(searchRequest).Result;
            Assert.IsTrue(foundList.Count == 1, "Purchase storage error: records duplicating found");

            var controlPurchase = foundList.FirstOrDefault();
            Assert.IsTrue(controlPurchase.DeepEquals(purchase), "Purchase storage error: not consistent");

            var foundList2 = StoreService.LoadPurchaseList(searchRequest2).Result;
            Assert.IsTrue(foundList2.Count == 1, "Purchase storage error: records duplicating found");

            var controlPurchase2 = foundList2.FirstOrDefault();
            Assert.IsTrue(controlPurchase2.DeepEquals(purchase2), "Purchase storage error: not consistent");

            var foundList3 = StoreService.LoadPurchaseList(searchRequest3).Result;
            Assert.IsTrue(foundList3.Count == 1, "Purchase storage error: records duplicating found");

            var controlPurchase3 = foundList3.FirstOrDefault();
            Assert.IsTrue(controlPurchase3.DeepEquals(purchase3), "Purchase storage error: not consistent");

            var foundList4 = StoreService.LoadPurchaseList(searchRequest4).Result;
            Assert.IsTrue(foundList4.Count == 1, "Purchase storage error: records duplicating found");

            var controlPurchase4 = foundList4.FirstOrDefault();
            Assert.IsTrue(controlPurchase4.DeepEquals(purchase4), "Purchase storage error: not consistent");

            //update test
            purchase.IsMonthly = false;
            purchase.Name = "Звичайне яблуко";

            purchase = StoreService.AddPurchase(purchase).Result;
            Assert.IsTrue(purchase != null, "Purchase saving error");
            //repeated read test
            var controlPurchase_repeat = StoreService.LoadPurchase(controlPurchase.Id).Result;
            Assert.IsTrue(controlPurchase_repeat.DeepEquals(purchase), "Purchase storage error: not consistent");

            //delete test
            Assert.IsTrue(StoreService.DeletePurchase(purchase).Result, "Purchase deletion error");

            var deletedPurchase = StoreService.LoadPurchase(purchase.Id).Result;
            Assert.IsNull(deletedPurchase, "Purchase deletion error: not deleted");

        }
    }
}
