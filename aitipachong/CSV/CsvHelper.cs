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

        public static DataTable Csv2Dt()
    }
}
/*
        /// <summary>
        /// 将Csv读入DataTable
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        public static DataTable csv2dt(string filePath, int n, DataTable dt)
        {
            StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8, false);
            int i = 0, m = 0;
            reader.Peek();
            while (reader.Peek() > 0)
            {
                m = m + 1;
                string str = reader.ReadLine();
                if (m >= n + 1)
                {
                    string[] split = str.Split(',');

                    System.Data.DataRow dr = dt.NewRow();
                    for (i = 0; i < split.Length; i++)
                    {
                        dr[i] = split[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
*/
