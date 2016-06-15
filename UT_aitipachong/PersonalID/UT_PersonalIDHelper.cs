using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aitipachong.PersonalID;

namespace UT_aitipachong.PersonalID
{
    [TestClass]
    public class UT_PersonalIDHelper
    {
        private const string LaiQiangID = "510502197907111916";

        [TestMethod]
        public void UT_Get_V1()
        {
            PersonalIDHelper id = PersonalIDHelper.Get(LaiQiangID);
            Assert.AreEqual("泸州市-江阳区", id.Area + "-" + id.City);
        }

        [TestMethod]
        public void UT_CheckIDCardNumber_V1()
        {
            bool result = PersonalIDHelper.CheckIDCardNumber(LaiQiangID);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UT_Radom_V1()
        {
            PersonalIDHelper radomID = PersonalIDHelper.Radom();
            bool result = radomID == null ? false : true;
            Assert.IsTrue(result);
        }
    }
}
