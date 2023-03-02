using LSlicer.BL.Interaction;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LSlicer.Implementations
{
    public class DbAppSettings : IAppSettings, ICloneable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public User User { get; set; }
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

        public Int64 WaitingUserActionTimeoutTicks { get; set; }
        [NotMapped]
        public TimeSpan WaitingUserActionTimeout
        {
            get { return TimeSpan.FromTicks(WaitingUserActionTimeoutTicks); }
            set { WaitingUserActionTimeoutTicks = value.Ticks; }
        }

        public Int64 AutoSaveIntervalTicks { get; set; }
        [NotMapped]
        public TimeSpan AutoSaveInterval
        {
            get { return TimeSpan.FromTicks(AutoSaveIntervalTicks); }
            set { AutoSaveIntervalTicks = value.Ticks; }
        }

        public object Clone() => this.MemberwiseClone();

        public void SetForUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}
