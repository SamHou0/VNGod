﻿using System.Diagnostics;
using System.IO;
using System.Windows;
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
            webDavUrlTextBox.Text = Settings.Default.WebDAVUrl;
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
    }
}
