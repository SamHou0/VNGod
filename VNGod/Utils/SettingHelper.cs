using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNGod.Properties;
using VNGod.Resource.Strings;
using System.IO;

namespace VNGod.Utils
{
    internal static class SettingHelper
    {
        public static async Task CheckSettingsAsync()
        {
            if (!File.Exists(Settings.Default.SevenZipPath))
            {
                Growl.Warning(Strings.SevenZipNotFound);
            }
            // Reinitialize WebDAV client with new settings
            Growl.Info(Strings.InitWebDAV);
            if (!await WebDAVHelper.InitializeClient())
                Growl.Warning(Strings.WebDAVInitFailed);
            else
                Growl.Success(Strings.WebDAVInitSuccess);
        }
    }
}
