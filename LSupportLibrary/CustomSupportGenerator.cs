using LSlicer.BL.Domain;
using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSlicer.Data.Operations;
using LSlicer.Helpers;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LSupportLibrary
{
    public class CustomSupportGenerator : ISupportGenerator, IObservable<string>
    {
        private readonly IPostProcessor<IPart> _postProcessor;
        private readonly IParametersProvider<ISupportParameters> _getParametersProvider;
        private readonly DispatcherAggregator<string> _dispatchAggregator;
        private readonly MessageObserver<string> _messageObserver;
        private readonly IOperationStack _operationStack;

        private IList<IObserver<string>> _observers = new List<IObserver<string>>();
        private Object _locker = new Object();

        public CustomSupportGenerator(
            IPostProcessor<IPart> postProcessor,
            IParametersProvider<ISupportParameters> getParametersProvider,
            DispatcherAggregator<string> dispatchAggregator,
            MessageObserver<string> messageObserver, 
            IOperationStack operationStack)
        {
            _postProcessor = postProcessor;
            _getParametersProvider = getParametersProvider;
            _dispatchAggregator = dispatchAggregator;
            _messageObserver = messageObserver;
            ProcessingMessageDispatcher<string> processingMessageDispatcher = ProcessingMessageDispatcher<string>.Create();
            _dispatchAggregator.AddDispatcher(processingMessageDispatcher);
            _messageObserver = new MessageObserver<string>(_dispatchAggregator);
            _operationStack = operationStack;
        }

        public void GenerateSupports(IPart[] parts, int numberFrom, FileInfo parameters, FileInfo resultInfo)
        {
            using (this.Subscribe(_messageObserver))
            {
                _dispatchAggregator.Dispatch(new TimeSpan(0,0,1));

                var customParameters = _getParametersProvider.GetParameters(parameters);


                    Task.Run(() =>
                    {
                        BlockingCollection<IPart> supports = new BlockingCollection<IPart>();

                        Parallel.ForEach(parts, part =>
                        {
                            IReadOnlyList<IOperation> partOperations = _operationStack.GetOperationsByPart(part.Id);
                            IOperation operation = partOperations.GetLastOperation<ISupportInfo>();
                            ISupportInfo info = partOperations.GetLastOperationResultInfo<ISupportInfo>();

                            if (info != null)
                            {
                                operation.Status = OperationStatus.Running;

                                CancellationToken cancellationToken = operation.Token;

                                SendStatus($"Generate supports for {part.PartSpec.MeshFilePath}");

                                IMakeSupportStrategy strategy = SupportStrategyFabric.Create(part, customParameters, numberFrom, cancellationToken);

                                strategy.Load(part);

                                strategy.ApplyTransform(part.ResultTransform);

                                strategy.Prepare(part);

                                if (cancellationToken.IsCancellationRequested)
                                {
                                    SendStatus($"Generate supports has been canceled.");
                                    return;
                                }

                                string supportFilePath = FileNameResolver.AddSuffix(info.SupportFilePath, "_" + FileNameResolver.ExtractSuffix(parameters.Name));

                                strategy.GenerateSupports(part, supportFilePath)
                                    .ToList().ForEach(x => supports.Add(x));

                                if (cancellationToken.IsCancellationRequested)
                                {
                                    SendStatus($"Generate supports has been canceled.");
                                    return;
                                }

                                SendStatus($"Write supports to file {supportFilePath}");

                                strategy.WriteToFile(supportFilePath);

                                operation.Status = OperationStatus.Done;

                                SendStatus($"Generate supports for {part.PartSpec.MeshFilePath} is {operation.Status}");
                            }
                        });

                        _postProcessor.HandleResult(supports.ToArray());

                        EndProcessing();
                    });
                
            }
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber<string>(_observers, observer);
        }

        private void EndProcessing()
        {
            foreach (var observer in _observers)
                if (_observers.Contains(observer))
                    observer.OnCompleted();
        }

        private void SendStatus(string status)
        {
            foreach (var observer in _observers)
                observer.OnNext(status);
        }

        public override string ToString() 
            => nameof(CustomSupportGenerator);
    }
}

