// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.CSV
// * 文件名称：		    CsvHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-06-14
// * 程序功能描述：
// *		1.把DataTable对象实例转换为CSV文件；
// *        2.把CSV文件内容该转换为DataTable对象实例；
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using System;
using System.Data;
using System.IO;

namespace aitipachong.CSV
{
    /// <summary>
    /// CSV转换类
    /// </summary>
    public class CsvHelper
    {
        /// <summary>
        /// 导出DataTable对象实例为CSV文件
        ///     注：Csv文件的列头由DataTable的Column获取
        /// </summary>
        /// <param name="dt">DataTable对象实例</param>
        /// <param name="strFilePath">csv文件存储路径</param>
        /// <returns>导出是否成功</returns>
        public static bool Dt2Csv(DataTable dt, string strFilePath)
        {
            try
            {
                string strBufferLine = "";
                StreamWriter strmWriterObj = new StreamWriter(strFilePath, false, System.Text.Encoding.UTF8);
                //输出datatable表头
                for(int i = 0; i < dt.Columns.Count;i++)
                {
                    if (i > 0) strBufferLine += ",";
                    strBufferLine += dt.Columns[i].ColumnName;
                }
                strmWriterObj.WriteLine(strBufferLine);
                //输出datatable内容
                for(int loopi = 0; loopi < dt.Rows.Count;loopi++)
                {
                    strBufferLine = "";
                    for(int loopj = 0;loopj < dt.Columns.Count;loopj++)
                    {
                        if(loopj > 0)
                        {
                            strBufferLine += ",";
                        }
                        strBufferLine += dt.Rows[loopi][loopj].ToString();
                    }
                    strmWriterObj.WriteLine(strBufferLine);
                }
                strmWriterObj.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将Csv读入DataTable
        ///     注:DataTable的列头由Csv文件中获取
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        public static DataTable Csv2Dt(string filePath, int n = 1)
        {
            DataTable dt = new DataTable();
            if (!File.Exists(filePath)) return null;
            try
            {
                StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8, false);
                int i = 0, m = 0;
                string tempStr = "";
                reader.Peek();
                while(reader.Peek() > 0)
                {
                    m += 1;
                    tempStr = reader.ReadLine();
                    //列头
                    if(m == n)
                    {
                        string[] columnSplit = tempStr.Split(',');
                        for(int loopi = 0; loopi < columnSplit.Length; loopi++)
                        {
                            dt.Columns.Add(columnSplit[loopi], Type.GetType("System.String"));
                        }
                    }

                    //内容
                    if(m >= n + 1)
                    {
                        string[] split = tempStr.Split(',');
                        System.Data.DataRow dr = dt.NewRow();
                        for(int loopi = 0; loopi < split.Length; loopi++)
                        {
                            dr[loopi] = split[loopi];
                        }
                        dt.Rows.Add(dr);
                    }
                }

                return dt;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}