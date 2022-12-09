using System;
using System.IO;
using System.Linq;

namespace LSlicer.Helpers
{
    public static class FileNameResolver
    {
        private const string _defaultSliceFormat = ".cli";

        public static string ResolveSupport(string meshFileName, int number = 0) =>
            GetFilePathWithoutExtention(meshFileName, number) + "_s" + Path.GetExtension(meshFileName);

        public static string ResolveSliced(string meshFileName, int number = 0) => 
            GetFilePathWithoutExtention(meshFileName, number) + _defaultSliceFormat;

        private static string GetFilePathWithoutExtention(string meshFileName, int number) =>
            Path.Combine(Path.GetDirectoryName(meshFileName), Path.GetFileNameWithoutExtension(meshFileName), (number == 0 ? "" : "_" + number.ToString()));

        public static bool IsSupport(string fileName) => fileName.Contains("_s.") || fileName.Contains("_s_");

        public static string UniqJobSpecNameGenerate(string jobName, string extention) => 
            string.Concat(jobName, "_spec_", DateTime.Now.ToString("HHmmss"), extention);

        public static string AddSuffix(string fileName, string suffix) =>
            Path.Combine(Path.GetDirectoryName(fileName), string.Concat(Path.GetFileNameWithoutExtension(fileName), suffix, Path.GetExtension(fileName)));

        public static string ExtractSuffix(string fileName) =>
            Path.GetFileNameWithoutExtension(fileName).Split('_').Last();
    }
}
