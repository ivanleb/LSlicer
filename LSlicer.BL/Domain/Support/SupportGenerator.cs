using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSlicer.Data.Operations;
using LSlicer.Helpers;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.IO;
using System.Linq;

namespace LSlicer.BL.Domain
{
    public class SupportGenerator<T> : ISupportGenerator
    {
        private readonly IAppSettings _appSettings;
        private readonly IEngineInvoker<T> _supportEngineInvoker;
        private readonly IEngineResultAwaiter _engineResultAwaiter;
        private readonly DispatcherAggregator<T> _dispatchAggregator;
        private readonly MessageObserver<T> _messageObserver;
        private readonly ILoggerService _logger;
        private readonly IOperationStack _operationStack;

        public SupportGenerator(
            IAppSettings appSettings,
            IEngineInvoker<T> supportEngineInvoker,
            IEngineResultAwaiter engineResultAwaiter,
            DispatcherAggregator<T> dispatchAggregator,
            MessageObserver<T> messageObserver,
            ILoggerService logger, 
            IOperationStack operationStack)
        {
            _appSettings = appSettings;
            _supportEngineInvoker = supportEngineInvoker;
            _engineResultAwaiter = engineResultAwaiter;
            _dispatchAggregator = dispatchAggregator;
            _messageObserver = messageObserver;
            _logger = logger;
            _operationStack = operationStack;
        }

        public void GenerateSupports(IPart[] parts, int numberFrom, FileInfo parameters, FileInfo resultInfo)
        {
            var part = parts.FirstOrDefault();
            if (part == default(IPart))
            {
                _logger.Info($"[{nameof(SupportGenerator<T>)}] No parts for produce supports.");
                return;
            }

            IOperation operation = _operationStack.GetOperationsByPart(part.Id).GetLastOperation<ISupportInfo>();
            operation.Status = OperationStatus.Running;

            IEngineTask task = GetEngineTask(parts, numberFrom, parameters, resultInfo, operation);
            using (_supportEngineInvoker.Subscribe(_messageObserver))
            {
                _dispatchAggregator.Dispatch(new TimeSpan(500));
                _supportEngineInvoker.Run(task, _engineResultAwaiter, operation.Token);
            }
        }

        private IEngineTask GetEngineTask(IPart[] parts, int numberFrom, FileInfo parameters, FileInfo resultInfo, IOperation operation)
        {
            FileInfo engine = new FileInfo(PathHelper.Resolve(_appSettings.SupportEnginePath));
            _logger.Info($"[{nameof(SupportGenerator<T>)}] Use engine {_appSettings.SupportEnginePath}.");
            IEngineTask task = EngineTaskCreator.Create(EJobType.MakeSupports, parts, engine, parameters, resultInfo, numberFrom, operation);
            return task;
        }

        public override string ToString() 
            => nameof(SupportGenerator<T>);
    }
}
