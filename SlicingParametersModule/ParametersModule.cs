using ParametersModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ParametersModule
{
    public class ParametersModule : IModule
    {
        IRegionManager _regionManager;

        public ParametersModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("ParametersRegion", typeof(ParametersUC));
            _regionManager.RegisterViewWithRegion("SupportParametersRegion", typeof(SupportParametersUC));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}