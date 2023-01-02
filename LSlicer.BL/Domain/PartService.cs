using LSlicer.BL.Events;
using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.BL.Interaction.Types;
using LSlicer.Data;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Data.Operations;
using LSlicer.Helpers;
using LSlicer.Data.Interaction;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity;

namespace LSlicer.BL.Domain
{
    public class PartService<T> : IPartService
    {
        private readonly ISliceService _sliceService;
        private readonly ISupportService _supportService;
        private readonly ILoggerService _loggerService;
        private readonly IAppSettings _appSettings;
        private readonly IPartRepository _partsRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly Lazy<IWorkStateManager> _workStateManager;
        private readonly IOperationStack _operationStack;
        private readonly IExternalPartManager _externalPartManager;
        private readonly IPartTransformer _partTransformer;

        public PartService(
            ILoggerService loggerService,
            ISliceService sliceService,
            IAppSettings appSettings,
            ISupportService supportService,
            IPartRepository partsRepository,
            IUnityContainer unityContainer,
            IOperationStack operationStack,
            IExternalPartManager externalPartManager, 
            IPartTransformer partTransformer)
        {
            _sliceService = sliceService;
            _supportService = supportService;
            _loggerService = loggerService;
            _appSettings = appSettings;
            _supportService = supportService;
            _partsRepository = partsRepository;
            _unityContainer = unityContainer;
            _workStateManager = new Lazy<IWorkStateManager>(InitializeWorkStateManger);
            _operationStack = operationStack;
            _externalPartManager = externalPartManager;
            _partTransformer = partTransformer;
        }

        public IList<IPart> Parts => _partsRepository.GetAll().ToList();

        public void ApplyTransform(IPartTransform partTransform, int[] partsForTransform)
        {
            foreach (int partId in partsForTransform)
            {
                TransfomPart(partId, new[] { partTransform });
            }
        }

        public void CancelActiveOperations()
        {
            List<IOperation> toRemove = new List<IOperation>();
            foreach (IOperation operation in _operationStack.GetActiveOperation())
            {
                if (operation.Status == OperationStatus.Running || operation.Status == OperationStatus.NotStarted)
                {
                    operation.Cancel();
                    toRemove.Add(operation);
                    _loggerService.Info($"[{nameof(PartService<T>)}] Operation was cancelled: {operation.Info.Name}");
                }

                if (operation.Status == OperationStatus.Done)
                    toRemove.Add(operation);
            }
        }

        public int Copy(int id) 
        {
            string copiedPartPath = _partsRepository.GetById(id).PartSpec.MeshFilePath;
            IOperation operation = new Operation(id, new PartCopyInfo($"Copy {copiedPartPath}", id), OperationStatus.Done);
            _operationStack.Put(operation);
            int newId = _partsRepository.Copy(id);
            _externalPartManager.Copy(id, newId);
            return newId;
        }

        public IEnumerable<ISlicingInfo> GetSlicingInfo(int partId)
        {
            return _operationStack.GetOperationsByPartSafe(partId)
                .Where(op => op.Info is ISlicingInfo)
                .Select(op => op.Info)
                .Cast<ISlicingInfo>();
        }

        public int LoadPart(FileInfo path)
        {
            IPartSpec spec = CreateSpec(path);

            AddToRepo(spec, int.MinValue);

            AddPartToAuxilary(spec);

            return spec.Id;
        }

        public void MakeSupports(DirectoryInfo path)
        {
            List<string> occupiedNames = new List<string>();

            IList<IPart> toSupport = new List<IPart>();

            foreach (var part in _partsRepository.GetAll().ToList().ExtractPart())
            {
                var resultFile = FileNameResolver.ResolveSupport(part.PartSpec.MeshFilePath);

                var fileName = Path.Combine(path.FullName, Path.GetFileName(resultFile));

                FileInfo file = new FileInfo(fileName);

                int i = 0;

                while (occupiedNames.Contains(file.FullName) || file.Exists)
                {
                    file = new FileInfo(
                        Path.Combine(
                            Path.GetDirectoryName(fileName),
                            String.Concat(Path.GetFileNameWithoutExtension(fileName),
                                          "_",
                                          (i++).ToString(),
                                          Path.GetExtension(fileName))
                        ));
                }

                occupiedNames.Add(file.FullName);

                var supportOperation = new Operation(
                    part.Id,
                    new SupportInfo { Name = "MakeSupports", SupportFilePath = file.FullName, MeshFilePath = part.PartSpec.MeshFilePath, EasySliceSupportStructure = file.FullName.Replace(".stl", ".struct"), SupportParameters = file.FullName.Replace(".stl", ".par") },
                    OperationStatus.NotStarted);

                _operationStack.Put(supportOperation);
                _loggerService.Info($"[{nameof(PartService<T>)}] Add operation {supportOperation.Info.Name} to {part.PartSpec.MeshFilePath}.");

                toSupport.Add(part);
            }

            _supportService.MakeSupports(toSupport);
        }

        public void LoadWorkState(LoadingWorkStateSpec spec)
        {
            if (Parts.Any())
                SaveWorkState(spec.SavingWorkStateSpec);

            Parts.ForEach(part => _partsRepository.Remove(part));

            IPartDataForSave[] partDataForSave = _workStateManager.Value.LoadSavedData(spec);
            LoadPartsToRepo(partDataForSave);
            ApplyOperationsFromLoadedPart(partDataForSave);
        }

        public void SaveWorkState(SavingWorkStateSpec spec)
        {
            IReadOnlyList<IOperation> operations = _operationStack.GetDoneOperations();
            IPart[] changedParts = Parts.ToArray();
            if (!changedParts.Any())
                return;

            _workStateManager.Value.AddChangedPartsIntoManager(changedParts);
            _workStateManager.Value.Save(spec);
        }

        public void SliceParts(DirectoryInfo path)
        {
            foreach (var part in _partsRepository.GetAll())
            {
                var resultFile = FileNameResolver.ResolveSliced(part.PartSpec.MeshFilePath);

                var fileName = Path.GetFileName(resultFile);

                var slicingOperation = new Operation(
                    part.Id,
                    new SlicingInfo { Name = "SlicePart", FilePath = Path.Combine(path.FullName, fileName) },
                    OperationStatus.NotStarted);

                _operationStack.Put(slicingOperation);
                _loggerService.Info($"[{nameof(PartService<T>)}] Add operation {slicingOperation.Info.Name} to {part.PartSpec.MeshFilePath}.");
            }
            //TODO: детали режутся все сразу, это неправильно
            _sliceService.MakeSlicing(_partsRepository.GetAll().ToList());
        }

        public void UnloadPart(int id)
        {
            _partsRepository.RemoveAt(id);
            _externalPartManager.Detach(id);
            _operationStack.RemoveOperationsForPart(id);
        }

        public void LoadSlicingInfos(ISlicingInfo[] infos)
        {
            foreach (ISlicingInfo info in infos)
            {
                IPart slicedPart = Parts.FirstOrDefault(p => p.Id == info.PartId);
                if (slicedPart != default(IPart))
                {
                    //TODO: брать последнюю операцию неправильно
                    _operationStack.GetOperationsByPart(slicedPart.Id).GetLastOperation<ISlicingInfo>().Info = info;
                }
            }
        }

        public IOperationManager GetOperationManager()
        {
            return _operationStack.GetOperationManager();
        }

        private IPartSpec CreateSpec(FileInfo path)
        {
            int id = _partsRepository.GetNextId();
            return new PartSpec(id, path.FullName);
        }

        private IWorkStateManager InitializeWorkStateManger()
        {
            return _unityContainer.Resolve<IWorkStateManager>();
        }

        private void LoadPartsToRepo(IPartDataForSave[] partDataForSave)
        {
            foreach (IPartDataForSave savedPart in partDataForSave)
                LoadSavedPart(savedPart); 
        }

        private void LoadSavedPart(IPartDataForSave savedPart)
        {
            AddToRepo(savedPart.Spec, savedPart.LinkToParentPart);

            AddPartToAuxilary(savedPart.Spec);
        }

        private void AddToRepo(IPartSpec spec, int linkToParent)
        {
            if (spec.PartType == PartType.Support)
                _partsRepository.Attach(new Support(spec, linkToParent), linkToParent);
            else
                _partsRepository.Add(new Part(spec));
        }

        private void AddPartToAuxilary(IPartSpec spec)
        {
            _operationStack.Put(new Operation(spec.Id, new LoadInfo($"Load part {spec.MeshFilePath}", spec.MeshFilePath), OperationStatus.Done));

            if (!_externalPartManager.Append(new ModelToSceneLoadingSpec(spec.MeshFilePath, spec.Id)))
            {
                UnloadPart(spec.Id);
                throw new Exception($"Load part exception. path:\"{spec.MeshFilePath}\", id:\"{spec.Id}\"");
            }
        }

        private void ApplyOperationsFromLoadedPart(IPartDataForSave[] partDataForSave)
        {
            foreach (var part in partDataForSave)
            {
                IEnumerable<IOperation> operations = part.Operations.Where(op => op.Type != OperationType.Transforming)
                                                                    .Select(op => new Operation(part.Spec.Id, op, OperationStatus.Done))
                                                                    .ForEach(op => _operationStack.Put(op));


                IPartTransform[] transformations = part.Operations.Where(op => op.Type == OperationType.Transforming)
                                                                  .Select(op => new Operation(part.Spec.Id, op, OperationStatus.Done))
                                                                  .Select(op => op.Info).Cast<IPartTransform>().ToArray();
                TransfomPart(part.Spec.Id, transformations);

                _operationStack.GetOperationsByPart(part.Spec.Id)
                               .ForEach(operation => operation.Status = OperationStatus.Saved);
            }
        }

        private void TransfomPart(int partId, IPartTransform[] transformations)
        {
            IPart part = _partsRepository.GetById(partId);
            if (part == default)
                return;
            foreach (IPartTransform transformation in transformations)
            {
                part.Transform(transformation);
                _operationStack.Put(new Operation(partId, transformation, OperationStatus.Done));
                _partTransformer.Transform(new ModelOnViewTransformSpec(partId, transformation));
            }
        }
    }
}
