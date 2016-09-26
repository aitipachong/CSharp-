// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Sys
// * 文件名称：		    SysHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-09-26
// * 程序功能描述：
// *        系统操作相关的公共类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace aitipachong.Sys
{
    /// <summary>
    /// 系统操作相关的公共类
    /// </summary>
    public static class SysHelper
    {
        #region 获取文件相关路径映射的物理路径
        /// <summary>
        /// 获取文件相关路径映射的物理路径(针对Web)
        /// </summary>
        /// <param name="virtualPath">相对路径</param>
        /// <returns></returns>
        public static string GetPath(string virtualPath)
        {
            return HttpContext.Current.Server.MapPath(virtualPath);
        }
        #endregion

        #region 获取指定调用层级的方法名
        /// <summary>
        /// 获取指定调用层级的方法名
        /// </summary>
        /// <param name="level">调用层数</param>
        /// <returns></returns>
        public static string GetMethodName(int level)
        {
            //创建一个堆栈跟踪
            StackTrace trace = new StackTrace();
            //获取指定调用层级的方法名
            return trace.GetFrame(level).GetMethod().Name;
        }
        #endregion

        #region 获取GUID值
        /// <summary>
        /// 获取新GUID值
        /// </summary>
        public static string NewGUID
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        #endregion

        #region 获取换行字符
        /// <summary>
        /// 获取换行字符
        /// </summary>
        public static string NewLine
        {
            get
            {
                return Environment.NewLine;
            }
        }
        #endregion

        #region 获取当前应用程序域
        /// <summary>
        /// 获取当前应用程序域
        /// </summary>
        public static AppDomain CurrentAppDomain
        {
            get
            {
                return Thread.GetDomain();
            }
        }
        #endregion
    }
}