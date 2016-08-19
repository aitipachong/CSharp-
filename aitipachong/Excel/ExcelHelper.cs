// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Excel
// * 文件名称：		    ExcelHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-07-13
// * 程序功能描述：
// *        Excel基本操作类，功能如下：
// *            1.获取Excel链接字符串
// *            2.获取Excel工作表名
// *            3.Excel导入DataSet
// *            4.DataSet导入Excel
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace aitipachong.Excel
{
    /// <summary>
    /// Excel基本操作类
    /// </summary>
    public class ExcelHelper
    {
        #region public field
        /// <summary>
        /// Excel版本
        /// </summary>
        public enum ExcelType
        {
            Excel2003,
            Excel2007,
            Excel2013
        }

        /// <summary>
        /// IMEX三种模式：
        ///     IMEX是用来告诉驱动程序使用Excel文件的模式，其值有0、1、2三种，分别代表导出、导入、混合模式.
        /// </summary>
        public enum IMEXType
        {
            ExportMode = 0,
            ImportMode = 1,
            LinkedMode = 2
        }
        #endregion

        #region 获取Excel链接字符串
        /// <summary>
        /// 返回Excel链接字符串[IMEX=1]
        /// </summary>
        /// <param name="excelPath">Excel文件路径</param>
        /// <param name="header">是否第一行为列名</param>
        /// <param name="eType">Excel版本</param>
        /// <returns>返回值</returns>
        public static string GetExcelConnectString(string excelPath, bool header, ExcelType eType)
        {
            return GetExcelConnectString(excelPath, header, eType, IMEXType.ImportMode);
        }

        /// <summary>
        /// 返回Excel链接字符串
        /// </summary>
        /// <param name="excelPath">Excel文件路径</param>
        /// <param name="header">是否第一行为列名</param>
        /// <param name="eType">Excel版本</param>
        /// <param name="imex">IMEX模式</param>
        /// <returns>返回值</returns>
        public static string GetExcelConnectString(string excelPath, bool header, ExcelType eType, IMEXType imex)
        {
            if (string.IsNullOrEmpty(excelPath)) throw new ArgumentNullException("Excel路径字符串为空!");
            if(!System.IO.File.Exists(excelPath)) throw new FileNotFoundException("Excel文件不存在!");

            string connectString = "";
            string hdr = "NO";
            if (header) hdr = "YES";

            if (eType == ExcelType.Excel2003)
                connectString = "Provider=Microsoft.Jet.OleDb.4.0; data source=" + excelPath +
                    ";Extended Properties='Excel 8.0; HDR=" + hdr + "; IMEX=" + imex.GetHashCode() + "'";
            else
                connectString = "Provider=Microsoft.ACE.OLEDB.12.0; data source=" + excelPath +
                    ";Extended Properties='Excel 12.0 Xml; HDR=" + hdr + "; IMEX=" + imex.GetHashCode() + "'";

            return connectString;
        }
        #endregion

        #region 获取Excel工作表名
        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="excelPath">Excel文件路径</param>
        /// <param name="eType">Excel版本</param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(string excelPath, ExcelType eType)
        {
            string connectString = GetExcelConnectString(excelPath, true, eType);
            return GetExcelTablesName(connectString);
        }

        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="connectString">Excel链接字符串</param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(string connectString)
        {
            using (var conn = new OleDbConnection(connectString))
            {
                return GetExcelTablesName(conn);
            }
        }

        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="connection">Excel链接</param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(OleDbConnection connection)
        {
            var list = new List<string>();
            if (connection.State == System.Data.ConnectionState.Closed) connection.Open();

            DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if(dt != null && dt.Rows.Count > 0)
            {
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(dt.Rows[i][2].ToString());
                }
            }

            return list;
        }

        /// <summary>
        /// 返回Excel第一个工作表表名
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="eType"></param>
        /// <returns></returns>
        public static string GetExcelFirstTableName(string excelPath, ExcelType eType)
        {
            string connectString = GetExcelConnectString(excelPath, true, eType);
            return GetExcelFirstTableName(connectString);
        }

        /// <summary>
        /// 返回Excel第一个工作表表名
        /// </summary>
        /// <param name="connectString"></param>
        /// <returns></returns>
        public static string GetExcelFirstTableName(string connectString)
        {
            using (var conn = new OleDbConnection(connectString))
            {
                return GetExcelFirstTableName(conn);
            }
        }

        public static string GetExcelFirstTableName(OleDbConnection connection)
        {
            string tableName = string.Empty;
            if (connection.State == ConnectionState.Closed) connection.Open();

            DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if(dt != null && dt.Rows.Count > 0)
            {
                tableName = dt.Rows[0][2].ToString();
            }

            return tableName;
        }

        /// <summary>
        /// 获取Excel文件中指定工作表的列
        /// </summary>
        /// <param name="excelPath">Excel文件路径</param>
        /// <param name="eType">Excel类型</param>
        /// <param name="table">工作表名 例如：Sheet1$</param>
        /// <returns></returns>
        public static List<string> GetColumnsList(string excelPath, ExcelType eType, string table)
        {
            DataTable tableColumns;
            string connectString = GetExcelConnectString(excelPath, true, eType);
            using (var conn = new OleDbConnection(connectString))
            {
                if(conn.State == ConnectionState.Closed) conn.Open();
                tableColumns = GetReaderSchema(table, conn);
            }

            return (from DataRow dr in tableColumns.Rows let columnName = dr["ColumnName"].ToString() let datatype = ((OleDbType)dr["ProviderType"]).ToString() let netType = dr["DataType"].ToString() select columnName).ToList();
        }

        private static DataTable GetReaderSchema(string tableName, OleDbConnection connection)
        {
            DataTable schemaTable;
            IDbCommand cmd = new OleDbCommand();
            cmd.CommandText = string.Format("select * from [{0}]", tableName);
            cmd.Connection = connection;

            using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly))
            {
                schemaTable = reader.GetSchemaTable();
            }

            return schemaTable;
        }
        #endregion

        #region Excel导入DataSet
        /// <summary>
        /// Excel导入DataSet
        /// </summary>
        /// <param name="excelPath">Excel文件路径</param>
        /// <param name="table">工作表表名，例如：Sheet1$</param>
        /// <param name="header">是否第一行是列名</param>
        /// <param name="eType">Excel版本</param>
        /// <returns></returns>
        public static DataSet ExcelToDataSet(string excelPath, string table, bool header, ExcelType eType)
        {
            string connectString = GetExcelConnectString(excelPath, header, eType);
            return ExcelToDataSet(connectString, table);
        }

        /// <summary>
        /// Excel导入DataSet
        /// </summary>
        /// <param name="connectString"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataSet ExcelToDataSet(string connectString, string table)
        {
            using (var conn = new OleDbConnection(connectString))
            {
                var ds = new DataSet();

                if(IsExistExcelTableName(conn, table))
                {
                    var adapter = new OleDbDataAdapter("SELECT * FROM [" + table + "]", conn);
                    adapter.Fill(ds, table);
                }

                return ds;
            }
        }

        /// <summary>
        /// Excel导入DataSet
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="header"></param>
        /// <param name="eType"></param>
        /// <returns></returns>
        public static DataSet ExcelToDataSet(string excelPath, bool header, ExcelType eType)
        {
            string connectString = GetExcelConnectString(excelPath, header, eType);
            return ExcelToDataSet(connectString);
        }

        /// <summary>
        /// Excel导入DataSet
        /// </summary>
        /// <param name="connectString"></param>
        /// <returns></returns>
        public static DataSet ExcelToDataSet(string connectString)
        {
            using (var conn = new OleDbConnection(connectString))
            {
                var ds = new DataSet();
                List<string> tableNames = GetExcelTablesName(conn);

                foreach(string tableName in tableNames)
                {
                    var adapter = new OleDbDataAdapter("SELECT * FROM [" + tableName + "]", conn);
                    adapter.Fill(ds, tableName);
                }

                return ds;
            }
        }

        private static bool IsExistExcelTableName(OleDbConnection connection, string table)
        {
            List<string> list = GetExcelTablesName(connection);
            return list.Any(tName => tName.ToLower() == table.ToLower());
        }
        #endregion

        #region DataSet导入Excel
        /// <summary>
        /// 把一个数据集中的数据导出到Excel文件中（XML格式操作）
        /// </summary>
        /// <param name="source">DataSet实体</param>
        /// <param name="fileName">保存Excel文件名</param>
        public static void DataSetToExcel(DataSet source, string fileName)
        {
            var excelDoc = new StreamWriter(fileName);
            #region Excel格式内容
            const string startExcelXML = "<xml version>\r\n<Workbook " +
                                         "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                                         " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                                         "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                                         "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                                         "office:spreadsheet\">\r\n <Styles>\r\n " +
                                         "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                                         "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                                         "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                                         "\r\n <Protection/>\r\n </Style>\r\n " +
                                         "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                                         "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                                         "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                                         " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                                         "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                                         "ss:Format=\"#,##0.###\"/>\r\n </Style>\r\n " +
                                         "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                                         "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                                         "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                                         "ss:Format=\"yyyy-mm-dd;@\"/>\r\n </Style>\r\n " +
                                         "</Styles>\r\n ";
            const string endExcelXML = "</Workbook>";
            #endregion

            int sheetCount = 1;
            excelDoc.Write(startExcelXML);
            for(int i = 0; i < source.Tables.Count; i++)
            {
                int rowCount = 0;
                DataTable dt = source.Tables[i];

                excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                excelDoc.Write("<Table>");
                excelDoc.Write("<Row>");
                //列头
                for(int x = 0; x < dt.Columns.Count; x++)
                {
                    excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                    excelDoc.Write(source.Tables[i].Columns[x].ColumnName);
                    excelDoc.Write("</Data></Cell>");
                }
                excelDoc.Write("</Row>");
                //数据
                foreach(DataRow x in dt.Rows)
                {
                    rowCount++;
                    //if the number of rows is > 64000 create a new page to continue output
                    if(rowCount == 64000)
                    {
                        rowCount = 0;
                        sheetCount++;
                        excelDoc.Write("</Table>");
                        excelDoc.Write(" </Worksheet>");
                        excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                        excelDoc.Write("<Table>");
                    }
                    excelDoc.Write("<Row>");
                    for(int y = 0; y < source.Tables[i].Columns.Count; y++)
                    {
                        Type rowType = x[y].GetType();
                        #region 根据不同数据类型生成内容
                        switch(rowType.ToString())
                        {
                            case "System.String":
                                string xmlString = x[y].ToString();
                                xmlString = xmlString.Trim();
                                xmlString = xmlString.Replace("&", "&");
                                xmlString = xmlString.Replace(">", ">");
                                xmlString = xmlString.Replace("<", "<");
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                    "<Data ss:Type=\"String\">");
                                excelDoc.Write(xmlString);
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.DataTime":
                                //Excel has a specific Date Format of yyyy-MM-dd followed by 
                                //the letter 'T' then HH:mm:ss.lll Example 2005-01-31T24:01:21.000
                                //The Following Code puts the date stored in XMLDate to the format above
                                var xmlDate = (DateTime)x[y];
                                string xmlDateToString = xmlDate.Year + "-" + (xmlDate.Month < 10 ? "0" + xmlDate.Month.ToString() : xmlDate.Month.ToString()) +
                                    "-" + (xmlDate.Day < 10 ? "0" + xmlDate.Day.ToString() : xmlDate.Day.ToString()) +
                                    "T" + (xmlDate.Hour < 10 ? "0" + xmlDate.Hour.ToString() : xmlDate.Hour.ToString()) +
                                    ":" + (xmlDate.Minute < 10 ? "0" + xmlDate.Minute.ToString() : xmlDate.Minute.ToString()) +
                                    ":" + (xmlDate.Second < 10 ? "0" + xmlDate.Second.ToString() : xmlDate.Second.ToString()) +
                                    ".000";
                                excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" + "<Data ss:Type=\"DateTime\">");
                                excelDoc.Write(xmlDateToString);
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Boolean":
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" + "<Data ss:Type=\"String\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                                               "<Data ss:Type=\"Number\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Decimal":
                            case "System.Double":
                                excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                                               "<Data ss:Type=\"Number\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.DBNull":
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                               "<Data ss:Type=\"String\">");
                                excelDoc.Write("");
                                excelDoc.Write("</Data></Cell>");
                                break;
                            default:
                                throw (new Exception(rowType + " not handled."));
                        }
                        #endregion
                    }
                    excelDoc.Write("</Row>");
                }
                excelDoc.Write("</Table>");
                excelDoc.Write(" </Worksheet>");

                sheetCount++;
            }
            excelDoc.Write(endExcelXML);
            excelDoc.Close();
        }
        
        /// <summary>
        /// 将DataTable导出到Excel（OleDb方式操作）
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="fileName"></param>
        public static void DataSetToExcel(DataTable dataTable, string fileName)
        {
            if(System.IO.File.Exists(fileName))
            {
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch
                {
                    throw new Exception("该文件正在使用中，关闭文件或重新命名导出文件再试!");
                }
            }
            var oleDbConn = new OleDbConnection();
            var oleDbCmd = new OleDbCommand();
            try
            {
                oleDbConn.ConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + fileName +
                    @";Extended ProPerties=""Excel 8.0;HDR=Yes;""";
                oleDbConn.Open();
                oleDbCmd.CommandType = CommandType.Text;
                oleDbCmd.Connection = oleDbConn;
                string sql = "CREATE TABLE sheet1 (";
                for(int i = 0; i < dataTable.Columns.Count; i++)
                {
                    //字段名臣出现关键字会导致错误
                    if (i < dataTable.Columns.Count - 1)
                    {
                        sql += "[" + dataTable.Columns[i].Caption + "] TEXT(100) ,";
                    }
                    else
                    {
                        sql += "[" + dataTable.Columns[i].Caption + "] TEXT(200) )";
                    }
                }
                oleDbCmd.CommandText = sql;
                oleDbCmd.ExecuteNonQuery();
                for(int j = 0; j < dataTable.Rows.Count; j++)
                {
                    sql = "INSERT INTO sheet1 VALUES('";
                    for(int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        if (i < dataTable.Columns.Count - 1)
                            sql += dataTable.Rows[j][i] + " ','";
                        else
                            sql += dataTable.Rows[j][i] + " ')";
                    }
                    oleDbCmd.CommandText = sql;
                    oleDbCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oleDbCmd.Dispose();
                oleDbConn.Close();
                oleDbConn.Dispose();
            }
        }
        #endregion
    }
}