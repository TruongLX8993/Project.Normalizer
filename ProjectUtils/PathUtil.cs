using System;
using System.Linq;

namespace ProjectUtils
{
    public static class PathUtil
    {
        public static string GetRelative(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                throw new ArgumentException("From is not null");
            }

            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException("To is not null");
            }

            var fromUri = new Uri(from);
            var toUri = new Uri(to);
            var diff = fromUri.MakeRelativeUri(toUri);
            return diff.OriginalString?.Replace("/", "\\");
        }

        public static string NormalizeDirPath(string src)
        {
            if (!src.EndsWith(@"\"))
            {
                src += @"\";
            }

            return src;
        }
        public static string GetFileName(string hintPath)
        {
            return hintPath.Split('\\')
                .LastOrDefault();
        }

    }
}