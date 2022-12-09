using System;

namespace LSlicer.Helpers
{
    public static class PathHelper
    {
        public static string Resolve(string rawPath)
        {
            string result;
            if (rawPath.ToLower().Contains("{mydocuments}"))
            {
                int firstSymbol = rawPath.IndexOf("{mydocuments}") + "{mydocuments}".Length + 1;
                result = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{rawPath.Substring(firstSymbol)}";
            }
            else if (rawPath.ToLower().Contains("{appdata}"))
            {
                int firstSymbol = rawPath.IndexOf("{appdata}") + "{appdata}".Length + 1;
                result = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{rawPath.Substring(firstSymbol)}";
            }
            else
            {
                result = rawPath;
            }
            return result;
        }
    }
}
