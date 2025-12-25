using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Shapes;
using VNGod.Data;
using VNGod.Properties;
using VNGod.Utils;

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
            webDavUrlTextBox.Text = Settings.Default.WebDAVUrl;
            webDavUsernameTextBox.Text = Settings.Default.WebDAVUsername;
            webDavPasswordBox.Password = Settings.Default.WebDAVPassword;
            bangumiTokenTextBox.Text = Settings.Default.BgmToken;
            vndbTokenTextBox.Text = Settings.Default.VNDBToken;
            versionBlock.Text = "Version " + Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        }
        private string HandleUrl(string url)
        {
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            return url;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // WebDAV settings
            Settings.Default.WebDAVUrl = webDavUrlTextBox.Text;
            Settings.Default.WebDAVUsername = webDavUsernameTextBox.Text;
            Settings.Default.WebDAVPassword = webDavPasswordBox.Password;
            // Tokens
            Settings.Default.BgmToken = bangumiTokenTextBox.Text;
            Settings.Default.VNDBToken = vndbTokenTextBox.Text;
            Settings.Default.Save();
        }
        private void OpenLogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"));
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ThrowExceptionButton_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("This is a test exception.");
        }

        private async void RunTokenTestButton_Click(object sender, RoutedEventArgs e)
        {
            Game testGame = new Game
            {
                DirectoryName = "Summer Pockets"
            };
            if (await NetworkHelper.GetBangumiSubjectAsync(testGame, true))
            {
                MessageBox.Show(testGame.Name + " " + testGame.BangumiID, "Success");
            }
            else
            {
                MessageBox.Show("Failed to get Bangumi info. Please check your token and network connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WebDavUrlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            webDavUrlTextBox.Text = HandleUrl(webDavUrlTextBox.Text);
        }

        private void GithubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/SamHou0/VNGod");
        }
    }
}
