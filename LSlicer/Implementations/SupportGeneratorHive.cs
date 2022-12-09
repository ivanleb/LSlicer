using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;

namespace LSlicer.Implementations
{
    public class SupportGeneratorHive : BaseGeneratorHive<ISupportGenerator>, IGeneratorHive<ISupportGenerator>
    {
        public SupportGeneratorHive(ISupportGenerator[] generators, IAppSettings appSettings, ILoggerService logger)
            : base(generators, appSettings, logger)
        {
            _appSettings.SupportEngineList = _engineList;
        }
    }
}
