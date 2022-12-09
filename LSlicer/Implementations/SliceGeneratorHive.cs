using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;

namespace LSlicer.Implementations
{
    public class SliceGeneratorHive : BaseGeneratorHive<ISliceGenerator>, IGeneratorHive<ISliceGenerator>
    {
        public SliceGeneratorHive(ISliceGenerator[] generators, IAppSettings appSettings, ILoggerService logger)
            :base(generators, appSettings, logger)
        {
            _appSettings.SliceEngineList = _engineList;
        }
    }
}
