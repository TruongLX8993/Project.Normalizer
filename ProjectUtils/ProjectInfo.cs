using System;
using System.Xml;

namespace ProjectUtils
{
    public class ProjectInfo
    {
        public string Guid { get; }
        public string Name { get; }
        public string AbsPath { get; }

        public ProjectInfo(string guid, string name, string absPath)
        {
            Guid = guid;
            Name = name;
            AbsPath = absPath;
        }

        public void InsertProjectRefToProject(XmlDocument xmlDocument, string currentProjectDirPath)
        {
            if (!currentProjectDirPath.EndsWith("\\"))
                currentProjectDirPath += "\\";
            var projectRefNode = xmlDocument.CreateElement("ProjectReference");
            // attribute
            var includeAttr = xmlDocument.CreateAttribute("Include");
            includeAttr.InnerText = PathUtil.GetRelative(currentProjectDirPath, AbsPath);
            projectRefNode.Attributes.Append(includeAttr);
            // project node
            var projectNode = xmlDocument.CreateElement("Project");
            projectNode.InnerText = "{" + Guid + "}";
            projectRefNode.AppendChild(projectNode);
            // name node
            var nameNode = xmlDocument.CreateElement("Name");
            nameNode.InnerText = Name;
            projectRefNode.AppendChild(nameNode);
            // append to item group
            var projectRefItemGroup = GetOrCreateProjectRefItemGroup(xmlDocument);
            projectRefItemGroup.AppendChild(projectRefNode);
        }

        private static XmlNode GetOrCreateProjectRefItemGroup(XmlDocument xmlDocument)
        {
            try
            {
                return xmlDocument.GetElementsByTagName("ProjectReference")[0]
                    .ParentNode;
            }
            catch (Exception e)
            {
                var res = xmlDocument.CreateElement("ItemGroup", xmlDocument.DocumentElement.NamespaceURI);
                xmlDocument.GetElementsByTagName("Project")[0]
                    .AppendChild(res);
                return res;
            }
        }
    }
}