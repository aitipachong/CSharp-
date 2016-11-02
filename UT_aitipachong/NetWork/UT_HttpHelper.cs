using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aitipachong.NetWork;
using System.Collections.Generic;

namespace UT_aitipachong.NetWork
{
    [TestClass]
    public class UT_HttpHelper
    {
        /// <summary>
        /// 测试HttpHelper和HttpCookieHelper的使用1
        /// </summary>
        [TestMethod]
        public void UT_HttpHelperUse_V1()
        {
            try
            {
                HttpHelper http = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = "http://www.sufeinet.com",        //URL地址
                    Method = "get",                         //请求方式：get
                    ResultType = ResultType.Byte
                };

                //得到HTML代码
                HttpResult result = http.GetHtml(item);

                //得到Cookie列表
                List<CookieItem> cookieList = HttpCookieHelper.GetCookieList(result.Cookie);
                
                //第一个Cookie项的Key值和Value值
                string key = cookieList[0].Key;
                string value = cookieList[0].Value;

                //格式化Cookie
                string cookieItem = HttpCookieHelper.CookieFormat(key, value);

                //根据Key取Value
                string strValue = HttpCookieHelper.GetCookieValue("domain", result.Cookie);

                Assert.IsTrue(true);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_HttpHelperUse_V2()
        {
            try
            {
                HttpHelper http = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = "http://bbs.xmfish.com/login.php",
                    Encoding = null,
                    Method = "post",
                    UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",
                    Accept = "text/html, application/xhtml+xml, */*",
                    ContentType = "application/x-www-form-urlencoded",
                    Postdata = "answer=&cktime=0&customquest=&forward=&hideid=0&jumpurl=&lgt=0&pwpwd=&pwuser=&question=0&step=2&submit=",
                    Allowautoredirect = true
                };

                HttpResult result = http.GetHtml(item);

                string cookie = string.Empty;
                foreach(CookieItem s in HttpCookieHelper.GetCookieList(result.Cookie))
                {
                    if(s.Key.Contains("24a79_"))
                    {
                        cookie += HttpCookieHelper.CookieFormat(s.Key, s.Value);
                    }
                }

                if(result.Html.IndexOf("您已经顺利登陆") > 0)
                {
                    item = new HttpItem()
                    {
                        URL = "http://bbs.xmfish.com/u.php",
                        Cookie = cookie
                    };

                    result = http.GetHtml(item);
                }

                Assert.IsTrue(true);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}