// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.SEO
// * 文件名称：		    BaiduMapHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-11-02
// * 程序功能描述：
// *        百度地图经纬度转换帮助类
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using aitipachong.NetWork;
using aitipachong.Serialize;
using System.Collections.Generic;
using System.Text;

namespace aitipachong.SEO
{
    /// <summary>
    /// 百度地图：经纬度转化帮助类
    /// </summary>
    public class BaiduMapHelper
    {
        /// <summary>
        /// 百度地图位置转化URL
        /// </summary>
        private static string CoordsUrl = "http://api.map.baidu.com/geoconv/v1/";

        /// <summary>
        /// 根据输入的经纬度List转换为百度地图的经纬度List，一一对应
        /// </summary>
        /// <param name="citems">待转化的经纬度List</param>
        /// <returns>转换后的百度经纬度List</returns>
        public static List<CoordsItem> GetCoords(List<CoordsItem> citems)
        {
            HttpHelper http = new HttpHelper();
            string strCoords = ListToString(citems);
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}?coords={1}&from=3&to=5&ak=17bc43866bbd51f7507e0c618f890e64", CoordsUrl, strCoords),
            };

            HttpResult result = http.GetHtml(item);
            string html = result.Html;
            citems = StringToList(html);
            return citems;
        }

        /// <summary>
        /// 根据输入的经纬度转换为百度地图的经纬度
        /// </summary>
        /// <param name="citem">待转化的经纬度</param>
        /// <returns>转换后的百度经纬度</returns>
        public static CoordsItem GetCoords(CoordsItem citem)
        {
            List<CoordsItem> list = new List<CoordsItem>();
            list.Add(citem);
            list = GetCoords(list);
            if(list != null & list.Count > 0)
            {
                return list[0];
            }

            return null;
        }

        /// <summary>
        /// 根据List换换为响应的字符串经纬度
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static string ListToString(List<CoordsItem> items)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var obj in items)
            {
                if(string.IsNullOrWhiteSpace(sb.ToString()))
                {
                    sb.Append(obj.x + "," + obj.y);
                }
                else
                {
                    sb.Append(";" + obj.x + "," + obj.y);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 根据Json格式转换为List
        /// </summary>
        /// <example>
        /// {"status":0,"result":[{"x":114.22539195429,"y":29.581585367458},{"x":114.2253919533,"y":29.581585366942}]}
        /// {"status":24,"message":"param error:coords format error","result":[]}
        /// </example>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<CoordsItem> StringToList(string str)
        {
            List<CoordsItem> items = new List<CoordsItem>();

            SerializeHelper serialize = new SerializeHelper();
            CoordsList serializeResult = serialize.FromJson<CoordsList>(str);

            return serializeResult.result;
        }
    }

    /// <summary>
    /// 返回参数类
    /// </summary>
    public class CoordsList
    {
        /// <summary>
        /// 状态：0为成功；其他为不成功
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 经纬度列表
        /// </summary>
        public List<CoordsItem> result { get; set; }
    }

    /// <summary>
    /// 经纬度类
    /// </summary>
    public class CoordsItem
    {
        /// <summary>
        /// 经度
        /// </summary>
        public double x { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double y { get; set; }
    }
}