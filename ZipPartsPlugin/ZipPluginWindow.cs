﻿using Microsoft.WindowsAPICodePack.Dialogs;
using PluginFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZipPartsPlugin
{
    public class ZipPluginWindow : MarshalByRefObject, IPlugin
    {
        private readonly ZipPlugin _zipPlugin;
        public ZipPluginWindow(string name)
        {
            _zipPlugin = new ZipPlugin(name);
        }

        public string Name => _zipPlugin.Name;

        public LoadType LoadType => LoadType.Manual;

        public void DoAction(PluginActionSpec spec)
        {
            string zipfilePath = "";

            string initialDirectory = String.IsNullOrEmpty(spec.ResultPath) ? AppDomain.CurrentDomain.BaseDirectory : spec.ResultPath;
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Choose folder";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = initialDirectory;

            dlg.AddToMostRecentlyUsedList = true;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = initialDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;


            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                zipfilePath = dlg.FileName;
            }

            Progress<string> progress = new Progress<string>();
            ZipWindow window = new ZipWindow();
            progress.ProgressChanged += (s, e) => window.AddToStatus(e);
            PluginActionSpec newSpec = new PluginActionSpec(zipfilePath, progress);
            _zipPlugin.DoAction(newSpec);
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.Show();
        }
    }
}
