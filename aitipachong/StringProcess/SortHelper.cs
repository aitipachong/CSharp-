// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.StringProcess
// * 文件名称：		    SortHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-08-30
// * 程序功能描述：
// *        List泛型集合排序类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Collections.Generic;
using System.IO;

namespace aitipachong.StringProcess
{
    /*
    //调用( 这里以读写文件内容举例 )
    //大数据排序 - 例如处理非常多，几十万几百万个账号，密码或者神马神马的，
    //功能:通用排序(由大到小);可以参考此方法写数组排序或者由小到大等,那些太简单,我就不写了.
    //速度不是自己写的插入、选择、冒泡排序可以比拟的,亲测168W数据排序仅需6秒(I5 4核台式机);
    //先声明一个排序对象
    SortClass sortClass = new SortClass();
    //数据形式一
    //123456----789456----999
    //123456----789456----568
    //123456----789456----1
    //123456----789456----
    //123456----789456----45

    //这里假定D盘的1.txt文本里面的内容是以上数据，分隔符为 ----，按照第3个元素进行排序
    sortClass.publicSort(@"D:\1.txt", "----", 3);
    //数据形式二
    //三肿/666/1
    //史莱姆/81556/3
    //eshit/056/4
    //Roc/486/5
    //JieZhou/777456/8
    //这里假定D盘的2.txt文本里面的内容是以上数据，分隔符为 /，按照第3个元素进行排序
    sortClass.publicSort(@"D:\2.txt", "/", 3);
    //数据形式三
    //三肿-aaa-33-1212
    //史莱姆-bb-11-2321
    //eshit-c-432-4324
    //Roc-add-343-12312
    //JieZhou-e-545-312

    //这里假定D盘的3.txt文本里面的内容是以上数据，分隔符为 -，按照第4个元素进行排序
    sortClass.publicSort(@"D:\3.txt", "-", 4);


    //排序结果:保存在当前程序目录下的result文件夹里;

    //方法可以经过改良，做成适合自己项目的方法，具体请自由发挥
    //被排序的元素，尽量是数字，不要把中文或者英文或者什么乱码的数据拿来排序
    */

    /// <summary>
    /// List泛型集合排序类
    /// </summary>
    public class SortHelper
    {
        /// <summary>
        /// 通用排序方法 -- 直接操作字符串（适合少量内容排序）
        /// </summary>
        /// <param name="importStr">待排序的内容</param>
        /// <param name="importSeparator">分隔符</param>
        /// <param name="publicSortElementsPosition">根据第几个元素进行排序</param>
        /// <returns>排序后的字符串集合</returns>
        public List<string> CommonSortByStr(string importStr, string importSeparator, int publicSortElementsPosition)
        {
            List<string> getStrList = new List<string>();
            List<string> listBackDoor = new List<string>();
            publicSortElementsPosition = publicSortElementsPosition - 1;
            string str = null;
            string[] strs = null;       //文本所有数据根据换行符拆分成的strs
            string[] strs_row = null;   //一行拆分成的strs
            str = importStr;
            try
            {
                strs = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            List<AccountInfo> list = new List<AccountInfo>();
            int intPublicSortElements = -1;
            for(int i = 0; i < strs.Length; i++)
            {
                try
                {
                    //如果此行为空，拆分长度肯定小于2，所以不用判断是否为空
                    strs_row = strs[i].Split(new string[] { importSeparator }, StringSplitOptions.None);
                    if(strs_row.Length >= publicSortElementsPosition + 1)
                    {
                        if(int.TryParse(strs_row[publicSortElementsPosition], out intPublicSortElements))
                        {
                            list.Add(new AccountInfo(intPublicSortElements, strs[i]));
                        }
                        else
                        {
                            list.Add(new AccountInfo(-1, strs[i]));
                        }
                    }
                    else   //没有实际内容的也加进入
                    {
                        list.Add(new AccountInfo(-1, strs[i]));
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            list.Sort();
            for(int i = 0; i < list.Count; i++)
            {
                getStrList.Add(list[i].info);
            }
            return getStrList;
        }

        /// <summary>
        /// 通用排序方法
        /// </summary>
        /// <param name="importFilePath">待排序的文本路径</param>
        /// <param name="importSeparator">分隔符</param>
        /// <param name="publicSortElementsPosition">根据第几个元素进行排序</param>
        public void publicSort(string importFilePath, string importSeparator, int publicSortElementsPosition)
        {
            //功能：根据文本文件的路径、分隔符和比较大小的元素，排序；将排序好的文件导出到当前目录下的result文件夹下
            //参数：
            //@importFilePath:文件路径
            //@importSeparator:分隔符
            //@publicSortElementsPosition：通用排序元素所在位置（即根据该行的第几个元素排序）
            List<string> listBackDoor = new List<string>();//服务端相关;
            publicSortElementsPosition = publicSortElementsPosition - 1;
            string str = null;
            string[] strs = null;//文本所有数据根据换行符拆分成的strs;
            string[] strs_row = null;//一行拆分成的strs;
            StreamReader sr = new StreamReader(importFilePath, System.Text.Encoding.Default);
            str = sr.ReadToEnd();
            try
            {
                strs = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            List<AccountInfo> list = new List<AccountInfo>();
            int intPublicSortElements = -1;
            for (int i = 0; i < strs.Length; i++)
            {
                strs_row = strs[i].Split(new string[] { importSeparator }, StringSplitOptions.None);//如果此行为空,拆分长度肯定小于2,所以不用判断是否为空;
                if (strs_row.Length >= publicSortElementsPosition + 1)
                {
                    if (int.TryParse(strs_row[publicSortElementsPosition], out intPublicSortElements))
                    {
                        list.Add(new AccountInfo(intPublicSortElements, strs[i]));
                    }
                    else
                    {
                        list.Add(new AccountInfo(-1, strs[i]));
                    }
                }
                else//没有实际内容的也加进去;
                {
                    list.Add(new AccountInfo(-1, strs[i]));
                }
            }
            sr.Close();
            list.Sort();
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "result"))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "result");
            }
            //导出结果数组数据;
            StreamWriter sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + @"result\通用排序_处理结果.txt", false, System.Text.Encoding.Default);
            str = null;
            for (int i = 0; i < list.Count; i++)
            {
                sw.WriteLine(list[i].info);
            }
            sw.Close();
        }
    }

    public class AccountInfo : IComparable
    {
        private int sortElements = 0;   //根据此元素比较大小
        public string info = null;      //一行数据

        public AccountInfo()
        { }

        public AccountInfo(int level, string info)
        {
            this.sortElements = level;
            this.info = info;
        }

        public int CompareTo(object obj)
        {
            int k = ((AccountInfo)obj).sortElements;
            if (k > sortElements)
                return 1;
            else if (k < sortElements)
                return -1;
            else
                return 0;
        }
    }
}
/*

*/
