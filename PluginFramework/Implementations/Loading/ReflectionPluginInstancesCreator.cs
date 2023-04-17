using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PluginFramework.Core.Loading;
using PluginFramework.CustomPlugin.Helpers;
using PluginFramework.Logging;

namespace PluginFramework.Implementations.Loading
{
    public class ReflectionPluginInstancesCreator<T> : IPluginInstancesCreator<T>
    {
        private readonly ILogger _logger;

        public ReflectionPluginInstancesCreator(ILogger logger)
        {
            _logger = logger;
        }

        public IReadOnlyList<T> CreateInstances(Assembly assembly, Type pluginType)
        {
            Type[] assemblyTypes = assembly.GetTypes();
            return GetInstances(assemblyTypes, assembly.GetName(), pluginType);
        }

        private IReadOnlyList<T> GetInstances(Type[] assemblyTypes, AssemblyName assemblyName, Type plugType)
        {
            List<T> instances = new List<T>();
            foreach (Type type in assemblyTypes)
            {
                if (IsInstanceTypeMeetsInterface(type, plugType) && type.IsPublic)
                {
                    //TODO: вставить проверку на наличие конструктора с одним аргументом - строкой
                    object pluginInstance = Activator.CreateInstance(type, ReflectionHelper.GeAssemblyName(assemblyName));
                    if (pluginInstance != null && pluginInstance is T castedPluginInstance)
                    {
                        instances.Add(castedPluginInstance);
                    }
                }
            }
            return instances;
        }

        private bool IsInstanceTypeMeetsInterface(Type instanceType, Type interfaceType)
        {
            return instanceType.FindInterfaces(new TypeFilter(NameInterfaceFilter), interfaceType).Any();
        }

        public static bool NameInterfaceFilter(Type instanceType, object interfaceType)
        {
            return instanceType.ToString() == interfaceType.ToString();
        }
    }
}
