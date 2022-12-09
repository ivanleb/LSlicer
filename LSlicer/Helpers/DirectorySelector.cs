using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public class DirectorySelector
    {
        public static string Select(string defaultDirectory)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Choose folder";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = PathHelper.Resolve(defaultDirectory);

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = defaultDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            }
            else return String.Empty;
        }
}
}
