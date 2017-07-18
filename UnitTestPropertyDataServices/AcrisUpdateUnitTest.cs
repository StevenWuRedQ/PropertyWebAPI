using ACRISDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyWebAPI.BAL;
using PropertyWebAPI.BAL.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestPropertyDataServices.DBSetup;

namespace UnitTestPropertyDataServices
{
    [TestClass]
    public class AcrisUpdateUnitTest
    {
        [TestInitialize]
        public void SetUpDB()
        {
            EffortProviderFactory.ResetDb();
            PrepareData();
        }
        private ACRISService _serivce;
        private void PrepareData()
        {
            var context = new ACRISDBTestContext();
            var service = new ACRISService(context);

            context.vwUpdateTrancations
                    .Add(new vwUpdateTrancation
                    {
                        BBL = "2057240685",
                        DateTimeProcessed = DateTime.Parse("2017-07-07 04:30PM")
                    });
            _serivce = service;
        }
        [TestMethod]
        public void GetRowTranscationTest()
        {

            PropertyUpdateTransaction transaction = _serivce.GetTransaction(DateTime.Parse("2017-07-07"));
            Assert.IsNotNull(transaction, "Can find Transaction after a data rage");
            Assert.IsTrue(transaction.Count > 0, "get transaction log");

            PropertyUpdateTransaction trasaction = _serivce.GetTransaction(DateTime.Parse("2017-09-07"));
            Assert.IsNotNull(transaction, "can not find Transaction without data inside");

        }
    }
}
