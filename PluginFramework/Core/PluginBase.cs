using CommonServiceLocator;
using System;

namespace PluginFramework.Core
{
    public abstract class PluginBase : MarshalByRefObject, IPlugin
    {
        public PluginBase(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public LoadType LoadType { get; protected set; } = LoadType.Manual;

        public abstract void DoAction(PluginActionSpec spec);
    }

    public abstract class PluginBase<Type> : PluginBase
    {
        protected readonly Lazy<Type> ServiceContainerItem1 = new Lazy<Type>(() => ServiceLocator.Current.GetInstance<Type>());

        protected PluginBase(string name) : base(name)
        {
        }
    }

    public abstract class PluginBase<Type1, Type2> : PluginBase<Type1>
    {
        protected readonly Lazy<Type2> ServiceContainerItem2 = new Lazy<Type2>(() => ServiceLocator.Current.GetInstance<Type2>());

        protected PluginBase(string name) : base(name)
        {
        }
    }

    public abstract class PluginBase<Type1, Type2, Type3> : PluginBase<Type1, Type2>
    {
        protected readonly Lazy<Type3> ServiceContainerItem3 = new Lazy<Type3>(() => ServiceLocator.Current.GetInstance<Type3>());

        protected PluginBase(string name) : base(name)
        {
        }
    }

    public abstract class PluginBase<Type1, Type2, Type3, Type4> : PluginBase<Type1, Type2, Type3>
    {
        protected readonly Lazy<Type4> ServiceContainerItem4 = new Lazy<Type4>(() => ServiceLocator.Current.GetInstance<Type4>());

        protected PluginBase(string name) : base(name)
        {
        }
    }

    public abstract class PluginBase<Type1, Type2, Type3, Type4, Type5> : PluginBase<Type1, Type2, Type3, Type4>
    {
        protected readonly Lazy<Type5> ServiceContainerItem5 = new Lazy<Type5>(() => ServiceLocator.Current.GetInstance<Type5>());

        protected PluginBase(string name) : base(name)
        {
        }
    }

    public abstract class PluginBase<Type1, Type2, Type3, Type4, Type5, Type6> : PluginBase<Type1, Type2, Type3, Type4, Type5>
    {
        protected readonly Lazy<Type6> ServiceContainerItem6 = new Lazy<Type6>(() => ServiceLocator.Current.GetInstance<Type6>());

        protected PluginBase(string name) : base(name)
        {
        }
    }

}
