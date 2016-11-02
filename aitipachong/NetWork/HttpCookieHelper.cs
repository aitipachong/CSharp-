// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.NetWork
// * 文件名称：		    HttpCookieHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-11-02
// * 程序功能描述：
// *        Cookie操作帮助类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aitipachong.NetWork
{
    /// <summary>
    /// Cookie操作帮助类
    /// </summary>
    public static class HttpCookieHelper
    {
        #region Cookie基本操作
        /// <summary>
        /// 根据字符生成Cookie列表
        /// </summary>
        /// <param name="cookie">Cookie字符串</param>
        /// <returns></returns>
        public static List<CookieItem> GetCookieList(string cookie)
        {
            List<CookieItem> cookieList = new List<CookieItem>();
            foreach(string item in cookie.Split(new string[] { ";", ","}, StringSplitOptions.RemoveEmptyEntries))
            {
                if(Regex.IsMatch(item, @"([\s\S]*?)=([\s\S]*?)$"))
                {
                    Match m = Regex.Match(item, @"([\s\S]*?)=([\s\S]*?)$");
                    cookieList.Add(new CookieItem() { Key = m.Groups[1].Value, Value = m.Groups[2].Value });
                }
            }

            return cookieList;
        }

        /// <summary>
        /// 根据Key值得到Cookie值，Key不区分大小写
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="cookie">字符串Cookie</param>
        /// <returns></returns>
        public static string GetCookieValue(string key, string cookie)
        {
            foreach(CookieItem item in GetCookieList(cookie))
            {
                if(item.Key.Trim().ToLower() == key.Trim().ToLower())
                {
                    return item.Value;
                }
            }

            return "";
        }

        /// <summary>
        /// 格式化Cookie为标准格式
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static string CookieFormat(string key, string value)
        {
            return string.Format("{0}={1};", key, value);
        }

        #endregion

        #region 使用WebBrowser提取Cookie

        /// <summary>
        /// 取当前webBrowser登录后的Cookie值 
        /// </summary>
        /// <param name="pchURL"></param>
        /// <param name="pchCookieName"></param>
        /// <param name="pchCookieData"></param>
        /// <param name="pcchCookieData"></param>
        /// <param name="dwFlags"></param>
        /// <param name="lpReserved"></param>
        /// <returns></returns>
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref int pcchCookieData, int dwFlags, object lpReserved);

        /// <summary>
        /// 提取Cookie，当登陆后才能取
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCookieString(string url)
        {
            int datasize = 256;
            StringBuilder cookieData = new StringBuilder(datasize);
            if(!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, null))
            {
                if(datasize < 0)
                    return null;
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, null))
                    return null;
            }

            return cookieData.ToString();
        }
        #endregion
    }

    /// <summary>
    /// Cookie对象
    /// </summary>
    public class CookieItem
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}