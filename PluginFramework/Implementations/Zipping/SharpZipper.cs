using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using PluginFramework.Logging;
using System.IO;

namespace PluginFramework.CustomPlugin.Zipping
{
    public class SharpZipper : IZipper
    {
        private readonly IZipFileFilter _fileFilter;
        private readonly ILogger _logger;

        public SharpZipper(IZipFileFilter fileFilter, ILogger logger)
        {
            _fileFilter = fileFilter;
            _logger = logger;
        }

        public void ExtractZipFile(string archivePath, string password, string outFolder)
        {
            using (FileStream fsInput = File.OpenRead(archivePath))
            using (ZipFile zippedFile = new ZipFile(fsInput))
            {
                if (!string.IsNullOrEmpty(password))
                    zippedFile.Password = password;

                foreach (ZipEntry zipEntry in zippedFile)
                {
                    if (!_fileFilter.Filter(zipEntry.Name))
                    {
                        _logger.Info($"File {zipEntry.Name} is plugin framework file, so it wasn't extracted");
                        continue; 
                    }

                    if (!zipEntry.IsFile)
                        continue;

                    string entryFileName = zipEntry.Name;

                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    if (File.Exists(fullZipToPath))
                        continue;

                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    byte[] buffer = new byte[4096];

                    using (var zipStream = zippedFile.GetInputStream(zipEntry))
                    using (Stream fsOutput = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, fsOutput, buffer);
                    }
                }
            }
        }

        public void ZipFolder(string outPathname, string password, string folderName)
        {
            using (FileStream fsOut = File.Create(outPathname))
            using (var zipStream = new ZipOutputStream(fsOut))
            {
                zipStream.SetLevel(3);

                zipStream.Password = password;

                int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                CompressFolder(folderName, zipStream, folderOffset);
            }
        }

        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string fileName in files)
            {
                if (!_fileFilter.Filter(fileName))
                {
                    _logger.Info($"File {fileName} is plugin framework file, so it was excluded");
                    continue;
                }

                FileInfo fi = new FileInfo(fileName);
                string entryName = fileName.Substring(folderOffset);
                entryName = ZipEntry.CleanName(entryName);

                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime;

                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream fsInput = File.OpenRead(fileName))
                {
                    StreamUtils.Copy(fsInput, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }
    }
}
