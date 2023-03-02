using System;
using System.Collections.Generic;
using System.Reflection;

namespace PluginFramework.CustomPlugin.Helpers
{
    public class AppDomainContainer
    {
        private readonly Dictionary<string, AppDomain> _container = new Dictionary<string, AppDomain>();
        private readonly object _locker = new object();

        public IEnumerable<string> GetAllDomainNames() => _container.Keys;

        public void UnloadDomain(string name)
        {
            lock (_locker)
            {
                if (_container.TryGetValue(name, out AppDomain domain))
                {
                    ReflectionHelper.UnloadDomain(domain);
                    _container.Remove(name);
                }
            }
        }

        public void UnloadAllDomains()
        {
            lock (_locker)
            {
                foreach (var domain in _container.Values)
                {
                    ReflectionHelper.UnloadDomain(domain);
                }
                _container.Clear();
            }
        }

        public Assembly LoadDomain(string assemblyPath)
        {
            lock (_locker)
            {
                (AppDomain domain, Assembly assembly, string domainName) = ReflectionHelper.LoadAssembly(assemblyPath);
                _container.Add(domainName, domain);
                return assembly;
            }
        }
    }
}
