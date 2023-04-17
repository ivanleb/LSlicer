using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace PluginFramework.CustomPlugin.Helpers
{
    internal static class ReflectionHelper
    {
        internal static List<T> GetType<T>(Assembly assembly)
        {
            return assembly.GetTypes()
                           .Where(type => type.IsAssignableFrom(typeof(T)) && type.IsPublic)
                           .Select(type => (T)Activator.CreateInstance(type))
                           .ToList();
        }

        private static List<T> CheckIfNeededTypesExists<T>(this List<T> resultList, Assembly assembly)
        {
            if (resultList.Count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements {nameof(T)} in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
            return resultList;
        }

        private static IEnumerable<T> PrintLists<T>(this IEnumerable<T> resultList)
        {
            foreach (T item in resultList)
                System.Diagnostics.Debugger.Log(0, "Error", item.ToString() + "\n");

            return resultList;
        }

        internal static IEnumerable<Type> GetTypesFormAssemblies<T>(string pluginDirectoryPath)
        {
            DirectoryInfo pluginDirectoryInfo = new DirectoryInfo(pluginDirectoryPath);
            Type interfaceType = typeof(T);

            IEnumerable<Type> existingPluginTypes = GetPluginTypesFromPluginFramework(interfaceType);

            List<(AssemblyName ASS, FileInfo FI)> names = GetAssemblyNames(pluginDirectoryInfo);

            if (!names.Any())
                throw new Exception($"Directory \"{pluginDirectoryPath}\" does not have suitable assemblies");

            IEnumerable<Type> pluginTypes = names
                      .Where(file => file.ASS.ProcessorArchitecture == ProcessorArchitecture.Amd64 && file.ASS.ContentType == AssemblyContentType.Default)
                      .Select(file => Assembly.LoadFile(file.FI.FullName))
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

        internal static string GeAssemblyName(AssemblyName assemblyName) => $"{assemblyName.Name}-{assemblyName.Version}";

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

        private static List<(AssemblyName, FileInfo)> GetAssemblyNames(DirectoryInfo pluginDirectoryInfo)
        {
            List<(AssemblyName, FileInfo)> names = new List<(AssemblyName, FileInfo)>();
            foreach (var file in pluginDirectoryInfo.GetFiles("*.dll"))
            {
                try
                {
                    names.Add((AssemblyName.GetAssemblyName(file.FullName), file));
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
