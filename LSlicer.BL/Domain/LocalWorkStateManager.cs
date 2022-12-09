using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Types;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Helpers;
using LSlicing.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace LSlicer.BL.Domain
{
    public class LocalWorkStateManager : IWorkStateManager
    {
        private readonly Object _fileLocker = new Object();
        private readonly ILoggerService _logger;
        private readonly IPartSerializer _serializer;
        private readonly IAppSettings _appSettings;
        private readonly IOperationStack _operationStack;
        private readonly AutoResetEvent _autoSaveEvent = new AutoResetEvent(false);
        private List<IPart> _changedParts = new List<IPart>();
        private string _autoSavePath;
        private bool _autoSaveEnabled;

        public LocalWorkStateManager(
            ILoggerService logger,
            IPartSerializer serializer,
            IAppSettings appSettings,
            IOperationStack operationStack)
        {
            _logger = logger;
            _serializer = serializer;
            _appSettings = appSettings;
            _operationStack = operationStack;
        }

        public void AddChangedPartsIntoManager(params IPart[] parts)
        {
            foreach (IPart part in parts)
            {
                if(!_changedParts.Contains(part))
                    _changedParts.Add(part);
            }
        }

        public IPartDataForSave[] LoadSavedData(LoadingWorkStateSpec spec)
        {
            try
            {
                if (spec.SavingWorkStateSpec.AutoSaveEnable)
                    DisableAutoSave();

                string changesPath = spec.LoadPathSelector?.Invoke(spec.LoadingFile.FullName);
                if (changesPath == null)
                {
                    _logger.Info($"[{nameof(LocalWorkStateManager)}] File path for current changes does not found.");
                    return new IPartDataForSave[0];
                }
                lock (_fileLocker)
                {
                    using (var fileStream = new FileStream(changesPath, FileMode.Open))
                    using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string line = reader.ReadToEnd();
                        IPartDataForSave[] savedData = _serializer.Deserialize(line);
                        _logger.Info($"[{nameof(LocalWorkStateManager)}] Changes was loaded from {PathHelper.Resolve(changesPath)}");
                        return savedData;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"[{nameof(LocalWorkStateManager)}] Load changes error.", e);
                throw;
            }
        }

        public void Save(SavingWorkStateSpec spec)
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                SaveChangesInternal(spec);
                foreach (var part in _changedParts)
                {
                    _operationStack.GetOperationsByPart(part.Id)
                                   .ForEach(o => o.Status = OperationStatus.Saved);
                }
            });
        }

        private void SaveChangesInternal(SavingWorkStateSpec spec)
        {
            try
            {                
                string changesPath = spec.PathSelector?.Invoke(_autoSaveEnabled ? _autoSavePath : "");
                if (String.IsNullOrEmpty(changesPath))
                {
                    _logger.Info($"[{nameof(LocalWorkStateManager)}] File path for current changes does not found.");
                    return;
                }

                lock (_fileLocker)
                {
                    using (var fileStream = new FileStream(PathHelper.Resolve(changesPath), FileMode.OpenOrCreate))
                    using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        PartDataForSave[] dataForSaving = GetDataForSaving();
                        string serializedData = _serializer.Serialize(dataForSaving);
                        writer.WriteLine(serializedData);
                    }
                }
                _logger.Info($"[{nameof(LocalWorkStateManager)}] Changes was saved at {PathHelper.Resolve(changesPath)}");
            }
            catch (Exception e)
            {
                _logger.Error($"[{nameof(LocalWorkStateManager)}] Save changes error.", e);
            }
            finally
            {
                if (spec.AutoSaveEnable)
                    EnableAutoSave(spec);
                else
                    DisableAutoSave();

                _autoSaveEvent.Set();
            }
        }

        private PartDataForSave[] GetDataForSaving()
        {
            List<PartDataForSave> result = new List<PartDataForSave>();
            foreach (var part in _changedParts)
            {
                IReadOnlyList<IOperation> operations = _operationStack.GetOperationsByPartSafe(part.Id);
                IOperationInfo[] operationInfos = operations.Select(op => op.Info).ToArray();
                result.Add(part.GetChangesForSave(operationInfos));
            }
            return result.ToArray();
        }

        private void EnableAutoSave(SavingWorkStateSpec spec)
        {
            if (_autoSaveEnabled) return;

            _autoSaveEnabled = true;
            Thread autosaveThread = new Thread(() =>
            {
                if (!_autoSaveEvent.WaitOne(_appSettings.WaitingUserActionTimeout))
                {
                    _logger.Info($"[{nameof(LocalWorkStateManager)}] Path selecting is too long, autosaving disabled.");
                    return;
                }

                while (_autoSaveEnabled)
                {
                    _autoSaveEvent.WaitOne(_appSettings.AutoSaveInterval);
                    Save(spec);
                }
            });
            autosaveThread.IsBackground = true;
            autosaveThread.Start();
        }

        internal void DisableAutoSave()
        {
            _autoSaveEnabled = false;
        }
    }
}
