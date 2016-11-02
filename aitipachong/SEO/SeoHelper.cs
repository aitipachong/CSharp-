// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.SEO
// * 文件名称：		    SeoHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-11-02
// * 程序功能描述：
// *        给搜索引擎提供操作帮助类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using aitipachong.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace aitipachong.SEO
{
    /// <summary>
    /// 给百度搜索引擎提供操作帮助类
    /// </summary>
    public class SeoHelper
    {
        /// <summary>
        /// 直接将提供的URL，发送到Ping百度http://ping.baidu.com/ping.html
        /// </summary>
        /// <param name="url">要发送的URL，注意带上"http://"字符串</param>
        /// <returns></returns>
        public static bool PingBaidu(string url)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\"?>");                sb.Append("<methodCall>");                sb.Append("<methodName>weblogUpdates.ping</methodName>");                sb.Append("<params>");                sb.Append("<param>");                sb.Append("<value><string>" + url + "</string></value>");                sb.Append("</param><param><value><string>" + url + "</string></value>");                sb.Append("</param>");                sb.Append("</params>");                sb.Append("</methodCall>");
                HttpHelper http = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = "http://ping.baidu.com/ping/RPC2",
                    Method = "POST",
                    Referer = "http://ping.baidu.com/ping.html",
                    Postdata = sb.ToString(),
                    ProtocolVersion = HttpVersion.Version10,
                };

                HttpResult result = http.GetHtml(item);
                if(result.Html.Contains("<int>0</int>"))
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return false;
        }

        /// <summary>
        /// 直接将提供的URL，提交给百度原创提交接口，需要自行申请Taken
        /// </summary>
        /// <param name="curl">要发送的url，注意带上"http://"字符串</param>
        /// <param name="token">TzIJxrHBBTH9VdsX默认的Token值</param>
        /// <returns></returns>
        public static bool OriginalPingBaidu(string curl, string token = "TzIJxrHBBTH9VdsX")
        {
            string url = string.Format("http://data.zz.baidu.com/urls?site={0}&token={1}", new Uri(curl).Host, token);            HttpHelper http = new HttpHelper();            HttpItem item = new HttpItem()            {                URL = url,                                  //URL       必需项
                Method = "POST",                            //URL       可选项 默认为Get
                Referer = curl,                             //来源URL   可选项
                Postdata = curl,                            //Post数据  可选项GET时不需要写
                ProtocolVersion = HttpVersion.Version10,                ContentType = "text/plain",                UserAgent = "curl/7.12.1"            };            HttpResult result = http.GetHtml(item);            if (result.Html.Contains("\"success\":1"))            {                return true;            }

            return false;        }
    }
}