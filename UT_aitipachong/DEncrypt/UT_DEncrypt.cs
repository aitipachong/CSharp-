using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aitipachong.DEncrypt;

namespace UT_aitipachong.DEncrypt
{
    [TestClass]
    public class UT_DEncrypt
    {
        [TestMethod]
        public void UT_Encrypt_V1()
        {
            string original = "这就是一个笑话吧！！！";
            string encrypted = aitipachong.DEncrypt.DEncrypt.Encrypt(original);
            Assert.IsTrue(!string.IsNullOrEmpty(encrypted));
        }

        [TestMethod]
        public void UT_Decrypt_V1()
        {
            string encrypted = "6aYiwuS3Lkl8u4AUUxQucaQKQ04SuW22";
            string original = aitipachong.DEncrypt.DEncrypt.Decrypt(encrypted);
            Assert.AreEqual("这就是一个笑话吧！！！", original);
        }
    }
}
