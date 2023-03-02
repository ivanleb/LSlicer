using System;

namespace LSlicer.Helpers
{
    internal class DefaultAppSettings : BL.Interaction.IAppSettings
    {
        public string SlicingEnginePath { get; set; } = "";
        public string SupportEnginePath { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";
        public string SlicingParametersRepoPath { get; set; } = "";
        public string SupportParametersRepoPath { get; set; } = "";
        public string DefaultSlicingParameters { get; set; } = "";
        public string SlicingResultDirectory { get; set; } = "";
        public string DefaultSupportParameters { get; set; } = "";
        public string SupportEngineList { get; set; } = "";
        public string SelectedSupportEngine { get; set; } = "";
        public string SliceEngineList { get; set; } = "";
        public string SelectedSliceEngine { get; set; } = "";
        public string CurrentChangesPath { get; set; } = "";
        public TimeSpan AutoSaveInterval { get; set; } = TimeSpan.FromSeconds(10000);
        public TimeSpan WaitingUserActionTimeout { get; set; } = TimeSpan.FromSeconds(10000);

        public void SetForUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}
