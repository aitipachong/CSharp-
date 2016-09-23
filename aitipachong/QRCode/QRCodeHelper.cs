// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.QRCode
// * 文件名称：		    QRCodeHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-09-23
// * 程序功能描述：
// *        二维码帮助类：生成、解读二维码
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using com.google.zxing;
using com.google.zxing.common;
using System;
using System.Drawing;
using System.IO;

namespace aitipachong.QRCode
{
    /// <summary>
    /// 二维码帮助类：生成、解读二维码
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 生成二维码，且保存为jpg图片
        /// </summary>
        /// <param name="context">二维码含义</param>
        /// <param name="pictureWidth">二维码图片宽度</param>
        /// <param name="pictureHeight">二维码图片高度</param>
        /// <returns>生成是否成功</returns>
        public bool CreateQRCodeImage(string context, int pictureWidth, int pictureHeight)
        {
            bool isResult = false;
            if (string.IsNullOrEmpty(context)) throw new ArgumentNullException("context", "二维码含义不能为空.");

            try
            {
                MultiFormatWriter mutiWriter = new MultiFormatWriter();
                ByteMatrix bm = mutiWriter.encode(context, BarcodeFormat.QR_CODE, pictureWidth, pictureHeight);
                Bitmap img = bm.ToBitmap();
                //自动保存图片到当前目录下的“QR文件夹下”
                string qrFolderPath = Path.Combine(System.Environment.CurrentDirectory, "QR");
                if (!System.IO.Directory.Exists(qrFolderPath)) System.IO.Directory.CreateDirectory(qrFolderPath);
                string fileName = Path.Combine(qrFolderPath, "QR_" + DateTime.Now.Ticks.ToString() + ".jpg");
                img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                isResult = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return isResult;
        }

        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="picturePath"></param>
        /// <returns></returns>
        public string UnscrambleQRCode(string picturePath)
        {
            string result = string.Empty;
            if (!System.IO.File.Exists(picturePath)) throw new FileNotFoundException("二维码图片不存在.", picturePath);
            try
            {
                MultiFormatReader mutiReader = new MultiFormatReader();
                Bitmap img = (Bitmap)Bitmap.FromFile(picturePath);
                if (img == null) return result;
                LuminanceSource ls = new RGBLuminanceSource(img, img.Width, img.Height);
                BinaryBitmap bb = new BinaryBitmap(new HybridBinarizer(ls));
                Result r = mutiReader.decode(bb);
                result = r.Text;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}