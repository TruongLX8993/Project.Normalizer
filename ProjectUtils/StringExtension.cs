using System;

namespace ProjectUtils
{
    public static class StringExtension
    {
        public static bool ContainIgnoreCase(this string source, string des)
        {
            source = source.ToLower();
            des = des.ToLower();
            return source.Contains(des);
        }
    }
}