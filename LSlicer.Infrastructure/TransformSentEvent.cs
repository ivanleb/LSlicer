using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using LSlicing.Data.Interaction.Contracts;
using PluginFramework;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
namespace LSlicer.Infrastructure
{
    public class TransformSentEvent : PubSubEvent<IPartTransform>
    {
    }

    public class AbsoluteCoordinateSentEvent : PubSubEvent<Vector3D>
    {
    }

    public class SlicingParametersSentEvent : PubSubEvent<ISlicingParameters>
    {
    }

    public class SlicingParametersAddEvent : PubSubEvent<ISlicingParameters>
    {
    }

    public class SlicingParametersDeleteEvent : PubSubEvent<ISlicingParameters>
    {
    }

    public class SlicingParametersBatchSentEvent : PubSubEvent<IEnumerable<ISlicingParameters>>
    {
    }

    public class SlicingParametersSaveEvent : PubSubEvent<IEnumerable<ISlicingParameters>>
    {
    }

    public class SupportParametersSentEvent : PubSubEvent<ISupportParameters>
    {
    }

    public class SupportParametersAddEvent : PubSubEvent<ISupportParameters>
    {
    }

    public class SupportParametersDeleteEvent : PubSubEvent<ISupportParameters>
    {
    }

    public class SupportParametersBatchSentEvent : PubSubEvent<IEnumerable<ISupportParameters>>
    {
    }

    public class SupportParametersSaveEvent : PubSubEvent<IEnumerable<ISupportParameters>>
    {
    }

    public class AppSettingsSentToViewEvent : PubSubEvent<IAppSettings>
    {
    }

    public class AppSettingsSentToModelEvent : PubSubEvent<IAppSettings>
    {
    }

    public class UserIdentitySentEvent : PubSubEvent<IUserIdentity>
    {
    }

    public class UserIdentityUpdateEvent : PubSubEvent<IUserIdentity>
    {
    }

    public class UserIdentityRemoveEvent : PubSubEvent<IUserIdentity>
    {
    }

    public class UserIdentityGetEvent : PubSubEvent<IUserIdentity>
    {
    }

    public class UserIdentityCreateEvent : PubSubEvent<IUserIdentity>
    {
    }

    public class UserGetAutorizedStateEvent : PubSubEvent
    {
    }

    public class UserSentAutorizedStateEvent : PubSubEvent<Boolean>
    {
    }

    public class UserSentAutorizedUserEvent : PubSubEvent<IUserIdentity>
    {
    }

    public class ReloadPartInfoUIListEvent : PubSubEvent
    {
    }

    public class LogEvent : PubSubEvent<LogData>
    {
    }

    public class PullPluginEvent : PubSubEvent
    {
    }

    public class SendPluginEvent : PubSubEvent<IEnumerable<IPlugin>>
    {
    }
    public class InstallPluginEvent : PubSubEvent<string>
    {
    }
    public class MakePluginEvent : PubSubEvent<(string pluginPath, string packagePath)>
    {
    }
    public class UninstallPluginEvent : PubSubEvent<IPlugin>
    {
    }
    public class SetDeleteUnusedFilesEvent : PubSubEvent<bool> 
    {
    }
}
