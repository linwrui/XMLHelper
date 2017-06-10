using System;
using System.IO;
using System.Xml;

namespace XMLHelper.xml
{
    public class XmlOperator : IDisposable
    {
        FileInfo _fileInfo = null;
        XmlDocument _xmlDoc = new XmlDocument();
        private XmlNode _root = null;

        public XmlDocument XmlDoc => _xmlDoc;

        /// <summary>
        /// 从xml文档加载
        /// </summary>
        /// <param name="filePath"></param>
        public XmlOperator(string filePath)
        {
            try
            {
                _fileInfo = new FileInfo(filePath);
                _xmlDoc.Load(filePath);
                _root = _xmlDoc.DocumentElement;
            }
            catch (XmlException xmlEx)
            {
                throw xmlEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新建一个空的xml文档
        /// </summary>
        /// <param name="filePath"></param>
        public XmlOperator()
        {

        }

        /// <summary>
        /// 根节点名称
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="prefix"></param>
        /// <param name="nameSpace"></param>
        public void CreateRootElement(string name, string prefix = "", string nameSpace = "")
        {
            //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
            XmlDeclaration xmldecl;
            xmldecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
            _xmlDoc.AppendChild(xmldecl);
            //新增根节点（在Declaration节点下只能有一个子节点）
            var root = _xmlDoc.CreateElement(prefix, name, nameSpace);
            _xmlDoc.AppendChild(root);
            _root = root;
        }

        public void Load(string xml)
        {
            _xmlDoc.Load(xml);
            _root = _xmlDoc.DocumentElement;
        }

        /// <summary>
        /// 新增一个节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="attributes">节点所具有的属性和对应的值</param>
        public XmlNode AppendNode(string name, params Tuple<string, object>[] attributes)
        {
            XmlNode node = _xmlDoc.CreateElement(name);
            CreateAttributes(attributes, node);
            _root.AppendChild(node);
            return node;
        }

        /// <summary>
        /// 新增为已存在节点的子节点
        /// </summary>
        /// <param name="exitNodeName"></param>
        /// <param name="nodeName"></param>
        /// <param name="attributes"></param>
        public XmlNode AppendNodeToExitsNode(string exitNodeName, string nodeName, params Tuple<string, object>[] attributes)
        {
            XmlNode exitNode = _xmlDoc[exitNodeName];
            XmlNode node = _xmlDoc.CreateElement(nodeName);
            CreateAttributes(attributes, node);
            exitNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// 新增为已存在节点的子节点
        /// </summary>
        /// <param name="exitNodeName"></param>
        /// <param name="nodeName"></param>
        /// <param name="attributes"></param>
        public XmlNode AppendNodeToExitsNode(XmlNode exitNode, string nodeName, params Tuple<string, object>[] attributes)
        {
            XmlNode node = _xmlDoc.CreateElement(nodeName);
            CreateAttributes(attributes, node);
            exitNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// 新建属性
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="node"></param>
        private void CreateAttributes(Tuple<string, object>[] attributes, XmlNode node)
        {
            foreach (var attr in attributes)
            {
                var attribute = _xmlDoc.CreateAttribute(attr.Item1);
                attribute.Value = attr.Item2.ToString();
                node.Attributes.Append(attribute);
            }
        }



        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            if (_fileInfo == null)
            {
                throw new XmlException("缺少文件路径信息。");
            }
            _xmlDoc.Save(_fileInfo.FullName);
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="openFileAfterSaved"></param>
        public void SaveAs(string filePath)
        {
            _fileInfo = new FileInfo(filePath);
            if (!Directory.Exists(_fileInfo.Directory.FullName))
            {
                Directory.CreateDirectory(_fileInfo.Directory.FullName);
            }
            this.Save();
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name => _fileInfo.Name;

        public void Dispose()
        {
            _xmlDoc = null;
            _fileInfo = null;
            GC.SuppressFinalize(this);//释放资源
        }

        /// <summary>
        /// 获取name=nodeName的节点的值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public string GetXmlNodeValue(string nodeName)
        {
            //获取当前XML文档的根 一级
            XmlNode oNode = _xmlDoc.DocumentElement;
            //获取根节点的所有子节点列表
            XmlNodeList oList = oNode.ChildNodes;
            XmlNode node = GetXmlChild(oNode, nodeName);
            return node.Value;
            //if (node.HasChildNodes) return node.FirstChild.Value;
            //else return node.Value;
        }

        /// <summary>
        /// 获取指定节点中某一属性的的值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public string GetNodeAttributeValue(string nodeName,string attributeName)
        {
            //获取根节点的所有子节点列表
            XmlNode node = GetXmlChild(_root, nodeName);
            if (node == null) return null;
            return node.Attributes[attributeName].Value;
            //if (node.HasChildNodes) return node.FirstChild.Value;
            //else return node.Value;
        }

        /// <summary>
        /// 从指定节点下获取某一个子节点（或节点本身）
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="childName">Name of the child.</param>
        /// <returns>XmlNode.</returns>
        private XmlNode GetXmlChild(XmlNode node, string childName)
        {
            XmlNode result = node;
            if (node.Name.ToLower() == childName.ToLower()) return node;
            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    result = GetXmlChild(child, childName);//递归
                    if (result != null)
                        return result;
                }
            }
            return null;
        }


    }
}
