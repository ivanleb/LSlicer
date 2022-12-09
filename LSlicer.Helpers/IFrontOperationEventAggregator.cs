using System;

namespace LSlicer.Helpers
{
    public interface IFrontOperationEventAggregator
    {
        void AddEvent<T>(Action<T> action) where T : IFrontOperationEventParameter;
        FrontOperationEvent GetEvent<T>() where T : IFrontOperationEventParameter;
    }

    public interface IFrontOperationEventParameter
    {
    }
}