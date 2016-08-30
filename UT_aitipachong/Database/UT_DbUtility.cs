using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aitipachong.Database;
using System.Data;

namespace UT_aitipachong.Database
{
    [TestClass]
    public class UT_DbUtility
    {
        [TestMethod]
        public void UT_MySql_V1()
        {
            string connectionString = @"Server=10.100.8.83;Database=pts_db;Uid=root;Pwd=pts123456;Port=3306;";
            string sql = "SELECT * FROM t_dic_assindex";
            DbUtility db = new DbUtility(connectionString, DbProviderType.MySql);
            DataTable dt = db.ExecuteDataTable(sql, null);
            if(dt == null || dt.Rows == null || dt.Rows.Count == 0)
            {
                Assert.Fail("链接MySql数据库获取表数据行不通.");
            }
            else
            {
                Assert.AreEqual(189, dt.Rows.Count);
            }
        }
    }
}
/*
//使用示例 MySql  
string connectionString = @"Server=localhost;Database=crawldb;Uid=root;Pwd=root;Port=3306;";  
string sql = "SELECT * FROM Weibo_Media order by Id desc limit 0,20000";  
DbUtility db = new DbUtility(connectionString, DbProviderType.MySql);  
DataTable data = db.ExecuteDataTable(sql, null);  
DbDataReader reader = db.ExecuteReader(sql, null);  
reader.Close();   
//使用示例 Execl  
string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("~/XLS/车型.xls") + ";Extended Properties=Excel 8.0;";  
string sql = "SELECT * FROM [Sheet1$]";  
DbUtility db = new DbUtility(connectionString, DbProviderType.OleDb);  
DataTable data = db.ExecuteDataTable(sql, null); 
//使用示例 SQLite  
string connectionString = @"Data Source=D:\VS2008\NetworkTime\CrawlApplication\CrawlApplication.db3";  
string sql = "SELECT * FROM Weibo_Media order by Id desc limit 0,20000";  
DbUtility db = new DbUtility(connectionString, DbProviderType.SQLite);  
DataTable data = db.ExecuteDataTable(sql, null);  
DbDataReader reader = db.ExecuteReader(sql, null);  
reader.Close();   
*/
