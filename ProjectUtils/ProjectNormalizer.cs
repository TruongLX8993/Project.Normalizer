using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ProjectUtils
{
    public class ProjectNormalizer
    {
        public ProjectNormalizer(
            string csProjPath,
            DirectoryProxy libDirectoryProxy,
            DirectoryProxy coreDllProxy,
            string internalSystemPrefix,
            SlnReader slnReader,
            ProjectDllCustomizer customizer)
        {
            _csProjectPath = csProjPath;
            _libDirectoryProxy = libDirectoryProxy;
            _coreDllProxy = coreDllProxy;
            _internalSystemPrefix = internalSystemPrefix;
            _slnReader = slnReader;
            _customizer = customizer;

            _csProjXmlDoc = new XmlDocument();
            _csProjXmlDoc.Load(csProjPath);
            _projectDirPath = Path.GetDirectoryName(csProjPath);
            _projectDirPath = PathUtil.NormalizeDirPath(_projectDirPath);
        }


        private readonly XmlDocument _csProjXmlDoc;
        private readonly DirectoryProxy _libDirectoryProxy;
        private readonly DirectoryProxy _coreDllProxy;
        private readonly string _projectDirPath;
        private readonly string _csProjectPath;
        private readonly string _internalSystemPrefix;
        private readonly SlnReader _slnReader;
        private readonly ProjectDllCustomizer _customizer;
        public ProjectRefCustomizer ProjectRefCustomizer;

        public ProjectNormalizer(
            string csProjPath,
            string coreDllPath,
            string libDllDirPath,
            string internalSystemPrefix,
            string slnPath,
            ProjectDllCustomizer customizer)
        {
            _csProjectPath = csProjPath;
            _projectDirPath = Path.GetDirectoryName(csProjPath);
            _projectDirPath = PathUtil.NormalizeDirPath(_projectDirPath);
            _csProjXmlDoc = new XmlDocument();
            _csProjXmlDoc.Load(csProjPath);
            _libDirectoryProxy = new DirectoryProxy(libDllDirPath);
            _coreDllProxy = new DirectoryProxy(coreDllPath);
            _internalSystemPrefix = internalSystemPrefix;
            _customizer = customizer;
            _slnReader = new SlnReader(slnPath);
        }

        public ProjectNormalizer NormalizeRef()
        {
            var projNormalize = new ProjectReferenceNormalizer(_csProjXmlDoc,
                _csProjectPath,
                _internalSystemPrefix,
                _libDirectoryProxy,
                _coreDllProxy,
                _slnReader,
                _customizer);
            projNormalize.ProjectRefCustomizer = ProjectRefCustomizer;
            projNormalize.NormalizeReference();
            return this;
        }


        public ProjectNormalizer NormalizeBuildEvent(IDictionary<string, string> keyPath)
        {
            var buildEventNormalizer = new ProjectBuildEventNormalizer(_projectDirPath,
                _csProjXmlDoc,
                keyPath);
            buildEventNormalizer.Normalize();
            return this;
        }


        public XmlDocument GetXmlDoc()
        {
            return _csProjXmlDoc;
        }

        public void Save(string outPath = null)
        {
            outPath = outPath ?? _csProjectPath;
            var xmlContent = _csProjXmlDoc.OuterXml;
            xmlContent = xmlContent.Replace("xmlns=\"\"", string.Empty);
            File.WriteAllText(outPath, xmlContent);
        }

        public string GetXml()
        {
            return _csProjXmlDoc.OuterXml;
        }
    }
}