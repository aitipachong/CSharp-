using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using aitipachong.Zip;

namespace UT_aitipachong.Zip
{
    [TestClass]
    public class UT_ZipFactory
    {
        [TestMethod]
        public void UT_PackFiles_V1()
        {
            string zipFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD");
            string zipFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD.rar");

            try
            {
                SharpZip.PackFiles(zipFileName, zipFolder);
                Assert.IsTrue(System.IO.File.Exists(zipFileName));
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_UnpackFiles_V1()
        {
            string zipFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "TEMPWORD");
            string zipFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD.rar");

            try
            {
                bool result = SharpZip.UnpackFiles(zipFileName, zipFolder);
                Assert.AreEqual(true, result);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
