using UtilModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace UtilModule
{
    public class UtilModule : IModule
    {
        IRegionManager _regionManager;

        public UtilModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("AppSettingsRegion", typeof(AppSettingsView));
            _regionManager.RegisterViewWithRegion("LoginRegion", typeof(LoginPage));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}