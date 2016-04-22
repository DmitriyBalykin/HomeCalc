using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeCalc.UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Initialization()
        {

        }
        [TestMethod]
        public void DefaultDBSchemaTest()
        {
            //check default DB schema
        }

        [TestMethod]
        public void PurchaseTypeDBWriteReadTest()
        {
            //write purchase type
            //read purchase type
            //rename puchase type
            //delete purchase type
        }

        [TestMethod]
        public void PurchaseDBWriteReadTest()
        {
            //create purchase
            //read purchase
            //edit purchase name
            //edit purchase cost
            //edit purchase number
            //edit purchase total cost
            //delete purchase
        }

        [TestMethod]
        public void SettingsDBWriteReadTest()
        {
            //read default settings
            //change one by one settings values
        }
    }
}
