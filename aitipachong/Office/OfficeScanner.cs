// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Office
// * 文件名称：		    OfficeScanner.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-10
// * 程序功能描述：
// *        用于将word文档、ppt文档和PDF文档中的页转换为图片。
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
    /// OfficeScanner 用于将word文档、ppt文档和PDF文档中的页转换为图片。
    /// </summary>
    public class OfficeScanner
    {
        #region 扫描为图片 
        /// <summary>
        /// 将一个Word、PowerPoint和PDF文档，转换为位图
        /// </summary>
        /// <param name="filePath">文档路径</param>
        /// <returns>位图数组</returns>
        public Bitmap[] Scan(string filePath)
        {
            //参数容错
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("\"filePath\" parameter is empty or null!");
            if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException("File is not exist!");

            //获取文档扩展名
            string[] ary = filePath.Split('.');
            string extendName = ary[ary.Length - 1];

            try
            {
                //Word文档
                if (extendName.Trim().ToLower() == "doc" ||
                    extendName.Trim().ToLower() == "docx")
                {
                    return Scan4Word(filePath);
                }
                //PowerPoint文档
                if (extendName.Trim().ToLower() == "ppt" ||
                    extendName.Trim().ToLower() == "pptx")
                {
                    return Scan4PowerPoint(filePath, null);
                }

                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 扫描PowerPoint文档为图片
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

        #region 扫描Word文档为图片
        /// <summary>
        /// 扫描Word文档为图片
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Bitmap[] Scan4Word(string filePath)
        {
            //复制目标文件，后续将操作副本
            string tmpFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(filePath) + ".tmp";
            System.IO.File.Copy(filePath, tmpFilePath);

            List<Bitmap> bmList = new List<Bitmap>();
            MSWord.ApplicationClass wordApplicationClass = new MSWord.ApplicationClass();
            wordApplicationClass.Visible = false;
            object missing = System.Reflection.Missing.Value;

            try
            {
                object readOnly = false;
                object filePathObject = tmpFilePath;

                MSWord.Document document = wordApplicationClass.Documents.Open(ref filePathObject, ref missing,
                    ref readOnly, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing);

                bool finished = false;
                while(!finished)
                {
                    document.Content.CopyAsPicture();   //拷贝到剪贴板
                    System.Windows.Forms.IDataObject data = Clipboard.GetDataObject();
                    if(data.GetDataPresent(DataFormats.MetafilePict))
                    {
                        object obj = data.GetData(DataFormats.MetafilePict);
                        Metafile metafile = MetafileHelper.GetEnhMetafileOnClipboard(IntPtr.Zero);  //从剪贴板获取数据
                        Bitmap bmp = new Bitmap(metafile.Width, metafile.Height);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(Color.White);
                            g.DrawImage(metafile, 0, 0, bmp.Width, bmp.Height);
                        }
                        bmList.Add(bmp);
                        Clipboard.Clear();
                    }

                    object what = MSWord.WdGoToItem.wdGoToPage;
                    object which = MSWord.WdGoToDirection.wdGoToFirst;
                    object startIndex = "1";
                    document.ActiveWindow.Selection.GoTo(ref what, ref which, ref missing, ref startIndex); //转到下一页
                    MSWord.Range start = document.ActiveWindow.Selection.Paragraphs[1].Range;
                    MSWord.Range end = start.GoToNext(MSWord.WdGoToItem.wdGoToPage);
                    finished = (start.Start == end.Start);
                    if(finished)    //最后一页
                    {
                        end.Start = document.Content.End;
                    }

                    object oStart = start.Start;
                    object oEnd = end.Start;
                    document.Range(ref oStart, ref oEnd).Delete(ref missing, ref missing);  //处理完一页，就删除一页
                }

                ((MSWord.Document)document).Close(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(document);
                return bmList.ToArray();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                wordApplicationClass.Quit(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApplicationClass);
                System.IO.File.Delete(tmpFilePath);
            }
        }
        #endregion
    }
}