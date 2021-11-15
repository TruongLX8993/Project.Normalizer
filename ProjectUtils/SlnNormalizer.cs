using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectUtils
{
    public class SlnNormalizer
    {
        private readonly SlnReader _slnReader;
        private readonly DirectoryProxy _coreDirProxy;
        private readonly DirectoryProxy _libDirProxy;
        private readonly string _sysPrefix;
        private readonly IDictionary<string, string> _outMap;
        private readonly ProjectDllCustomizer _customizer;
        public ProjectRefCustomizer ProjectRefCustomizer;

        public SlnNormalizer(
            string slnPath,
            string coreDllPath,
            string libDllPath,
            string sysPrefix,
            IDictionary<string, string> outMap, ProjectDllCustomizer customizer)
        {
            _slnReader = new SlnReader(slnPath);
            _coreDirProxy = new DirectoryProxy(coreDllPath);
            _libDirProxy = new DirectoryProxy(libDllPath);
            _sysPrefix = sysPrefix;
            _outMap = outMap;
            _customizer = customizer;
        }


        public void Normalize()
        {
            var csProjPaths = _slnReader.GetCsProjPaths();
            foreach (var csProjPath in csProjPaths)
            {
                Console.WriteLine(csProjPath);
                string outPath = null;
                // outPath = $@"E:\out\{Path.GetFileNameWithoutExtension(csProjPath)}.xml";
                var newProjectNormalize = new ProjectNormalizer(csProjPath,
                    _libDirProxy,
                    _coreDirProxy,
                    _sysPrefix,
                    _slnReader,
                    _customizer);
                newProjectNormalize.ProjectRefCustomizer = ProjectRefCustomizer;
                newProjectNormalize.NormalizeBuildEvent(_outMap)
                    .NormalizeRef()
                    .Save(outPath);
            }
        }
    }
}