using System;

namespace LSlicer.BL.Interaction
{
    public interface IAppSettings
    {
        string SlicingEnginePath { get; set; }
        string SupportEnginePath { get; set; }
        string WorkingDirectory { get; set; }
        string SlicingParametersRepoPath { get; set; }
        string SupportParametersRepoPath { get; set; }
        string DefaultSlicingParameters { get; set; }
        string SlicingResultDirectory { get; set; }
        string DefaultSupportParameters { get; set; }
        string SupportEngineList { get; set; }
        string SelectedSupportEngine { get; set; }
        string SliceEngineList { get; set; }
        string SelectedSliceEngine { get; set; }
        string CurrentChangesPath { get; set; }
        TimeSpan AutoSaveInterval { get; set; }
        TimeSpan WaitingUserActionTimeout { get; set; }
        void SetForUser(int id);
    }

    public static class AppSettingsExtention
    {
        public static void CopyFrom(this IAppSettings settings, IAppSettings source)
        {
            settings.SlicingEnginePath = source.SlicingEnginePath;
            settings.SupportEnginePath = source.SupportEnginePath;
            settings.WorkingDirectory = source.WorkingDirectory;
            settings.SlicingParametersRepoPath = source.SlicingParametersRepoPath;
            settings.SupportParametersRepoPath = source.SupportParametersRepoPath;
            settings.DefaultSlicingParameters = source.DefaultSlicingParameters;
            settings.SlicingResultDirectory = source.SlicingResultDirectory;
            settings.DefaultSupportParameters = source.DefaultSupportParameters;
            settings.SupportEngineList = source.SupportEngineList;
            settings.SelectedSupportEngine = source.SelectedSupportEngine;
            settings.SliceEngineList = source.SliceEngineList;
            settings.SelectedSliceEngine = source.SelectedSliceEngine;
            settings.CurrentChangesPath = source.CurrentChangesPath;
            settings.AutoSaveInterval = source.AutoSaveInterval;
            settings.WaitingUserActionTimeout = source.WaitingUserActionTimeout;
        }

        public static void CopyTo(this IAppSettings settings, IAppSettings dest)
        {
            dest.SlicingEnginePath = settings.SlicingEnginePath;
            dest.SupportEnginePath = settings.SupportEnginePath;
            dest.WorkingDirectory = settings.WorkingDirectory;
            dest.SlicingParametersRepoPath = settings.SlicingParametersRepoPath;
            dest.SupportParametersRepoPath = settings.SupportParametersRepoPath;
            dest.DefaultSlicingParameters = settings.DefaultSlicingParameters;
            dest.SlicingResultDirectory = settings.SlicingResultDirectory;
            dest.DefaultSupportParameters = settings.DefaultSupportParameters;
            dest.SupportEngineList = settings.SupportEngineList;
            dest.SelectedSupportEngine = settings.SelectedSupportEngine;
            dest.SliceEngineList = settings.SliceEngineList;
            dest.SelectedSliceEngine = settings.SelectedSliceEngine;
            dest.CurrentChangesPath = settings.CurrentChangesPath;
            dest.AutoSaveInterval = settings.AutoSaveInterval;
            dest.WaitingUserActionTimeout = settings.WaitingUserActionTimeout;
        }

        public static void SetProperty(this IAppSettings settings, string propertyName, string value)
        {
            typeof(IAppSettings).GetProperty(propertyName).SetValue(settings, value);
        }
    }
}
