using LSlicer.BL.Interaction;
using System;
using System.Data.Entity;
using System.Linq;

namespace LSlicer.Implementations
{
    public class DbCurrentAppSettings : IAppSettings
    {
        private AppSettingsSQLiteContext _context;
        private DbAppSettings _settings;

        public DbCurrentAppSettings(AppSettingsSQLiteContext context, AppSettingsResourceFile defaultSettings)
        {
            _context = context;
            _settings = _context.Settings.FirstOrDefault();
            if (_settings == null)
            {
                _settings = new DbAppSettings();
                _settings.CopyFrom(defaultSettings);
                _context.Settings.Add(_settings);
                _context.SaveChanges();
            }
        }

        public void SetForUser(int id) 
        {
            var settings = _context.Settings.Where(x => x.User.Id == id).FirstOrDefault();
            if (settings != null)
            {
                _settings = settings;
            }
        }
        
        public string SlicingEnginePath 
        { 
            get => _settings.SlicingEnginePath; 
            set 
            {
                _settings.SlicingEnginePath = value;
                SetValue();
            } 
        }
        public string SupportEnginePath
        {
            get => _settings.SupportEnginePath;
            set
            {
                _settings.SupportEnginePath = value;
                SetValue();
            }
        }
        public string WorkingDirectory
        {
            get => _settings.WorkingDirectory;
            set
            {
                _settings.WorkingDirectory = value;
                SetValue();
            }
        }
        public string SlicingParametersRepoPath
        {
            get => _settings.SlicingParametersRepoPath;
            set
            {
                _settings.SlicingParametersRepoPath = value;
                SetValue();
            }
        }
        public string SupportParametersRepoPath
        {
            get => _settings.SupportParametersRepoPath;
            set
            {
                _settings.SupportParametersRepoPath = value;
                SetValue();
            }
        }
        public string DefaultSlicingParameters
        {
            get => _settings.DefaultSlicingParameters;
            set
            {
                _settings.DefaultSlicingParameters = value;
                SetValue();
            }
        }
        public string SlicingResultDirectory
        {
            get => _settings.SlicingResultDirectory;
            set
            {
                _settings.SlicingResultDirectory = value;
                SetValue();
            }
        }
        public string DefaultSupportParameters
        {
            get => _settings.DefaultSupportParameters;
            set
            {
                _settings.DefaultSupportParameters = value;
                SetValue();
            }
        }
        public string SupportEngineList
        {
            get => _settings.SupportEngineList;
            set
            {
                _settings.SupportEngineList = value;
                SetValue();
            }
        }
        public string SelectedSupportEngine
        {
            get => _settings.SelectedSupportEngine;
            set
            {
                _settings.SelectedSupportEngine = value;
                SetValue();
            }
        }
        public string SliceEngineList 
        { 
            get => _settings.SliceEngineList; 
            set 
            {
                _settings.SliceEngineList = value;
                SetValue();
            } 
        }
        public string SelectedSliceEngine
        {
            get => _settings.SelectedSliceEngine;
            set
            {
                _settings.SelectedSliceEngine = value;
                SetValue();
            }
        }
        public string CurrentChangesPath
        { 
            get => _settings.CurrentChangesPath;
            set
            {
                _settings.CurrentChangesPath = value;
                SetValue();
            }
        }
        public TimeSpan AutoSaveInterval
        {
            get => _settings.AutoSaveInterval;
            set
            {
                _settings.AutoSaveInterval = value;
                SetValue();
            }
        }
        public TimeSpan WaitingUserActionTimeout
        {
            get => _settings.WaitingUserActionTimeout;
            set
            {
                _settings.WaitingUserActionTimeout = value;
                SetValue();
            }
        }

        private void SetValue() 
        {
            var entry = _context.Entry(_settings);
            entry.CurrentValues.SetValues(_settings);
            entry.State = EntityState.Modified;
            _context.SaveChangesAsync();
        }
    }
}
