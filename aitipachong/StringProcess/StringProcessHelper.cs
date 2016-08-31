// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.StringProcess
// * 文件名称：		    StringProcessHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-08-30
// * 程序功能描述：
// *        字符串处理类
// *            1.去掉字符串中的空格
// *            2.MD5加密字符串
// *            3.（62进制内）10进制转换为指定的进制形式字符串
// *            4.byte数组与字符串互换
// *            5.byte数组形式的字符串分割成byte数组
// *            6.KMP算法
// *            7.List与数组互换
// *            8.List去掉自身重复数据
// *            9.字符串、字符转按键码
// *            10.得到字符串中汉字的数量
// *            11.随机打乱字符串集合内的元素
// *            12.
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aitipachong.StringProcess
{
    /// <summary>
    /// 字符串处理类
    /// </summary>
    public class StringProcessHelper
    {
        /// <summary>
        /// 去掉字符串中的空格
        /// </summary>
        /// <param name="str">待去掉空格的字符串</param>
        /// <returns></returns>
        public static string ThickNull(string str)
        {
            char[] chr = str.ToCharArray();
            IEnumerator ienumerator = chr.GetEnumerator();
            StringBuilder sb = new StringBuilder();
            while(ienumerator.MoveNext())
            {
                sb.Append((char)ienumerator.Current != ' ' ? ienumerator.Current.ToString() : string.Empty);
            }
            return sb.ToString();
        }

        /// <summary>
        /// MD5加密字符串
        /// </summary>
        /// <param name="encryptStr"></param>
        /// <returns></returns>
        public static string MD5(string encryptStr)
        {
            byte[] result = System.Text.Encoding.Default.GetBytes(encryptStr);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string encryptResult = BitConverter.ToString(output).Replace("-", "");
            return encryptResult;
        }

        /// <summary>
        /// （62进制内）10进制转换为指定的进制形式字符串
        /// </summary>
        /// <param name="number">待转换的数字</param>
        /// <param name="coverindex">需要转换的进制（必须在62以内）</param>
        /// <returns>转换进制后的字符串</returns>
        public string ConvertIntToAny(long number, int coverindex)
        {
            //进制索引表
            string mapcode = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string cutmap = mapcode.Substring(0, coverindex);
            List<string> result = new List<string>();
            long t = number;
            int length = cutmap.Length;
            while(t > 0)
            {
                var mod = t % length;
                t = Math.Abs(t / length);
                var character = cutmap[Convert.ToInt32(mod)].ToString();
                result.Insert(0, character);
            }
            return string.Join("", result.ToArray());
        }

        #region byte数组与字符串互换
        ///<remarks>
        /// 字符串转byte[]后，如果要以字符串形式输出byte[],请参考下方
        ///     string GetNewStr = "";
        ///     byt = StringToBytes_Ansi("2356");
        ///     foreach (var b in byt)
        ///     {
        ///         GetNewStr = GetNewStr + String.Format("{0:X2} ", b);
        ///     }
        /// </remarks>

        /// <summary>
        /// string转byte[] -- UTF-8编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] StringToBytesByUTF8(string data)
        {
            return System.Text.Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// byte[]转string -- UTF-8编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BytesToStringByUTF8(byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// string转byte[] -- Ucode编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] StringToBytesByUcode(string data)
        {
            return System.Text.Encoding.Unicode.GetBytes(data);
        }

        /// <summary>
        /// byte[]转string -- Ucode编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BytesToStringByUcode(byte[] data)
        {
            return System.Text.Encoding.Unicode.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// string转byte[] -- Ansi编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] StringToBytesByAnsi(string data)
        {
            return System.Text.Encoding.Default.GetBytes(data);
        }

        /// <summary>
        /// byte[]转string -- Ansi编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BytesToStringByAnsi(byte[] data)
        {
            return System.Text.Encoding.Default.GetString(data, 0, data.Length);
        }
        #endregion

        /// <summary>
        /// byte数组形式的字符串分割成byte数组
        /// </summary>
        /// <remarks>
        ///     例子：有一个字符串是“64 66 64 00”要转成byte数组，里面也是64 66 64 00
        ///         string aa = "64 66 64 00";
        ///         byte[] bArray = GetByteArray(aa);
        /// </remarks>
        /// <param name="shex"></param>
        /// <returns></returns>
        public static byte[] GetByteArray(string shex)
        {
            string[] ssArray = shex.Split(' ');
            List<byte> bytList = new List<byte>();
            foreach(var s in ssArray)
            {
                //将十六进制的字符串转换成数值
                bytList.Add(Convert.ToByte(s, 16));
            }
            //返回字节数组
            return bytList.ToArray();
        }

        #region KMP算法
        /// <summary>
        /// 生成KMP算法跳转表
        /// </summary>
        /// <remarks>
        /// KMP：求一个字符串的回溯函数。
        /// 预定序列下标从0开始。
        /// 回溯函数是整数集[0,n-1]到N的映射，n为字符串长度。
        /// 回溯函数的定义：
        /// 设存在非空序列L，i为其合法下标；
        /// L[i]的前置序列集为：{空集，L中所有以i-1为最后一个元素下标的子序列}
        /// L的前置序列集为：{空集,L中所有以0为第一个元素下标的子序列}
        /// 下标i的回溯函数值的定义为：
        /// 如果i=0,回溯函数值为-1
        /// 否则i的回溯函数值为i的前置序列集和L的前置序列集中相等元素的最大长度,但是相等的两个元素不能是L中的同一个子串，例如[0-i,1]~[i-1,0]reversed
        /// 换句话说是，设集合V={x,x属于i的前置序列集,并且x属于L的前置序列集，并且x的长度小于i}，回溯函数值为max(V).length
        /// 当i=0时并不存在这样的一个x，所以约定此时的回溯函数值为-1
        /// 回溯函数的意义：
        /// 如果子串中标号为j的字符同主串失配，那么将子串回溯到next[j]继续与主串匹配，如果next[j]=-1,则主串的匹配点后移一位，同子串的第一个元素开始匹配。
        /// 同一般的模式匹配算法相比，kmp通过回溯函数在失配的情况下跳过了若干轮匹配(向右滑动距离可能大于1)
        /// kmp算法保证跳过去的这些轮匹配一定是失配的，这一点可以证明
        /// </remarks>
        /// <param name="pattern">模式串，上面的注释里将其称为子串</param>
        /// <returns>回溯函数是kmp算法的核心，本函数依照其定义求出回溯函数，KMP函数依照其意义使用回溯函数</returns>
        public static int[] Next(string pattern)
        {
            int[] next = new int[pattern.Length];
            next[0] = -1;
            if(pattern.Length < 2)  //如果只有1个元素不用kmp效率会更好
            {
                return next;
            }

            //第二个元素的回溯函数值必须是0，可以证明：
            // 1的前置序列集为{空集，L[0]},L[0]的长度不小于1，所以淘汰，空集的长度为0，故回溯函数值为0
            next[1] = 0;
            int i = 2;  //正被计算next值的字符的索引
            int j = 0;  //计算next值所需要的中间变量，每一轮迭代初始时j总为next[i-1]
            //很明显当i == pattern.Length时，所有字符的next值都已计算完毕，任务完成
            while(i < pattern.Length)
            {
                //状态点
                if(pattern[i - 1] == pattern[i])        //首先必须记住在本函数实现中，迭代计算next值是从第三个元素开始的
                {
                    //如果L[i-1]等于L[j],那么next[i]=j+1
                    next[i++] = ++j;
                }
                else
                {
                    //如果不相等则检查next[i]的下一个可能只 ---- next[j]
                    j = next[j];
                    if(j == -1)     //如果 j == -1则表示next[i]的值时1
                    {
                        //可以把这一部分提取出来与外层判断合并
                        next[i++] = ++j;
                    }
                }
            }
            return next;
        }

        /// <summary>
        /// KMP函数通普通的模式匹配函数的差别在于使用了next函数来使模式串一次向右滑动多位称为可能
        /// next函数的本质是提取重复的计算
        /// </summary>
        /// <example>
        ///     //主串
        ///     string Hoststring = "123456789";
        ///     //子串
        ///     string Pattern = "78"
        ///     //调用方法求子串在主串中的位置
        ///     int index = ExecuteKMP(Hoststring, Pattern);
        /// </example>
        /// <param name="source">主串</param>
        /// <param name="pattern">用于查找主串中一个位置的模式串</param>
        /// <returns>-1 表示没有匹配，否则返回匹配的标号</returns>
        public static int ExecuteKMP(string source, string pattern)
        {
            int[] next = Next(pattern);
            int i = 0;  //主串指针
            int j = 0;  //模式串指针
            //如果子串没有匹配完毕并且主串没有检索完成
            while(j < pattern.Length && i < source.Length)
            {
                if(source[i] == pattern[j])     //i和j的逻辑意义体检于此，用于指示本轮迭代中要判断是否相等的主串字符和模式串字符
                {
                    i++;
                    j++;
                }
                else
                {
                    j = next[j];        //依照指示迭代回溯
                    if(j == -1)         //回溯有情况，这是第二种
                    {
                        i++;
                        j++;
                    }
                }
            }
            //如果j==pattern.Length则表示循环的退出是由于子串已经匹配完毕而不是主串用尽
            return j < pattern.Length ? -1 : i - j;
        }
        #endregion

        #region List与数组互换
        /// <summary>
        /// 数组转换为List
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<T> ArraysToList<T>(T[] array)
        {
            List<T> result = new List<T>();
            foreach(T a in array)
            {
                result.Add(a);
            }
            return result;
        }

        /// <summary>
        /// List转换为数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] ListToArrays<T>(List<T> list)
        {
            T[] result = new T[list.Count];
            for(int i = 0; i < list.Count; i++)
            {
                result[i] = list[i];
            }
            return result;
        }
        #endregion

        #region List去掉自身重复数据
        /// <summary>
        /// List去掉自身重复数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void RemoveDuplicateElements<T>(List<T> list)
        {
            list = list.Distinct<T>().ToList<T>();
        }
        #endregion

        #region 字符串、字符转按键码
        /// <summary>
        /// 字符转按键码 包涵所有按键字符、非按键字符
        /// </summary>
        /// <param name="cha">字符</param>
        /// <returns>返回按键码</returns>
        public static int GetNumberCode(string cha)
        {
            switch (cha)
            {
                case "Back":
                    return 8;
                case "Tab":
                    return 9;
                case "Return":
                    return 13;
                case "ShiftKey":
                    return 16;
                case "ControlKey":
                    return 17;
                case "Menu":
                    return 18;
                case "Pause":
                    return 19;
                case "Capital":
                    return 20;
                case "Escape":
                    return 27;
                case "space":
                    return 32;
                case "PageUp":
                    return 33;
                case "Next":
                    return 34;
                case "end":
                    return 35;
                case "home":
                    return 36;
                case "left":
                    return 37;
                case "up":
                    return 38;
                case "right":
                    return 39;
                case "down":
                    return 40;
                case "PrintScreen":
                    return 44;
                case "Insert":
                    return 45;
                case "Delete":
                    return 46;
                case "OemQuestion":
                    return 47;
                case "D0":
                    return 48;
                case "D1":
                    return 49;
                case "D2":
                    return 50;
                case "D3":
                    return 51;
                case "D4":
                    return 52;
                case "D5":
                    return 53;
                case "D6":
                    return 54;
                case "D7":
                    return 55;
                case "D8":
                    return 56;
                case "D9":
                    return 57;
                case "A":
                case "a":
                    return 65;
                case "B":
                case "b":
                    return 66;
                case "C":
                case "c":
                    return 67;
                case "D":
                case "d":
                    return 68;
                case "E":
                case "e":
                    return 69;
                case "F":
                case "f":
                    return 70;
                case "G":
                case "g":
                    return 71;
                case "H":
                case "h":
                    return 72;
                case "I":
                case "i":
                    return 73;
                case "J":
                case "j":
                    return 74;
                case "K":
                case "k":
                    return 75;
                case "L":
                case "l":
                    return 76;
                case "M":
                case "m":
                    return 77;
                case "N":
                case "n":
                    return 78;
                case "O":
                case "o":
                    return 79;
                case "P":
                case "p":
                    return 80;
                case "Q":
                case "q":
                    return 81;
                case "R":
                case "r":
                    return 82;
                case "S":
                case "s":
                    return 83;
                case "T":
                case "t":
                    return 84;
                case "U":
                case "u":
                    return 85;
                case "V":
                case "v":
                    return 86;
                case "W":
                case "w":
                    return 87;
                case "X":
                case "x":
                    return 88;
                case "Y":
                case "y":
                    return 89;
                case "Z":
                case "z":
                    return 90;
                case "RWin":
                    return 91;
                case "NumPad0":
                    return 96;
                case "NumPad1":
                    return 97;
                case "NumPad2":
                    return 98;
                case "NumPad3":
                    return 99;
                case "NumPad4":
                    return 100;
                case "NumPad5":
                    return 101;
                case "NumPad6":
                    return 102;
                case "NumPad7":
                    return 103;
                case "NumPad8":
                    return 104;
                case "NumPad9":
                    return 105;
                case "Multiply":
                    return 106;
                case "Add":
                    return 107;
                case "Subtract":
                    return 109;
                case "Decimal":
                    return 110;
                case "Divide":
                    return 111;
                case "F1":
                    return 112;
                case "F2":
                    return 113;
                case "F3":
                    return 114;
                case "F4":
                    return 115;
                case "F5":
                    return 116;
                case "F6":
                    return 117;
                case "F7":
                    return 118;
                case "F8":
                    return 119;
                case "F9":
                    return 120;
                case "F10":
                    return 121;
                case "F11":
                    return 122;
                case "F12":
                    return 123;
                case "Oem5":
                    return 124;
                case "Oemtilde":
                    return 126;
                case "NumLock":
                    return 144;
                case "Scroll":
                    return 145;
                case "Oem1":
                    return 186;
                case "Oemplus":
                    return 187;
                case "Oemcomma":
                    return 188;
                case "OemMinus":
                    return 189;
                case "OemOpenBrackets":
                    return 219;
                case "Oem6":
                    return 221;
            }
            return -1;
        }

        /// <summary>
        /// 字符转按键码 0-9 a-z
        /// </summary>
        /// <param name="cha">字符</param>
        /// <returns>返回按键码</returns>
        public static int GetNumberCode(char cha)
        {
            switch (cha)
            {
                case '0':
                    return 48;
                case '1':
                    return 49;
                case '2':
                    return 50;
                case '3':
                    return 51;
                case '4':
                    return 52;
                case '5':
                    return 53;
                case '6':
                    return 54;
                case '7':
                    return 55;
                case '8':
                    return 56;
                case '9':
                    return 57;
                case 'A':
                case 'a':
                    return 65;
                case 'B':
                case 'b':
                    return 66;
                case 'C':
                case 'c':
                    return 67;
                case 'D':
                case 'd':
                    return 68;
                case 'E':
                case 'e':
                    return 69;
                case 'F':
                case 'f':
                    return 70;
                case 'G':
                case 'g':
                    return 71;
                case 'H':
                case 'h':
                    return 72;
                case 'I':
                case 'i':
                    return 73;
                case 'J':
                case 'j':
                    return 74;
                case 'K':
                case 'k':
                    return 75;
                case 'L':
                case 'l':
                    return 76;
                case 'M':
                case 'm':
                    return 77;
                case 'N':
                case 'n':
                    return 78;
                case 'O':
                case 'o':
                    return 79;
                case 'P':
                case 'p':
                    return 80;
                case 'Q':
                case 'q':
                    return 81;
                case 'R':
                case 'r':
                    return 82;
                case 'S':
                case 's':
                    return 83;
                case 'T':
                case 't':
                    return 84;
                case 'U':
                case 'u':
                    return 85;
                case 'V':
                case 'v':
                    return 86;
                case 'W':
                case 'w':
                    return 87;
                case 'X':
                case 'x':
                    return 88;
                case 'Y':
                case 'y':
                    return 89;
                case 'Z':
                case 'z':
                    return 90;
            }
            return -1;
        }
        #endregion

        #region 得到字符串中汉字的数量

        /// <summary>
        /// 得到字符串中汉字的数量
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>汉字数量</returns>
        public static int GetHanZiCount(string str)
        {
            int scalar = 0;
            Regex regex = new Regex("^[\u4E00-\u9FA5]{0,}$");
            for(int i = 0; i < str.Length; i++)
            {
                scalar = regex.IsMatch(str[i].ToString()) ? ++scalar : scalar;
            }
            return scalar;
        }

        #endregion

        #region 得到指定字符串的指定位置前的一个字符串(只记录a-z,A-Z)
        /// <summary>
        /// 得到指定字符串的指定位置前的一个字符串(只记录a-z,A-Z)
        ///     特殊用途：例如文本框输入事件等
        /// </summary>
        /// <example>
        /// 例如：
        ///     string holestr = "What is your Name?";
        ///     int nowindex = 4;
        ///     string getstr = GetLastWord(holestr, nowindex);
        ///     Console.WriteLine(getstr);      //返回What
        /// 
        ///     string holestr = "What is your Name?";
        ///     int nowindex = 6;
        ///     string getstr = GetLastWord(holestr, nowindex);
        ///     Console.WriteLine(getstr);//返回i
        /// </example>
        /// <param name="str"></param>
        /// <param name="nowIndex"></param>
        /// <returns></returns>
        public static string GetLastWord(string str, int nowIndex)
        {
            nowIndex--;
            string lastword = "";
            while(true)
            {
                if(nowIndex >= 0)
                {
                    char nowc = str[nowIndex];
                    if(((int)nowc >= 65 && (int)nowc <= 90) || ((int)nowc >= 97 && (int)nowc <= 122))
                    {
                        lastword = nowc.ToString() + lastword;
                    }
                    else
                    {
                        break;
                    }
                    nowIndex--;
                }
                else
                {
                    break;
                }
            }
            return lastword;
        }
        #endregion

        #region 随机打乱字符串集合内的元素

        /// <summary>
        /// 随机打乱字符串集合内的元素
        /// </summary>
        /// <example>
        ///     List<string> speakList = new List<string>();
        ///     DisorganizeList(ref speakList);
        /// </example>
        /// <param name="strList"></param>
        public static void DisorganizeList(ref List<string> strList)
        {
            Random rad = new Random();
            List<string> newList = new List<string>();
            newList.Clear();
            bool havesame = false;
            for(int i = 0; i < strList.Count; i++)
            {
                int getIndex = 0;
                while(true)
                {
                    havesame = false;
                    getIndex = rad.Next(0, strList.Count);
                    foreach(string str in newList)
                    {
                        if(str == strList[getIndex])
                        {
                            havesame = true;
                            break;
                        }
                    }
                    if(!havesame)
                    {
                        break;
                    }
                }
                newList.Add(strList[getIndex]);
            }
            strList = newList;
        }
        #endregion
    }
}
/*

*/

