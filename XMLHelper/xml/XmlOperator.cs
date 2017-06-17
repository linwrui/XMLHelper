//*Author:linwrui
//*DateTime:2017-06-10

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

        public XmlNode RootNode => _root;

        public string DefaultNamespaceURI => RootNode.NamespaceURI;

        public string DefaultPrefix => RootNode.Prefix;

        #region 初始化
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

        public void LoadFromString(string xml)
        {
            _xmlDoc.LoadXml(xml);
            _root = _xmlDoc.DocumentElement;
        }

        public void LoadFromFile(string filePath)
        {
            _xmlDoc.Load(filePath);
            _root = _xmlDoc.DocumentElement;
            _fileInfo = new FileInfo(filePath);
        }
        #endregion

        public XmlNode CreateNode(string name, bool useDefaultNamespace=false)
        {
            XmlNode node = _xmlDoc.CreateElement(useDefaultNamespace?DefaultPrefix:null, name, useDefaultNamespace ? DefaultNamespaceURI : "");
            return node;
        }

        public XmlNode CreateNode(string name, string prefix , string namespaceURI = "", XmlNodeType nodeType = XmlNodeType.Element)
        {
            XmlNode node = _xmlDoc.CreateNode(nodeType, prefix, name, namespaceURI);
            return node;
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
        public void SaveAs(string filePath, bool openFileAfterSaved = false)
        {
            _fileInfo = new FileInfo(filePath);
            if (!Directory.Exists(_fileInfo.Directory.FullName))
            {
                Directory.CreateDirectory(_fileInfo.Directory.FullName);
            }
            this.Save();
            if (openFileAfterSaved)
            {
                System.Diagnostics.Process.Start(filePath);
            }
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

        public XmlNode GetXmlNode(Predicate<XmlNode> match)
        {
            return GetNodeByPredicate(_root, match);
        }


        /// <summary>
        /// 从指定节点下获取某一个子节点（或节点本身）
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="childName">Name of the child.</param>
        /// <returns>XmlNode.</returns>
        private XmlNode GetNodeByPredicate(XmlNode node, Predicate<XmlNode> match)
        {
            XmlNode result = node;
            if (match.Invoke(node)) return node;
            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    result = GetNodeByPredicate(child, match);//递归
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return XmlDoc.OuterXml;
        }

    }
}
