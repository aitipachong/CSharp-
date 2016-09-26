// ********************************************************************
// * 项目名称：		    aitipachong
// * 程序集名称：	    aitipachong.XML
// * 文件名称：		    XmlHelper.cs
// * 编写者：		    Lai.Qiang
// * 编写日期：		    2016-09-26
// * 程序功能描述：
// *        XML公共操作类(XPath表达式)
// *
// * 程序变更日期：
// * 程序变更者：
// * 变更说明：
// * 
// ********************************************************************
using aitipachong.File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace aitipachong.XML
{
    /// <summary>
    /// XML公共操作类(XPath表达式)
    /// </summary>
    public class XmlHelper
    {
        #region 字段定义
        /// <summary>
        /// XML文件的物理路径
        /// </summary>
        private string filePath = string.Empty;
        /// <summary>
        /// Xml文档对象
        /// </summary>
        private XmlDocument xml;
        /// <summary>
        /// Xml的根节点
        /// </summary>
        private XmlElement element;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public XmlHelper(string xmlFilePath)
        {
            this.filePath = xmlFilePath;
        }
        #endregion

        #region 创建XML的根节点
        /// <summary>
        /// 创建XML的根节点
        /// </summary>
        private void CreateXmlElement()
        {
            //创建一个XML对象
            this.xml = new XmlDocument();
            if(DirFileHelper.IsExistFile(this.filePath))
            {
                //加载Xml文件
                this.xml.Load(this.filePath);
            }
            //为XML的根节点赋值
            this.element = this.xml.DocumentElement;
        }
        #endregion

        #region 获取指定XPath表达式的节点对象
        /// <summary>
        /// 获取指定XPath表达式的节点对象
        /// </summary>
        /// <param name="xPath">XPath表达式，
        /// 范例1：@"Skill/First/SkillItem",等效于"//Skill//First//SkillItem"
        /// 范例2：@"Table[USERNAME='a']",[]表示筛选，USERNAME是Table下的一个子节点
        /// 范例3：@"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性
        /// </param>
        /// <returns></returns>
        public XmlNode GetNode(string xPath)
        {
            //创建Xml的根节点
            this.CreateXmlElement();
            //返回XPath节点
            return this.element.SelectSingleNode(xPath);
        }
        #endregion

        #region 获取指定XPath表达式节点的值
        /// <summary>
        /// 获取指定XPath表达式节点的值
        /// </summary>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <returns></returns>
        public string GetValue(string xPath)
        {
            this.CreateXmlElement();
            return this.element.SelectSingleNode(xPath).InnerText;
        }
        #endregion

        #region 获取指定XPath表达式节点的属性值
        /// <summary>
        /// 获取指定XPath表达式节点的属性值
        /// </summary>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <param name="attributeName">属性名</param>
        public string GetAttributeValue(string xPath, string attributeName)
        {
            this.CreateXmlElement();
            return this.element.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }
        #endregion

        #region 新增节点
        /// <summary>
        /// 1.功能：新增节点
        /// 2.使用条件：将任意节点插入到当前XML文件中
        /// </summary>
        /// <param name="xmlNode">要插入的Xml节点</param>
        public void AppendNode(XmlNode xmlNode)
        {
            this.CreateXmlElement();
            //导入节点
            XmlNode node = this.xml.ImportNode(xmlNode, true);
            //将节点插入到根节点下
            this.element.AppendChild(node);
        }

        /// <summary>
        /// 1. 功能：新增节点。
        /// 2. 使用条件：将DataSet中的第一条记录插入Xml文件中。
        /// </summary>        
        /// <param name="ds">DataSet的实例，该DataSet中应该只有一条记录</param>
        public void AppendNode(DataSet ds)
        {
            XmlDataDocument xmlDataDocument = new XmlDataDocument(ds);
            XmlNode node = xmlDataDocument.DocumentElement.FirstChild;
            AppendNode(node);
        }
        #endregion

        #region 删除节点

        /// <summary>
        /// 删除指定XPath表达式的节点
        /// </summary>        
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public void RemoveNode(string xPath)
        {
            this.CreateXmlElement();
            XmlNode node = this.xml.SelectSingleNode(xPath);
            this.element.RemoveChild(node);
        }
        #endregion

        #region 保存XML文件
        public void Save()
        {
            this.CreateXmlElement();
            this.xml.Save(this.filePath);
        }
        #endregion

        #region 静态方法

        #region 创建根节点对象
        /// <summary>
        /// 创建根节点对象
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的路径</param>
        /// <returns></returns>
        private static XmlElement CreateRootElement(string xmlFilePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            return xmlDocument.DocumentElement;

        }

        #endregion

        #region 获取指定XPath表达式节点的值
        /// <summary>
        /// 获取指定XPath表达式节点的值
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的路径</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public static string GetValue(string xmlFilePath, string xPath)
        {
            XmlElement rootElement = CreateRootElement(xmlFilePath);
            return rootElement.SelectSingleNode(xPath).InnerText;
        }
        #endregion

        #region 获取指定XPath表达式节点的属性值
        /// <summary>
        /// 获取指定XPath表达式节点的属性值
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的路径</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <param name="attributeName">属性名</param>
        public static string GetAttributeValue(string xmlFilePath, string xPath, string attributeName)
        {
            XmlElement rootElement = CreateRootElement(xmlFilePath);
            return rootElement.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }
        #endregion

        #endregion
    }
}