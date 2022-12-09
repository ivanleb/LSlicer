using System;

namespace LSlicer.BL.Interaction
{
    public interface ICloseApplicationHandler
    {
        void Add(string actionName, Action action);
        bool Remove(string actionName);
        void Handle();
    }
}
