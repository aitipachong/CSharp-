// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.DEncrypt
// * 文件名称：		    DEncrypt.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-06-16
// * 程序功能描述：
// *        对字符串或字节数组进行给定或缺省密码加密、解密。
// *            1.使用缺省密钥字符串，加密/解密String
// *            2.使用给定密钥字符串，加密/解密String
// *            3.使用缺省密钥字符串，加密/解密byte[]
// *            4.使用给定密钥字符串，加密/解密byte[]
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
    /// 对字符串或字节数组进行给定或缺省密码加密、解密。
    ///     1.使用缺省密钥字符串，加密/解密String
    ///     2.使用给定密钥字符串，加密/解密String
    ///     3.使用缺省密钥字符串，加密/解密byte[]
    ///     4.使用给定密钥字符串，加密/解密byte[]
    /// </summary>
    public class DEncrypt
    {
        private const string DEFAULT_KEY = "AITIPACHONGSOFT";
        /// <summary>
        /// 构造函数
        /// </summary>
        public DEncrypt() { }

        #region 1.使用缺省密钥字符串，加密/解密String
        /// <summary>
        /// 使用缺省密钥字符串加密String
        /// </summary>
        /// <param name="original">明文</param>
        /// <returns>密文</returns>
        public static string Encrypt(string original)
        {
            return Encrypt(original, DEFAULT_KEY);
        }

        /// <summary>
        /// 使用缺省密钥字符串解密String
        /// </summary>
        /// <param name="original">密文</param>
        /// <returns>明文</returns>
        public static string Decrypt(string original)
        {
            return Decrypt(original, DEFAULT_KEY, System.Text.Encoding.Default);
        }

        #endregion

        #region 2.使用给定密钥字符串，加密/解密String
        /// <summary>
        /// 使用给定密钥字符串加密String
        /// </summary>
        /// <param name="original">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>密文</returns>
        public static string Encrypt(string original, string key)
        {
            byte[] buff = System.Text.Encoding.Default.GetBytes(original);
            byte[] kb = System.Text.Encoding.Default.GetBytes(key);
            return Convert.ToBase64String(Encrypt(buff, kb));
        }

        /// <summary>
        /// 使用给定密钥字符串解密String
        /// </summary>
        /// <param name="original">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static string Decrypt(string original, string key)
        {
            return Decrypt(original, key, System.Text.Encoding.Default);
        }

        /// <summary>
        /// 使用给定密钥字符串解密String
        /// </summary>
        /// <param name="original">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码</param>
        /// <returns>明文</returns>
        public static string Decrypt(string original, string key, Encoding encoding)
        {
            byte[] buff = Convert.FromBase64String(original);
            byte[] kb = System.Text.Encoding.Default.GetBytes(key);
            return encoding.GetString(Decrypt(buff, kb));
        }
        #endregion

        #region 3.使用缺省密钥字符串，加密/解密byte[]
        /// <summary>
        /// 使用缺省密钥字符串解密byte[]
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <returns>明文</returns>
        public static byte[] Decrypt(byte[] encrypted)
        {
            byte[] key = System.Text.Encoding.Default.GetBytes(DEFAULT_KEY);
            return Decrypt(encrypted, key);
        }

        /// <summary>
        /// 使用缺省密钥字符串加密byte[]
        /// </summary>
        /// <param name="original">明文</param>
        /// <returns>密文</returns>
        public static byte[] Encrypt(byte[] original)
        {
            byte[] key = System.Text.Encoding.Default.GetBytes(DEFAULT_KEY);
            return Encrypt(original, key);
        }
        #endregion

        #region 4.使用给定密钥，加密/解密byte[]
        /// <summary>
        /// 生成MD5摘要
        /// </summary>
        /// <param name="original">数据源</param>
        /// <returns>摘要</returns>
        private static byte[] MakeMD5(byte[] original)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyHash = hashmd5.ComputeHash(original);
            hashmd5 = null;
            return keyHash;
        }

        /// <summary>
        /// 使用给定密钥，加密byte[]
        /// </summary>
        /// <param name="original">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>密文</returns>
        public static byte[] Encrypt(byte[] original, byte[] key)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD5(key);
            des.Mode = CipherMode.ECB;
            return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
        }

        /// <summary>
        /// 使用给定密钥，解密byte[]
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD5(key);
            des.Mode = CipherMode.ECB;
            return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }
        #endregion
    }
}