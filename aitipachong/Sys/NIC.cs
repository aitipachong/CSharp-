// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Sys
// * 文件名称：		    NIC.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-18
// * 程序功能描述：
// *        网卡相关操作类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System.Collections.Generic;
using System.Management;

namespace aitipachong.Sys
{
    /// <summary>
    /// 网卡相关操作类
    /// </summary>
    public class NIC
    {
        /// <summary>
        /// 获取网卡列表
        /// </summary>
        /// <returns></returns>
        public IList<string> NetWorkList()
        {
            string manage = "SELECT * FROM Win32_NetworkAdapter";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(manage);
            ManagementObjectCollection collection = searcher.Get();
            List<string> netWorkList = new List<string>();
            foreach(ManagementObject obj in collection)
            {
                if(obj["NetConnectionID"] != null) netWorkList.Add(obj["Name"].ToString());
            }

            return netWorkList;
        }

        /// <summary>
        /// 禁用网卡
        /// </summary>
        /// <param name="network">网卡名</param>
        /// <returns></returns>
        public bool DisableNetWork(ManagementObject network)
        {
            try
            {
                network.InvokeMethod("Disable", null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 启用网卡
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public bool EnableNetWork(ManagementObject network)
        {
            try
            {
                network.InvokeMethod("Enable", null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 得到指定网卡
        /// </summary>
        /// <param name="networkname">网卡名字</param>
        /// <returns></returns>
        public ManagementObject NetWorkName(string networkname)
        {
            string netState = "SELECT * FROM Win32_NetworkAdapter";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(netState);
            ManagementObjectCollection collection = searcher.Get();
            foreach(ManagementObject obj in collection)
            {
                if((obj["Name"].ToString() == networkname) && (obj["NetConnectionID"] != null))
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// 网卡状态
        /// </summary>
        /// <param name="networkName">网卡名</param>
        /// <returns></returns>
        public bool NetWorkState(string networkName)
        {
            string netState = "SELECT * From Win32_NetworkAdapter";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(netState);
            ManagementObjectCollection collection = searcher.Get();
            foreach (ManagementObject manage in collection)
            {
                if (manage["Name"].ToString() == networkName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}