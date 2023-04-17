namespace PluginFramework.CustomPlugin.Zipping
{
    public interface IZipper
    {
        void ZipFolder(string outPathname, string password, string folderName);
        void ExtractZipFile(string archivePath, string password, string outFolder);
    }
}
