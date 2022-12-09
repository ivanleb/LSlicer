using System;

namespace LSlicer.BL.Interaction
{
    public interface IEngineResultAwaiter
    {
        Action GetEngineTaskAwaiter(IEngineTask awaitedTask);
    }
}
