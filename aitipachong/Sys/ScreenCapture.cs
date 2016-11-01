// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Sys
// * 文件名称：		    ScreenCapture.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-18
// * 程序功能描述：
// *        屏幕捕捉类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace aitipachong.Sys
{
    class GDI32
    {
        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(int hdcDest, int nXDest, int nYDest,
                                         int nWidth, int nHeight, int hdcSrc,
                                         int nXSrc, int nYSrc, int dwRop);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleBitmap(int hdc, int nWidth,
                                                         int nHeight);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(int hObject);
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);
        [DllImport("GDI32.dll")]
        public static extern int SelectObject(int hdc, int hgdiobj);
    }

    class User32
    {
        [DllImport("User32.dll")]
        public static extern int GetDesktopWindow();
        [DllImport("User32.dll")]
        public static extern int GetWindowDC(int hWnd);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);
    }

    /// <summary>
    /// 屏幕捕捉类
    /// </summary>
    public class ScreenCapture : IDisposable
    {
        private static ScreenCapture instance = null;
        public static ScreenCapture Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ScreenCapture();
                }
                return ScreenCapture.instance;
            }
        }

        int hdcSrc, hdcDest;

        private ScreenCapture()
        {
            hdcSrc = User32.GetWindowDC(User32.GetDesktopWindow());
            hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
        }

        /// <summary>
        /// 屏幕捕捉
        /// </summary>
        /// <param name="rct">要捕捉的桌面范围</param>
        /// <returns>捕捉后的图形</returns>
        public Bitmap Capture(Rectangle rct)
        {
            int hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, rct.Width, rct.Height);

            GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, rct.Width, rct.Height,
                            hdcSrc, rct.Left, rct.Top, 0x00CC0020);
            Bitmap image = new Bitmap(Image.FromHbitmap(new IntPtr(hBitmap)),
                                        Image.FromHbitmap(new IntPtr(hBitmap)).Width,
                                        Image.FromHbitmap(new IntPtr(hBitmap)).Height);

            GDI32.DeleteObject(hBitmap);
            return image;
        }

        public void Dispose()
        {
            User32.ReleaseDC(User32.GetDesktopWindow(), hdcSrc);
            GDI32.DeleteDC(hdcDest);
        }
    }
}