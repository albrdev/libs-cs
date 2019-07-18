using System;
using System.IO;

namespace Libs.Utilities
{
    public static class PathUtilities
    {
        public static string FullPath(params string[] subPaths)
        {
            string result = string.Empty;
            foreach(var subPath in subPaths)
            {
                result = Path.Combine(result, subPath);
            }

            return result;
        }
    }
}
