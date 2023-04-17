using LSlicer.Helpers;
using System.Windows.Forms;

namespace LSlicer.Infrastructure
{
    public static class WindowHelper
    {
        public static Maybe<string> ChooseFolder(string initPath, string title)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() 
            {
                Description = title,
                InitialDirectory = initPath,
                ShowHiddenFiles = false,
            };

            if(DialogResult.OK  == folderBrowserDialog.ShowDialog())
                return folderBrowserDialog.SelectedPath;

            return null;
        }
    }
}
