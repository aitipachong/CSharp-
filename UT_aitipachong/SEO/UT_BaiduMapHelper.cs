using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aitipachong.SEO;

namespace UT_aitipachong.SEO
{
    [TestClass]
    public class UT_BaiduMapHelper
    {
        [TestMethod]
        public void UT_GetCoords_V1()
        {
            CoordsItem item = new CoordsItem()
            {
                x = 113.691872,
                y = 34.784988
            };

            try
            {
                CoordsItem result = BaiduMapHelper.GetCoords(item);

                Assert.IsNotNull(result, "百度地图经纬度转换失败!");
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
