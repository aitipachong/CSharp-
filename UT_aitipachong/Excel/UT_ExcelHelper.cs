using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Data;

namespace UT_aitipachong.Excel
{
    [TestClass]
    public class UT_ExcelHelper
    {
        [TestMethod]
        public void UT_GetExcelTablesName_V1()
        {
            string excelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempCSV.xlsx");
            if(!File.Exists(excelPath))
            {
                Assert.Fail("\"tempCSV.xlsx\"文件不存在,路径：" + excelPath);
                return;
            }
            List<string> tableNames = aitipachong.Excel.ExcelHelper.GetExcelTablesName(excelPath, aitipachong.Excel.ExcelHelper.ExcelType.Excel2013);
            if(tableNames == null || tableNames.Count <= 0)
            {
                Assert.Fail("读取tempCSV.xlsx文件工作表失败!");
                return;
            }
            Assert.AreEqual("tempCSV$", tableNames[0].Trim());
        }

        [TestMethod]
        public void UT_ExcelToDataSet_V1()
        {
            string excelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempCSV.xlsx");
            if (!File.Exists(excelPath))
            {
                Assert.Fail("\"tempCSV.xlsx\"文件不存在,路径：" + excelPath);
                return;
            }
            DataSet ds = aitipachong.Excel.ExcelHelper.ExcelToDataSet(excelPath, true, aitipachong.Excel.ExcelHelper.ExcelType.Excel2013);
            if(ds == null || ds.Tables == null || ds.Tables.Count == 0)
            {
                Assert.Fail("Excel导入DataSet失败！");
                return;
            }
            Assert.AreEqual(33, ds.Tables[0].Rows.Count);
        }

        [TestMethod]
        public void UT_DataSetToExcel_V1()
        {
            string excelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempCSV.xlsx");
            string source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempabc.xls");
            if (!File.Exists(excelPath))
            {
                Assert.Fail("\"tempCSV.xlsx\"文件不存在,路径：" + excelPath);
                return;
            }
            DataSet ds = aitipachong.Excel.ExcelHelper.ExcelToDataSet(excelPath, true, aitipachong.Excel.ExcelHelper.ExcelType.Excel2013);
            if (ds == null || ds.Tables == null || ds.Tables.Count == 0)
            {
                Assert.Fail("Excel导入DataSet失败！");
                return;
            }
            aitipachong.Excel.ExcelHelper.DataSetToExcel(ds, source);
            Assert.AreEqual(true, File.Exists(source));
        }
    }
}
