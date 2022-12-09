using System;

namespace LSlicer.BL.Interaction.Types
{
    public class SavingWorkStateSpec
    {
        public static SavingWorkStateSpec Create(bool autoSaveEnable, string defaultPath, Func<string, string> pathSelector) 
        {
            if (pathSelector == null || String.IsNullOrEmpty(defaultPath))
                throw new ArgumentNullException(nameof(pathSelector) + " " + nameof(defaultPath));
            return new SavingWorkStateSpec() { AutoSaveEnable = autoSaveEnable, DefaultPath = defaultPath, PathSelector = pathSelector };
        }

        public bool AutoSaveEnable { get; private set; }
        public string DefaultPath { get; private set; }
        public Func<string, string> PathSelector { get; private set; }
    }
}
