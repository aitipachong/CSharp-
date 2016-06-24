using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aitipachong.DEncrypt;
using System.IO;

namespace UT_aitipachong.DEncrypt
{
    [TestClass]
    public class UT_MySecurity
    {
        private string plaintext = "这就是一个笑话吧！！！";
        private string ciphertext = "16E3C3AEC0235A715C9BB25896C6B816F13F6503E5F329DC";

        [TestMethod]
        public void UT_SEncryptString_V1()
        {
            string result = MySecurity.SEncryptString(plaintext, "pachong@01");
            Assert.AreEqual(ciphertext, result);
        }

        [TestMethod]
        public void UT_SDecryptString_V1()
        {
            string result = MySecurity.SDecryptString(ciphertext, "pachong@01");
            Assert.AreEqual(plaintext, result);
        }

        [TestMethod]
        public void UT_EncryptFile_V1()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Demo", "阿里云 ubuntu apt更新源.txt");
            if(!File.Exists(filePath))
            {
                Assert.Fail(string.Format("待加密文件不存在，路径：{0}", filePath));
                return;
            }
            string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Demo", "加密_阿里云 ubuntu apt更新源.txt");
            if (File.Exists(savePath)) File.Delete(savePath);
            MySecurity security = new MySecurity();
            bool result = security.EncryptFile(filePath, savePath, "pachong@01");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UT_DecryptFile_V1()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Demo", "加密_阿里云 ubuntu apt更新源.txt");
            if(!File.Exists(filePath))
            {
                Assert.Fail(string.Format("待解密文件不存在，路径：{0}", filePath));
                return;
            }
            string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Demo", "解密_阿里云 ubuntu apt更新源.txt");
            if (File.Exists(savePath)) File.Delete(savePath);
            MySecurity security = new MySecurity();
            bool result = security.DecryptFile(filePath, savePath, "pachong@01");
            Assert.IsTrue(result);
        }
    }
}
