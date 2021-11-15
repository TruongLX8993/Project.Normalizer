using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ProjectUtils
{
    public class ProjectRefNormalizerContext
    {
        internal ProjectRefNormalizerContext(
            XmlDocument csProjDocument,
            string csProjectPath)
        {
            CsProjectDocument = csProjDocument;
            CsProjectPath = csProjectPath;
        }

        public string CsProjectPath { get; }
        public XmlDocument CsProjectDocument { get; }

        public string GetProjectFileName()
        {
            return Path.GetFileNameWithoutExtension(CsProjectPath);
        }

        public string ProjectDir()
        {
            return Path.GetDirectoryName(CsProjectPath);
        }

        public bool IsProjectCurrent(string name)
        {
            return CsProjectPath.ContainIgnoreCase(name);
        }
    }

    public class ProjectReferenceNormalizer
    {
        private readonly XmlDocument _csProjXmlDoc;
        private readonly string _projectDirPath;
        private readonly string _internalSystemPrefix;
        private readonly DirectoryProxy _libDirectoryProxy;
        private readonly DirectoryProxy _coreDllProxy;
        private readonly DirectoryProxy _slnCoreDllProxy;
        private readonly SlnReader _slnReader;
        private readonly ProjectRefNormalizerContext _projectRefNormalizerContext;

        private readonly ProjectDllCustomizer _projectDllCustomizer;
        public ProjectRefCustomizer ProjectRefCustomizer { get; set; }

        public ProjectReferenceNormalizer(
            XmlDocument csProjXmlDoc,
            string csProjectFilePath,
            string internalSystemPrefix,
            DirectoryProxy libDirectoryProxy,
            DirectoryProxy coreDllProxy,
            SlnReader slnReader,
            ProjectDllCustomizer projectDllCustomizer)
        {
            _csProjXmlDoc = csProjXmlDoc;
            _projectDirPath = Path.GetDirectoryName(csProjectFilePath);
            _internalSystemPrefix = internalSystemPrefix?.ToLower();
            _libDirectoryProxy = libDirectoryProxy;
            _coreDllProxy = coreDllProxy;
            _slnReader = slnReader;
            _projectDllCustomizer = projectDllCustomizer;
            _projectRefNormalizerContext = new ProjectRefNormalizerContext(csProjXmlDoc, csProjectFilePath);
            _slnCoreDllProxy = new DirectoryProxy(Directory.GetParent(_projectDirPath) + @"\CoreDlls\");
        }

        public XmlDocument NormalizeReference()
        {
            var refNodes = _csProjXmlDoc.SelectNodes(
                $"//{XPathConstance.MsBuildNameSpacePrefix}:ItemGroup/{XPathConstance.MsBuildNameSpacePrefix}:Reference",
                XPathConstance.MsBuildNameSpace);
            if (refNodes == null)
            {
                return _csProjXmlDoc;
            }

            for (var i = 0; i < refNodes.Count; i++)
            {
                var refNode = refNodes[i];
                UpdateRefNode(refNode);
            }

            UpdateProjRefNode();

            return _csProjXmlDoc;
        }

        private void UpdateProjRefNode()
        {
            var refNodes = _csProjXmlDoc.SelectNodes(
                $"//{XPathConstance.MsBuildNameSpacePrefix}:ItemGroup/{XPathConstance.MsBuildNameSpacePrefix}:ProjectReference",
                XPathConstance.MsBuildNameSpace);
            if (refNodes == null)
            {
                return;
            }

            for (var i = 0; i < refNodes.Count; i++)
            {
                var refNode = refNodes[i];
                var nameProjNode = refNode.SelectSingleNode($"{XPathConstance.MsBuildNameSpacePrefix}:Name",
                    XPathConstance.MsBuildNameSpace);
                if (nameProjNode == null)
                    return;
                var action =
                    ProjectRefCustomizer?.Normalize(_projectRefNormalizerContext, refNode, nameProjNode.InnerText);
                if (action == CustomizerAction.REMOVE)
                {
                    refNode.ParentNode.RemoveChild(refNode);
                }
                else if (action == CustomizerAction.GET_FROM_CORE)
                {
                    MoveProjectRefToDllRef(refNode, nameProjNode.InnerText);
                }
            }
        }


        private void UpdateRefNode(XmlNode refNode)
        {
            var hintNode = refNode.SelectSingleNode($"{XPathConstance.MsBuildNameSpacePrefix}:HintPath",
                XPathConstance.MsBuildNameSpace);
            if (hintNode == null)
                return;
            var hintPath = hintNode.InnerText;
            var fileName = PathUtil.GetFileName(hintPath);
            if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception($"File name is null.Ref node:{refNode.InnerXml}");
            }


            if (_projectDllCustomizer != null)
            {
                var customStatus =
                    _projectDllCustomizer.NormalizeDllRef(_projectRefNormalizerContext, refNode, hintNode);
                switch (customStatus)
                {
                    case CustomizerAction.NO_ACTION:
                        return;
                    case CustomizerAction.GET_FROM_LIB:
                        UpdateRefFromLib(fileName, hintNode);
                        return;
                    case CustomizerAction.GET_FROM_CORE:
                        UpdateRefFromCoreDll(fileName, hintNode);
                        return;
                    case CustomizerAction.GET_FROM_CORE_SLN:
                        UpdateRefFromCoreSln(fileName, hintNode);
                        return;
                    case CustomizerAction.APPENDED:
                        return;
                    case CustomizerAction.NORMAL:
                        break;
                    case CustomizerAction.REMOVE:
                        RemoveRef(refNode);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var isInternalSystemDll = IsInternalSystem(fileName);
            if (isInternalSystemDll)
            {
                UpdateInternalRefPath(fileName, hintNode, refNode);
                return;
            }

            if (!IsRefPackage(hintNode) && _libDirectoryProxy.IsContainFile(fileName))
            {
                UpdateRefFromLib(fileName, hintNode);
            }
        }

        private void RemoveRef(XmlNode refNode)
        {
            refNode.ParentNode.RemoveChild(refNode);
        }


        private static bool IsRefPackage(XmlNode hintNode)
        {
            return hintNode.InnerXml.Contains("packages");
        }

        private void UpdateRefFromLib(string fileName, XmlNode hintNode)
        {
            var newPath = _libDirectoryProxy.GetRelativePath(_projectDirPath, fileName);
            hintNode.InnerText = newPath;
        }

        private void UpdateRefFromCoreSln(string fileName, XmlNode hintNode)
        {
            var newPath = _slnCoreDllProxy.GetRelativePath(_projectDirPath, fileName);
            hintNode.InnerText = newPath;
        }

        private void UpdateInternalRefPath(
            string fileName,
            XmlNode hintNode,
            XmlNode refElement)
        {
            // Filename is usually dll file.
            var isBelongSln = _slnReader.ContainProject(fileName);
            if (!isBelongSln)
            {
                UpdateRefFromCoreDll(fileName, hintNode);
            }
            else
            {
                UpdateRefFromInternalProject(fileName, refElement);
                // UpdateRefFromCoreSln(fileName, hintNode);
            }
        }

        private void UpdateRefFromInternalProject(string fileName, XmlNode refElement)
        {
            refElement.ParentNode?.RemoveChild(refElement);
            UpdateRefProjectNode(fileName);
        }

        private void MoveProjectRefToDllRef(XmlNode projectRefNode, string fileName)
        {
            Console.WriteLine($"{nameof(MoveProjectRefToDllRef)}:{fileName}");
            var projectInfo = _slnReader.GetProjectInfo(fileName);
            var xmlRef = _csProjXmlDoc.CreateElement("Reference");
            var attr = _csProjXmlDoc.CreateAttribute("Include");
            attr.InnerText = projectInfo.Name;
            xmlRef.Attributes.Append(attr);
            
            var hintNode = _csProjXmlDoc.CreateElement("HintPath");
            hintNode.InnerText = _coreDllProxy.GetRelativePath(_projectDirPath, fileName);
            xmlRef.AppendChild(hintNode);

            _csProjXmlDoc.GetElementsByTagName("Reference")[0]
                .ParentNode.AppendChild(xmlRef);

            projectRefNode.ParentNode.RemoveChild(projectRefNode);
        }

        private void UpdateRefFromCoreDll(string fileName, XmlNode hintNode)
        {
            hintNode.InnerText = _coreDllProxy.GetRelativePath(_projectDirPath, fileName);
        }

        private void UpdateRefProjectNode(string fileName)
        {
            var projInfo = _slnReader.GetProjectInfo(fileName);
            projInfo.InsertProjectRefToProject(_csProjXmlDoc, _projectDirPath);
        }


        private bool IsInternalSystem(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) &&
                   fileName.ToLower()
                       .Contains(_internalSystemPrefix);
        }
    }
}