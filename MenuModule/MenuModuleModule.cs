using LSlicer.BL.Interaction.Contracts;
using LSlicer.BL.Domain;
using MenuModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MenuModule
{
    public class MenuModule : IModule
    {
        IRegionManager _regionManager;

        public MenuModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("PositionRegionTranslate", typeof(TranslatePage));
            _regionManager.RegisterViewWithRegion("PositionRegionRotate", typeof(RotatePage));
            _regionManager.RegisterViewWithRegion("PositionRegionScale", typeof(ScalePage));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}