using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using HomeCalc.Presentation.Models;

namespace FunctionalTests
{
    [TestClass]
    public class FunctionalTest
    {
        private static StorageService StoreService;
        [TestMethod]
        private void DatabaseTest()
        {
            StoreService = StorageService.GetInstance();

            Task.Factory.StartNew(async () =>
            {
                var purchase = new Purchase
                {
                    Date = DateTime.Now,
                    Id = 0,
                    ItemCost = 10.4,
                    ItemsNumber = 135.2,
                    TotalCost = 1406.08,
                    MonthlyPurchase = true,
                    Name = "Cote d'Azure apple",
                    PurchaseComment = "Воно чудо'ве",
                    PurchaseRate = 10,
                    StoreComment = "Цікави'й магазин",
                    StoreName = "Нову'с магазин",
                    StoreRate = 5,
                    Type = new PurchaseType { Name = "Їжа", TypeId = 1 },
                    SubType = new PurchaseSubType { Name = "Фрукт", Id = 1 }
                };
                //Save test
                if (!await StoreService.AddPurchase(purchase))
                {
                    throw new Exception("Purchase saving error");
                }
                //read test

                var searchRequest = new SearchRequestModel
                {
                    Name = purchase.Name,
                    SearchByName = true
                };
                var controlPurchase = (await StoreService.LoadPurchaseList(searchRequest).ConfigureAwait(false)).FirstOrDefault();
                if (!controlPurchase.DeepEquals(purchase))
                {
                    throw new Exception("Purchase storage error: not consistent");
                }
                //update test
                purchase.MonthlyPurchase = false;
                purchase.Name = "Звичайне яблуко";

                if (!await StoreService.AddPurchase(purchase))
                {
                    throw new Exception("Purchase saving error");
                }
                //repeated read test
                var controlPurchase2 = await StoreService.LoadPurchase(controlPurchase.Id);
                if (!controlPurchase2.DeepEquals(purchase))
                {
                    throw new Exception("Purchase storage error: not consistent");
                }

                //delete test
                if (!await StoreService.RemovePurchase(purchase.Id))
                {
                    throw new Exception("Purchase deletion error");
                }
                var deletedPurchase = StoreService.LoadPurchase(purchase.Id);
                if (deletedPurchase != null)
                {
                    throw new Exception("Purchase deletion error: not deleted");
                }
            });

        }
    }
}
