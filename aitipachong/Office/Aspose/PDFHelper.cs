// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Office.Aspose
// * 文件名称：		    PDFHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-13
// * 程序功能描述：
// *        PDF文档帮助类，主要内容如下：
// *            1.转换导出
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aitipachong.Office.Aspose
{
    /// <summary>
    /// PDF帮助类
    /// </summary>
    public class PDFHelper
    {
        #region 转换导出

        /// <summary>
        /// PDF转换为图片
        /// </summary>
        /// <param name="pdfInputPath">PDF文档路径</param>
        /// <param name="imageOutputDirPath">图片保存目录路径</param>
        public bool ConvertPdfToImage(string pdfInputPath, string imageOutputDirPath)
        {
            return ConvertPdfToImage(pdfInputPath, imageOutputDirPath, 0, 0, 200);
        }

        /// <summary>
        /// PDF转换为图片
        /// </summary>
        /// <param name="pdfInputPath">PDF文档路径</param>
        /// <param name="imageOutputDirPath">图片保存目录路径</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换，如果为0，默认值为1</param>
        /// <param name="endPageNum">从PDF文档的第几页停止转换，如果为0，默认为PDF总页数</param>
        /// <param name="resolution">设置图片像素，数字越大越清晰，如果为0，默认值为128，建议最大值不要超过1024</param>
        /// <returns></returns>
        public bool ConvertPdfToImage(string pdfInputPath, string imageOutputDirPath, int startPageNum, int endPageNum, int resolution)
        {
            bool result = false;
            //参数容错
            if (string.IsNullOrEmpty(pdfInputPath)) throw new ArgumentNullException("Pdf file path is empty or null.");
            if (!System.IO.File.Exists(pdfInputPath)) throw new FileNotFoundException("Pdf file is not exist.");
            if (string.IsNullOrEmpty(imageOutputDirPath)) imageOutputDirPath = Path.GetDirectoryName(pdfInputPath);
            if (!Directory.Exists(imageOutputDirPath)) Directory.CreateDirectory(imageOutputDirPath);

            try
            {
                Document doc = new Document(pdfInputPath);
                if (doc == null) throw new ArgumentNullException("pdf文件无效或者pdf文件被加密！");
                if (startPageNum <= 0) startPageNum = 1;
                if (endPageNum > doc.Pages.Count || endPageNum <= 0) endPageNum = doc.Pages.Count;
                if(startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum;
                    startPageNum = endPageNum;
                    endPageNum = tempPageNum;
                }
                if (resolution <= 0) resolution = 128;
                string imageName = Path.GetFileNameWithoutExtension(pdfInputPath);

                for(int i = startPageNum; i <= endPageNum; i++)
                {
                    MemoryStream stream = new MemoryStream();
                    string imgPath = Path.Combine(imageOutputDirPath, imageName + "_" + i.ToString("000") + ".jpg");
                    Resolution reso = new Resolution(resolution);
                    JpegDevice jpegDevice = new JpegDevice(reso, 100);
                    jpegDevice.Process(doc.Pages[i], stream);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    Bitmap bm = ESBasic.Helpers.ImageHelper.Zoom(img, 0.6f);
                    bm.Save(imgPath, ImageFormat.Jpeg);
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


        #endregion
    }
}