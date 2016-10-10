// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Database.DBUtility
// * 文件名称：		    CommandInfo.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-09-26
// * 程序功能描述：
// *        命令信息类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using System;
using System.Data.SqlClient;

namespace aitipachong.Database.DBUtility
{
    /// <summary>
    /// 类型影响枚举
    /// </summary>
    public enum EffentNextType
    {
        /// <summary>
        /// 对其他语句无任何影响
        /// </summary>
        None,
        /// <summary>
        /// 当前语句必须为"select count(1) from ..."格式，如果存在则继续执行，不存在回滚事务
        /// </summary>
        WhenHaveContine,
        /// <summary>
        /// 当前语句必须为"select count(1) from ..."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        WhenNoHaveContine,
        /// <summary>
        /// 当前语句影响到的行数必须大于0，否则回滚事务
        /// </summary>
        ExcuteEffectRows,
        /// <summary>
        /// 引发事件--当前语句必须为“select count(1) from ...”格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        SolicitationEvent
    }

    public class CommandInfo
    {
        public object ShareObject = null;   //共享对象
        public object OriginaData = null;   //原始数据
        event EventHandler _solicitationEvent;  //引发事件声明
        public event EventHandler SolicitationEvent
        {
            add
            {
                _solicitationEvent += value;
            }
            remove
            {
                _solicitationEvent -= value;
            }
        }

        public void OnSolicitationEvent()
        {
            if(_solicitationEvent != null)
            {
                _solicitationEvent(this, new EventArgs());
            }
        }

        public string CommandText;
        public System.Data.Common.DbParameter[] Parameters;
        public EffentNextType EffentNextType = EffentNextType.None;

        public CommandInfo()
        { }
        
        public CommandInfo(string sqlText, SqlParameter[] paras)
        {
            this.CommandText = sqlText;
            this.Parameters = paras;
        }

        public CommandInfo(string sqlText, SqlParameter[] paras, EffentNextType type)
        {
            this.CommandText = sqlText;
            this.Parameters = paras;
            this.EffentNextType = type;
        }

    }
}