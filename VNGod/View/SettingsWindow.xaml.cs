using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VNGod.Properties;

namespace VNGod
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : HandyControl.Controls.Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            // Load settings
            webDavUrlTextBox.Text =Settings.Default.WebDAVUrl;
            webDavUsernameTextBox.Text = Settings.Default.WebDAVUsername;
            webDavPasswordBox.Password = Settings.Default.WebDAVPassword;
            bangumiTokenTextBox.Text = Settings.Default.BgmToken;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // WebDAV settings
            Settings.Default.WebDAVUrl = webDavUrlTextBox.Text;
            Settings.Default.WebDAVUsername = webDavUsernameTextBox.Text;
            Settings.Default.WebDAVPassword = webDavPasswordBox.Password;
            // Bangumi token
            Settings.Default.BgmToken = bangumiTokenTextBox.Text;

            Settings.Default.Save();
        }
    }
}
