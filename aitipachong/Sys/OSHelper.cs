// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Sys
// * 文件名称：		    OSHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-17
// * 程序功能描述：
// *        操作系统帮助类
// *            1.获取操作系统位数
// *            2.获取操作系统的登录用户名
// *            3.获取物理内存大小
// *            4.获取系统空间
// *            5.执行DOS命令
// *            6.判断本机是否联网
// *            7.获得本机内网IP
// *            8.获得本机外网IP
// *            9.机器码相关（CPU序列号、硬盘ID、网卡物理地址（MAC码）、网卡的IP地址）
// *            10.注销、关机、重启
// *            11.监测端口状态
// *            12.获得PC类型
// *            13.获得内存使用情况
// *            14.获得当前屏幕分辨率
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace aitipachong.Sys
{
    /// <summary>
    /// 系统空间枚举
    /// </summary>
    public enum SystemSpace
    {
        /// <summary>
        /// 总空间
        /// </summary>
        TotalSpace = 0,
        /// <summary>
        /// 已用空间
        /// </summary>
        UsedSpace = 1,
        /// <summary>
        /// 剩余空间
        /// </summary>
        ResidualSpace = 2
    }

    /// <summary>
    /// 内存使用类型
    /// </summary>
    public enum MemoryUsageType
    {
        /// <summary>
        /// 物理内存总量
        /// </summary>
        TotalPhysicalMemory,
        /// <summary>
        /// 可用物理内存
        /// </summary>
        AvailablePhysicalMemory,
        /// <summary>
        /// 虚拟内存总量
        /// </summary>
        TotalVirtualMemory,
        /// <summary>
        /// 可用虚拟内存
        /// </summary>
        AvailableVirtualMemory
    }

    /// <summary>
    /// 操作系统帮助类
    /// </summary>
    public class OSHelper
    {
        #region 获取操作系统位数
        /// <summary>
        /// 获取操作系统位数
        /// </summary>
        /// <returns></returns>
        public string GetOsBit()
        {
            try
            {
                ConnectionOptions oConn = new ConnectionOptions();
                ManagementScope managementScope = new ManagementScope("\\\\localhost", oConn);
                ObjectQuery objectQuery = new ObjectQuery("select AddressWidth from Win32_Processor");
                ManagementObjectSearcher moSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
                ManagementObjectCollection moReturnCollection = null;
                string addressWidth = null;
                moReturnCollection = moSearcher.Get();
                foreach(ManagementObject oReturn in moReturnCollection)
                {
                    addressWidth = oReturn["AddressWidth"].ToString();
                }
                return addressWidth;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取操作系统的登录用户名
        /// <summary>
        /// 获取操作系统的登录用户名
        /// </summary>
        /// <returns></returns>
        public string GetSystemLoginUserName()
        {
            try
            {
                string str = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach(ManagementObject mo in moc)
                {
                    str = mo["UserName"].ToString();
                }
                moc = null;
                mc = null;
                return str;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取物理内存大小

        const int GB = 1024 * 1024 * 1024;  //定义GB的计算常量
        const int MB = 1024 * 1024;         //定义MB的计算常量
        const int KB = 1024;                //定义KB的计算常量

        /// <summary>
        /// 将字节转换成GB，MB或KB
        /// </summary>
        /// <param name="kSize">字节大小</param>
        /// <returns></returns>
        public string ByteConvertToGBMBKB(Int64 kSize)
        {
            if (kSize / GB >= 1)
                return (Math.Round(kSize / (float)GB, 2)).ToString() + "GB";
            else if (kSize / MB >= 1)
                return (Math.Round(kSize / (float)MB, 2)).ToString() + "MB";
            else if (kSize / KB >= 1)
                return (Math.Round(kSize / (float)KB, 2)).ToString() + "KB";
            else
                return kSize.ToString() + "Byte";
        }

        /// <summary>
        /// 获得物理内存
        /// </summary>
        /// <returns></returns>
        public string GetTotalPhysicalMemory()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach(ManagementObject mo in moc)
                {
                    st = mo["TotalPhysicalMemory"].ToString();
                }
                moc = null;
                mc = null;

                return ByteConvertToGBMBKB(Convert.ToInt64(st));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 系统空间
        /// <summary>
        /// 获取系统空间
        /// </summary>
        /// <param name="spacetype">系统空间类型枚举</param>
        /// <returns></returns>
        public List<string> GetSystemSpace(SystemSpace spacetype)
        {
            List<string> driveSpaceList = new List<string>();
            System.IO.DriveInfo[] drive = System.IO.DriveInfo.GetDrives();  //获取所有驱动器
            for (int i = 0; i < drive.Length; i++)
            {
                if (drive[i].DriveType == System.IO.DriveType.Fixed)
                {
                    switch (spacetype)
                    {
                        case SystemSpace.TotalSpace:
                            driveSpaceList.Add(drive[i].Name + ": 总空间：" + ByteConvertToGBMBKB(Convert.ToInt64(drive[i].TotalSize)));
                            break;
                        case SystemSpace.ResidualSpace:
                            driveSpaceList.Add(drive[i].Name + ": 剩余空间：" + ByteConvertToGBMBKB(Convert.ToInt64(drive[i].TotalFreeSpace)));
                            break;
                        case SystemSpace.UsedSpace:
                            driveSpaceList.Add(drive[i].Name + ": 已用空间：" + ByteConvertToGBMBKB(Convert.ToInt64(drive[i].TotalSize - drive[i].TotalFreeSpace)));
                            break;
                        default:
                            break;
                    }
                }
            }
            return driveSpaceList;
        }

        #endregion

        #region 执行DOS命令
        /// <summary>
        /// 执行DOS命令，返回DOS命令的输出
        /// </summary>
        /// <param name="command">dos命令</param>
        /// <param name="seconds">等待执行命令的时间（单位：毫秒）</param>
        /// <returns>返回DOS命令的输出</returns>
        public string ExecuteCmd(string command, int seconds)
        {
            string output = "";     //输出字符串
            if(command != null && !command.Equals(""))
            {
                Process process = new Process();        //创建进程对象
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";         //设定需要执行的命令
                startInfo.Arguments = "/C" + command;   //“/C”表示执行完命令后马上退出
                startInfo.UseShellExecute = false;      //不使用系统外壳程序启动
                startInfo.RedirectStandardInput = false;//不重定向输入
                startInfo.RedirectStandardOutput = true;//重定向输出
                startInfo.CreateNoWindow = true;        //不创建窗口
                process.StartInfo = startInfo;
                try
                {
                    if(process.Start()) //开始进程
                    {
                        if(seconds == 0)
                        {
                            process.WaitForExit();      //这里无限等待进程结束
                        }
                        else
                        {
                            process.WaitForExit(seconds);//等待进程结束，等待时间为指定的毫秒
                        }
                        output = process.StandardOutput.ReadToEnd();    //读取进程的输出
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (process != null) process.Close();
                }
            }
            return output;
        }
        #endregion

        #region 判断本机是否联网
        /// <summary>
        /// 判断网络状态的方法，返回值true为连接，false为未连接
        /// </summary>
        /// <param name="conState"></param>
        /// <param name="reder"></param>
        /// <returns></returns>
        [DllImport("wininet.dll", EntryPoint = "InternetGetConnectedState")]
        public extern static bool InternetGetConnectedState(out int conState, int reder);

        /// <summary>
        /// 判断本机是否联网
        /// </summary>
        /// <returns></returns>
        public bool GetSystemNetworkingState()
        {
            int n = 0;
            return InternetGetConnectedState(out n, 0);
        }
        #endregion

        #region 获得本机内网IP

        /// <summary>
        /// 获得本机内网IP
        /// </summary>
        /// <returns></returns>
        public string GetLocalInnerIP()
        {
            string s = "";
            System.Net.IPAddress[] addressList = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            for(int i = 0; i < addressList.Length; i++)
            {
                s += addressList[i].ToString() + "\n";
            }

            return s;
        }
        #endregion

        #region 获得本机外网IP
        /// <summary>
        /// 获得本机外网IP
        /// </summary>
        /// <returns></returns>
        public string GetLocalOuterIP()
        {
            string ip = "";
            try
            {
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ips138.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, System.Text.Encoding.Default);
                string all = sr.ReadToEnd();

                int start = all.IndexOf("您的IP地址是：[") + 9;
                int end = all.IndexOf("]", start);
                ip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return ip;
        }

        #endregion

        #region 机器码相关（CPU序列号、硬盘ID、网卡物理地址（MAC码）、网卡的IP地址）
        /// <summary>
        /// 获取CPU序列号
        /// </summary>
        /// <returns></returns>
        public string GetCpuNumber()
        {
            try
            {
                string cpuInfo = "";
                ManagementClass cimobject = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = cimobject.GetInstances();
                foreach(ManagementObject mo in moc)
                {
                    cpuInfo += mo.Properties["ProcessorId"].Value.ToString();
                }

                return cpuInfo.ToString();
            }
            catch
            {
                return "unknown";
            }
        }

        /// <summary>
        /// 获取硬盘ID
        /// </summary>
        /// <returns></returns>
        public string GetHardDiskID()
        {
            try
            {
                ManagementClass mcHD = new ManagementClass("win32_logicaldisk");
                ManagementObjectCollection mocHD = mcHD.GetInstances();
                foreach (ManagementObject m in mocHD)
                {
                    if (m["DeviceID"].ToString() == "C:")
                    {
                        return m["VolumeSerialNumber"].ToString();
                    }
                }
                return "";
            }
            catch
            {
                return "unknown";
            }
        }

        /// <summary>
        /// 获取网卡物理地址
        /// </summary>
        /// <returns></returns>
        public string GetMac()
        {
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac += mo["MacAddress"].ToString();
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknown";
            }
        }

        /// <summary>
        /// 获取网卡的IP地址
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {
            try
            {
                //获取IP地址
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st += ar.GetValue(0).ToString();
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 注销、关机、重启
        [DllImport("user32.dll", EntryPoint = "ExitWindowsEx", CharSet = CharSet.Ansi)]
        private static extern int ExitWindowsEx(int uFlags, int dwReserved);

        /// <summary>
        /// 注销计算机
        /// </summary>
        public void Cancellation()
        {
            ExitWindowsEx(0, 0);
        }

        /// <summary>
        /// 关机计算机
        /// </summary>
        public void Shutdown()
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = "cmd.exe";                   //启动cmd命令
            myProcess.StartInfo.UseShellExecute = false;                //是否使用系统外壳程序启动进程
            myProcess.StartInfo.RedirectStandardInput = true;           //是否从流中读取
            myProcess.StartInfo.RedirectStandardOutput = true;          //是否写入流
            myProcess.StartInfo.RedirectStandardError = true;           //是否将错误信息写入流
            myProcess.StartInfo.CreateNoWindow = true;                  //是否在新窗口中启动进程
            myProcess.Start();                                          //启动进程
            myProcess.StandardInput.WriteLine("shutdown -s -t 0");      //执行关机命令
        }

        /// <summary>
        /// 重启计算机
        /// </summary>
        public void Restart()
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = "cmd.exe";                   //启动cmd命令
            myProcess.StartInfo.UseShellExecute = false;                //是否使用系统外壳程序启动进程
            myProcess.StartInfo.RedirectStandardInput = true;           //是否从流中读取
            myProcess.StartInfo.RedirectStandardOutput = true;          //是否写入流
            myProcess.StartInfo.RedirectStandardError = true;           //是否将错误信息写入流
            myProcess.StartInfo.CreateNoWindow = true;                  //是否在新窗口中启动进程
            myProcess.Start();                                          //启动进程
            myProcess.StandardInput.WriteLine("shutdown -r -t 0");      //执行重启计算机命令
        }

        #endregion

        #region 监测端口状态
        /// <summary>
        /// 监测端口状态:判断端口是否打开
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool PortState(string ip, string port)
        {
            bool state;
            bool tcpListen = false;
            bool udpListen = false;
            System.Net.IPAddress myIpAddress = null;
            System.Net.IPEndPoint myIpEndPoint = null;
            try
            {
                myIpAddress = IPAddress.Parse(ip);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            try
            {
                myIpEndPoint = new IPEndPoint(myIpAddress, int.Parse(port));
                System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient();
                tcpClient.Connect(myIpEndPoint);
                tcpListen = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            try
            {
                System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient();
                udpClient.Connect(myIpEndPoint);
                udpListen = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            if (tcpListen == true && udpListen == true)
                state = true;
            else
                state = false;

            return state;
        }
        #endregion

        #region 获得PC类型
        /// <summary>
        /// 获得PC类型
        /// </summary>
        /// <returns></returns>
        public string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["SystemType"].ToString();
                }
                moc = null;
                mc = null;
                return "电脑类型:" + st;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 获得内存使用情况
        /// <summary>
        /// 获取内存使用信息
        /// </summary>
        /// <param name="memoryUsageType">内存使用类型</param>
        /// <returns></returns>
        public string GetMemoryInfo(MemoryUsageType memoryUsageType)
        {
            string memoryMsg = "unknow";
            Computer myComputer = new Computer();
            switch(memoryUsageType)
            {
                case MemoryUsageType.TotalPhysicalMemory:
                    memoryMsg = ByteConvertToGBMBKB(Convert.ToInt64(myComputer.Info.TotalPhysicalMemory));
                    break;
                case MemoryUsageType.AvailablePhysicalMemory:
                    memoryMsg = ByteConvertToGBMBKB(Convert.ToInt64(myComputer.Info.AvailablePhysicalMemory));
                    break;
                case MemoryUsageType.TotalVirtualMemory:
                    memoryMsg = ByteConvertToGBMBKB(Convert.ToInt64(myComputer.Info.TotalVirtualMemory));
                    break;
                case MemoryUsageType.AvailableVirtualMemory:
                    memoryMsg = ByteConvertToGBMBKB(Convert.ToInt64(myComputer.Info.AvailableVirtualMemory));
                    break;
                default:
                    break;
            }

            return memoryMsg;
        }
        #endregion

        #region 获得当前屏幕分辨率
        /// <summary>
        /// 获得当前屏幕分辨率
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GetCurrentScreenResolution(ref int width, ref int height)
        {
            width = SystemInformation.VirtualScreen.Width;
            height = SystemInformation.VirtualScreen.Height;
        }
        #endregion
    }

}

