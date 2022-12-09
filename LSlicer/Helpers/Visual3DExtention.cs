using System.Reflection;
using System.Windows.Media.Media3D;

namespace LSlicer.Helpers
{
    public static class Visual3DExtention
    {
        public static T GetPrivateProperty<T>(this Visual3D obj, string propertyName) =>
            ReflectionExtention.GetPrivateProperty<T>(obj, propertyName);

        public static void SetPrivateProperty<T>(this Visual3D obj, string propertyName, T value) =>
            ReflectionExtention.SetPrivateProperty(obj, propertyName, value);
    }

    public static class ReflectionExtention
    {
        public static T GetPrivateProperty<T>(object obj, string propertyName)
        {
            return (T)obj.GetType()
                          .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)
                          .GetValue(obj);
        }

        public static void SetPrivateProperty<T>(object obj, string propertyName, T value)
        {
            obj.GetType()
               .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)
               .SetValue(obj, value);
        }
    }
}
