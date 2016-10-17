// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Array
// * 文件名称：		    ArrayOperationHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-17
// * 程序功能描述：
// *        数组操作类
// *            1.冒泡排序
// *            2.插入排序
// *            3.选择排序
// *            4.快速排序
// *            5.判断数组中是否有重复元素
// *            6.按条件检索数组元素
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
using System.Text;
using System.Threading.Tasks;

namespace aitipachong.Array
{
    /// <summary>
    /// 数组操作帮助类
    /// </summary>
    public class ArrayOperationHelper
    {
        #region 排序
        /// <summary>
        /// 将数组进行冒泡排序
        /// </summary>
        /// <param name="sortArray">需要排序的数组</param>
        public void SortByBubble(ref int[] sortArray)
        {
            //参数容错
            if (sortArray == null || sortArray.Length < 2) throw new Exception("排序数组为空，或数组内元素小于2个.");
            //定义两个int类型变量，分别用来表示数组下标和存储新的数组元素
            int j, temp;
            for(int i = 0; i < sortArray.Length - 1; i++)
            {
                j = i + 1;
                id: //定义一个标识，以便从这里开始执行语句
                if(sortArray[i] > sortArray[j])
                {
                    temp = sortArray[i];
                    sortArray[i] = sortArray[j];
                    sortArray[j] = temp;
                    goto id;
                }
                else if (j < sortArray.Length - 1)
                {
                    j++;
                    goto id;
                }
            }
        }

        /// <summary>
        /// 将数组进行插入排序
        /// </summary>
        /// <param name="sortArray"></param>
        public void SortByInsertSort(ref int[] sortArray)
        {
            for(int i = 0; i < sortArray.Length; ++i)
            {
                int temp = sortArray[i];
                int j = i;
                while((j > 0) && (sortArray[j - 1] > temp))
                {
                    sortArray[j] = sortArray[j - 1];
                    --j;
                }
                sortArray[j] = temp;
            }
        }
        
        /// <summary>
        /// 将数组进行选择排序
        /// </summary>
        /// <param name="sortArray"></param>
        public void SortBySelect(ref int[] sortArray)
        {
            int min;
            for(int i = 0; i < sortArray.Length - 1; i++)
            {
                min = i;
                for(int j = i+1;j < sortArray.Length;j++)
                {
                    if(sortArray[j] < sortArray[min])
                    {
                        min = j;
                    }
                }
                int t = sortArray[min];
                sortArray[min] = sortArray[i];
                sortArray[i] = t;
            }
        }
     
        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="sortArray"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public void SortByFastSort(ref int[] sortArray, int low, int high)
        {
            int pivot;
            int l, r;
            int mid;
            if (high <= low)
            {
                return;
            }
            else if(high == low + 1)
            {
                if (sortArray[low] > sortArray[high])
                    swap(ref sortArray[low], ref sortArray[high]);
                return;
            }
            mid = (low + high) >> 1;
            pivot = sortArray[mid];
            swap(ref sortArray[low], ref sortArray[mid]);
            l = low + 1;
            r = high;
            try
            {
                do
                {
                    while (l <= r && sortArray[l] < pivot)
                        l++;
                    while (sortArray[r] >= pivot)
                        r--;
                    if (l < r)
                        swap(ref sortArray[l], ref sortArray[r]);
                }
                while (l < r);

                sortArray[low] = sortArray[r];
                sortArray[r] = pivot;
                if (low + 1 < r)
                    SortByFastSort(ref sortArray, low, r - 1);
                if (r + 1 < high)
                    SortByFastSort(ref sortArray, r + 1, high);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void swap(ref int l, ref int r)
        {
            int temp;//临时值
            temp = l;//记录前一个值
            l = r;//记录后一个值
            r = temp;//前后交换数据
        }

        #endregion

        /// <summary>
        /// 判断数组中是否有重复元素，有重复元素则将重复的以-1代替
        /// </summary>
        /// <param name="checkArray">待对比数组</param>
        /// <param name="newArray">新数组</param>
        /// <returns>如果发现重复，则返回true</returns>
        public bool GetCopyArray(ArrayList checkArray, ref ArrayList newArray)
        {
            bool getCopy = false;
            ArrayList arrCopy = new ArrayList();
            arrCopy = checkArray;

            for(int i = 0; i < arrCopy.Count; i++)
            {
                for(int j = i + 1; j < arrCopy.Count; j++)
                {
                    if(arrCopy[i] == checkArray[j] && arrCopy[i].ToString() != "-1")
                    {
                        getCopy = true;
                        checkArray[j] = "-1";
                    }
                    if(j == arrCopy.Count - 1)
                    {
                        newArray.Add(checkArray[i]);
                    }
                }
            }
            return getCopy;
        }

        /// <summary>
        /// 根据指定条件，检索数组中的元素
        /// </summary>
        /// <param name="strArray">待检索的字符串数组</param>
        /// <param name="find">检索关键字</param>
        /// <returns>返回：满足条件的元素重新组成的动态数组</returns>
        public ArrayList FindStr(string[] strArray, string find)
        {
            ArrayList getStr = new ArrayList();
            string[] p_str_temp = System.Array.FindAll(strArray, (s) => s.Contains(find));
            if(p_str_temp.Length > 0)
            {
                foreach(string s in p_str_temp)
                {
                    getStr.Add(s);
                }
            }

            return getStr;
        }
    }
}