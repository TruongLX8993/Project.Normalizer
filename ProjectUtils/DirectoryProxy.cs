using System;
using System.IO;
using System.Linq;

namespace ProjectUtils
{
    public class DirectoryProxy
    {
        private readonly string _path;


        public DirectoryProxy(string path)
        {
            _path = path;
        }

        public bool IsContainFile(string fileName)
        {
            return Directory.GetFiles(_path, $"*{fileName}*", SearchOption.AllDirectories)
                .Length > 0;
        }

        public string GetPath(string fileName)
        {
            return Directory.GetFiles(_path, $"*{fileName}*", SearchOption.AllDirectories)
                .FirstOrDefault();
        }

        public string GetRelativePath(string currentFolder, string fileName)
        {
            var pathFile = GetPath(fileName);
            if (string.IsNullOrEmpty(pathFile))
            {
                throw new Exception($"Not found {fileName} in {_path} directory");
            }

            if (!currentFolder.EndsWith("\\"))
            {
                currentFolder += "\\";
            }

            return PathUtil.GetRelative(currentFolder, pathFile)
                .Replace('/', '\\');
        }
    }
}