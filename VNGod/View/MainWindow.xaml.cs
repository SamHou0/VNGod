using HandyControl.Controls;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using VNGod.Data;
using VNGod.Network;
using VNGod.Properties;
using VNGod.Resource.Strings;
using VNGod.Services;
using VNGod.View;

namespace VNGod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        readonly DispatcherTimer timer = new()
        {
            IsEnabled = false,
            Interval = new TimeSpan(0, 0, 1)
        };
        public MainWindow()
        {
            InitializeComponent();
            EnableGlobalButtons(false);
            // Load repo from settings if available
            string repoPath = Settings.Default.Repo;
            Task.Run(WebDavService.InitializeClient).Wait();
            if (!string.IsNullOrWhiteSpace(repoPath) && Directory.Exists(repoPath))
            {
                InitializeGameRepo(repoPath);
                // Local may be missing, pull remote changes first
                SaveAndSync(false, true);
            }
            timer.Tick += Timer_Tick;
        }
        #region Tools
        private void InitializeGameRepo(string repoPath)
        {
            Resources["gameRepo"] = FileService.InitializeRepo(repoPath);
            EnableGlobalButtons(true);
        }
        private Repo GetRepo()
        {
            return Resources["gameRepo"] as Repo ?? throw new Exception("Error getting repo.");
        }
        private static void ShowFirstRunHelp()
        {
            System.Windows.MessageBox.Show(Strings.Welcome, "First Run Help", MessageBoxButton.OK, MessageBoxImage.Information);
            Settings.Default.FirstRun = false;
            Settings.Default.Save();
        }
        private void EnableGlobalButtons(bool enable)
        {
            rescanButton.IsEnabled = enable;
            refreshInfoButton.IsEnabled = enable;
        }
        /// <summary>
        /// Use a dialog to choose the game executable if not set.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private bool ChooseGameExecutable(Game game)
        {
            Repo repo = GetRepo();
            GameSelectionWindow gameSelectionWindow = new(System.IO.Path.Combine(repo.LocalPath, game.DirectoryName));
            gameSelectionWindow.ShowDialog();
            if (gameSelectionWindow.DialogResult == true)
            {
                game.ExecutableName = GameSelectionWindow.Result;
                game.ProcessName = Path.GetFileNameWithoutExtension(game.ExecutableName);
                SaveAndSync(true);
            }
            else return false;
            return true;
        }
        /// <summary>
        /// Check if the game is still running every second, and update playtime accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (gameList.SelectedItem is Game game)
            {
                if (Process.GetProcessesByName(game.ProcessName).Length > 0)
                    game.PlayTime += new TimeSpan(0, 0, 1);
                else
                {
                    timer.Stop();
                    SaveAndSync(true);
                    playButton.IsEnabled = true;
                    Show();
                    await SyncGameSaveAsync();
                }
            }
        }
        private Game GetCurrentGame()
        {
            return gameList.SelectedItem as Game ?? throw new Exception("No game selected.");
        }
        /// <summary>
        /// Save metadata and sync to WebDAV server if initialized.
        /// </summary>
        /// <param name="overwrite">Determine if local data is updated. Always overwrite remote if true. Only set to false when initializing or pulling remote change by hand.</param>
        /// <param name="missingLocal">Determine if local data is missing. Set to true to pull remote change first. </param>
        private async void SaveAndSync(bool overwrite, bool missingLocal = false)
        {
            Repo repo = GetRepo();
            if (!missingLocal)
                FileService.SaveMetadata(repo, overwrite);
            if (WebDavService.IsInitialized)
            {
                if (await WebDavService.SyncMetadataAsync(repo))
                    Growl.Success(Strings.WebDAVSyncSuccess);
                else Growl.Warning(Strings.WebDAVSyncFailed);
                int index = gameList.SelectedIndex;
                FileService.ReadMetadata(repo);
                if (index >= 0) gameList.SelectedIndex = index;
            }
        }
        /// <summary>
        /// Sync current game's save file to WebDAV server
        /// </summary>
        /// <returns></returns>
        private async Task SyncGameSaveAsync()
        {
            if (string.IsNullOrEmpty(GetCurrentGame().SavePath))
            {
                Growl.Warning(Strings.NoSavePath);
                return;
            }
            syncButton.IsEnabled = false;
            Growl.Info(Strings.StartingSync);
            if (await WebDavService.SyncGameAsync(GetCurrentGame()))
                Growl.Success(Strings.GameSaveSyncSuccess);
            else
                Growl.Error(Strings.GameSaveSyncFail);
            syncButton.IsEnabled = true;
        }
        #endregion
        private void RescanButton_Click(object sender, RoutedEventArgs e)
        {
            EnableGlobalButtons(false);
            FileService.ScanGames(GetRepo());
            EnableGlobalButtons(true);
        }

        private async void RefreshInfoButton_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info(Strings.StartingInfoRefresh);
            EnableGlobalButtons(false);
            repoButton.IsEnabled = false;
            Repo games = GetRepo();
            // Refresh info for each game, with progress bar
            int count = games.Count;
            int cnt = 0;
            foreach (var game in games)
            {
                // First try Bangumi, then VNDB
                if (string.IsNullOrEmpty(game.BangumiID))
                {
                    await NetworkService.GetBangumiSubjectAsync(game, true);
                    if (string.IsNullOrEmpty(game.Name))
                    {
                        Growl.Warning("No Bangumi info found for " + game.DirectoryName + ", trying VNDB...");
                        await NetworkService.GetVNDBSubjectAsync(game, true);
                    }
                }
                // No need to overwrite
                if (string.IsNullOrEmpty(game.VNDBID))
                    await NetworkService.GetVNDBSubjectAsync(game, false);
                cnt++;
                progressBar.Value = (double)cnt / count * 100;
            }
            SaveAndSync(true);
            EnableGlobalButtons(true);
            repoButton.IsEnabled = true;
            Growl.Success(Strings.InfoRefreshComplete);
        }


        private void OpenGameFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", gameList.SelectedItem is Game game ?
                System.IO.Path.Combine(GetRepo().LocalPath, game.DirectoryName) :
                throw new Exception("No game selected."));
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Repo repo = GetRepo();
            Game game = GetCurrentGame();
            if (string.IsNullOrEmpty(game.ExecutableName))
            {
                bool isSuccess = ChooseGameExecutable(game);
                if (!isSuccess) return;
            }
            // If first run, show help
            if (Settings.Default.FirstRun)
            {
                ShowFirstRunHelp();
            }
            EnableGlobalButtons(false);
            playButton.IsEnabled = false;
            // Sync Save
            await SyncGameSaveAsync();
            // Launch game
            Process.Start(new ProcessStartInfo()
            {
                FileName = Path.Combine(repo.LocalPath, game.DirectoryName, game.ExecutableName ?? throw new Exception("Executable not set.")),
                WorkingDirectory = Path.Combine(repo.LocalPath, game.DirectoryName)
            });
            EnableGlobalButtons(true);
            Hide();
            Thread.Sleep(2000); // Wait for the game to launch
            timer.Start();
        }


        private void EditGameButton_Click(object sender, RoutedEventArgs e)
        {
            GameEditWindow gameEditWindow = new(GetCurrentGame());
            gameEditWindow.ShowDialog();
            SaveAndSync(true);
        }

        private void BangumiButton_Click(object sender, RoutedEventArgs e)
        {
            Game game = GetCurrentGame();
            if (!string.IsNullOrEmpty(game.BangumiID))
            {
                Process.Start("explorer.exe", "https://bgm.tv/subject/" + game.BangumiID);
            }
            else
            {
                Growl.Warning(Strings.NoBgmID);
                return;
            }
        }

        private void VndbButton_Click(object sender, RoutedEventArgs e)
        {
            Game game = GetCurrentGame();
            if (!string.IsNullOrEmpty(game.VNDBID))
            {
                Process.Start("explorer.exe", "https://vndb.org/" + game.VNDBID);
            }
            else
            {
                Growl.Warning(Strings.NoVNDBID);
                return;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new();
            settingsWindow.ShowDialog();
            // Reinitialize WebDAV client with new settings
            Task.Run(WebDavService.InitializeClient);
        }

        private async void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            await SyncGameSaveAsync();
        }


        private void RepoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new()
            {
                Multiselect = false,
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFolderDialog.ShowDialog() == true)
            {
                InitializeGameRepo(openFolderDialog.FolderName);
                SaveAndSync(false);
                // Save repo path to settings
                Settings.Default.Repo = openFolderDialog.FolderName;
                Settings.Default.Save();
            }
        }

        private async void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible == true)
            {
                Background = new ImageBrush(await RandomImage.GetImageAsync())
                {
                    Stretch = Stretch.UniformToFill
                };
            }
        }
    }
}