// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Conversion
// * 文件名称：		    ConvertHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-08-30
// * 程序功能描述：
// *        处理数据类型转换、数制转换、编码转换相关类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;

namespace aitipachong.Conversion
{
    /// <summary>
    /// 处理数据类型转换、数制转换、编码转换相关类
    /// </summary>
    public sealed class ConvertHelper
    {
        #region 补足位数
        /// <summary>
        /// 指定字符串的固定长度，如果字符串小于固定长度，则在字符串的前面补足零，可设置的固定长度最大为9位
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="limitedLength">字符串的固定长度</param>
        /// <returns></returns>
        public static string RepairZero(string text, int limitedLength)
        {
            string temp = "";       //补足0的字符串
            for(int i = 0; i < limitedLength - text.Length; i++)
            {
                temp += "0";
            }
            temp += text;
            return temp;
        }
        #endregion

        #region 各种进制转换
        /// <summary>
        /// 实现各种进制间转换。
        /// </summary>
        /// <remarks>
        ///     例如：ConvertBase("15", 10, 16);
        ///             表示将十进制数15转换为16进制数
        /// </remarks>
        /// <param name="value">要转换的值</param>
        /// <param name="from">原值的进制，只能是2,8,10,16四个值</param>
        /// <param name="to">要转换到的目标进制，只能是2,8,10,16四个值</param>
        /// <returns></returns>
        public static string ConvertBase(string value, int from, int to)
        {
            try
            {
                int intValue = Convert.ToInt32(value, from);        //先转换为10进制
                string result = Convert.ToString(intValue, to);     //再转换成目标进制
                if(to == 2)
                {
                    int resultLength = result.Length;       //获取二进制的长度
                    switch(resultLength)
                    {
                        case 7:
                            result = "0" + result;
                            break;
                        case 6:
                            result = "00" + result;
                            break;
                        case 5:
                            result = "000" + result;
                            break;
                        case 4:
                            result = "0000" + result;
                            break;
                        case 3:
                            result = "00000" + result;
                            break;
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 使用指定字符集将string转换成byte[]

        /// <summary>
        /// 使用指定字符集将string转换成byte[]
        /// </summary>
        /// <param name="text">要转换的字符串</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static byte[] StringToBytes(string text, System.Text.Encoding encoding)
        {
            return encoding.GetBytes(text);
        }
        #endregion

        #region 使用指定字符集将byte[]转换成string

        /// <summary>
        /// 使用指定字符集将byte[]转换成string
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes, System.Text.Encoding encoding)
        {
            return encoding.GetString(bytes);
        }
        #endregion

        #region 将byte[]转换成int
        /// <summary>
        /// 将byte[]转换成int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        /// <returns></returns>
        public static int BytesToInt32(byte[] data)
        {
            if (data.Length < 4) return 0;  //如果传入的字节数组长度小于4，则返回0
            int num = 0;        //定义要返回的整数
            
            if(data.Length >= 4)        //如果传入的字节数组长度大于4，需要进行处理
            {
                byte[] tempBuffer = new byte[4];        //创建一个临时缓冲区
                Buffer.BlockCopy(data, 0, tempBuffer, 0, 4);    //将传入的字节数组的前4个字节复制到临时缓冲区
                num = BitConverter.ToInt32(tempBuffer, 0);      //将临时缓冲区的值转换成整数，并赋值num
            }
            return num;
        }
        #endregion
    }
}