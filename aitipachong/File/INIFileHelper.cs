// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.File
// * 文件名称：		    INIFileHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-08-19
// * 程序功能描述：
// *       INI文件读写操作类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace aitipachong.File
{
    /// <summary>
    /// INI文件读写操作类
    /// </summary>
    public class INIFileHelper
    {
        public string path;

        public INIFileHelper(string INIPath)
        {
            path = INIPath;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);


        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }

        public byte[] IniReadValues(string section, string key)
        {
            byte[] temp = new byte[255];
            int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
            return temp;

        }

        /// <summary>
        /// 删除ini文件下所有段落
        /// </summary>
        public void ClearAllSection()
        {
            IniWriteValue(null, null, null);
        }
        /// <summary>
        /// 删除ini文件下personal段落下的所有键
        /// </summary>
        /// <param name="Section"></param>
        public void ClearSection(string Section)
        {
            IniWriteValue(Section, null, null);
        }
    }
}
