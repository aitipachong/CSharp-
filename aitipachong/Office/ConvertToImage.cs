// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Office
// * 文件名称：		    ConvertToImage.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-10
// * 程序功能描述：
// *        1.PowerPoint文档转换为图片
// *        2.Word文档转换为PDF
// *        3.PDF文档转换为图片
// *        4.Word文档转换为图片
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Collections.Generic;
using MSWord = Microsoft.Office.Interop.Word;
using MSPowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Drawing;
using System.IO;
using Microsoft.Office.Core;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using O2S.Components.PDFRender4NET;

namespace aitipachong.Office
{
    #region 从剪切板获取MetaFile数据
    /// <summary>
    /// 从剪切板获取MetaFile数据
    /// </summary>
    internal class MetafileHelper
    {
        [DllImport("user32.dll")]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
        [DllImport("user32.dll")]
        static extern IntPtr GetClipboardData(uint uFormat);
        [DllImport("user32.dll")]
        static extern bool IsClipboardFormatAvailable(uint uFormat);
        [DllImport("user32.dll")]
        static extern bool CloseClipboard();
        [DllImport("gdi32.dll")]
        static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
        [DllImport("gdi32.dll")]
        static extern IntPtr CopyEnhMetaFileA(IntPtr hemfSrc, string filename);
        [DllImport("gdi32.dll")]
        static extern bool DeleteEnhMetaFile(IntPtr hemf);

        /// <summary>
        /// Metafile mf is set to a state that is not valid inside this function.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="mf"></param>
        /// <returns></returns>
        public static bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile mf)
        {
            bool bResult = false;
            IntPtr hEMF, hEMF2;
            hEMF = mf.GetHenhmetafile();    //invalidates mf
            if(!hEMF.Equals(new IntPtr(0)))
            {
                hEMF2 = CopyEnhMetaFile(hEMF, new IntPtr(0));
                if(!hEMF2.Equals(new IntPtr(0)))
                {
                    if(OpenClipboard(hWnd))
                    {
                        if(EmptyClipboard())
                        {
                            IntPtr hRes = SetClipboardData(14 /*CF_ENHMETAFILE*/, hEMF2);
                            bResult = hRes.Equals(hEMF2);
                            CloseClipboard();
                        }
                    }
                }
                DeleteEnhMetaFile(hEMF);
            }
            return bResult;
        }

        public static Metafile GetEnhMetafileOnClipboard(IntPtr hWnd)
        {
            if(OpenClipboard(hWnd))
            {
                uint format = 14/*CF_ENHMETAFILE*/;
                if(IsClipboardFormatAvailable(format))
                {
                    IntPtr hRes = GetClipboardData(format);
                    CloseClipboard();
                    if(!hRes.Equals(new IntPtr(0)))
                    {
                        IntPtr hEMF = CopyEnhMetaFile(hRes, new IntPtr(0));
                        Metafile mf = new Metafile(hEMF, true);
                        return mf;
                    }
                }
            }
            return null;
        }

        public static bool IsClipboardFormatAvailableENHMETAFILE(IntPtr hWnd)
        {
            if (OpenClipboard(hWnd))
            {
                uint format = 14/*CF_ENHMETAFILE*/;
                if (IsClipboardFormatAvailable(format))
                {
                    CloseClipboard();
                    return true;
                }
            }
            return false;
        }

        public static bool SaveMetafile(Metafile mf, string filename)
        {
            IntPtr hEMF, hEMF2;
            hEMF = mf.GetHenhmetafile(); // invalidates mf
            if (!hEMF.Equals(new IntPtr(0)))
            {
                //hEMF = CopyEnhMetaFile(hEMF, new IntPtr(0));
                hEMF2 = CopyEnhMetaFileA(hEMF, filename);
                DeleteEnhMetaFile(hEMF2);
                if (hEMF2 != IntPtr.Zero)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
    #endregion

    /// <summary>
    /// 图片清晰度枚举（值越大越清晰）
    /// </summary>
    public enum PictureDefinition
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10
    }

    /// <summary>
    /// 1.PowerPoint文档转换为图片
    /// 2.Word文档转换为PDF
    /// 3.PDF文档转换为图片
    /// 4.Word文档转换为图片
    /// </summary>
    public class ConvertToImage
    {
        #region PowerPoint文档转换为图片

        /// <summary>
        /// PowerPoint文档转换为图片
        /// </summary>
        /// <param name="pptInputPath">PPT文档路径</param>
        /// <param name="imageOutputPath">图片输出路径</param>
        /// <param name="imageName">生成图片的名字</param>
        /// <param name="convertPageIndexCollection">待扫描PPT页面页面集合，如果该参数为null，表示全部转换</param>
        /// <param name="imageFormat">图片格式</param>
        public void Ppt2Image(string pptInputPath, string imageOutputPath, string imageName, 
            ICollection<int> convertPageIndexCollection, ImageFormat imageFormat)
        {
            if (string.IsNullOrEmpty(pptInputPath)) throw new ArgumentNullException("PPT file path is empty or null.");
            if (!System.IO.File.Exists(pptInputPath)) throw new FileNotFoundException("PPT file is not exist.");
            if (!Directory.Exists(imageOutputPath)) Directory.CreateDirectory(imageOutputPath);
            if (string.IsNullOrEmpty(imageName)) imageName = Path.GetFileNameWithoutExtension(pptInputPath);

            try
            {
                Bitmap[] bmps = this.Scan4PowerPoint(pptInputPath, convertPageIndexCollection);
                if(bmps != null && bmps.Length > 0)
                {
                    //转换
                    for (int i = 0; i < bmps.Length; i++)
                    {
                        Bitmap bmp = bmps[i];
                        bmp.Save(Path.Combine(imageOutputPath, imageName + (i + 1).ToString() + "." + imageFormat.ToString()), imageFormat);
                        bmp.Dispose();
                    }

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 扫描PowerPoint文档为图片
        /// </summary>
        /// <param name="filePath">PPT文档路径</param>
        /// <param name="pageIndexCollection">待扫描PPT页面页面集合</param>
        /// <returns>位图数组</returns>
        private Bitmap[] Scan4PowerPoint(string filePath, ICollection<int> pageIndexCollection)
        {
            List<Bitmap> bmList = new List<Bitmap>();
            //实例化一个PowerPoint的Application实例
            MSPowerPoint.ApplicationClass pptApplicationClass = new MSPowerPoint.ApplicationClass();

            try
            {
                //打开一个PPT演示文档
                MSPowerPoint.Presentation presentation = pptApplicationClass.Presentations.Open(filePath, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
                //获取幻灯片总页数
                int totalCount = presentation.Slides.Count;
                if (pageIndexCollection != null) totalCount = pageIndexCollection.Count;

                //循环幻灯片，转换为位图
                int index = 0;
                foreach(MSPowerPoint.Slide slide in presentation.Slides)
                {
                    //判断是否为需要转换为位图的幻灯片下标
                    if(pageIndexCollection == null || pageIndexCollection.Contains(index))
                    {
                        slide.Copy();       //复制幻灯片到剪贴板

                        //从剪贴板获取复制数据
                        System.Windows.Forms.IDataObject data = System.Windows.Forms.Clipboard.GetDataObject(); 
                        //判断从剪贴板获取的数据是否为图形对象（进行数据格式关联）
                        if(data.GetDataPresent(DataFormats.MetafilePict))
                        {
                            object obj = data.GetData(DataFormats.MetafilePict);
                            Metafile metafile = MetafileHelper.GetEnhMetafileOnClipboard(IntPtr.Zero);
                            Bitmap bmp = new Bitmap(metafile);
                            bmList.Add(bmp);
                            Clipboard.Clear();
                        }
                    }
                    ++index;
                }
                presentation.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(presentation);
                return bmList.ToArray();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                pptApplicationClass.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pptApplicationClass);
            }
        }
        #endregion

        #region Word文档转换为PDF

        /// <summary>
        /// Word文档转换为PDF
        /// </summary>
        /// <param name="wordInputPath">待转换的Word文档路径</param>
        /// <param name="pdfOutputPath">转换后PDF保存文件夹路径</param>
        /// <param name="pdfName">转换后PDF文档名称</param>
        public void Word2PDF(string wordInputPath, string pdfOutputPath, string pdfName)
        {
            try
            {
                string pdfExtension = ".pdf";

                //验证参数
                if (string.IsNullOrEmpty(wordInputPath)) throw new ArgumentNullException("Word file path is empty or null.");
                if (!System.IO.File.Exists(wordInputPath)) throw new FileNotFoundException("Word file is not exist.");
                if (!Directory.Exists(pdfOutputPath)) Directory.CreateDirectory(pdfOutputPath);
                if (string.IsNullOrEmpty(pdfName)) pdfName = Path.GetFileNameWithoutExtension(wordInputPath);
                if (!(Path.GetExtension(pdfName).ToUpper() == pdfExtension.ToUpper())) pdfName = pdfName + pdfExtension;

                object paramSourceDocPath = wordInputPath;
                object paramMissing = Type.Missing;
                string paramExportFilePath = Path.Combine(pdfOutputPath, pdfName);

                //创建一个WordApplication对象
                MSWord.ApplicationClass wordApplication = new MSWord.ApplicationClass();
                MSWord.Document wordDocument = null;

                //设置输出格式为pdf
                MSWord.WdExportFormat paramExportFormat = MSWord.WdExportFormat.wdExportFormatPDF;
                bool paramOpenAfterExport = false;
                MSWord.WdExportOptimizeFor paramExportOptimizeFor = MSWord.WdExportOptimizeFor.wdExportOptimizeForPrint;
                MSWord.WdExportRange paramExportRange = MSWord.WdExportRange.wdExportAllDocument;
                int paramStartPage = 0;
                int paramEndPage = 0;
                MSWord.WdExportItem paramExportItem = MSWord.WdExportItem.wdExportDocumentContent;
                bool paramIncludeDocProps = true;
                bool paramKeepIRM = true;
                MSWord.WdExportCreateBookmarks paramCreateBookmarks = MSWord.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                bool paramDocStructureTags = true;
                bool paramBitmapMissingFonts = true;
                bool paramUseISO19005_1 = false;

                try
                {
                    //打开Word文档
                    wordDocument = wordApplication.Documents.Open(
                        ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                        ref paramMissing, ref paramMissing, ref paramMissing,
                        ref paramMissing, ref paramMissing, ref paramMissing,
                        ref paramMissing, ref paramMissing, ref paramMissing,
                        ref paramMissing, ref paramMissing, ref paramMissing,
                        ref paramMissing);

                    //以指定格式导出
                    if(wordDocument != null)
                    {
                        wordDocument.ExportAsFixedFormat(paramExportFilePath,
                            paramExportFormat, paramOpenAfterExport,
                            paramExportOptimizeFor, paramExportRange, paramStartPage,
                            paramEndPage, paramExportItem, paramIncludeDocProps,
                            paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                            paramBitmapMissingFonts, paramUseISO19005_1,
                            ref paramMissing);
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //关闭并释放Document对象
                    if(wordDocument != null)
                    {
                        wordDocument.Close(ref paramMissing, ref paramMissing, ref paramMissing);
                        wordDocument = null;
                    }
                    //退出并释放Word Application对象
                    if(wordApplication != null)
                    {
                        wordApplication.Quit(ref paramMissing, ref paramMissing, ref paramMissing);
                        wordApplication = null;
                    }

                    //启动回收GC机制
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region PDF转换为图片
        /// <summary>
        /// 将PDF文档转换为图片
        /// </summary>
        /// <param name="pdfInputPath">PDF文件路径</param>
        /// <param name="imageOutputPath">图片输出路径</param>
        /// <param name="imageName">生成图片的名字</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="definition">设置图片清晰度，数字越大越清晰</param>
        public void Pdf2Image(string pdfInputPath, string imageOutputPath, string imageName, int startPageNum, int endPageNum, 
                                ImageFormat imageFormat, PictureDefinition definition)
        {
            //参数验证
            if (string.IsNullOrEmpty(pdfInputPath)) throw new ArgumentNullException("PDF file path is empty or null.");
            if (!System.IO.File.Exists(pdfInputPath)) throw new FileNotFoundException("PDF file is not exist.");
            if (!Directory.Exists(imageOutputPath)) Directory.CreateDirectory(imageOutputPath);
            if (string.IsNullOrEmpty(imageName)) imageName = Path.GetFileNameWithoutExtension(pdfInputPath);

            PDFFile pdfFile = null;

            try
            {
                pdfFile = PDFFile.Open(pdfInputPath);
                if (startPageNum <= 0) startPageNum = 1;
                if (endPageNum <= 0) endPageNum = pdfFile.PageCount;
                if (endPageNum > pdfFile.PageCount) endPageNum = pdfFile.PageCount;
                if (startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum;
                    startPageNum = endPageNum;
                    endPageNum = startPageNum;
                }

                //转换
                for (int i = startPageNum; i <= endPageNum; i++)
                {
                    Bitmap pageImage = pdfFile.GetPageImage(i - 1, 56 * (int)definition);
                    pageImage.Save(Path.Combine(imageOutputPath, imageName + i.ToString() + "." + imageFormat.ToString()), imageFormat);
                    pageImage.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(pdfFile != null) pdfFile.Dispose();
            }
        }
        #endregion

        #region Word文档转换为图片

        /// <summary>
        /// Word文档转换为图片
        /// </summary>
        /// <remarks>
        ///     该Word转换为图片，首先是转换为临时的PDF文档（临时存储在当前Word文档所在目录下），然后，从PDF转换为图片。
        ///     在转换完成后，临时的PDF文档将被删除
        /// </remarks>
        /// <param name="wordInputPath">待转换的Word文档路径</param>
        /// <param name="imageOutputPath">图片输出路径</param>
        /// <param name="imageName">生成图片的名字</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="definition">设置图片清晰度，数字越大越清晰</param>
        public void Word2Image(string wordInputPath, string imageOutputPath, string imageName, int startPageNum, int endPageNum,
                                ImageFormat imageFormat, PictureDefinition definition)
        {
            if (string.IsNullOrEmpty(wordInputPath)) throw new ArgumentNullException("Word file path is empty or null.");
            if (!System.IO.File.Exists(wordInputPath)) throw new FileNotFoundException("Word file is not exist.");
            if (!Directory.Exists(imageOutputPath)) Directory.CreateDirectory(imageOutputPath);
            if (string.IsNullOrEmpty(imageName)) imageName = Path.GetFileNameWithoutExtension(wordInputPath);

            string pdfName = "temp" + Path.GetFileNameWithoutExtension(wordInputPath) + ".pdf";
            string pdfOutputPath =Path.GetDirectoryName(wordInputPath);

            try
            {
                this.Word2PDF(wordInputPath, pdfOutputPath, pdfName);
                this.Pdf2Image(Path.Combine(pdfOutputPath, pdfName), imageOutputPath, imageName, startPageNum, endPageNum, imageFormat, definition);

                System.IO.File.Delete(Path.Combine(pdfOutputPath, pdfName));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}