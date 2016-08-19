// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.File
// * 文件名称：		    DirFileHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-07-19
// * 程序功能描述：
// *       目录、文件基本操作类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aitipachong.File
{
    /// <summary>
    /// 目录和文件基本操作类
    /// </summary>
    public class DirFileHelper
    {
        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsExistFile(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        /// <summary>
        /// 获取指定目录中所有文件列表（当前目录，不包括子目录）
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static string[] GetFileNames(string directoryPath)
        {
            if(!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            return Directory.GetFiles(directoryPath);
        }

        /// <summary>
        /// 获取指定目录的下一层子目录列表
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch(IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild"></param>
        /// <returns>是否搜索子目录</returns>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            if (!IsExistDirectory(directoryPath)) throw new FileNotFoundException();
            try
            {
                if(isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch(IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检查指定目录是否为空
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0) return false;

                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0) return false;

                return true;
            }
            catch(IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测指定目录中是否存在指定的文件，若要搜索子目录请使用重载方法。
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>        
        /// <returns></returns>
        public static bool Contains(string directoryPath, string searchPattern)
        {
            try
            {
                string[] fileNames = GetFileNames(directoryPath, searchPattern, false);
                if (fileNames.Length == 0)
                    return false;
                else
                    return true;
            }
            catch(IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测指定目录中是否存在指定的文件
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>        
        /// <param name="isSeachChild">是否搜索子目录</param>
        /// <returns></returns>
        public static bool Contains(string directoryPath, string searchPattern, bool isSeachChild)
        {
            try
            {
                string[] fileNames = GetFileNames(directoryPath, searchPattern, isSeachChild);
                if (fileNames.Length == 0)
                    return false;
                else
                    return true;
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建目录(Web)
        /// </summary>
        /// <param name="dir"></param>
        public static void CreateDir(string dir)
        {
            if (string.IsNullOrEmpty(dir)) return;
            if (!Directory.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir))
                Directory.CreateDirectory(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir);
        }

        /// <summary>
        /// 删除目录(Web)
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteDir(string dir)
        {
            if (string.IsNullOrEmpty(dir)) return;
            if (Directory.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir))
                Directory.Delete(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir);
        }

        /// <summary>
        /// 删除文件(Web)
        /// </summary>
        /// <param name="file">要删除的文件路径和名称</param>
        public static void DeleteFile(string file)
        {
            if (System.IO.File.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file))
                System.IO.File.Delete(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file);
        }

        /// <summary>
        /// 创建文件(Web)
        /// </summary>
        /// <param name="dir">带后缀的文件名</param>
        /// <param name="pageStr">文件内容</param>
        public static void CreateFile(string dir, string pageStr)
        {
            dir = dir.Replace("/", "\\");
            if (dir.IndexOf("\\") > -1) CreateDir(dir.Substring(0, dir.LastIndexOf("\\")));
            System.IO.StreamWriter sw = new StreamWriter(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" +
                dir, false, System.Text.Encoding.GetEncoding("GB2312"));
            sw.Write(pageStr);
            sw.Close();
        }

        /// <summary>
        /// 移动文件(Web)（剪切 -- 粘贴）
        /// </summary>
        /// <param name="dir1">要移动的文件的路径及全名（包括后缀）</param>
        /// <param name="dir2">文件移动到新的位置，并制定新的文件名</param>
        public static void MoveFile(string dir1, string dir2)
        {
            dir1 = dir1.Replace("/", "\\");
            dir2 = dir2.Replace("/", "\\");
            if (System.IO.File.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir1))
                System.IO.File.Move(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir1,
                    System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir2);
        }

        /// <summary>
        /// 复制文件(Web)
        /// </summary>
        /// <param name="dir1">要复制的文件的路径及全名（包括后缀）</param>
        /// <param name="dir2">目标位置，并制定新的文件名</param>
        public static void CopyFile(string dir1, string dir2)
        {
            dir1 = dir1.Replace("/", "\\");
            dir2 = dir2.Replace("/", "\\");
            if (System.IO.File.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir1))
                System.IO.File.Copy(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir1,
                    System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + dir2, true);
        }

        /// <summary>
        /// 复制文件夹及文件夹内所有内容(Web)(递归)
        /// </summary>
        /// <param name="varFormDirectory">源文件夹路径</param>
        /// <param name="varToDirectory">目标文件夹路径</param>
        public static void CopyFolder(string varFormDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);
            if (!Directory.Exists(varFormDirectory)) return;
            string[] directories = Directory.GetDirectories(varFormDirectory);
            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    CopyFolder(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }
            string[] files = Directory.GetFiles(varFormDirectory);
            if(files.Length > 0)
            {
                foreach(string f in files)
                {
                    System.IO.File.Copy(f, varToDirectory + f.Substring(f.LastIndexOf("\\")), true);
                }
            }
        }

        /// <summary>
        /// 检查文件，如果文件不存在则创建
        /// </summary>
        /// <param name="filePath">路径，包括文件名及后缀</param>
        public static void ExistsFile(string filePath)
        {
            if(!System.IO.File.Exists(filePath))
            {
                FileStream fs = System.IO.File.Create(filePath);
                fs.Close();
            }
        }

        /// <summary>
        /// 删除指定文件夹对应其他文件夹里同名文件(删除的是其他文件夹内同名文件)
        /// </summary>
        /// <param name="varFromDirectory">指定文件夹路径</param>
        /// <param name="varToDirectory">对应其他文件夹路径</param>
        public static void DeleteFolderFiles(string varFromDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);
            if (!Directory.Exists(varFromDirectory)) return;
            string[] directories = Directory.GetDirectories(varFromDirectory);
            if(directories.Length > 0)
            {
                foreach(string d in directories)
                {
                    DeleteFolderFiles(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }

            string[] files = Directory.GetFiles(varFromDirectory);
            if(files.Length > 0)
            {
                foreach(string f in files)
                {
                    System.IO.File.Delete(varToDirectory + f.Substring(f.LastIndexOf("\\")));
                }
            }
        }

        /// <summary>
        /// 从文件的绝对路径中获取文件名（包含扩展名）
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <returns></returns>
        public static string GetFileName(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }

        /// <summary>
        /// 创建一个目录
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void CreateDirectory(string directoryPath)
        {
            if (!IsExistDirectory(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        /// <summary>
        /// 创建一个文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void CreateFile(string filePath)
        {
            try
            {
                if(!IsExistFile(filePath))
                {
                    //创建一个FileInfo对象
                    FileInfo fi = new FileInfo(filePath);
                    FileStream fs = fi.Create();
                    fs.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建一个文件，并将字节流写入文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                if (!IsExistFile(filePath))
                {
                    FileInfo fi = new FileInfo(filePath);
                    FileStream fs = fi.Create();
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取文本文件的行数
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <returns></returns>
        public static int GetTxtLineCount(string filePath)
        {
            string[] rows = System.IO.File.ReadAllLines(filePath);
            return rows.Length;
        }

        /// <summary>
        /// 获取文件的长度，单位为Byte
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static int GetFileSize(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return (int)fi.Length;
        }

        /// <summary>
        /// 获取指定目录及子目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符
        ///     范例："Log*.xml"表示搜索所有以Log开头的xml文件
        /// </param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        /// <returns></returns>
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if(isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch(IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 向文本文件中写入内容
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="text">写入内容</param>
        /// <param name="encoding">编码</param>
        public static void WriteText(string filePath, string text, System.Text.Encoding encoding)
        {
            System.IO.File.WriteAllText(filePath, text, encoding);
        }

        /// <summary>
        /// 向文本文件末尾追加内容
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="content">追加内容</param>
        public static void AppendText(string filePath, string content)
        {
            System.IO.File.AppendAllText(filePath, content);
        }

        /// <summary>
        /// 将源文件的内容复制到目标文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destFilePath"></param>
        public static void Copy(string sourceFilePath, string destFilePath)
        {
            System.IO.File.Copy(sourceFilePath, destFilePath);
        }

        /// <summary>
        /// 将文件移动到指定目录
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destDirectoryPath"></param>
        public static void Move(string sourceFilePath, string destDirectoryPath)
        {
            string sourceFileName = GetFileName(sourceFilePath);
            if(IsExistDirectory(destDirectoryPath))
            {
                if (IsExistFile(destDirectoryPath + "\\" + sourceFileName))
                {
                    DeleteFile(destDirectoryPath + "\\" + sourceFileName);
                }

                System.IO.File.Move(sourceFilePath, destDirectoryPath + "\\" + sourceFileName);
            }
        }

        /// <summary>
        /// 从文件的绝对路径中获取文件名（不包含扩展名）
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileNameNoExtension(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Name.Split('.')[0];
        }

        /// <summary>
        /// 从文件的绝对路径中获取扩展名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetExtension(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Extension;
        }

        /// <summary>
        /// 清空指定目录下所有文件及子目录，但该目录依然保存
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void ClearDirectory(string directoryPath)
        {
            if(IsExistDirectory(directoryPath))
            {
                string[] fileNames = GetFileNames(directoryPath);
                for(int i = 0; i < fileNames.Length; i++)
                {
                    DeleteFile(fileNames[i]);
                }

                string[] directoryNames = GetDirectories(directoryPath);
                for(int i = 0; i < directoryNames.Length; i++)
                {
                    DeleteDirectory(directoryNames[i]);
                }
            }
        }

        /// <summary>
        /// 清空文件内容
        /// </summary>
        /// <param name="filePath"></param>
        public static void ClearFile(string filePath)
        {
            System.IO.File.Delete(filePath);
            CreateFile(filePath);
        }

        /// <summary>
        /// 删除指定目录及其左右子目录
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
                Directory.Delete(directoryPath, true);
        }
    }
}