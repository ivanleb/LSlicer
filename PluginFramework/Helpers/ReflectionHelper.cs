using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginFramework.Installation
{
    internal static class ReflectionHelper
    {
        internal static (AppDomain domain, Assembly assembly, string domainName) LoadAssembly(string assemblyPath)
        {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            string domainName = GetDomainName(assemblyName);
            AppDomainSetup domainSetup = new AppDomainSetup() { PrivateBinPath = FileHelper.GetSubDirectoryPath(assemblyPath) };

            ResolveEventHandler loader = SubscribeLoader(assemblyName);
            try
            {
                AppDomain dynamicDomain = AppDomain.CreateDomain(domainName, null, domainSetup);
                Assembly dynamicallyLoadedAssembly = dynamicDomain.Load(assemblyName);
                return (dynamicDomain, dynamicallyLoadedAssembly, domainName);
            }
            finally
            {
                UnsubscribeLoader(loader);
            }
        }

        internal static void UnloadDomain(AppDomain domain)
            => AppDomain.Unload(domain);

        internal static List<T> GetType<T>(Assembly assembly) 
        {
            return assembly.GetTypes()
                           .Where(type => type.IsAssignableFrom(typeof(T)) && type.IsPublic)
                           .Select(type => (T)Activator.CreateInstance(type))
                           .ToList();
        }

        internal static IEnumerable<T> CreateInstances<T>(Assembly assembly)
        {
            List<T> instances = new List<T>();
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(T).IsAssignableFrom(type) && type.IsPublic)
                    {
                        //TODO: вставить проверку на наличие конструктора с одним аргументом - строкой
                        object result = Activator.CreateInstance(type, GetDomainName(assembly.GetName()));
                        if (result != null && result is T typedResult)
                        {
                            instances.Add(typedResult);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            CheckIfNeededTypesExists(assembly, instances);
            return instances;
        }

        private static void CheckIfNeededTypesExists<T>(Assembly assembly, List<T> resultList)
        {
            if (resultList.Count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements {nameof(T)} in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        internal static IEnumerable<Type> GetTypesFormAssemblies<T>(string pluginDirectoryPath)
        {
            DirectoryInfo pluginDirectoryInfo = new DirectoryInfo(pluginDirectoryPath);
            Type interfaceType = typeof(T);

            IEnumerable<Type> existingPluginTypes = GetPluginTypesFromPluginFramework(interfaceType);

            List<AssemblyName> names = GetAssemblyNames(pluginDirectoryInfo);

            if (!names.Any())
                throw new Exception($"Directory \"{pluginDirectoryPath}\" does not have suitable assemblies");

            IEnumerable<Type> pluginTypes = names
                      .Where(ass => ass.ProcessorArchitecture == ProcessorArchitecture.Amd64 && ass.ContentType == AssemblyContentType.Default)
                      .Select(file => Assembly.Load(file))
                      .SelectMany(s => s.GetTypes())
                      .Where(p => interfaceType.IsAssignableFrom(p) && p.IsPublic)
                      .Where(pt => !existingPluginTypes.Contains(pt));

            int count = pluginTypes.Count();
            if (count > 1)
                throw new Exception($"Assemblies in directory \"{pluginDirectoryPath}\" have more than one \"{typeof(T).Name}\" type [{count}]");
            if (count < 1)
                throw new Exception($"Assemblies in directory \"{pluginDirectoryPath}\" do not have any \"{typeof(T).Name}\" type [{count}]");

            return pluginTypes;
        }

        internal static string GetDomainName(AssemblyName assemblyName) 
            => $"{assemblyName.Name}-{assemblyName.Version}";

        private static ResolveEventHandler SubscribeLoader(AssemblyName assemblyName)
        {
            ResolveEventHandler loader = GetResolveEventHandler(assemblyName);
            AppDomain.CurrentDomain.AssemblyResolve += loader;
            return loader;
        }

        private static void UnsubscribeLoader(ResolveEventHandler loader)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= loader;
        }

        private static ResolveEventHandler GetResolveEventHandler(AssemblyName assemblyName)
        {
            return (sender, args) =>
            {
                if (string.Compare(args.Name, assemblyName.FullName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    try
                    {
                        return Assembly.Load(File.ReadAllBytes(Path.GetFullPath(new Uri(assemblyName.CodeBase).LocalPath)));
                    }
                    catch { }
                }
                return null;
            };
        }

        private static IEnumerable<Type> GetPluginTypesFromPluginFramework(Type interfaceType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                      .Where(ass => ass.GetName().Name.Contains(nameof(PluginFramework)))
                      .SelectMany(s => s.GetTypes())
                      .Where(p => interfaceType.IsAssignableFrom(p) && p.IsPublic);
        }

        private static List<AssemblyName> GetAssemblyNames(DirectoryInfo pluginDirectoryInfo)
        {
            List<AssemblyName> names = new List<AssemblyName>();
            foreach (var file in pluginDirectoryInfo.GetFiles("*.dll"))
            {
                try
                {
                    names.Add(AssemblyName.GetAssemblyName(file.FullName));
                }
                catch (Exception)
                {
                    //ignore unloadable files (non .Net, etc.)
                }
            }

            return names;
        }
    }
}
