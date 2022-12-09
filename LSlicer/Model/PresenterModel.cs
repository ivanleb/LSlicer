using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.BL.Interaction.Types;
using LSlicer.Data.Interaction;
using LSlicer.Data.Operations;
using LSlicer.Helpers;
using LSlicer.Infrastructure;
using LSlicing.Data.Interaction.Contracts;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media.Media3D;

namespace LSlicer.Model
{
    public class PresenterModel
    {
        private readonly IPartService _partService;

        private readonly IEventAggregator _ea;
        private readonly IAppSettings _appSettings;
        private readonly ILoggerService _logger;
        private readonly DispatcherAggregator<string> _messageAggregator;
        private readonly IWorkSaver _workSaver;

        private IDictionary<int, Visual3D> _partIdToDisplayObject = new Dictionary<int, Visual3D>();

        public IList<int> SelectedPartIds { get; set; } = new List<int>();

        public ObservableCollection<UIPartInfo> SelectedParts = new ObservableCollection<UIPartInfo>();

        public IList<Visual3D> DisplayObjects { get; set; }

        public Func<int, IList<string>> GetSupportParametersIdentifiersDelegate { get; set; } = i => new List<string> { i.ToString() };
        
        public Func<int, IList<string>> GetSlicingParametersIdentifiersDelegate { get; set; } = i => new List<string> { i.ToString() };

        public PresenterModel(
            IPartService partService,
            DispatcherAggregator<string> messageAggregator,
            IEventAggregator ea,
            IAppSettings appSettings,
            ILoggerService logger,
            IWorkSaver workSaver)
        {
            _partService = partService;
            _messageAggregator = messageAggregator;
            _ea = ea;
            _appSettings = appSettings;
            _logger = logger;
            _workSaver = workSaver;
        }

        public void SubscribeMessageStream(IObserver<string> observer) 
            => _messageAggregator.Subscribe(observer);
               
        #region Part managment
        public int LoadPart(string path)
        {
            FileInfo file = new FileInfo(PathHelper.Resolve(path));
            return _partService.LoadPart(file);
        }

        public void LoadPartOnScene(int partId, Visual3D displayObject) 
        {
            _partIdToDisplayObject.Add(partId, displayObject);
            SelectedParts.Clear();
            SelectedParts.Add(GetPartInfo(partId));
        }

        public void UnloadPart(int id)
        {
            _partService.UnloadPart(id);
        }

        internal Visual3D DetachPartFromScene(int partId)
        {
            try
            {
                Visual3D visual = _partIdToDisplayObject[partId];
                _partIdToDisplayObject.Remove(partId);
                return visual;
            }
            catch
            {
                return null;
            }
        }

        public void SaveResult(string name)
        {
            string pathToResult = _workSaver.Save(name, _partService.Parts);
            if (!String.IsNullOrEmpty(pathToResult))
            {
                ActionHelper.ShowSuccessMessage($"Result was been saved in \"{pathToResult}\"");
                Process.Start(PathHelper.Resolve(pathToResult));
            }
        }

        public void ApplyTransform(int partId, IPartTransform transform)
        {
            _partService.ApplyTransform(transform, new[] { partId });
            _logger.Info($"[{nameof(PresenterModel)}] For {partId} apply transform {transform.Name}.");
        }

        private void ApplyTransformAll(IPartTransform transform) =>
            _partService.ApplyTransform(transform, _partService.Parts.Select(p => p.Id).ToArray() );

        public IList<IPart> Parts 
            => _partService.Parts; 

        public IEnumerable<UIPartInfo> GetPartInfos() 
            => _partService.Parts.Select(part 
                => new UIPartInfo(part,
                                  GetSupportParametersIdentifiersDelegate(part.Id),
                                  GetSlicingParametersIdentifiersDelegate(part.Id),
                                  GetSlicingInfoForPart));

        private string GetSlicingInfoForPart(int partId) 
        {
            ISlicingInfo slicingInfo = _partService.GetSlicingInfo(partId).Reverse().FirstOrDefault();
            return slicingInfo == default ? "" : slicingInfo.FilePath;
        }

        public UIPartInfo GetPartInfo(int partId)
        {
            IPart part = _partService.Parts.FirstOrDefault(p => p.Id == partId);
            return new UIPartInfo(part,
                           GetSupportParametersIdentifiersDelegate(part.Id),
                           GetSlicingParametersIdentifiersDelegate(part.Id),
                           GetSlicingInfoForPart); 
        }

        public void LoadSlicingInfos(ISlicingInfo[] infos) 
            => _partService.LoadSlicingInfos(infos);

        public void UndoAction(int[] partIds)
        {
            foreach (int partId in partIds)
                _partService.GetOperationManager().Undo(partId); 
        }

        public Maybe<Visual3D> GetDisplayObjectById(int partId) 
        {
            if (_partIdToDisplayObject.ContainsKey(partId))
                return _partIdToDisplayObject[partId];
            return Maybe.None;
        }

        public Maybe<int> GetDisplayObjectId(Visual3D visual)
        {
            foreach (var pair in _partIdToDisplayObject)
                if (pair.Value == visual) 
                    return pair.Key;
            return Maybe.None;
        }

        internal IList<Visual3D> GetAllDisplayObjects() 
            => _partIdToDisplayObject.Values.ToList();

        public int CopyPart(int id)
        {
            return _partService.Copy(id);
        }

        public void CopyPartOnScene(int newId, Visual3D displayObject)
        {
            _partIdToDisplayObject.Add(newId, displayObject);
        }
        #endregion 
    }
}
