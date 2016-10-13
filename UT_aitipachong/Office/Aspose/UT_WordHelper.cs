using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using aitipachong.Office.Aspose;
using System.Drawing.Imaging;

namespace UT_aitipachong.Office.Aspose
{
    [TestClass]
    public class UT_WordHelper
    {
        [TestMethod]
        public void UT_AddDocs2Doc_V1()
        {
            string wordFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD");
            string[] paths = Directory.GetFiles(wordFolder, "*.docx");
            string newWordSavePath = Path.Combine(wordFolder, "new_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx");

            try
            {
                WordHelper helper = new WordHelper();
                bool result = helper.AddDocs2Doc(paths, newWordSavePath);
                if(result)
                {
                    Assert.AreEqual(true, System.IO.File.Exists(newWordSavePath));
                }
                else
                {
                    Assert.Fail("合并失败");
                }
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_AddDocs2DocContinue_V1()
        {
            string wordFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD");
            string[] paths = Directory.GetFiles(wordFolder, "*.docx");
            string newWordSavePath = Path.Combine(wordFolder, "new_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx");

            try
            {
                WordHelper helper = new WordHelper();
                bool result = helper.AddDocs2DocContinuous(paths, newWordSavePath);
                if (result)
                {
                    Assert.AreEqual(true, System.IO.File.Exists(newWordSavePath));
                }
                else
                {
                    Assert.Fail("合并失败");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_MergeWords_V1()
        {
            string wordFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD");
            string[] paths = Directory.GetFiles(wordFolder, "*.docx");
            string newWordSavePath = Path.Combine(wordFolder, "new_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx");

            try
            {
                WordHelper helper = new WordHelper();
                bool result = helper.MergeWords(new List<string>(paths), newWordSavePath);
                if (result)
                {
                    Assert.AreEqual(true, System.IO.File.Exists(newWordSavePath));
                }
                else
                {
                    Assert.Fail("合并失败");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetPicturesInWord_V1()
        {
            string wordInputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "C#开发实战1200例（第Ⅱ卷）光盘使用说明.doc");
            string imgOutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "Pictures");
            string imageName = "C#开发实战1200例";

            try
            {
                WordHelper helper = new WordHelper();
                bool result = helper.GetPicturesInWord(wordInputPath, imgOutputPath, imageName, ImageFormat.Jpeg);
                if (result)
                {
                    Assert.AreEqual(true, result);
                }
                else
                {
                    Assert.Fail("提取图像失败");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_ConvertImageToPdf_V1()
        {
            //Documents\WORD\Pictures
            string imageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "Pictures", "C#开发实战1200例_2.Jpeg");
            string pdfFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "PDF", "开发实战1200例_2.pdf");

            try
            {
                WordHelper helper = new WordHelper();
                bool result = helper.ConvertImageToPdf(imageFilePath, pdfFilePath);
                if (result)
                {
                    Assert.AreEqual(true, result);
                }
                else
                {
                    Assert.Fail("图像转换为PDF失败");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_ConvertWordToPdf_V1()
        {
            string wordFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "设置.docx");
            string pdfFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "PDF", "设置.pdf");

            try
            {
                WordHelper helper = new WordHelper();
                bool result = helper.ConvertWordToPdf(wordFilePath, pdfFilePath);
                if (result)
                {
                    Assert.AreEqual(true, result);
                }
                else
                {
                    Assert.Fail("图像转换为PDF失败");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_ConvertWordToImage_V1()
        {
            string wordFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "License指导说明书.docx");
            string imgDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "Pictures");

            try
            {
                WordHelper helper = new WordHelper();
                bool result = helper.ConvertWordToImage(wordFilePath, imgDirPath);
                if (result)
                {
                    Assert.AreEqual(true, result);
                }
                else
                {
                    Assert.Fail("图像转换为PDF失败");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
