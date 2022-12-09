using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using System.Windows;

namespace SlicerInstaller
{
    public static class CreateDirectoryActions
    {
        [CustomAction]
        public static ActionResult CreateLogDir(Session session)
        {
            try
            {
                MessageBox.Show("Hello World!", "Embedded Managed CA");
                session.Log("Begin MyAction Hello World");
            }
            catch (System.Exception)
            {
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CreateTempDir(Session session)
        {
            MessageBox.Show(Directory.GetCurrentDirectory());
            session.Log("Begin MyAction Hello World");

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CreateSettingsDir(Session session)
        {
            MessageBox.Show("Hello World!", "Embedded Managed CA");
            session.Log("Begin MyAction Hello World");

            return ActionResult.Success;
        }
    }
}
