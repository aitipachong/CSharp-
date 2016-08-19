// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.DEncrypt
// * 文件名称：		    MySecurity.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-06-22
// * 程序功能描述：
// *        安全类
// *            1.字符串的加密、解密；
// *            2.文件的加密、解密；
// *            3.MD5加密
// *            4.Base64加密、解密
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace aitipachong.DEncrypt
{
    /// <summary>
    /// 安全类
    /// </summary>
    public class MySecurity
    {
        private string key = "";        //默认密钥

        private byte[] sKey;
        private byte[] sIV;

        /// <summary>
        /// 初始化安全类
        /// </summary>
        public MySecurity()
        {
            this.key = "AITIPACHNGSOFT";
        }

        #region 加密字符串

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="inputStr">明文</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>密文</returns>
        public static string SEncryptString(string inputStr, string keyStr)
        {
            MySecurity ms = new MySecurity();
            return ms.EncryptString(inputStr, keyStr);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="inputStr">明文</param>
        /// <returns>密文</returns>
        public static string SEncryptString(string inputStr)
        {
            MySecurity ws = new MySecurity();
            return ws.EncryptString(inputStr, "");
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="inputStr">明文</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>密文</returns>
        public string EncryptString(string inputStr, string keyStr)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            if (string.IsNullOrEmpty(keyStr)) keyStr = this.key;
            byte[] inputByteArray = System.Text.Encoding.Default.GetBytes(inputStr);
            byte[] keyByteArray = System.Text.Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            sKey = new byte[8];
            sIV = new byte[8];
            for (int i = 0; i < 8; i++)
                sKey[i] = hb[i];
            for (int i = 8; i < 16; i++)
                sIV[i - 8] = hb[i];
            des.Key = sKey;
            des.IV = sIV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach(byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            cs.Close();
            ms.Close();
            return ret.ToString();
        }
        #endregion

        #region 解密字符串
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="inputStr">密文</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>明文</returns>
        public static string SDecryptString(string inputStr, string keyStr)
        {
            MySecurity ws = new MySecurity();
            return ws.DecryptString(inputStr, keyStr);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="inputStr">密文</param>
        /// <returns>明文</returns>
        public static string SDecryptString(string inputStr)
        {
            MySecurity ws = new MySecurity();
            return ws.DecryptString(inputStr, "");
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="inputStr">密文</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>明文</returns>
        public string DecryptString(string inputStr, string keyStr)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            if (string.IsNullOrEmpty(keyStr)) keyStr = this.key;
            byte[] inputByteArray = new byte[inputStr.Length / 2];
            for(int i = 0; i < inputStr.Length / 2; i++)
            {
                int index = (Convert.ToInt32(inputStr.Substring(i * 2, 2), 16));
                inputByteArray[i] = (byte)index;
            }
            byte[] keyByteArray = System.Text.Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            sKey = new byte[8];
            sIV = new byte[8];
            for (int i = 0; i < 8; i++)
                sKey[i] = hb[i];
            for (int i = 8; i < 16; i++)
                sIV[i - 8] = hb[i];
            des.Key = sKey;
            des.IV = sIV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        #endregion

        #region 加密文件

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="filePath">明文文件路径</param>
        /// <param name="savePath">密文文件存储路径</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>是否加密成功</returns>
        public bool EncryptFile(string filePath, string savePath, string keyStr)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            if (string.IsNullOrEmpty(keyStr)) keyStr = this.key;
            FileStream fs = System.IO.File.OpenRead(filePath);
            byte[] inputByteArray = new byte[fs.Length];
            fs.Read(inputByteArray, 0, (int)fs.Length);
            fs.Close();
            byte[] keyByteArray = System.Text.Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            sKey = new byte[8];
            sIV = new byte[8];
            for (int i = 0; i < 8; i++)
                sKey[i] = hb[i];
            for (int i = 8; i < 16; i++)
                sIV[i - 8] = hb[i];
            des.Key = sKey;
            des.IV = sIV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            fs = System.IO.File.OpenWrite(savePath);
            foreach(byte b in ms.ToArray())
            {
                fs.WriteByte(b);
            }
            fs.Close();
            cs.Close();
            ms.Close();
            return true;
        }
        #endregion

        #region 解密文件
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="savePath">解密后输出文件路径</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns></returns>    
        public bool DecryptFile(string filePath, string savePath, string keyStr)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            if (keyStr == "")
                keyStr = key;
            FileStream fs = System.IO.File.OpenRead(filePath);
            byte[] inputByteArray = new byte[fs.Length];
            fs.Read(inputByteArray, 0, (int)fs.Length);
            fs.Close();
            byte[] keyByteArray = System.Text.Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            sKey = new byte[8];
            sIV = new byte[8];
            for (int i = 0; i < 8; i++)
                sKey[i] = hb[i];
            for (int i = 8; i < 16; i++)
                sIV[i - 8] = hb[i];
            des.Key = sKey;
            des.IV = sIV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            fs = System.IO.File.OpenWrite(savePath);
            foreach (byte b in ms.ToArray())
            {
                fs.WriteByte(b);
            }
            fs.Close();
            cs.Close();
            ms.Close();
            return true;
        }
        #endregion

        #region MD5加密（不可逆）

        /// <summary>
        /// 128位MD5算法加密字符串
        /// </summary>
        /// <param name="text">明文</param>
        /// <returns>密文</returns>
        public static string MD5(string text)
        {
            if (Tools.Tools.IsNullOrEmpty<string>(text)) return "";
            return MD5(System.Text.Encoding.Default.GetBytes(text));
        }

        /// <summary>
        /// 128位MD5算法加密byte数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MD5(byte[] data)
        {
            //如果Byte数组为空，则返回
            if (Tools.Tools.IsNullOrEmpty<byte[]>(data)) return "";
            try
            {
                //创建MD5密码服务提供服务
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                //计算传入的字节数组的哈希值
                byte[] result = md5.ComputeHash(data);
                //释放资源
                md5.Clear();
                //返回MD5值的字符串表示
                return Convert.ToBase64String(result);
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region Base64加密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <returns>密文</returns>
        public static string EncodeBase64(string text)
        {
            if (Tools.Tools.IsNullOrEmpty<string>(text)) return "";

            try
            {
                char[] Base64Code = new char[]{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T',
                                            'U','V','W','X','Y','Z','a','b','c','d','e','f','g','h','i','j','k','l','m','n',
                                            'o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7',
                                            '8','9','+','/','='};
                byte empty = (byte)0;
                ArrayList byteMessage = new ArrayList(System.Text.Encoding.Default.GetBytes(text));
                StringBuilder outmessage;
                int messageLen = byteMessage.Count;
                int page = messageLen / 3;
                int use = 0;
                if((use = messageLen % 3) > 0)
                {
                    for (int i = 0; i < 3 - use; i++)
                        byteMessage.Add(empty);
                    page++;
                }
                outmessage = new System.Text.StringBuilder(page * 4);
                for(int i = 0; i < page; i++)
                {
                    byte[] instr = new byte[3];
                    instr[0] = (byte)byteMessage[i * 3];
                    instr[1] = (byte)byteMessage[i * 3 + 1];
                    instr[2] = (byte)byteMessage[i * 3 + 2];
                    int[] outstr = new int[4];
                    outstr[0] = instr[0] >> 2;
                    outstr[1] = ((instr[0] & 0x03) << 4) ^ (instr[1] >> 4);
                    if (!instr[1].Equals(empty))
                        outstr[2] = ((instr[1] & 0x0f) << 2) ^ (instr[2] >> 6);
                    else
                        outstr[2] = 64;
                    if (!instr[2].Equals(empty))
                        outstr[3] = (instr[2] & 0x3f);
                    else
                        outstr[3] = 64;
                    outmessage.Append(Base64Code[outstr[0]]);
                    outmessage.Append(Base64Code[outstr[1]]);
                    outmessage.Append(Base64Code[outstr[2]]);
                    outmessage.Append(Base64Code[outstr[3]]);
                }
                return outmessage.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Base64解密
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <returns>明文</returns>
        public static string DecodeBase64(string text)
        {
            if (Tools.Tools.IsNullOrEmpty<string>(text)) return "";
            //将功课替换为加号
            text = text.Replace(" ", "+");

            try
            {
                if((text.Length % 4) != 0)
                {
                    return "包含不正确的BASE64编码";
                }
                if(!Regex.IsMatch(text, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
                {
                    return "包含不正确的BASE64编码";
                }
                string base64Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
                int page = text.Length / 4;
                ArrayList outMessage = new ArrayList(page * 3);
                char[] message = text.ToCharArray();
                for(int i = 0; i < page; i++)
                {
                    byte[] instr = new byte[4];
                    instr[0] = (byte)base64Code.IndexOf(message[i * 4]);
                    instr[1] = (byte)base64Code.IndexOf(message[i * 4 + 1]);
                    instr[2] = (byte)base64Code.IndexOf(message[i * 4 + 2]);
                    instr[3] = (byte)base64Code.IndexOf(message[i * 4 + 3]);
                    byte[] outstr = new byte[3];
                    outstr[0] = (byte)((instr[0] << 2) ^ ((instr[1] & 0x30) >> 4));
                    if (instr[2] != 64)
                        outstr[1] = (byte)((instr[1] << 4) ^ ((instr[2] & 0x3c) >> 2));
                    else
                        outstr[2] = 0;
                    if (instr[3] != 64)
                        outstr[2] = (byte)((instr[2] << 6) ^ instr[3]);
                    else
                        outstr[2] = 0;
                    outMessage.Add(outstr[0]);
                    if (outstr[1] != 0)
                        outMessage.Add(outstr[1]);
                    if (outstr[2] != 0)
                        outMessage.Add(outstr[2]);
                }
                byte[] outbyte = (byte[])outMessage.ToArray(Type.GetType("System.Byte"));
                return System.Text.Encoding.Default.GetString(outbyte);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}