using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT_aitipachong.DEncrypt
{
    [TestClass]
    public class UT_DESEncrypt
    {
        [TestMethod]
        public void UT_Encrypt_V2()
        {
            string original = "这就是一个笑话吧！！！";
            string encrypted = aitipachong.DEncrypt.DESEncrypt.Encrypt(original);
            Assert.IsTrue(!string.IsNullOrEmpty(encrypted));
        }

        [TestMethod]
        public void UT_Decrypt_V2()
        {
            string encrypted = "ABB7448BACA04565561F2A6925C8CE008711BCD65DE17A7A";
            string original = aitipachong.DEncrypt.DESEncrypt.Decrypt(encrypted);
            Assert.AreEqual("这就是一个笑话吧！！！", original);
        }
    }
}
