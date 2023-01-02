using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSlicer.Data.Operations;
using LSlicer.Helpers;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSlicer.BL.Domain
{
    public class SliceGenerator<T> : ISliceGenerator
    {
        private readonly IEngineInvoker<T> _slicingEngineInvoker;
        private readonly IEngineResultAwaiter _engineResultAwaiter;
        private readonly ILoggerService _logger;
        private readonly IAppSettings _appSettings;
        private readonly DispatcherAggregator<T> _dispatchAggregator;
        private readonly MessageObserver<T> _messageObserver;
        private readonly IOperationStack _operationStack;

        public SliceGenerator(
            ILoggerService loggerService,
            IAppSettings appSettings,
            IEngineInvoker<T> slicingEngineInvoker,
            DispatcherAggregator<T> messageAggregator,
            IEngineResultAwaiter engineResultAwaiter, 
            IOperationStack operationStack)
        {
            _slicingEngineInvoker = slicingEngineInvoker;
            _logger = loggerService;
            _appSettings = appSettings;
            _dispatchAggregator = messageAggregator;
            ProcessingMessageDispatcher<T> processingMessageDispatcher = ProcessingMessageDispatcher<T>.Create();
            _dispatchAggregator.AddDispatcher(processingMessageDispatcher);
            _messageObserver = new MessageObserver<T>(_dispatchAggregator);
            _engineResultAwaiter = engineResultAwaiter;
            _operationStack = operationStack;
        }

        public void SliceParts(IPart[] parts, FileInfo parameters, FileInfo resultInfo)
        {
            var part = parts.FirstOrDefault();
            if (part == default(IPart))
            {
                _logger.Info($"[{nameof(SupportGenerator<T>)}] No parts for produce slicing.");
                return;
            }

            IOperation operation = _operationStack.GetOperationsByPart(part.Id).GetLastOperation<ISlicingInfo>();

            operation.Status = OperationStatus.Running;

            IEngineTask task = GetEngineTask(parts, parameters, resultInfo, operation);

            using (_slicingEngineInvoker.Subscribe(_messageObserver))
            {
                _dispatchAggregator.Dispatch(new TimeSpan(500));
                _slicingEngineInvoker.Run(task, _engineResultAwaiter, operation.Token);
            }
        }

        private IEngineTask GetEngineTask(IPart[] parts, FileInfo parameters, FileInfo resultInfo, IOperation operation)
        {
            FileInfo engine = new FileInfo(PathHelper.Resolve(_appSettings.SlicingEnginePath));
            return EngineTaskCreator.Create(EJobType.Slice, parts, engine, parameters, resultInfo, 0, operation);
        }

        public override string ToString() => nameof(SliceGenerator<T>);
    }
}
