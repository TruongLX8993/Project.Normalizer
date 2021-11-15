using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProjectUtils
{
    public class SlnReader
    {
        private readonly string _slnPath;
        private IList<string> _csProjPaths;
        private IDictionary<string, string> _csProjPathMap;

        public SlnReader(string slnPath)
        {
            _slnPath = slnPath;
            Init();
        }

        private void Init()
        {
            if (_csProjPaths != null)
            {
                return;
            }

            var slnDir = Path.GetDirectoryName(_slnPath);
            var projectDirs = System.IO.Directory.GetDirectories(slnDir, "*", System.IO.SearchOption.TopDirectoryOnly);
            _csProjPaths = projectDirs.Select(proDir =>
                {
                    var dir = new DirectoryInfo(proDir);
                    return dir.GetFiles("*.csproj")
                        .Select(fileInfo => fileInfo.FullName)
                        .FirstOrDefault();
                })
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            _csProjPathMap = new Dictionary<string, string>();
            foreach (var csProjPath in _csProjPaths)
            {
                _csProjPathMap.Add(Path.GetFileName(csProjPath)
                    .ToLower(), csProjPath);
            }
        }

        public string SlnPrefix()
        {
            var fileName = Path.GetFileNameWithoutExtension(_slnPath);
            return fileName;
        }


        public IList<string> GetCsProjPaths()
        {
            return _csProjPaths;
        }

        public ProjectInfo GetProjectInfo(string fileName)
        {
            fileName = MapToProjectFileNameAndToLower(fileName);
            var path = _csProjPathMap.ContainsKey(fileName) ? _csProjPathMap[fileName] : null;
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception($"Not found {fileName}");
            }

            const string guidRegexPattern = @"<ProjectGuid>{(.*)}<\/ProjectGuid>";
            var fileContent = File.ReadAllText(path);

            var regex = new Regex(guidRegexPattern);
            var match = regex.Match(fileContent);
            if (!match.Success)
            {
                throw new Exception("can not read guid project");
            }

            var guidProject = match.Groups[1]
                .Value;

            const string rootNamespaceRegex = @"<RootNamespace>(.*)<\/RootNamespace>";
            regex = new Regex(rootNamespaceRegex);
            match = regex.Match(File.ReadAllText(path));
            if (!match.Success)
            {
                throw new Exception("can not read root project");
            }

            var rootName = match.Groups[1]
                .Value;

            return new ProjectInfo(guidProject, rootName, path);
        }

        public bool ContainProject(string fileName)
        {
            fileName = MapToProjectFileNameAndToLower(fileName);
            return _csProjPathMap.ContainsKey(fileName);
        }

        private static string MapToProjectFileNameAndToLower(string fileName)
        {
            fileName = fileName.Replace("dll", "csproj");
            if (!fileName.EndsWith(".csproj"))
            {
                fileName += ".csproj";
            }

            fileName = fileName.ToLower();
            return fileName;
        }
    }
}