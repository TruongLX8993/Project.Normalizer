using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ProjectUtils
{
    public class ProjectBuildEventNormalizer
    {
        private readonly string _currentDir;
        private readonly XmlDocument _xmlDocument;
        private readonly IDictionary<string, string> _keyPath;


        public ProjectBuildEventNormalizer(
            string currentDir,
            XmlDocument xmlDocument,
            IDictionary<string, string> keyPath)
        {
            _keyPath = keyPath.ToDictionary(kp => kp.Key.ToLower(), kp => kp.Value);
            _currentDir = currentDir;
            _xmlDocument = xmlDocument;
        }

        public XmlDocument Normalize()
        {
            var buildEvents = _xmlDocument.GetElementsByTagName("PostBuildEvent");
            if (buildEvents.Count == 0)
            {
                return _xmlDocument;
            }

            var buildEvent = buildEvents[0];
            var newBuildEvent = GetNewBuildEvent(buildEvent.InnerText);
            buildEvent.InnerText = newBuildEvent;
            return _xmlDocument;
        }

        private string GetNewBuildEvent(string originBuildEvent)
        {
            originBuildEvent = originBuildEvent.Replace("\n", "")
                .Replace("\r", "");

            const string regexPattern = @"copy\s*\$\(TargetPath\)\s";
            const string removeWhiteSpaceRegex = @"\s+";
            originBuildEvent = Regex.Replace(originBuildEvent, regexPattern, " ");
            originBuildEvent = Regex.Replace(originBuildEvent, removeWhiteSpaceRegex, " ");

            var originOutputDirs = originBuildEvent.Split(' ');
            var newOutPutDirs = new List<string>();
            foreach (var originOutputDir in originOutputDirs)
            {
                // One origin may be have more one new output dir.
                var outPutDirsFromOneOriginOutputDir = GetNewOutPutDir(originOutputDir);
                if (outPutDirsFromOneOriginOutputDir == null)
                {
                    continue;
                }

                foreach (var outPutDir in outPutDirsFromOneOriginOutputDir)
                {
                    if (!newOutPutDirs.Contains(outPutDir))
                    {
                        newOutPutDirs.Add(outPutDir);
                    }
                }
            }

            var sb = new StringBuilder();
            foreach (var desDir in newOutPutDirs)
            {
                sb.Append($@"copy $(TargetPath) {desDir} ");
                sb.Append("\n");
            }

            return sb.ToString()
                .TrimEnd();
        }

        private IEnumerable<string> GetNewOutPutDir(string originOutputDir)
        {
            if (string.IsNullOrEmpty(originOutputDir))
            {
                return null;
            }

            var desDirParts = originOutputDir.Split('\\');
            var newJoinedAbsPath = "";
            foreach (var desDirPart in desDirParts)
            {
                var key = desDirPart.ToLower();
                if (!_keyPath.ContainsKey(key)) continue;
                newJoinedAbsPath = _keyPath[key];
                break;
            }

            if (string.IsNullOrEmpty(newJoinedAbsPath))
            {
                return null;
            }

            var newAbsPath = newJoinedAbsPath.Split(';');
            return newAbsPath.Select(absPath => string.IsNullOrEmpty(absPath)
                    ? null
                    : PathUtil.GetRelative(GetSlnDirPath(), absPath))
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => "$(SolutionDir)" + s)
                .ToList();
        }

        private string GetSlnDirPath()
        {
            return Directory.GetParent(_currentDir)
                .FullName;
        }
    }
}