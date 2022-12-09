using PluginManagementModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PluginManagementModule
{
    public class PluginManagementModule : IModule
    {
        IRegionManager _regionManager;

        public PluginManagementModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("PluginControllerUIRegion", typeof(PluginControllerUI));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}