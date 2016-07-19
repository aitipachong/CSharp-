// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Excel
// * 文件名称：		    ExportExcel.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-07-15
// * 程序功能描述：
// *        网页、DataView内容导出到Excel
// *            1.将整个网页导出来Excel
// *            2.将GridView数据导出Excel
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aitipachong.Excel
{
    /// <summary>
    /// 网页、DataView内容导出到Excel
    /// </summary>
    public class ExportExcel
    {
        /// <summary>
        /// 将整个网页导出到Excel
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="fileName"></param>
        public void WebPageExportToExcel(string strContent, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("Excel存储路径为空.");
            fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmsslll");
            if(File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                    throw new Exception("该文件正在使用中，关闭文件或重新命名导出文件再试!");
                }
            }

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Charset = "gb2312";
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            //增加头信息，为“文件下载/另存为”对话框指定默认文件名
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xls");
            //把文件流发送到客户端
            HttpContext.Current.Response.Write("<html><head><meta http-equiv=Content-Type content=\"text/html; charset=utf-8\">");
            HttpContext.Current.Response.Write(strContent);
            HttpContext.Current.Response.Write("</body></html>");
        }

        /// <summary>
        /// 将GridView数据导出到Excel
        /// </summary>
        /// <param name="obj">GridView实体对象</param>
        public void ExportData(GridView obj)
        {
            try
            {
                string style = "";
                if(obj.Rows.Count > 0)
                {
                    style = @"<style> .text { mso-number-format:\@; } </script> ";
                }
                else
                {
                    style = "no data.";
                }

                HttpContext.Current.Response.ClearContent();
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmsslll");
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=ExportData" + fileName + ".xls");
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                obj.RenderControl(htw);
                HttpContext.Current.Response.Write(style);
                HttpContext.Current.Response.Write(sw.ToString());
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}