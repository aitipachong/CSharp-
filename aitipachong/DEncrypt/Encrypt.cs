// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.DEncrypt
// * 文件名称：		    Encrypt.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-06-16
// * 程序功能描述：
// *        加密/解密实用类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Security.Cryptography;
using System.IO;

namespace aitipachong.DEncrypt
{
    /// <summary>
    /// 加密/解密实用类
    /// </summary>
    public class Encrypt
    {
        //密钥
        private static byte[] arrDESKey = new byte[] { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static byte[] arrDESIV = new byte[] { 55, 103, 246, 79, 36, 99, 167, 3 };

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <returns>密文</returns>
        public static string Encode(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                throw new Exception("Error:\n源字符串为空!");
            }

            DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
            MemoryStream objMemoryStream = new MemoryStream();
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateEncryptor(arrDESKey, arrDESIV), CryptoStreamMode.Write);
            StreamWriter objStreamWriter = new StreamWriter(objCryptoStream);
            objStreamWriter.Write(text);
            objStreamWriter.Flush();
            objCryptoStream.FlushFinalBlock();
            objMemoryStream.Flush();
            return Convert.ToBase64String(objMemoryStream.GetBuffer(), 0, (int)objMemoryStream.Length);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <returns>明文</returns>
        public static string Decode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("Error: \n源字符串为空!");
            }
            DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
            byte[] arrInput = Convert.FromBase64String(text);
            MemoryStream objMemoryStream = new MemoryStream(arrInput);
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateDecryptor(arrDESKey, arrDESIV), CryptoStreamMode.Read);
            StreamReader objStreamReader = new StreamReader(objCryptoStream);
            return objStreamReader.ReadToEnd();
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="encypStr">明文</param>
        /// <returns>密文</returns>
        public static string Md5(string encypStr)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();
            byte[] inputBye;
            byte[] outputBye;
            inputBye = System.Text.Encoding.ASCII.GetBytes(encypStr);
            outputBye = m5.ComputeHash(inputBye);
            retStr = Convert.ToBase64String(outputBye);
            return retStr;
        }
    }
}