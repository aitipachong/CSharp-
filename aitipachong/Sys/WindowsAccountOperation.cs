// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Sys
// * 文件名称：		    WindowsAccountOperation.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-11-08
// * 程序功能描述：
// *       Windows账户操作类
// *          1.更改Windows账户密码
// *          2.重置指定账户密码
// *          3.创建Windows账户
// *          4.判断Windows账户是否存在
// *          5.删除Windows账户
// *          6.启用/禁用Windows账户
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using System;
using System.DirectoryServices;

namespace aitipachong.Sys
{
    /// <summary>
    /// Windows账户操作类
    /// </summary>
    public class WindowsAccountOperation
    {
        /// <summary>
        /// 创建Windows账户
        /// </summary>
        /// <param name="username">账户名</param>
        /// <param name="password">密码</param>
        /// <param name="description">描述</param>
        public static void CreateWinUser(string username, string password, string description)
        {
            try
            {
                DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                var newUser = localMachine.Children.Add(username, "user");
                newUser.Invoke("SetPassword", new object[] { password });
                newUser.Invoke("Put", new object[] { "Description", description });
                newUser.CommitChanges();
                localMachine.Close();
                newUser.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更改Windows账户密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        public static void ChangeWinUserPassword(string username, string oldPwd, string newPwd)
        {
            try
            {
                DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                DirectoryEntry user = localMachine.Children.Find(username, "user");
                object[] password = new object[] { oldPwd, newPwd };
                object ret = user.Invoke("ChangePassword", password);
                user.CommitChanges();
                localMachine.Close();
                user.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 判断Windows用户是否存在
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public static bool ExistWinUser(string username)
        {
            try
            {
                using (DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer"))
                {
                    var user = localMachine.Children.Find(username, "user");
                    return user != null;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除Windows用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool DeleteWinUser(string username)
        {
            try
            {
                using (DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer"))
                {
                    var delUser = localMachine.Children.Find(username, "user");
                    if (delUser != null)
                        localMachine.Children.Remove(delUser);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 启用/禁用Windows账户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="isDisable"></param>
        public static void Disable(string username, bool isDisable)
        {
            try
            {
                var userDn = "WinNT://" + Environment.MachineName + "/" + username + ",user";
                DirectoryEntry user = new DirectoryEntry(userDn);
                user.InvokeSet("AccountDisabled", isDisable);
                user.CommitChanges();
                user.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}