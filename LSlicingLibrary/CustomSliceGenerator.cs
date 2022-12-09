using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSlicer.Data.Operations;
using LSlicer.Helpers;
using LSlicing.Data.Interaction.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LSlicingLibrary
{
    public class CustomSliceGenerator : ISliceGenerator, IObservable<string>
    {
        private readonly IPostProcessor<ISlicingInfo> _postProcessor;
        private readonly IParametersProvider<ISlicingParameters> _getParametersProvider;
        private readonly DispatcherAggregator<string> _dispatchAggregator;
        private readonly MessageObserver<string> _messageObserver;
        private readonly IOperationStack _operationStack;

        private IList<IObserver<string>> _observers = new List<IObserver<string>>();
        private Object _locker = new Object();

        public CustomSliceGenerator(
            IPostProcessor<ISlicingInfo> postProcessor,
            IParametersProvider<ISlicingParameters> getParametersProvider,
            DispatcherAggregator<string> dispatchAggregator,
            MessageObserver<string> messageObserver, 
            IOperationStack operationStack)
        {
            _postProcessor = postProcessor;
            _getParametersProvider = getParametersProvider;
            _dispatchAggregator = dispatchAggregator;
            _messageObserver = messageObserver;
            _operationStack = operationStack;
        }

        public void SliceParts(IPart[] parts, FileInfo parameters, FileInfo resultInfo)
        {
            using (this.Subscribe(_messageObserver))
            {
                _dispatchAggregator.Dispatch(new TimeSpan(0, 0, 1));

                
                var customParameters = _getParametersProvider.GetParameters(parameters);

                    Task.Run(() =>
                    {
                        BlockingCollection<ISlicingInfo> slicingResults = new BlockingCollection<ISlicingInfo>();

                        Parallel.ForEach(parts, part =>
                        {
                            IReadOnlyList<IOperation> partOperations = _operationStack.GetOperationsByPart(part.Id);
                            IOperation operation = partOperations.GetLastOperation<ISlicingInfo>();
                            ISlicingInfo info = partOperations.GetLastOperationResultInfo<ISlicingInfo>();
                            if (info != null)
                            {
                                operation.Status = OperationStatus.Running;

                                SendStatus($"Generate slicing for {part.PartSpec.MeshFilePath}");

                                ISliceStrategy strategy = SliceStrategyFabric.Create(part, customParameters);

                                strategy.Load(part);

                                strategy.ApplyTransform(part.ResultTransform);

                                var supportSlicedFileName = FileNameResolver.AddSuffix(info.FilePath, "_" + part.Id);

                                var sliceResult = strategy.Slice(part, supportSlicedFileName);

                                slicingResults.Add(sliceResult);

                                lock (_locker)
                                {
                                    SendStatus($"Write slicing to file {supportSlicedFileName}");

                                    strategy.WriteToFile(supportSlicedFileName);
                                }

                                operation.Status = OperationStatus.Done;

                                SendStatus($"Generate slicing for {supportSlicedFileName} is {operation.Status}");
                            }
                        });

                        _postProcessor.HandleResult(slicingResults.ToArray());

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

        public override string ToString() => nameof(CustomSliceGenerator);
    }
}
