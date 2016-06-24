// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.DEncrypt
// * 文件名称：		    HashEncode.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-06-16
// * 程序功能描述：
// *        得到随机安全码（哈希加密）
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Text;
using System.Security.Cryptography;     

namespace aitipachong.DEncrypt
{
    /// <summary>
    /// 得到随机安全码（哈希加密）
    /// </summary>
    public class HashEncode
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HashEncode()
        { }

        /// <summary>
        /// 得到随机哈希加密字符串
        /// </summary>
        /// <returns></returns>
        public static string GetSecurity()
        {
            string security = HashEncoding(GetRandomValue());
            return security;
        }

        /// <summary>
        /// 得到一个随机数值
        /// </summary>
        /// <returns></returns>
        private static string GetRandomValue()
        {
            Random seed = new Random();
            string randomValue = seed.Next(1, int.MaxValue).ToString();
            return randomValue;
        }

        /// <summary>
        /// 哈希加密一个字符串
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public static string HashEncoding(string security)
        {
            byte[] value;
            UnicodeEncoding code = new UnicodeEncoding();
            byte[] message = code.GetBytes(security);
            SHA512Managed arithmetic = new SHA512Managed();
            value = arithmetic.ComputeHash(message);
            security = "";
            foreach(byte b in value)
            {
                security += (int)b + "O";
            }
            return security;
        }
    }
}
