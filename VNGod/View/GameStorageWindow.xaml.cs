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
            GetStatus().IsBusy = true;
            var progress = new Progress<StagedProgressInfo>(value =>
            {
                workProgress.Value = value.StagePercentage;
                statusText.Text = value.StageName;
            });
            if (await WebDAVHelper.UploadGameAsync(GetLoaclGames(), GetLoaclSeletecedGame(), progress))
            {
                Growl.Success("Successfully zipped and uploaded!");
            }
            else
            {
                Growl.Error("Failed to upload, see log for more detail.");
            }
            GetStatus().IsBusy = false;
            await RefreshRemoteData();
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (remoteGameList.SelectedIndex < 0) return;
            HandyControl.Controls.MessageBox.Show("Before downloading, if the game is present at local, please backup the user content (like saves) inside the game dir. Downloading will overrite all conflicting files!");
            GetStatus().IsBusy = true;
            var progress = new Progress<StagedProgressInfo>(value =>
            {
                workProgress.Value = value.StagePercentage;
                statusText.Text = value.StageName;
            });
            if (await WebDAVHelper.DownloadGameAsync(GetLoaclGames(), remoteGameList.SelectedItem as Game ?? throw new Exception("Error getting selected remote game"), progress))
            {
                Growl.Success("Successfully downloaded and extracted!");
            }
            else
            {
                Growl.Error("Failed to download, see log for more detail.");
            }
            GetStatus().IsBusy = false;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (GetStatus().IsBusy)
            {
                if (HandyControl.Controls.MessageBox.Show("An operation is in progress. Are you sure you want to close the window? The work will be running in background.", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
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
            remoteGameList.ItemsSource = await WebDAVHelper.GetRemoteGamesAsync();
        }

        private async void DeleteRemoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (remoteGameList.SelectedIndex < 0) return;
            if (HandyControl.Controls.MessageBox.Show("Are you sure you want to delete the selected remote game? This action cannot be undone.", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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
            if (HandyControl.Controls.MessageBox.Show("Are you sure you want to delete the selected local game? This action cannot be undone.", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                FileHelper.RemoveGameAsync(GetLoaclGames(), GetLoaclSeletecedGame());
            }
        }
    }
}
