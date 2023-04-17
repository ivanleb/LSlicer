using ICSharpCode.SharpZipLib.Zip;
using LSlicer.BL.Interaction;
using PluginFramework.Core;
using PluginFramework.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ZipPartsPlugin
{
    internal class ZipPlugin : PluginBase<IPartService, ILogger>, IPlugin
    {
        private ILogger _logger;
        public ZipPlugin(string name) : base(name)
        {
        }

        public override void DoAction(PluginActionSpec spec)
        {
            spec.Progress.Report("Start zipping");

            if (_logger == null)
                _logger = ServiceContainerItem2.Value;

            IPartService partService = ServiceContainerItem1.Value;

            List<string> paths = partService.Parts
                                    .Select(part => part.PartSpec.MeshFilePath)
                                    .ToList();

            Zip(spec, paths);

            spec.Progress.Report("End zipping");
            paths.ForEach(path => _logger.Info($"[{nameof(ZipPlugin)}] Zipping {path}"));
        }

        private void Zip(PluginActionSpec spec, List<string> paths)
        {
            try
            {
                if (paths.Count == 0)
                    throw new ArgumentException("No parts for saving");

                DirectoryInfo outputDectory = new DirectoryInfo(spec.ResultPath);
                if (!outputDectory.Exists)
                    throw new ArgumentException("Output directory does not exists");

                string fullFileName = $"{outputDectory.FullName}\\{outputDectory.Name}.zip";

                using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(fullFileName)))
                {
                    int compressionLevel = 4;
                    zipStream.SetLevel(compressionLevel);

                    byte[] buffer = new byte[4096];
                    foreach (string partPath in paths)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(partPath));
                        entry.DateTime = DateTime.Now;
                        zipStream.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(partPath))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                zipStream.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                        spec.Progress.Report($"Zip \"{partPath}\" into \"{fullFileName}\"");
                    }

                    zipStream.Finish();
                    zipStream.Close();
                }

            }
            catch (Exception e)
            {
                spec.Progress.Report($"[{Name}] Error: {e.Message}");
                _logger.Error($"[{nameof(ZipPlugin)}] {e.Message}");
            }
        }
    }
}
