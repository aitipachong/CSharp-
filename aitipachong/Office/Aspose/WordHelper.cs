// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Office.Aspose
// * 文件名称：		    WordHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-13
// * 程序功能描述：
// *        Word文档帮助类，主要内容如下：
// *            1.文档合并
// *            2.转换导出
// *            3.文档操作
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace aitipachong.Office.Aspose
{
    /// <summary>
    /// Word文档帮助类
    /// </summary>
    public class WordHelper
    {
        #region 文档合并

        /// <summary>
        /// 合并Word文档（在前文档后继续合并下个文档）
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public bool AddDocs2Doc(string[] paths, string outputPath)
        {
            bool result = false;
            //参数验证
            if (paths == null || paths.Length <= 1) return result;
            if (string.IsNullOrEmpty(outputPath)) throw new ArgumentNullException("New word document save's path is not empty or null.");
            string folderPath = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            Document docAll = null;
            ArrayList docArrayList = null;
            try
            {
                docAll = new Document();
                docArrayList = this.GetDocumentList(paths);
                foreach(Document doc in docArrayList)
                {
                    doc.FirstSection.PageSetup.SectionStart = SectionStart.Continuous;
                    docAll.AppendDocument(doc, ImportFormatMode.KeepSourceFormatting);
                }
                docAll.Save(outputPath);
                result = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (docAll != null) docAll = null;
                if (docArrayList != null && docArrayList.Count > 0) docArrayList.Clear();
            }

            return result;
        }

        /// <summary>
        /// 合并Word文档（在前文档后，创建新页合并下个文档）
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public bool AddDocs2DocContinuous(string[] paths, string outputPath)
        {
            bool result = false;
            //参数验证
            if (paths == null || paths.Length <= 1) return result;
            if (string.IsNullOrEmpty(outputPath)) throw new ArgumentNullException("New word document save's path is not empty or null.");
            string folderPath = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            Document docAll = null;
            ArrayList docArrayList = null;
            try
            {
                docAll = new Document();
                docArrayList = this.GetDocumentList(paths);
                foreach (Document doc in docArrayList)
                {
                    doc.FirstSection.PageSetup.SectionStart = SectionStart.NewPage;
                    docAll.AppendDocument(doc, ImportFormatMode.KeepSourceFormatting);
                }
                docAll.Save(outputPath);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (docAll != null) docAll = null;
                if (docArrayList != null && docArrayList.Count > 0) docArrayList.Clear();
            }

            return result;
        }

        /// <summary>
        /// 根据word文档路径，获取一个Aspose.Words.Document对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Document GetDocument(string path)
        {
            Document doc = new Document(path);
            return doc;
        }

        /// <summary>
        /// 根据word文档路径数组，获取Aspose.Words.Document对象动态数组
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private ArrayList GetDocumentList(string[] paths)
        {
            ArrayList docArrayList = null;
            if (paths.Length == 0) return docArrayList;
            Document doc;
            docArrayList = new ArrayList();
            for(int i = 0; i < paths.Length; i++)
            {
                doc = new Document(paths[i]);
                docArrayList.Add(doc);
            }
            return docArrayList;
        }

        /// <summary>
        /// 合并Word文档（官网解决方案）
        /// </summary>
        /// <param name="wordFilePathList"></param>
        /// <param name="newWordSavePath"></param>
        /// <returns></returns>
        public bool MergeWords(List<string> wordFilePathList, string newWordSavePath)
        {
            bool result = false;
            //参数验证
            if (wordFilePathList == null || wordFilePathList.Count <= 1) return result;
            if (string.IsNullOrEmpty(newWordSavePath)) throw new ArgumentNullException("New word document save's path is not empty or null.");
            string folderPath = Path.GetDirectoryName(newWordSavePath);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            Document doc = new Document();
            doc.RemoveAllChildren();
            try
            {
                for(int i = 0; i < wordFilePathList.Count; i++)
                {
                    if (System.IO.File.Exists(wordFilePathList[i]))
                    {
                        //Open the document to join.
                        Document srcDoc = new Document(wordFilePathList[i]);

                        //Append the source document at the end of the destination document.
                        doc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

                        //If this is the second document or above being appended the unlink all headers footers in the section
                        //from the headers and footers of the previous section.
                        if (i > 1) doc.Sections[i].HeadersFooters.LinkToPrevious(false);
                    }
                }
                SaveOutputParameters output = doc.Save(newWordSavePath, SaveFormat.Docx);
                result = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (doc != null) doc = null;
            }

            return result;
        }

        #endregion

        #region 转换导出

        /// <summary>
        /// 从Word文档中提取图片，且保存
        /// </summary>
        /// <param name="wordInputPath">Word文档路径</param>
        /// <param name="imgOutputPath">图片保存目录路径</param>
        /// <param name="imageName">图片名称</param>
        /// <param name="imageFormat">图片格式</param>
        /// <returns></returns>
        public bool GetPicturesInWord(string wordInputPath, string imgOutputPath, string imageName, ImageFormat imageFormat)
        {
            bool result = false;
            //参数验证
            if (string.IsNullOrEmpty(wordInputPath)) throw new ArgumentNullException("Word file path is empty or null.");
            if (!System.IO.File.Exists(wordInputPath)) throw new FileNotFoundException("Word file is not exist.");
            if (!Directory.Exists(imgOutputPath)) Directory.CreateDirectory(imgOutputPath);
            if (string.IsNullOrEmpty(imageName)) imageName = Path.GetFileNameWithoutExtension(wordInputPath);

            Document doc = new Document(wordInputPath);
            try
            {
                NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);
                int imageIndex = 1;
                foreach(Shape shape in shapes)
                {
                    if(shape.HasImage)
                    {
                        string imagePath = Path.Combine(imgOutputPath, imageName + "_" + imageIndex + "." + imageFormat.ToString());
                        shape.ImageData.Save(imagePath);
                        ++imageIndex;
                    }
                }

                NodeCollection dmlShapes = doc.GetChildNodes(NodeType.DrawingML, true);
                foreach(DrawingML dml in dmlShapes)
                {
                    if(dml.HasImage)
                    {
                        string imagePath = Path.Combine(imgOutputPath, imageName + "_" + imageIndex + "." + imageFormat.ToString());
                        dml.ImageData.Save(imagePath);
                        ++imageIndex;
                    }
                }

                result = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 把图片转换为PDF
        /// </summary>
        /// <param name="imageFileName">图片文件路径</param>
        /// <param name="pdfFileName">pdf文件存储路径</param>
        /// <returns></returns>
        public bool ConvertImageToPdf(string imageFileName, string pdfFileName)
        {
            bool result = false;
            //参数容错
            if (string.IsNullOrEmpty(imageFileName)) throw new ArgumentNullException("image file path is empty or null.");
            if (!System.IO.File.Exists(imageFileName)) throw new FileNotFoundException("image file is not exist.");
            string folder = Path.GetDirectoryName(pdfFileName);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            try
            {
                using (Image image = Image.FromFile(imageFileName))
                {
                    FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);

                    //int framesCount = image.GetFrameCount(dimension);
                    int framesCount = image.GetFrameCount(FrameDimension.Page);

                    for(int frameIdx = 0; frameIdx < framesCount; frameIdx++)
                    {
                        if (frameIdx != 0) builder.InsertBreak(BreakType.SectionBreakNewPage);
                        image.SelectActiveFrame(dimension, frameIdx);

                        PageSetup ps = builder.PageSetup;
                        ps.PageWidth = ConvertUtil.PixelToPoint(image.Width, image.HorizontalResolution);
                        ps.PageHeight = ConvertUtil.PixelToPoint(image.Height, image.VerticalResolution);

                        builder.InsertImage(
                            image,
                            RelativeHorizontalPosition.Page,
                            0,
                            RelativeVerticalPosition.Page,
                            0,
                            ps.PageWidth,
                            ps.PageHeight,
                            WrapType.None);
                    }
                }
                doc.Save(pdfFileName);
                result = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Word文档转换为PDF文件
        /// </summary>
        /// <param name="docFileName">Word文档路径</param>
        /// <param name="pdfFileName">pdf文件存储路径</param>
        /// <returns></returns>
        public bool ConvertWordToPdf(string docFileName, string pdfFileName)
        {
            bool result = false;
            //参数验证
            if (string.IsNullOrEmpty(docFileName)) throw new ArgumentNullException("Word or text file path is empty or null.");
            if (!System.IO.File.Exists(docFileName)) throw new FileNotFoundException("Word or text file is not exist.");
            string folder = Path.GetDirectoryName(pdfFileName);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            try
            {
                Document doc = new Document(docFileName);
                doc.Save(pdfFileName, SaveFormat.Pdf);
                result = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 将Word文档转换为图片
        /// </summary>
        /// <param name="wordInputPath">Word文档路径</param>
        /// <param name="imageOutputDirPath">图片输出路径，如果为空，默认为Word文档所在目录</param>
        /// <param name="startPageNum">从Word文档的第几页开始转换，如果为0，默认值为1</param>
        /// <param name="endPageNum">从Word文档的第几页停止转换，如果为0，默认值为Word总页数</param>
        /// <param name="imageFormat">设置所需图片格式，如果为null,默认格式为JPEG</param>
        /// <param name="resolution">设置图片的橡树，数字越大越清晰，如果为0，默认值为128，建议最大值不要超过1024</param>
        /// <returns></returns>
        public bool ConvertWordToImage(string wordInputPath, string imageOutputDirPath, int startPageNum, int endPageNum, ImageFormat imageFormat, int resolution)
        {
            bool result = false;
            //参数容错
            if (string.IsNullOrEmpty(wordInputPath)) throw new ArgumentNullException("Word file path is empty or null.");
            if (!System.IO.File.Exists(wordInputPath)) throw new FileNotFoundException("Word file is not exist.");
            if (string.IsNullOrEmpty(imageOutputDirPath)) imageOutputDirPath = Path.GetDirectoryName(wordInputPath);
            if (!Directory.Exists(imageOutputDirPath)) Directory.CreateDirectory(imageOutputDirPath);

            try
            {
                Document doc = new Document(wordInputPath);
                if (doc == null) throw new ArgumentNullException("Word文件无效或Word文件被加密.");
                if (startPageNum <= 0) startPageNum = 1;
                if (endPageNum > doc.PageCount || endPageNum <= 0) endPageNum = doc.PageCount;
                if(startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum;
                    startPageNum = endPageNum;
                    endPageNum = tempPageNum;
                }
                if (imageFormat == null) imageFormat = ImageFormat.Jpeg;
                if (resolution <= 0) resolution = 128;

                string imageName = Path.GetFileNameWithoutExtension(wordInputPath);
                ImageSaveOptions imageSaveOptions = new ImageSaveOptions(SaveFormat.Jpeg);
                imageSaveOptions.Resolution = resolution;
                for(int i = startPageNum; i <= endPageNum; i++)
                {
                    MemoryStream stream = new MemoryStream();
                    imageSaveOptions.PageIndex = i - 1;
                    string imgPath = Path.Combine(imageOutputDirPath, imageName) + "_" + i.ToString("000") + "." + imageFormat.ToString();
                    doc.Save(stream, imageSaveOptions);
                    Image img = Image.FromStream(stream);
                    Bitmap bm = ESBasic.Helpers.ImageHelper.Zoom(img, 0.6f);
                    bm.Save(imgPath, imageFormat);
                    img.Dispose();
                    stream.Dispose();
                    bm.Dispose();
                }
                result = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 将Word文档转换为图片
        /// </summary>
        /// <param name="wordInputPath">Word文档路径</param>
        /// <param name="imageOutputDirPath">图片输出路径，如果为空，默认为Word文档所在目录</param>
        /// <returns></returns>
        public bool ConvertWordToImage(string wordInputPath, string imageOutputDirPath)
        {
            return this.ConvertWordToImage(wordInputPath, imageOutputDirPath, 0, 0, null, 200);
        }
        #endregion
    }
}