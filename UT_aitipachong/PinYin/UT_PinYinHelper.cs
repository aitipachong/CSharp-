using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aitipachong.PinYin;

namespace UT_aitipachong.PinYin
{
    [TestClass]
    public class UT_PinYinHelper
    {
        [TestMethod]
        public void UT_GetFirstLetter_V1()
        {
            string hz = "赖强";
            try
            {
                string firstLetter = PinYinHelper.GetFirstLetter(hz);
                Assert.AreEqual("LQ", firstLetter);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetCodstring_V1()
        {
            string hz = "赖强";
            try
            {
                string firstLetter = PinYinHelper.GetCodstring(hz);
                Assert.AreEqual("LQ", firstLetter);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_ConvertPinYin_V1()
        {
            string hz = "赖强";
            try
            {
                PinYinHelper helper = new PinYinHelper();
                string pinyin = helper.ConvertPinYin(hz);
                Assert.AreEqual("laiqiang", pinyin.Trim().ToLower());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
