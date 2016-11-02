// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.NetWork
// * 文件名称：		    HttpHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-11-01
// * 程序功能描述：
// *        用来实现HTTP访问，Post或者Get方式的，直接访问，带Cookie的，带证书等方式，可以设置代理。
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace aitipachong.NetWork
{
    /// <summary>
    /// Http连接操作帮助类
    /// </summary>
    public class HttpHelper
    {
        #region 预定义变量
        /// <summary>
        /// 默认编码
        /// </summary>
        private System.Text.Encoding encoding = System.Text.Encoding.Default;
        /// <summary>
        /// Post数据默认编码
        /// </summary>
        private System.Text.Encoding postencoding = System.Text.Encoding.Default;
        /// <summary>
        /// HttpWebRequest对象用来发起请求
        /// </summary>
        private HttpWebRequest request = null;
        /// <summary>
        /// HttpWebResponse对象用来获取响应流的数据
        /// </summary>
        private HttpWebResponse response = null;
        #endregion

        #region 公有函数
        /// <summary>
        /// 根据传入的数据，得到相应页面数据
        /// </summary>
        /// <param name="item">参数类对象</param>
        /// <returns>返回HttpResult类型</returns>
        public HttpResult GetHtml(HttpItem item)
        {
            HttpResult result = new HttpResult();
            try
            {
                //准备参数
                SetRequest(item);
            }
            catch(Exception ex)
            {
                result.Cookie = string.Empty;
                result.Header = null;
                result.Html = ex.Message;
                result.StatusDescription = "配置参数时出错：" + ex.Message;
                return result;
            }

            try
            {
                //请求数据
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    GetData(item, result);
                }
            }
            catch(WebException ex)
            {
                if(ex.Message != null)
                {
                    using (response = (HttpWebResponse)ex.Response)
                    {
                        GetData(item, result);
                    }
                }
                else
                {
                    result.Html = ex.Message;
                }
            }
            catch(Exception ex)
            {
                result.Html = ex.Message;
            }

            if (item.IsToLower) result.Html = result.Html.ToLower();
            return result;
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 获取数据，并解析
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        private void GetData(HttpItem item, HttpResult result)
        {
            #region base
            //获取StatusCode
            result.StatusCode = response.StatusCode;
            //获取StatusDescription
            result.StatusDescription = response.StatusDescription;
            //获取Headers
            result.Header = response.Headers;
            //获取CookieCollection
            if (response.Cookies != null) result.CookieCollection = response.Cookies;
            //获取set-cookie
            if (response.Headers["set-cookie"] != null) result.Cookie = response.Headers["set-cookie"];
            #endregion

            #region byte
            //处理网页Byte
            byte[] responseByte = GetByte();
            #endregion

            #region Html
            if(responseByte != null && responseByte.Length > 0)
            {
                //设置编码
                SetEncoding(item, result, responseByte);
                //得到返回的HTML
                result.Html = encoding.GetString(responseByte);
            }
            else
            {
                //没有返回任何HTML代码
                result.Html = string.Empty;
            }
            #endregion
        }

        /// <summary>
        /// 设置编码
        /// </summary>
        /// <param name="item">HttpItem</param>
        /// <param name="result">HttpResult</param>
        /// <param name="responseByte">byte[]</param>
        private void SetEncoding(HttpItem item, HttpResult result, byte[] responseByte)
        {
            //是否返回Byte类型数据
            if (item.ResultType == ResultType.Byte) result.ResultByte = responseByte;
            
            if(encoding == null)
            {
                Match meta = Regex.Match(System.Text.Encoding.Default.GetString(responseByte),
                                "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                string c = string.Empty;
                if(meta != null && meta.Groups.Count > 0)
                {
                    c = meta.Groups[1].Value.ToLower().Trim();
                }
                if(c.Length > 2)
                {
                    try
                    {
                        encoding = System.Text.Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                    }
                    catch
                    {
                        if(string.IsNullOrEmpty(response.CharacterSet))
                        {
                            encoding = System.Text.Encoding.UTF8;
                        }
                        else
                        {
                            encoding = System.Text.Encoding.GetEncoding(response.CharacterSet);
                        }
                    }
                }
                else
                {
                    if(string.IsNullOrEmpty(response.CharacterSet))
                    {
                        encoding = System.Text.Encoding.UTF8;
                    }
                    else
                    {
                        encoding = System.Text.Encoding.GetEncoding(response.CharacterSet);
                    }
                }
            }
        }

        /// <summary>
        /// 提取网页Byte
        /// </summary>
        /// <returns></returns>
        private byte[] GetByte()
        {
            byte[] responseByte = null;
            MemoryStream stream = new MemoryStream();

            //GZIIP处理
            if(response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
            {
                //开始读取流并设置编码方式
                stream = GetMemoryStream(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
            }
            else
            {
                //开始读取流并设置编码方式
                stream = GetMemoryStream(response.GetResponseStream());
            }
            //获取byte
            responseByte = stream.ToArray();
            stream.Close();
            return responseByte;
        }

        /// <summary>
        /// 4.0以下.net版本取数据使用
        /// </summary>
        /// <param name="streamResponse"></param>
        /// <returns></returns>
        private MemoryStream GetMemoryStream(Stream streamResponse)
        {
            MemoryStream stream = new MemoryStream();
            int length = 256;
            Byte[] buffer = new Byte[length];
            int bytesRead = streamResponse.Read(buffer, 0, length);
            while(bytesRead > 0)
            {
                stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, length);
            }
            return stream;
        }

        #region 设置请求
        /// <summary>
        /// 为请求准备参数
        /// </summary>
        /// <param name="item"></param>
        private void SetRequest(HttpItem item)
        {
            //验证证书
            SetCer(item);
            //设置Header参数
            if(item.Header != null && item.Header.Count > 0)
            {
                foreach(string key in item.Header.AllKeys)
                {
                    request.Headers.Add(key, item.Header[key]);
                }
            }
            //设置代理
            SetProxy(item);
            if (item.ProtocolVersion != null) request.ProtocolVersion = item.ProtocolVersion;
            request.ServicePoint.Expect100Continue = item.Expect100Continue;
            //请求方式Get或Post
            request.Method = item.Method;
            request.Timeout = item.Timeout;
            request.KeepAlive = item.KeepAlive;
            request.ReadWriteTimeout = item.ReadWriteTimeout;
            if (item.IfModifiedSince != null) request.IfModifiedSince = Convert.ToDateTime(item.IfModifiedSince);
            //Accept
            request.Accept = item.Accept;
            //ContentType返回类型
            request.ContentType = item.ContentType;
            //UserAgent客户端的访问类型，包括浏览器版本和操作系统信息
            request.UserAgent = item.UserAgent;
            //编码
            encoding = item.Encoding;
            //设置安全凭证
            request.Credentials = item.ICredentials;
            //设置Cookie
            SetCookie(item);
            //来源地址
            request.Referer = item.Referer;
            //是否执行跳转功能
            request.AllowAutoRedirect = item.Allowautoredirect;
            if(item.MaximumAutomaticRedirections > 0)
            {
                request.MaximumAutomaticRedirections = item.MaximumAutomaticRedirections;
            }
            //设置Post数据
            SetPostData(item);
            //设置最大链接
            if (item.Connectionlimit > 0) request.ServicePoint.ConnectionLimit = item.Connectionlimit;
        }

        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="item"></param>
        private void SetCer(HttpItem item)
        {
            if(!string.IsNullOrEmpty(item.CerPath))
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //初始化对象，并设置请求的URL地址
                request = (HttpWebRequest)WebRequest.Create(item.URL);
                SetCerList(item);
                //将证书添加到请求里
                request.ClientCertificates.Add(new X509Certificate(item.CerPath));
            }
            else
            {
                //初始化对象，并设置请求的URL地址
                request = (HttpWebRequest)WebRequest.Create(item.URL);
                SetCerList(item);
            }
        }

        /// <summary>
        /// 设置多个证书
        /// </summary>
        /// <param name="item"></param>
        private void SetCerList(HttpItem item)
        {
            if(item.ClentCertificates != null && item.ClentCertificates.Count > 0)
            {
                foreach(X509Certificate c in item.ClentCertificates)
                {
                    request.ClientCertificates.Add(c);
                }
            }
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="item"></param>
        private void SetCookie(HttpItem item)
        {
            if (!string.IsNullOrEmpty(item.Cookie)) request.Headers[HttpRequestHeader.Cookie] = item.Cookie;
            //设置CookieCollection
            if(item.ResultCookieType == ResultCookieType.CookieCollection)
            {
                request.CookieContainer = new CookieContainer();
                if(item.CookieCollection != null && item.CookieCollection.Count > 0)
                {
                    request.CookieContainer.Add(item.CookieCollection);
                }
            }
        }

        /// <summary>
        /// 设置Post数据
        /// </summary>
        /// <param name="item"></param>
        private void SetPostData(HttpItem item)
        {
            //验证在得到结果时，是否有传入数据
            if(!request.Method.Trim().ToLower().Contains("get"))
            {
                if (item.PostEncoding != null) postencoding = item.PostEncoding;
                byte[] buffer = null;
                //写入Byte类型
                if(item.PostDataType == PostDataType.Byte && item.PostdataByte != null && item.PostdataByte.Length > 0)
                {
                    //验证在得到结果时，是否有传入数据
                    buffer = item.PostdataByte;
                }
                else if(item.PostDataType == PostDataType.FilePath && !string.IsNullOrEmpty(item.Postdata))
                {
                    //写入文件
                    StreamReader r = new StreamReader(item.Postdata, postencoding);
                    buffer = postencoding.GetBytes(r.ReadToEnd());
                    r.Close();
                }
                else if(!string.IsNullOrEmpty(item.Postdata))
                {
                    //写入字符串
                    buffer = postencoding.GetBytes(item.Postdata);
                }

                if(buffer != null)
                {
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="item"></param>
        private void SetProxy(HttpItem item)
        {
            bool isIeProxy = false;
            if(!string.IsNullOrEmpty(item.ProxyIp))
            {
                isIeProxy = item.ProxyIp.ToLower().Contains("ieproxy");
            }
            if(!string.IsNullOrEmpty(item.ProxyIp) && !isIeProxy)
            {
                //设置代理服务器
                if(item.ProxyIp.Contains(":"))
                {
                    string[] plist = item.ProxyIp.Split(':');
                    WebProxy myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
                    //建议链接
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
                else
                {
                    WebProxy myProxy = new WebProxy(item.ProxyIp, false);
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    request.Proxy = myProxy;
                }
            }
            else if(isIeProxy)
            {
                //设置为IE代理
            }
            else
            {
                request.Proxy = item.WebProxy;
            }
        }
        #endregion

        #region private main
        /// <summary>
        /// 回调验证证书
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        #endregion
        #endregion
    }

    #region HTTP相关参数类
    /// <summary>
    /// Http请求参考类
    /// </summary>
    public class HttpItem
    {
        /// <summary>
        /// 请求URL（必须填写）
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 请求方式，默认为GET方式；当为POST方式时，必须设置Postdata值
        /// </summary>
        public string Method { get; set; } = "GET";
        
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout { get; set; } = 100000;

        /// <summary>
        /// 默认写入Post数据超时时间
        /// </summary>
        public int ReadWriteTimeout { get; set; } = 30000;

        /// <summary>
        /// 获取或设置一个值，该值表示是否与Internet资源建立持久性链接，默认为true.
        /// </summary>
        public Boolean KeepAlive { get; set; } = true;

        /// <summary>
        /// 请求标头值，默认为：text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept { get; set; } = "text/html, application/xhtml+xml, */*";

        /// <summary>
        /// 请求返回类型，默认：text/html
        /// </summary>
        public string ContentType { get; set; } = "text/html";

        /// <summary>
        /// 客户端访问信息，默认：Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";

        /// <summary>
        /// 返回数据编码，默认为：null, 可以自动识别，一般为:utf-8, gbk, gb2312
        /// </summary>
        public System.Text.Encoding Encoding { get; set; } = null;

        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType { get; set; } = PostDataType.String;

        /// <summary>
        /// Post请求时，要发送的字符串Post数据
        /// </summary>
        public string Postdata { get; set; } = string.Empty;

        /// <summary>
        /// Post请求时，要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte { get; set; } = null;

        /// <summary>
        /// 设置代理对象，不像使用IE默认配置就设置为Null,而且不要设置ProxyIP
        /// </summary>
        public WebProxy WebProxy { get; set; }

        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; } = null;

        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie { get; set; } = string.Empty;

        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer { get; set; } = string.Empty;

        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath { get; set; } = string.Empty;

        /// <summary>
        /// 是否设置为全文小写，默认为：不转化
        /// </summary>
        public Boolean IsToLower { get; set; } = false;

        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public Boolean Allowautoredirect { get; set; } = false;

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit { get; set; } = 1024;

        /// <summary>
        /// 代理Proxy服务器用户名
        /// </summary>
        public string ProxyUserName { get; set; } = string.Empty;

        /// <summary>
        /// 代理Proxy服务器密码
        /// </summary>
        public string ProxyPwd { get; set; } = string.Empty;

        /// <summary>
        /// 代理Proxy服务器IP，如果要使用IE代理，就设置为ieproxy
        /// </summary>
        public string ProxyIp { get; set; } = string.Empty;

        /// <summary>
        /// 设置返回类型：String或Byte
        /// </summary>
        public ResultType ResultType { get; set; } = ResultType.String;

        /// <summary>
        /// Header对象集合
        /// </summary>
        public WebHeaderCollection Header { get; set; } = new WebHeaderCollection();

        /// <summary>
        /// 获取或设置用于请求的HTTP版本。返回结果：用于请求的HTTP版本。默认为：System.Net.HttpVersion.Version11
        /// </summary>
        public Version ProtocolVersion { get; set; } = System.Net.HttpVersion.Version11;

        /// <summary>
        /// 获取或设置一个System.Boolean值，该值确定是否使用 100-Continue 行为。如果Post请求需要100-Continue响应，则为true;默认值为true.
        /// </summary>
        public Boolean Expect100Continue { get; set; } = true;

        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClentCertificates { get; set; }

        /// <summary>
        /// 获取或设置Post参数编码，默认为:Default编码
        /// </summary>
        public System.Text.Encoding PostEncoding { get; set; } = System.Text.Encoding.Default;

        /// <summary>
        /// Cookie返回类型，默认的是只返回字符串类型
        /// </summary>
        public ResultCookieType ResultCookieType { get; set; } = ResultCookieType.String;

        /// <summary>
        /// 获取或设置请求的身份验证信息
        /// </summary>
        public ICredentials ICredentials { get; set; } = CredentialCache.DefaultCredentials;

        /// <summary>
        /// 设置请求将跟随的重定向的最大数目
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; }

        /// <summary>
        /// 获取或设置IfModifiedSince，默认为当前日期和时间
        /// </summary>
        public DateTime? IfModifiedSince { get; set; } = null;
    }

    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie { get; set; }

        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }

        /// <summary>
        /// 返回的String类型数据，只有ResultType.String时，才返回数据，其他情况为空
        /// </summary>
        public string Html { get; set; } = string.Empty;

        /// <summary>
        /// 返回的Byte数组，只有ResultType.Byte时，才返回数据，其他情况为空
        /// </summary>
        public byte[] ResultByte { get; set; }

        /// <summary>
        /// Header对象
        /// </summary>
        public WebHeaderCollection Header { get; set; }

        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// 返回状态码，默认为：OK
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
    #endregion

    #region 枚举定义
    /// <summary>
    /// Post数据格式，默认为:string
    /// </summary>
    public enum PostDataType
    {
        /// <summary>
        /// 字符串类型，这时编码Encoding可不设置.
        /// </summary>
        String,
        /// <summary>
        /// Byte类型，需要设置，PostDataByte参数的值编码Encoding可设置为空
        /// </summary>
        Byte,
        /// <summary>
        /// 传文件，PostData必须设置为文件的绝对路径，必须设置Encoding的值
        /// </summary>
        FilePath
    }

    /// <summary>
    /// 返回类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 表示只返回字符串，只有Html有数据
        /// </summary>
        String,
        /// <summary>
        /// 表示返回字符串和字节流，ResultType和Html都有数据返回
        /// </summary>
        Byte
    }

    /// <summary>
    /// Cookie返回类型
    /// </summary>
    public enum ResultCookieType
    {
        /// <summary>
        /// 只返回字符串类型的Cookie
        /// </summary>
        String,
        /// <summary>
        /// CookieCollection格式的Cookie集合，同时也返回String类型的Cookie
        /// </summary>
        CookieCollection
    }
    #endregion
}
