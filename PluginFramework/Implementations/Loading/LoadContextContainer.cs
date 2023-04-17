using PluginFramework.CustomPlugin.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PluginFramework.Implementations.Loading
{
    public class LoadContextContainer
    {
        private readonly Dictionary<string, PluginLoadContext> _container = new Dictionary<string, PluginLoadContext>();
        private readonly object _locker = new object();

        public IEnumerable<string> GetAllDomainNames() => _container.Keys;

        public void UnloadAssembly(string name)
        {
            lock (_locker)
            {
                if (_container.TryGetValue(name, out PluginLoadContext domain))
                {
                    UnloadAssembly(domain);
                    _container.Remove(name);
                }
            }
        }

        public void UnloadAllAssemblies()
        {
            lock (_locker)
            {
                foreach (var domain in _container.Values)
                {
                    UnloadAssembly(domain);
                }
                _container.Clear();
            }
        }

        public Assembly LoadAssembly(string assemblyPath)
        {
            lock (_locker)
            {
                (PluginLoadContext loadСontext, Assembly assembly) = TryLoadAssembly(assemblyPath);
                if (_container.TryAdd(assembly.FullName, loadСontext))
                    return assembly;
                throw new Exception($"Assembly with name {assembly.FullName} is already added");
            }
        }

        private static (PluginLoadContext loadСontext, Assembly assembly) TryLoadAssembly(string assemblyPath)
        {
            var loadContext = new PluginLoadContext(assemblyPath);
            Assembly assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath)));
            return (loadContext, assembly);
        }

        private static void UnloadAssembly(PluginLoadContext loadContext) => loadContext.Unload();
    }
}
