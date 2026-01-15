using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using VNGod.Data;
using VNGod.Models;
using VNGod.Services;
using VNGod.Utils;
using VNGod.Resource.Strings;

namespace VNGod.View
{
    /// <summary>
    /// GameStorageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameStorageWindow : HandyControl.Controls.Window
    {
        public GameStorageWindow(Repo games)
        {
            InitializeComponent();
            localGameList.ItemsSource = games;
        }
        private WindowStatus GetStatus() => Resources["windowStatus"] as WindowStatus ?? throw new Exception("Error getting window status");
        private Repo GetLoaclGames() => localGameList.ItemsSource as Repo ?? throw new Exception("Error getting local games");
        private Game GetLoaclSeletecedGame() => localGameList.SelectedItem as Game ?? throw new Exception("Error getting selected game");
        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (localGameList.SelectedIndex < 0) return;
            GetStatus().IsIdle = false;
            var progress = new Progress<StagedProgressInfo>(value =>
            {
                workProgress.Value = value.StagePercentage;
                statusText.Text = value.StageName;
            });
            if (await WebDAVHelper.UploadGameAsync(GetLoaclGames(), GetLoaclSeletecedGame(), progress))
            {
                Growl.Success(Strings.SuccessfullyZippedAndUploaded);
            }
            else
            {
                Growl.Error(Strings.FailedToUpload);
            }
            GetStatus().IsIdle = true;
            await RefreshRemoteData();
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (remoteGameList.SelectedIndex < 0) return;
            HandyControl.Controls.MessageBox.Show(Strings.BeforeDownloadInfo);
            GetStatus().IsIdle = false;
            var progress = new Progress<StagedProgressInfo>(value =>
            {
                workProgress.Value = value.StagePercentage;
                statusText.Text = value.StageName;
            });
            if (await WebDAVHelper.DownloadGameAsync(GetLoaclGames(), remoteGameList.SelectedItem as Game ?? throw new Exception("Error getting selected remote game"), progress))
            {
                Growl.Success(Strings.SuccessDownloadInfo);
                Repo games = GetLoaclGames();
                // Add the game to local games
                FileHelper.ScanGames(games);
                // Save metadata from remote
                await WebDAVHelper.SyncMetadataAsync(games);
                // Load the metadata on disk
                FileHelper.ReadRepoMetadata(games);
                // Read icons from executable
                await IconHelper.GetIcons(games);
            }
            else
            {
                Growl.Error(Strings.FailDownloadInfo);
            }
            GetStatus().IsIdle = true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (!GetStatus().IsIdle)
            {
                if (HandyControl.Controls.MessageBox.Show(Strings.OperationInProgressWarn, Strings.Confirm, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }
            Hide();
        }

        private async void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
            {
                await RefreshRemoteData();
            }
        }

        private async Task RefreshRemoteData()
        {
            if (WebDAVHelper.IsInitialized)
            {
                remoteGameList.ItemsSource = await WebDAVHelper.GetRemoteGamesAsync();
                uploadButton.Visibility = Visibility.Visible;
                downloadButton.Visibility = Visibility.Visible;
                deleteRemoteButton.Visibility = Visibility.Visible;

            }
            else
            {
                // Completely hide upload and download buttons
                Growl.Warning(Strings.WebDAVNotEnabledWarn);
                uploadButton.Visibility = Visibility.Collapsed;
                downloadButton.Visibility = Visibility.Collapsed;
                deleteRemoteButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void DeleteRemoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (remoteGameList.SelectedIndex < 0) return;
            if (HandyControl.Controls.MessageBox.Show(Strings.DeleteRemoteWarning, Strings.ConfirmDeletion, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await WebDAVHelper.RemoveRemoteGameAsync(GetRemoteSeletedGame());
                await RefreshRemoteData();
            }
        }

        private Game GetRemoteSeletedGame()
        {
            return remoteGameList.SelectedItem as Game ?? throw new Exception("Error getting selected remote game");
        }

        private void DeleteLocalButton_Click(object sender, RoutedEventArgs e)
        {
            if (localGameList.SelectedIndex < 0) return;
            if (HandyControl.Controls.MessageBox.Show(Strings.DeleteLocalWarning, Strings.ConfirmDeletion, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                FileHelper.RemoveGameAsync(GetLoaclGames(), GetLoaclSeletecedGame());
            }
        }
    }
}
