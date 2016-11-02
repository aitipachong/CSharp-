// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.DEncrypt
// * 文件名称：		    RSACryption.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-06-23
// * 程序功能描述：
// *        RSA加密、解密（即：公钥、私钥加解密）
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
    /// RSA加密、解密（即：公钥、私钥加解密）
    /// </summary>
    public class RSACryption
    {
        public RSACryption()
        {
        }

        #region RAS加密解密

        #region RSA密钥产生
        /// <summary>
        /// 生成RSA的密钥，产生私钥和公钥
        /// </summary>
        /// <param name="xmlPrivateKeys">私钥</param>
        /// <param name="xmlPublicKey">公钥</param>
        public void RSAKey(out string xmlPrivateKeys, out string xmlPublicKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
            xmlPrivateKeys = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        }
        #endregion

        #region RSA加密
        //####################################################
        //##    RAS方式加密                                 ##
        //##    说明KEY必须是XML的形式，返回的是字符串      ##
        //##    在有一点需要说明：该加密方式有长度限制      ##
        //####################################################
        
        /// <summary>
        /// RSA加密字符串
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="m_strEncryptString">明文</param>
        /// <returns>密文</returns>
        public string RSAEncrypt(string xmlPublicKey, string encryptString)
        {
            byte[] plainTextBArray;
            byte[] cypherTextBArray;
            string result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            plainTextBArray = (new UnicodeEncoding()).GetBytes(encryptString);
            cypherTextBArray = rsa.Encrypt(plainTextBArray, false);
            result = Convert.ToBase64String(cypherTextBArray);
            return result;
        }

        /// <summary>
        /// RSA加密Byte数组
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="encryptBytes">明文</param>
        /// <returns>密文</returns>
        public string RSAEncrypt(string xmlPublicKey, byte[] encryptBytes)
        {
            byte[] cypherTextBArray;
            string result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            cypherTextBArray = rsa.Encrypt(encryptBytes, false);
            result = Convert.ToBase64String(cypherTextBArray);
            return result;
        }
        #endregion

        #region RSA解密
        /// <summary>
        /// RSA解密字符串
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="decryptString">密文</param>
        /// <returns>明文</returns>
        public string RSADecrypt(string xmlPrivateKey, string decryptString)
        {
            byte[] plainTextBArray;
            byte[] dypherTextBArray;
            string result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            plainTextBArray = Convert.FromBase64String(decryptString);
            dypherTextBArray = rsa.Decrypt(plainTextBArray, false);
            result = (new UnicodeEncoding()).GetString(dypherTextBArray);
            return result;
        }

        /// <summary>
        /// RSA解密Byte数组
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="decryptBytes">密文</param>
        /// <returns>明文</returns>
        public string RSADecrypt(string xmlPrivateKey, byte[] decryptBytes)
        {
            byte[] dypherTextBArray;
            string result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            dypherTextBArray = rsa.Decrypt(decryptBytes, false);
            result = (new UnicodeEncoding()).GetString(dypherTextBArray);
            return result;
        }
        #endregion

        #endregion

        #region RSA数字签名

        #region 获取Hash描述表
        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="strSource">源字符串</param>
        /// <param name="hashData">哈希数据(byte数组)</param>
        /// <returns></returns>
        public bool GetHash(string strSource, ref byte[] hashData)
        {
            //从源字符串中获取Hash描述
            byte[] buffer;
            HashAlgorithm md5 = HashAlgorithm.Create("MD5");
            buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(strSource);
            hashData = md5.ComputeHash(buffer);

            return true;
        }

        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="strSource">源字符串</param>
        /// <param name="strHashData">哈希字符串</param>
        /// <returns></returns>
        public bool GetHash(string strSource, ref string strHashData)
        {
            //从源字符串中取得Hash描述
            byte[] buffer;
            byte[] hashData;
            HashAlgorithm md5 = HashAlgorithm.Create("MD5");
            buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(strSource);
            hashData = md5.ComputeHash(buffer);
            strHashData = Convert.ToBase64String(hashData);
            return true;
        }

        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="objFile"></param>
        /// <param name="hashData"></param>
        /// <returns></returns>
        public bool GetHash(System.IO.FileStream objFile, ref byte[] hashData)
        {
            //从文件中取得Hash描述
            HashAlgorithm md5 = HashAlgorithm.Create("MD5");
            hashData = md5.ComputeHash(objFile);
            objFile.Close();
            return true;
        }

        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="objFile"></param>
        /// <param name="strHashData"></param>
        /// <returns></returns>
        public bool GetHash(System.IO.FileStream objFile, ref string strHashData)
        {
            byte[] hashData;
            //从文件中取得Hash描述
            HashAlgorithm md5 = HashAlgorithm.Create("MD5");
            hashData = md5.ComputeHash(objFile);
            objFile.Close();

            strHashData = Convert.ToBase64String(hashData);
            return true;
        }
        #endregion

        #region RSA签名

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="hashByteSignature">待签名的byte[]</param>
        /// <param name="encryptedSignatureData">加密签名后的byte[]</param>
        /// <returns></returns>
        public bool SignatureFormatter(string strKeyPrivate, byte[] hashByteSignature, ref byte[] encryptedSignatureData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strKeyPrivate);

            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            //设置签名的算法为MD5
            rsaFormatter.SetHashAlgorithm("MD5");
            //执行签名
            encryptedSignatureData = rsaFormatter.CreateSignature(hashByteSignature);
            return true;
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="hashByteSignature">待签名的byte[]</param>
        /// <param name="encryptedSignatureStr">假面签名后返回的String</param>
        /// <returns></returns>
        public bool SignatureFormatter(string strKeyPrivate, byte[] hashByteSignature, ref string encryptedSignatureStr)
        {
            byte[] encryptedSignatureData;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strKeyPrivate);
            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            //设置签名的算法为MD5
            rsaFormatter.SetHashAlgorithm("MD5");
            //执行签名
            encryptedSignatureData = rsaFormatter.CreateSignature(hashByteSignature);
            encryptedSignatureStr = Convert.ToBase64String(encryptedSignatureData);
            return true;
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="strHashByteSignature">待签名的字符串</param>
        /// <param name="encryptedSignatureData">加密签名后的byte数组</param>
        /// <returns></returns>
        public bool SignatureFormatter(string strKeyPrivate, string strHashByteSignature, ref byte[] encryptedSignatureData)
        {
            byte[] hashByteSignature;
            hashByteSignature = Convert.FromBase64String(strHashByteSignature);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strKeyPrivate);
            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            //设置签名的算法为MD5
            rsaFormatter.SetHashAlgorithm("MD5");
            //执行签名
            encryptedSignatureData = rsaFormatter.CreateSignature(hashByteSignature);
            return true;
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="strHashByteSignature">待签名的字符串</param>
        /// <param name="strEncryptedSignature">加密签名后的字符串</param>
        /// <returns></returns>
        public bool SignatureFormatter(string strKeyPrivate, string strHashByteSignature, ref string strEncryptedSignature)
        {
            byte[] hashByteSignature;
            byte[] encrpyedSignatrueData;

            hashByteSignature = Convert.FromBase64String(strHashByteSignature);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strKeyPrivate);
            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            //设置签名的算法为MD5
            rsaFormatter.SetHashAlgorithm("MD5");
            //执行签名
            encrpyedSignatrueData = rsaFormatter.CreateSignature(hashByteSignature);
            strEncryptedSignature = Convert.ToBase64String(encrpyedSignatrueData);
            return true;
        }
        #endregion

        #region RSA签名验证
        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strKeyPublic"></param>
        /// <param name="hashByteDeformatter"></param>
        /// <param name="deformatterData"></param>
        /// <returns></returns>
        public bool SignatureDeformatter(string strKeyPublic, byte[] hashByteDeformatter, ref byte[] deformatterData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strKeyPublic);
            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            //指定解密的时候HASH算法为MD5
            rsaDeformatter.SetHashAlgorithm("MD5");
            if(rsaDeformatter.VerifySignature(hashByteDeformatter, deformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SignatureDeformatter(string strKeyPublic, string strHashByteDeformatter,ref byte[] deformatterData)
        {
            byte[] hashByteDeformatter;
            hashByteDeformatter = Convert.FromBase64String(strHashByteDeformatter);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strKeyPublic);
            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            //指定解密HASH算法为MD5
            rsaDeformatter.SetHashAlgorithm("MD5");
            if(rsaDeformatter.VerifySignature(hashByteDeformatter, deformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SignatureDeformatter(string strKeyPublic, byte[] hashByteDeformatter,ref string strDeformatter)
        {
            byte[] deformatterData;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strKeyPublic);
            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            //指定解密算法为MD5
            rsaDeformatter.SetHashAlgorithm("MD5");
            deformatterData = Convert.FromBase64String(strDeformatter);
            if(rsaDeformatter.VerifySignature(hashByteDeformatter, deformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)
        {

            byte[] DeformatterData;
            byte[] HashbyteDeformatter;

            HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);
            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPublic);
            System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");
            DeformatterData = Convert.FromBase64String(p_strDeformatterData);
            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
    }
}