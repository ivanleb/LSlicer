using LiteDB;
using LSlicer.BL.Interaction;
using System;
using System.Linq;

namespace LaserAprSlicer.Infrastructure.DB
{
    public class AppSettingsLiteDBContext : IDisposable
    {
        private readonly LiteDatabase _db;
        public AppSettingsLiteDBContext(string dbPath) 
        {
            _db = new LiteDatabase(dbPath);
        }

        public IAppSettings GetSettings() 
        {
            ILiteCollection<AppSettingsDB> collection = _db.GetCollection<AppSettingsDB>("settings");
            AppSettingsDB settings = collection.Find(x => x != null).LastOrDefault();
            if(settings != null)
                return settings;
            settings = CreateSettings(collection);
            if (settings != null)
                return settings;
            throw new NullReferenceException("DB does not contain settings");
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

        private AppSettingsDB CreateSettings(ILiteCollection<AppSettingsDB> collection)
        {
            AppSettingsDB initialSettings = GetDefaultSettings();

            collection.Insert(initialSettings);
            _db.Commit();
            return initialSettings;
        }

        private static AppSettingsDB GetDefaultSettings()
        {
            var initialSettings = new AppSettingsDB();
            initialSettings.SlicingEnginePath = @"..\\..\\..\\..\\TestEngineStub\\bin\\Debug\\TestEngineStub.exe";
            initialSettings.SlicingParametersRepoPath = @"{APPDATA}\LSlicer\Resources";
            initialSettings.WorkingDirectory = @"{APPDATA}\LSlicer\Temp\";
            initialSettings.DefaultSlicingParameters = @"..\\..\\..\\..\\TestEngineStub\\slicing_d_parameters.json";
            initialSettings.SlicingResultDirectory = @"{APPDATA}\LSlicer\Jobs\";
            initialSettings.SupportEnginePath = @"..\\..\\..\\..\\SupportEngine\\bin\\x64\\Debug\\SupportEngine.exe";
            initialSettings.DefaultSupportParameters = @"..\\..\\..\\..\\TestEngineStub\\support_d_parameters.json";
            initialSettings.SupportParametersRepoPath = @"{APPDATA}\LSlicer\Resources";
            initialSettings.SupportEngineList = @"CustomSupportGenerator;Cura;";
            initialSettings.SelectedSupportEngine = @"CustomSupportGenerator";
            initialSettings.SliceEngineList = @"CustomSliceGenerator;Cura;";
            initialSettings.SelectedSliceEngine = @"CustomSliceGenerator";
            initialSettings.CurrentChangesPath = @"{APPDATA}\LSlicer\Temp\current_changes_file";
            initialSettings.DefaultSupportParameters = @"..\\..\\..\\..\\TestEngineStub\\support_d_parameters.json";
            initialSettings.AutoSaveInterval = TimeSpan.FromSeconds(20);
            initialSettings.WaitingUserActionTimeout = TimeSpan.FromMinutes(3);
            return initialSettings;
        }
    }
}
