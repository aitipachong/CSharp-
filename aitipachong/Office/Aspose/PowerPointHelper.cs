// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Office.Aspose
// * 文件名称：		    WordHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-13
// * 程序功能描述：
// *        PowerPoint文档帮助类，主要内容如下：
// *            1.转换导出
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using Aspose.Slides;
using Aspose.Slides.Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aitipachong.Office.Aspose
{
    /// <summary>
    /// PowerPoint文档帮助类
    /// </summary>
    public class PowerPointHelper
    {
        #region 转换导出

        /// <summary>
        /// 把PPT文档各页转换为图片
        /// </summary>
        /// <param name="pptInputPath">PowerPoint文档路径</param>
        /// <param name="imageOutputDirPath">图片保存目录路径</param>
        public bool ConvertPptToImage(string pptInputPath, string imageOutputDirPath)
        {
            return ConvertPptToImage(pptInputPath, imageOutputDirPath, 0, 0, 200);
        }

        /// <summary>
        /// 把PPT文档转换为图片
        /// </summary>
        /// <param name="pptInputPath">PowerPoint文档路径</param>
        /// <param name="imageOutputDirPath">图片保存目录路径</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换，如果为0，默认值为1</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换，如果为0，默认值为pdf总页数</param>       
        /// <param name="resolution">设置图片的像素，数字越大越清晰，如果为0，默认值为128，建议最大值不要超过1024</param>
        /// <returns></returns>
        public bool ConvertPptToImage(string pptInputPath, string imageOutputDirPath, int startPageNum, int endPageNum, int resolution)
        {
            bool result = false;
            //参数容错
            if (string.IsNullOrEmpty(pptInputPath)) throw new ArgumentNullException("PowerPoint file path is empty or null.");
            if (!System.IO.File.Exists(pptInputPath)) throw new FileNotFoundException("PowerPoint file is not exist.");
            if (string.IsNullOrEmpty(imageOutputDirPath)) imageOutputDirPath = Path.GetDirectoryName(pptInputPath);
            if (!Directory.Exists(imageOutputDirPath)) Directory.CreateDirectory(imageOutputDirPath);

            try
            {
                Presentation doc = new Presentation(pptInputPath);
                if (doc == null) throw new ArgumentNullException("PPT文件无效或者PPT文件被加密！");
                if (startPageNum <= 0) startPageNum = 1;
                if (endPageNum > doc.Slides.Count || endPageNum <= 0) endPageNum = doc.Slides.Count;
                if(startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum;
                    startPageNum = endPageNum;
                    endPageNum = tempPageNum;
                }
                if (resolution <= 0) resolution = 128;

                //将ppt转换为pdf临时文件
                string tempPdfPath = pptInputPath + ".pdf";
                doc.Save(tempPdfPath, SaveFormat.Pdf);

                //在把pdf转换为图片
                PDFHelper helper = new PDFHelper();
                helper.ConvertPdfToImage(tempPdfPath, imageOutputDirPath);

                //删除pdf临时文件
                System.IO.File.Delete(tempPdfPath);

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