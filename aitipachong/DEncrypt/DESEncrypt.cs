// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.DEncrypt
// * 文件名称：		    DESEncrypt.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-06-16
// * 程序功能描述：
// *        对字符串进行DES加密/解密类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Security.Cryptography;
using System.Text;

namespace aitipachong.DEncrypt
{
    /// <summary>
    /// 对字符串进行DES加密/解密类
    /// </summary>
    public class DESEncrypt
    {
        private const string DEFAULT_KEY = "AITIPACHONGSOFT";
        /// <summary>
        /// 构造函数
        /// </summary>
        public DESEncrypt() { }

        #region DES加密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <returns>密文</returns>
        public static string Encrypt(string text)
        {
            return Encrypt(text, DEFAULT_KEY);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>密文</returns>
        public static string Encrypt(string text, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach(byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }
        #endregion

        #region DES解密
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <returns>明文</returns>
        public static string Decrypt(string text)
        {
            return Decrypt(text, DEFAULT_KEY);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static string Decrypt(string text, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for(x = 0; x < len; x++)
            {
                i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
        #endregion
    }
}