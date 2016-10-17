// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Zip
// * 文件名称：		    ZipFactory.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-10-14
// * 程序功能描述：
// *        压缩&解压工厂
// *            1.SharpZipLib开源技术加压和解压
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace aitipachong.Zip
{
    /// <summary>
    /// 使用SharpZipLib技术进行加压和解压
    /// </summary>
    public class SharpZip
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="filename">压缩后待保存的文件名（包含物理路径）</param>
        /// <param name="directory">待压缩的文件夹（包含物理路径）</param>
        public static void PackFiles(string filename, string directory)
        {
            try
            {
                FastZip fz = new FastZip();
                fz.CreateEmptyDirectories = true;
                fz.CreateZip(filename, directory, true, "");
                fz = null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="file">待解压文件名（包括物理路径）</param>
        /// <param name="dir">解压到的目录（包含物理路径）</param>
        /// <returns></returns>
        public static bool UnpackFiles(string file, string dir)
        {
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                ZipInputStream zis = new ZipInputStream(System.IO.File.OpenRead(file));
                ZipEntry theEntry;
                while((theEntry = zis.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (!string.IsNullOrEmpty(directoryName)) Directory.CreateDirectory(Path.Combine(dir, directoryName));
                    if(!string.IsNullOrEmpty(fileName))
                    {
                        FileStream streamWriter = System.IO.File.Create(Path.Combine(dir, theEntry.Name));
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while(true)
                        {
                            size = zis.Read(data, 0, data.Length);
                            if(size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
                zis.Close();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }

    public class ClassZip
    {
        #region 私有方法
        /// <summary>
        /// 递归压缩文件夹方法
        /// </summary>
        /// <param name="folderToZip"></param>
        /// <param name="s"></param>
        /// <param name="parentFolderName"></param>
        /// <returns></returns>
        private static bool ZipFileDictory(string folderToZip, ZipOutputStream s, string parentFolderName)
        {
            string[] folders, filenames;
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();
            try
            {
                entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
                s.PutNextEntry(entry);
                s.Flush();
                filenames = Directory.GetFiles(folderToZip);
                foreach(string file in filenames)
                {
                    fs = System.IO.File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(file)));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if(fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if(entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            folders = Directory.GetDirectories(folderToZip);
            foreach(string folder in folders)
            {
                if(!ZipFileDictory(folder, s, Path.Combine(parentFolderName, Path.Combine(folderToZip))))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="folderToZip">待压缩的文件夹（包括物理路径）</param>
        /// <param name="zipedFile">压缩后的文件名（包括物理路径）</param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool ZipFileDictory(string folderToZip, string zipedFile, int level)
        {
            bool res;
            if (!Directory.Exists(folderToZip)) return false;
            ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(zipedFile));
            res = ZipFileDictory(folderToZip, s, "");
            s.Finish();
            s.Close();
            return res;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipedFile">压缩后生成的压缩文件名</param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool ZipFile(string fileToZip, string zipedFile, int level)
        {
            if (!System.IO.File.Exists(fileToZip)) throw new FileNotFoundException("指定要压缩的文件：" + fileToZip + " 不存在!");
            FileStream zipFile = null;
            ZipOutputStream zipStream = null;
            ZipEntry zipEntry = null;
            bool res = true;

            try
            {
                zipFile = System.IO.File.OpenRead(fileToZip);
                byte[] buffer = new byte[zipFile.Length];
                zipFile.Read(buffer, 0, buffer.Length);
                zipFile.Close();

                zipFile = System.IO.File.Create(zipedFile);
                zipStream = new ZipOutputStream(zipFile);
                zipEntry = new ZipEntry(Path.GetFileName(fileToZip));
                zipStream.PutNextEntry(zipEntry);
                zipStream.SetLength(level);

                zipStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (zipEntry != null) zipEntry = null;
                if(zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
                if(zipFile != null)
                {
                    zipFile.Close();
                    zipFile = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return res;
        }
        #endregion

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="fileToZip">待压缩的文件或目录</param>
        /// <param name="zipedFile">生成的目标文件</param>
        /// <param name="level">默认：6</param>
        /// <returns></returns>
        public static bool Zip(string fileToZip, string zipedFile, int level = 6)
        {
            if (Directory.Exists(fileToZip))
            {
                return ZipFileDictory(fileToZip, zipedFile, level);
            }
            else if (System.IO.File.Exists(fileToZip))
            {
                return ZipFile(fileToZip, zipedFile, level);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="fileToUpZip">待解压的文件</param>
        /// <param name="zipedFolder">解压目标存放目录</param>
        public static void UnZip(string fileToUpZip, string zipedFolder)
        {
            if (!System.IO.File.Exists(fileToUpZip))
            {
                return;
            }
            if (!Directory.Exists(zipedFolder))
            {
                Directory.CreateDirectory(zipedFolder);
            }
            ZipInputStream s = null;
            ZipEntry theEntry = null;
            string fileName;
            FileStream streamWriter = null;
            try
            {
                s = new ZipInputStream(System.IO.File.OpenRead(fileToUpZip));
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.Name != String.Empty)
                    {
                        fileName = Path.Combine(zipedFolder, theEntry.Name);
                        if (fileName.EndsWith("/") || fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }
                        streamWriter = System.IO.File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter = null;
                }
                if (theEntry != null)
                {
                    theEntry = null;
                }
                if (s != null)
                {
                    s.Close();
                    s = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
        }
    }

    public class ZipHelper
    {
        #region 私有变量
        String the_rar;
        RegistryKey the_Reg;
        Object the_Obj;
        String the_Info;
        ProcessStartInfo the_StartInfo;
        Process the_Process;
        #endregion

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="zipname">要解压的文件名</param>
        /// <param name="zippath">要压缩的文件目录</param>
        /// <param name="dirpath">初始目录</param>
        public void EnZip(string zipname, string zippath, string dirpath)
        {
            try
            {
                the_Reg = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRAR.exe\Shell\Open\Command");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                the_rar = the_rar.Substring(1, the_rar.Length - 7);
                the_Info = " a    " + zipname + "  " + zippath;
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_StartInfo.WorkingDirectory = dirpath;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zipname">要解压的文件名</param>
        /// <param name="zippath">要解压的文件路径</param>
        public void DeZip(string zipname, string zippath)
        {
            try
            {
                the_Reg = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRar.exe\Shell\Open\Command");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                the_rar = the_rar.Substring(1, the_rar.Length - 7);
                the_Info = " X " + zipname + " " + zippath;
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}