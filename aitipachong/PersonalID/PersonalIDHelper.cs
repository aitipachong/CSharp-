// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.PersonalID
// * 文件名称：		    PersonalIDHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-06-15
// * 程序功能描述：
// *        个人身份证号码解析与生成
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace aitipachong.PersonalID
{
    /// <summary>
    /// 个人身份证号码解析与生成
    /// </summary>
    public class PersonalIDHelper
    {
        #region 身份证信息属性
        /// <summary>
        /// 所在省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 所在地区
        /// </summary>
        public string Area { get; set; } 
        /// <summary>
        /// 所在区县
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public DateTime Age { get; set; }        
        /// <summary>
        /// 性别：0，女；1，男；
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// Json字符串
        /// </summary>
        public string Json { get; set; }
        #endregion

        #region 静态方法
        private static readonly List<string[]> Areas = new List<string[]>();
        /// <summary>
        /// 获取区域信息
        /// </summary>
        private static void FillAreas()
        {
            XmlDocument docXml = new XmlDocument();
            //从“IDReginCode.xml”文件中加载
            string file = Path.Combine(Environment.CurrentDirectory, "PersonalID", "IDReginCode.xml");
            docXml.Load(file);
            XmlNodeList nodeList = docXml.GetElementsByTagName("area");
            foreach(XmlNode node in nodeList)
            {
                string code = node.Attributes["code"].Value;
                string name = node.Attributes["name"].Value;
                PersonalIDHelper.Areas.Add(new string[] { code, name });
            }
        }
        
        /// <summary>
        /// 解析身份证信息
        /// </summary>
        /// <param name="idCardNumber"></param>
        /// <returns></returns>
        public static PersonalIDHelper Get(string idCardNumber)
        {
            if (PersonalIDHelper.Areas.Count < 1) PersonalIDHelper.FillAreas();
            if (!PersonalIDHelper.CheckIDCardNumber(idCardNumber)) throw new Exception("非法的身份证号码");

            PersonalIDHelper cardInfo = new PersonalIDHelper(idCardNumber);
            return cardInfo;
        }

        /// <summary>
        /// 校验身份证号码是否合法
        /// </summary>
        /// <param name="idCardNumber"></param>
        /// <returns></returns>
        public static bool CheckIDCardNumber(string idCardNumber)
        {
            Regex rg = new Regex(@"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
            Match mc = rg.Match(idCardNumber);
            if (!mc.Success) return false;
            //加权码
            string code = idCardNumber.Substring(17, 1);
            double sum = 0;
            string checkCode = null;
            for(int i = 2; i <= 18; i++)
            {
                sum += int.Parse(idCardNumber[18 - i].ToString(), NumberStyles.HexNumber) * (Math.Pow(2, i - 1) % 11);
            }
            string[] checkCodes = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            checkCode = checkCodes[(int)sum % 11];
            if (checkCode != code) return false;
            return true;
        }

        /// <summary>
        /// 随机生成一个身份证号
        /// </summary>
        /// <returns></returns>
        public static PersonalIDHelper Radom()
        {
            long tick = DateTime.Now.Ticks;
            return new PersonalIDHelper(_radomCardNumber((int)tick));
        }

        /// <summary>
        /// 批量生成身份证号
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<PersonalIDHelper> Radom(int count)
        {
            List<PersonalIDHelper> list = new List<PersonalIDHelper>();
            string cardNumber;
            bool isExist;
            for(int i = 0; i < count; i++)
            {
                do
                {
                    isExist = false;
                    int tick = (int)DateTime.Now.Ticks;
                    cardNumber = PersonalIDHelper._radomCardNumber(tick * (i + 1));
                    foreach (PersonalIDHelper p in list)
                    {
                        if (p.CardNumber == cardNumber)
                        {
                            isExist = true;
                            break;
                        }
                    }
                } while (isExist);
                list.Add(new PersonalIDHelper(cardNumber));
            }

            return list;
        }

        /// <summary>
        /// 生成随机身份证号码
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        private static string _radomCardNumber(int seed)
        {
            if (PersonalIDHelper.Areas.Count < 1) PersonalIDHelper.FillAreas();
            System.Random rd = new Random(seed);
            //随机生成发证地
            string area = "";
            do
            {
                area = PersonalIDHelper.Areas[rd.Next(0, PersonalIDHelper.Areas.Count - 1)][0];
            } while (area.Substring(4, 2) == "00");
            //随机生成出生日期
            DateTime birthday = DateTime.Now;
            birthday = birthday.AddYears(-rd.Next(16, 60));
            birthday = birthday.AddMonths(-rd.Next(0, 12));
            birthday = birthday.AddDays(-rd.Next(0, 31));
            //随机码
            string code = rd.Next(1000, 9999).ToString("####");
            //生成完整的身份证号
            string codeNumber = area + birthday.ToString("yyyyMMdd") + code;
            double sum = 0;
            string checkCode = null;
            for(int i = 2; i <= 18; i++)
            {
                sum += int.Parse(codeNumber[18 - i].ToString(), NumberStyles.HexNumber) * (Math.Pow(2, i - 1) % 11);
            }
            string[] checkCodes = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            checkCode = checkCodes[(int)sum % 11];
            codeNumber = codeNumber.Substring(0, 17) + checkCode;
            return codeNumber;
        }
        #endregion

        #region 身份证解析方法

        private PersonalIDHelper(string idCardNumber)
        {
            this.CardNumber = idCardNumber;
            Analysis();
        }

        /// <summary>
        /// 解析
        /// </summary>
        private void Analysis()
        {
            //取省份、地区、区县
            string provCode = CardNumber.Substring(0, 2).PadRight(6, '0');
            string areaCode = CardNumber.Substring(0, 4).PadRight(6, '0');
            string cityCode = CardNumber.Substring(0, 6).PadRight(6, '0');
            for(int i = 0; i < PersonalIDHelper.Areas.Count; i++)
            {
                if (provCode == PersonalIDHelper.Areas[i][0])
                    this.Province = PersonalIDHelper.Areas[i][1];
                if (areaCode == PersonalIDHelper.Areas[i][0])
                    this.Area = PersonalIDHelper.Areas[i][1];
                if (cityCode == PersonalIDHelper.Areas[i][0])
                    this.City = PersonalIDHelper.Areas[i][1];
                if (Province != null && Area != null && City != null) break;
            }
            //取年龄
            string ageCode = CardNumber.Substring(6, 8);
            try
            {
                int year = Convert.ToInt16(ageCode.Substring(0, 4));
                int month = Convert.ToInt16(ageCode.Substring(4, 2));
                int day = Convert.ToInt16(ageCode.Substring(6, 2));
                Age = new DateTime(year, month, day);
            }
            catch
            {
                throw new Exception("非法的出生日期");
            }
            //取性别
            string orderCode = CardNumber.Substring(14, 3);
            this.Sex = Convert.ToInt16(orderCode) % 2 == 0 ? 0 : 1;
            //生成JSON
            Json = @"prov:'{0}',area:'{1}',city:'{2}',year:{3},month:{4},day:{5},sex:{6},number:'{7}'";
            Json = string.Format(Json, Province, Area, City, Age.Year, Age.Month, Age.Day, Sex, CardNumber);
            Json = "{" + Json + "}";
        }
        #endregion
    }
}
/*

//****************************调用***************************

注意几个地方：
1：调用此类，必须添加应用System.Web, 然后开头写下下面这句using
using System.Web.Hosting;
2：因为要获得地区代码，所以这里要导入一个XML文件，XML文件内容在本文最后
3：将XML文件放入Debug目录和Release目录

//随机生成一个身份证号
IDCardNumber GetId = IDCardNumber.Radom();
//身份证号码
string Idcard = GetId.CardNumber;
//这个身份证的持有者的地区
string Name = GetId.City;
//这个身份证持有者的年龄
int Age = GetId.Age;
//这个身份证持有者的性别 0女 1男
int Sex = GetId.Sex;

//随机生成10个身份证号码
List<IDCardNumber> GetIdList = IDCardNumber.Radom(10);

//校验身份证号码是否有误
if (IDCardNumber.CheckIDCardNumber(GetId.CardNumber))
{
    //身份证符合
}*/
