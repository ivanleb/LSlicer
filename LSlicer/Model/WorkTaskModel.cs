using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Types;
using LSlicer.Data.Interaction;
using LSlicer.Helpers;
using LSlicer.Infrastructure;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Model
{
    public class WorkTaskModel
    {
        private readonly IAppSettings _appSettings;
        private readonly IPartService _partService;
        private readonly IEventAggregator _ea;

        public WorkTaskModel(
            IAppSettings appSettings, 
            IPartService partService, 
            IEventAggregator ea)
        {
            _appSettings = appSettings;
            _partService = partService;
            _ea = ea;
        }

        public void SetWorkingDirectory(DirectoryInfo directory)
            => _appSettings.WorkingDirectory = directory.FullName;

        public void SliceAll()
        {
            var directory = new DirectoryInfo(PathHelper.Resolve(_appSettings.WorkingDirectory));
            _partService.SliceParts(directory);
            _ea.GetEvent<ReloadPartInfoUIListEvent>().Publish();
        }

        public void MakeSupportsForAll()
        {
            var directory = new DirectoryInfo(PathHelper.Resolve(_appSettings.WorkingDirectory));
            _partService.MakeSupports(directory);
            _ea.GetEvent<ReloadPartInfoUIListEvent>().Publish();
        }

        public void CancelAllTasks()
            => _partService.CancelActiveOperations();

        public void SaveCurrentChanges(SavingWorkStateSpec spec)
            => _partService.SaveWorkState(spec);

        public void OpenWorkState(LoadingWorkStateSpec spec)
            => _partService.LoadWorkState(spec);

        public void LoadSupportInfos(ISupportInfo[] infos)
        {

        }
    }
}
