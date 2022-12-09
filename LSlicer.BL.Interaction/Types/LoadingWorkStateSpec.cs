using SharpDX.Direct3D11;
using System;
using System.IO;

namespace LSlicer.BL.Interaction.Types
{
    public class LoadingWorkStateSpec
    {
        public FileInfo LoadingFile { get; private set; }
        public Func<string, string> LoadPathSelector { get; private set; }
        public SavingWorkStateSpec SavingWorkStateSpec { get; private set; }

        public static LoadingWorkStateSpec Create(FileInfo loadingFile, Func<string, string> loadPathSelector, SavingWorkStateSpec savingCurrentStateSpec) 
        {
            if (loadingFile == null || !loadingFile.Exists)
                throw new ArgumentException(nameof(loadingFile));
            if (loadPathSelector == null)
                throw new ArgumentException(nameof(loadPathSelector));
            return new LoadingWorkStateSpec() { LoadingFile = loadingFile, LoadPathSelector = loadPathSelector, SavingWorkStateSpec = savingCurrentStateSpec };
        }
    }
}
