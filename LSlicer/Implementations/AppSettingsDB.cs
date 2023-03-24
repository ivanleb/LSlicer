using LSlicer.BL.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserAprSlicer.Infrastructure.DB
{
    internal class AppSettingsDB : IAppSettings
    {
        public string SlicingEnginePath { get; set; }
        public string SupportEnginePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string SlicingParametersRepoPath { get; set; }
        public string SupportParametersRepoPath { get; set; }
        public string DefaultSlicingParameters { get; set; }
        public string SlicingResultDirectory { get; set; }
        public string DefaultSupportParameters { get; set; }
        public string SupportEngineList { get; set; }
        public string SelectedSupportEngine { get; set; }
        public string SliceEngineList { get; set; }
        public string SelectedSliceEngine { get; set; }
        public string CurrentChangesPath { get; set; }
        public TimeSpan AutoSaveInterval { get; set; }
        public TimeSpan WaitingUserActionTimeout { get; set; }

        public void SetForUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}
