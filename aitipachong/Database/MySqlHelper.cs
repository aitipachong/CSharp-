// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Database
// * 文件名称：		    MySqlHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-08-29
// * 程序功能描述：
// *		MySql操作类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace aitipachong.Database
{
    /// <summary>
    /// MySql操作类
    /// </summary>
    public class MySqlHelper
    {
        /// <summary>
        /// MySql链接字符串
        ///     例如：Database='device_manage';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true;Allow Zero Datetime=True
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionStr"></param>
        public MySqlHelper(string connectionStr)
        {
            this.ConnectionString = connectionStr;
        }

        /// <summary>
        /// 对数据库执行增删改操作，返回受影响的行数
        /// </summary>
        /// <remarks>
        /// 例如：int result = ExecuteNonQuery(CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="cmdType">命令类型（存储过程，SQL文本等）</param>
        /// <param name="cmdText">存储过程名或Sql语句</param>
        /// <param name="commandParameters">执行命令所用参数集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, MySqlParameter[] commandParameters)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                using (MySqlConnection conn = new MySqlConnection(this.ConnectionString))
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long InsertNewRecordAndReturnId(CommandType cmdType, string cmdText, MySqlParameter[] commandParameters)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                using (MySqlConnection conn = new MySqlConnection(this.ConnectionString))
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return cmd.LastInsertedId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 使用MySql事务对数据库执行增删改操作，返回受影响的行数
        /// </summary>
        /// <param name="trans">一个现有的MySql事务</param>
        /// <param name="cmdType">命令类型（存储过程，SQL文本等）</param>
        /// <param name="cmdText">存储过程名或Sql语句</param>
        /// <param name="commandParameters">执行命令所用参数集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] commandParameters)
        {
            try
            {
                if (trans.Connection == null || trans.Connection.ConnectionString.Trim().ToLower() == this.ConnectionString.Trim().ToLower())
                {
                    throw new ArgumentException("MySql事务对象中数据库链接对象为Null或数据库链接对象中的链接字符串与MySqlHelper实例中给定链接字符串不一致.");
                }
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行一个查询语句，返回一个关联的MySqlDataReader实例
        /// </summary>
        /// <param name="cmdType">命令类型（存储过程，SQL文本等）</param>
        /// <param name="cmdText">存储过程名或Sql语句</param>
        /// <param name="commandParameters">执行命令所用参数集合</param>
        /// <returns>包含查询结果的MySqlDataReader实例</returns>
        public MySqlDataReader ExecuteReader(CommandType cmdType, string cmdText, MySqlParameter[] commandParameters)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                MySqlConnection conn = new MySqlConnection(this.ConnectionString);
                try
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                    return reader;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行查询语句，返回一个DataSet实例
        /// </summary>
        /// <param name="cmdType">命令类型（存储过程，SQL文本等）</param>
        /// <param name="cmdText">存储过程名或Sql语句</param>
        /// <param name="commandParameters">执行命令所用参数集合</param>
        /// <returns></returns>
        public DataSet GetDataSet(CommandType cmdType, string cmdText, MySqlParameter[] commandParameters)
        {
            DataSet ds = null;
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(this.ConnectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                ds = new DataSet();
                adapter.Fill(ds);
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return ds;
        }

        /// <summary>
        /// 执行一个查询语句，返回查询结果的第一行第一列
        /// </summary>
        /// <param name="cmdType">命令类型（存储过程，SQL文本等）</param>
        /// <param name="cmdText">存储过程名或Sql语句</param>
        /// <param name="commandParameters">执行命令所用参数集合</param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType cmdType, string cmdText, MySqlParameter[] commandParameters)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                using (MySqlConnection conn = new MySqlConnection(this.ConnectionString))
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    object val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 准备命令
        /// </summary>
        /// <param name="cmd">MySql命令对象实例</param>
        /// <param name="conn">链接对象实例</param>
        /// <param name="trans">事务对象实例</param>
        /// <param name="cmdType">命令类型（存储过程，SQL文本等）</param>
        /// <param name="cmdText">存储过程名或Sql语句</param>
        /// <param name="cmdParameters">执行命令所用参数集合</param>
        private void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParameters)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null) cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParameters != null && cmdParameters.Length > 0)
            {
                foreach (MySqlParameter param in cmdParameters)
                {
                    cmd.Parameters.Add(param);
                }
            }
        }

        public void ExecuteSqlTran(string cmdText, List<MySqlParameter[]> paramsList, CommandType cmdType)
        {
            using (MySqlConnection conn = new MySqlConnection(this.ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                MySqlTransaction tx = conn.BeginTransaction();
                try
                {
                    for (int i = 0; i < paramsList.Count; i++)
                    {
                        PrepareCommand(cmd, conn, tx, cmdType, cmdText, paramsList[i]);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        //事务提交（注：500条数据，提交一次）
                        if (i > 0 && (i % 500 == 0 || i == paramsList.Count - 1))
                        {
                            tx.Commit();
                            tx = conn.BeginTransaction();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    tx.Rollback();      //异常，回滚数据
                    throw ex;
                }
            }

        }

        /// <summary>
        /// 批量执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="sqlStringList">sql语句列表</param>
        public void ExecuteSqlTran(List<string> sqlStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(this.ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int i = 0; i < sqlStringList.Count; i++)
                    {
                        string strSql = sqlStringList[i].ToString();
                        if (strSql.Trim().Length > 1)
                        {
                            cmd.CommandText = strSql;
                            cmd.ExecuteNonQuery();
                        }
                        //事务提交（注：500条数据，提交一次）
                        if (i > 0 && (i % 500 == 0 || i == sqlStringList.Count - 1))
                        {
                            tx.Commit();
                            tx = conn.BeginTransaction();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    tx.Rollback();      //异常，回滚数据
                    throw ex;
                }
            }
        }
    }
}
