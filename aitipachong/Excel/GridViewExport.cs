// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.Excel
// * 文件名称：		    GridViewExport.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-07-19
// * 程序功能描述：
// *       GridView内容导出到Excel
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aitipachong.Excel
{
    /// <summary>
    /// GridView控件内容导出
    /// </summary>
    public class GridViewExport
    {
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="gv"></param>
        public static void Export(string fileName, GridView gv)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.Charset = "utf-8";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //Create a form to contain the grid
                    Table table = new Table();
                    table.GridLines = GridLines.Both;       //单元格之间添加实线
                    
                    //add the header row to the table
                    if(gv.HeaderRow != null)
                    {
                        PrepareControlForExport(gv.HeaderRow);
                        table.Rows.Add(gv.HeaderRow);
                    }

                    //add each of the data rows to the table
                    foreach(GridViewRow row in gv.Rows)
                    {
                        PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }

                    //add the footer row to the table
                    if(gv.FooterRow != null)
                    {
                        PrepareControlForExport(gv.FooterRow);
                        table.Rows.Add(gv.FooterRow);
                    }

                    //render the table into the htmlwriter
                    table.RenderControl(htw);
                    //render the htmlwriter into the response
                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.End();
                }
            }
        }

        /// <summary>
        /// Replace any of the contained controls with literals
        /// </summary>
        /// <param name="control"></param>
        private static void PrepareControlForExport(Control control)
        {
            for(int i = 0; i < control.Controls.Count; i++)
            {
                Control current = control.Controls[i];
                if(current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
                }
                else if(current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if(current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if(current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if(current is CheckBox)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                }

                if(current.HasControls())
                {
                    PrepareControlForExport(current);
                }
            }
        }

        /// <summary>
        /// 导出Grid的数据（全部）到Excel
        ///     字段全部为BoundField类型时可用
        ///     要是字段为TemplateField模板型时就取不到数据
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="dt"></param>
        /// <param name="excelFileName"></param>
        public static void OutputExcel(GridView grid, DataTable dt, string excelFileName)
        {
            Page page = (Page)HttpContext.Current.Handler;
            page.Response.Clear();
            string fileName = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.UTF8.GetBytes(excelFileName));
            page.Response.AddHeader("Content-Disposition", "attachment:filename=" + fileName + ".xls");
            page.Response.ContentType = "application/vnd.ms-excel";
            page.Response.Charset = "utf-8";

            StringBuilder sb = new StringBuilder();
            sb.Append("<HTML><HEAD><TITLE>" + fileName + "</TITLE><META http-equiv-\"Content-Type\" content=\"text/html; charset=utf-8\"></HEAD><body>");

            int count = grid.Columns.Count;
            sb.Append("<table border=1>");
            sb.AppendLine("<tr>");
            for(int i = 0; i < count; i++)
            {
                if (grid.Columns[i].GetType() == typeof(BoundField))
                    sb.Append("<td>" + grid.Columns[i].HeaderText + "</td>");
            }
            sb.AppendLine("</tr>");

            foreach(DataRow dr in dt.Rows)
            {
                sb.AppendLine("<tr>");
                for(int n = 0; n < count; n++)
                {
                    if (grid.Columns[n].Visible && grid.Columns[n].GetType() == typeof(BoundField))
                        sb.Append("<td>" + dr[((BoundField)grid.Columns[n]).DataField].ToString() + "</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.Append("</table>");
            sb.Append("</body></HTML>");

            page.Response.BinaryWrite(System.Text.Encoding.GetEncoding("utf-8").GetBytes(sb.ToString()));
            page.Response.End();
        }
    }
}