using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using aitipachong.Office;
using System.Drawing;
using System.Drawing.Imaging;

namespace UT_aitipachong.Office
{
    [TestClass]
    public class UT_ConvertToImage
    {
        [TestMethod]
        public void UT_Word2PDF_V1()
        {
            try
            {
                string wordFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "cppcheck_rule.docx");
                string pdfFoderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD");
                string pdfName = "cppcheck_rule.pdf";

                ConvertToImage convert = new ConvertToImage();
                convert.Word2PDF(wordFilePath, pdfFoderPath, pdfName);

                bool result = System.IO.File.Exists(Path.Combine(pdfFoderPath, pdfName));
                Assert.AreEqual(true, result);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_Pdf2Image_V1()
        {
            try
            {
                string pdfFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "cppcheck_rule.pdf");
                string imageFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "Pictures");
                string imageName = "CppCheckRule_";

                ConvertToImage convert = new ConvertToImage();
                convert.Pdf2Image(pdfFilePath, imageFolderPath, imageName, -1, -1, ImageFormat.Jpeg, PictureDefinition.Six);

                string[] files = Directory.GetFiles(imageFolderPath);
                if(files == null || files.Length == 0)
                {
                    Assert.Fail("转换失败");
                }
                else
                {
                    Assert.AreEqual(4, files.Length);
                }
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_Word2Image_V1()
        {
            try
            {
                string wordFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "cppcheck_rule.docx");
                string imageFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "WORD", "Pictures");
                string imageName = "CppCheckRule_";

                ConvertToImage convert = new ConvertToImage();
                convert.Word2Image(wordFilePath, imageFolderPath, imageName, -1, -1, ImageFormat.Jpeg, PictureDefinition.Ten);

                string[] files = Directory.GetFiles(imageFolderPath);
                if (files == null || files.Length == 0)
                {
                    Assert.Fail("转换失败");
                }
                else
                {
                    Assert.AreEqual(4, files.Length);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_Ppt2Image_V1()
        {
            try
            {
                string pptFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "PPT", "CAS.pptx");
                string imageFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "PPT", "Pictures");
                string imageName = "CAS_";

                ConvertToImage convert = new ConvertToImage();
                convert.Ppt2Image(pptFilePath, imageFolderPath, imageName, null, ImageFormat.Jpeg);

                string[] files = Directory.GetFiles(imageFolderPath);
                if (files == null || files.Length == 0)
                {
                    Assert.Fail("转换失败");
                }
                else
                {
                    Assert.AreEqual(9, files.Length);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

    }
}
