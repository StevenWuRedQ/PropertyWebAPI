using ACRISDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyWebAPI.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestPropertyDataServices
{
    [TestClass]
    public class AcrisUpdateUnitTest
    {
        [TestInitialize]
        public void SetUpDB()
        {
            EffortProviderFactory.ResetDb();
            // PrepareData();
        }
        private void PrepareData()
        {
            using (ACRISEntities ctx = new ACRISEntities())
            {
                ctx.vwUpdateTrancations
                    .Add(new vwUpdateTrancation
                    {
                        BBL = "2057240685",
                        DateTimeProcessed = DateTime.Parse("2017-07-07 04")
                    });
            }
        }
        [TestMethod]
        public void GetRowTranscationTest()
        {
            PropertyUpdateTransaction transaction = ACRIS.GetTransaction(DateTime.Parse("2017-07-07"));
            Assert.IsNotNull(transaction, "Can find Transaction after a data rage");
            Assert.IsTrue(transaction.Count > 0, "get transaction log");

            PropertyUpdateTransaction trasaction = ACRIS.GetTransaction(DateTime.Parse("2017-09-07"));
            Assert.IsNotNull(transaction, "can not find Transaction without data inside");
            
        }
    }
}
