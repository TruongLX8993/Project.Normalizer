using System.Xml;

namespace ProjectUtils
{
    public class XPathConstance
    {
        private static XmlNamespaceManager _xmlNamespaceManager;

        public const string MsBuildNameSpacePrefix = "msbuild";
        public static XmlNamespaceManager MsBuildNameSpace => _xmlNamespaceManager ?? CreateNewMsBuildNameSpace();

        private static XmlNamespaceManager CreateNewMsBuildNameSpace()
        {
            var xmlDoc = new XmlDocument();
            var xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            xmlNamespaceManager.AddNamespace(MsBuildNameSpacePrefix,
                "http://schemas.microsoft.com/developer/msbuild/2003");
            return xmlNamespaceManager;
        }
    }
}