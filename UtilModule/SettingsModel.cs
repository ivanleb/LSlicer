using LSlicer.BL.Interaction;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace UtilModule
{
    public class SettingsModel : IAppSettings
    {
        public string SlicingEnginePath             { get; set; }
        public string SupportEnginePath             { get; set; }
        public string WorkingDirectory              { get; set; }
        public string SlicingParametersRepoPath     { get; set; }
        public string SupportParametersRepoPath     { get; set; }
        public string DefaultSlicingParameters      { get; set; }
        public string SlicingResultDirectory        { get; set; }
        public string DefaultSupportParameters      { get; set; }
        public string SupportEngineList             { get; set; }
        public string SelectedSupportEngine         { get; set; }
        public string SliceEngineList               { get; set; }
        public string SelectedSliceEngine           { get; set; }
        public string CurrentChangesPath            { get; set; }
        public TimeSpan AutoSaveInterval            { get; set; }
        public TimeSpan WaitingUserActionTimeout    { get; set; }


        public ObservableCollection<UIPair> Get()
        {
            ObservableCollection<UIPair> result = new ObservableCollection<UIPair>();
            var properties = typeof(SettingsModel).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                result.Add(new UIPair(properties[i].Name, properties[i].GetValue(this).ToString()));
            }
            return result;
        }

        public void Set(ObservableCollection<UIPair> pairs)
        {
            var properties = typeof(SettingsModel).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var pair = pairs.FirstOrDefault(x => x.PKey == properties[i].Name);
                if (!String.IsNullOrEmpty(pair.PKey))
                {
                    switch (properties[i].PropertyType.FullName)
                    {
                        case "System.TimeSpan": //TODO: переделать более типизированным образом
                            TimeSpan result;
                            if (TimeSpan.TryParse(pair.PValue, out result))
                                properties[i].SetValue(this, result);
                            break;
                        default:
                            properties[i].SetValue(this, pair.PValue);
                            break;
                    }
                }
            }
        }

        public void SetForUser(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class UIPair
    {
        public UIPair(string key, string value)
        {
            PKey = key;
            PValue = value;
        }

        public string PKey { get; set; }
        public string PValue { get; set; }
    }
}
