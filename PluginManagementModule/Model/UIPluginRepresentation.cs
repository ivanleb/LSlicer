using PluginFramework;

namespace PluginManagementModule.Model
{
    public class UIPluginRepresentation
    {
        private IPlugin _plugin;

        public UIPluginRepresentation(IPlugin plugin)
        {
            _plugin = plugin;
        }

        public string PluginName { get => _plugin.Name; set { } }

        public void RunPlugin()
        {
            _plugin.DoAction(PluginActionSpec.CreateEmpty());
        }

        public IPlugin GetPlugin() => _plugin;

    }
}
