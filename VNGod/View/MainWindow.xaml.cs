using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandyControl;
using HandyControl.Controls;
using Microsoft.Win32;
using VNGod.Data;
using VNGod.Services;
using VNGod.Properties;
using System.IO;
using VNGod.View;
using System.Windows.Threading;

namespace VNGod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        DispatcherTimer timer = new DispatcherTimer()
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
            if (!string.IsNullOrWhiteSpace(repoPath) && Directory.Exists(repoPath))
            {
                Resources["gameRepo"] = FileService.InitializeRepo(repoPath);
                EnableGlobalButtons(true);
            }
            timer.Tick += Timer_Tick;
        }
        /// <summary>
        /// Check if the game is still running every second, and update playtime accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (gameList.SelectedItem is Game game)
            {
                if (Process.GetProcessesByName(game.ProcessName).Length > 0)
                    game.PlayTime += new TimeSpan(0, 0, 1);
                else
                {
                    timer.Stop();
                    FileService.SaveMetadata(Resources["gameRepo"] as Repo ?? throw new Exception("Error getting repo."), true);
                    Show();
                }
            }
        }
        private void gameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void rescanButton_Click(object sender, RoutedEventArgs e)
        {
            EnableGlobalButtons(false);
            FileService.ScanGames(Resources["gameRepo"] as Repo ??
                throw new Exception("Invalid Repo"));
            EnableGlobalButtons(true);
        }

        private async void refreshInfoButton_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info("Starting info refresh...");
            EnableGlobalButtons(false);
            repoButton.IsEnabled = false;
            Repo games = Resources["gameRepo"] as Repo ?? throw new Exception("Error getting repo.");
            int count = games.Count;
            int cnt = 0;
            foreach (var game in games)
            {
                try
                {
                    await NetworkService.GetBangumiSubjectAsync(game);
                }
                catch (Exception ex)
                {
                    Growl.Error(ex.Message);
                }
                finally
                {
                    cnt++;
                    progressBar.Value = (double)cnt / count * 100;
                }
            }
            FileService.SaveMetadata(Resources["gameRepo"] as Repo ?? throw new Exception("Error getting repo."), true);
            EnableGlobalButtons(true);
            repoButton.IsEnabled = true;
            Growl.Success("Info refresh complete.");
        }

        private void openGameFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", gameList.SelectedItem is Game game ?
                System.IO.Path.Combine((Resources["gameRepo"] as Repo ?? throw new Exception("Invalid Repo")).LocalPath, game.DirectoryName) :
                throw new Exception("No game selected."));
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (Resources["gameRepo"] is Repo repo)
                if (gameList.SelectedItem is Game game)
                {
                    if (string.IsNullOrEmpty(game.ExecutableName))
                    {
                        //Choose executable
                        GameSelectionWindow gameSelectionWindow = new GameSelectionWindow(System.IO.Path.Combine(repo.LocalPath, game.DirectoryName));
                        gameSelectionWindow.ShowDialog();
                        if (gameSelectionWindow.DialogResult == true)
                        {
                            game.ExecutableName = GameSelectionWindow.Result;
                            game.ProcessName = System.IO.Path.GetFileNameWithoutExtension(game.ExecutableName);
                            FileService.SaveMetadata(repo, true);
                        }
                        else return;
                    }
                    // If first run, show help
                    if (Settings.Default.FirstRun)
                    {
                        ShowFirstRunHelp();
                    }
                    // Launch game
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = System.IO.Path.Combine(repo.LocalPath, game.DirectoryName, game.ExecutableName),
                        WorkingDirectory = System.IO.Path.Combine(repo.LocalPath, game.DirectoryName)
                    });
                    timer.Start();
                    Hide();
                }

        }

        private static void ShowFirstRunHelp()
        {
            System.Windows.MessageBox.Show("Welcome to VNGod! It looks like this is your first time running a game. Please note that VNGod will hide during playing, and it will showup if you close the game. If that doesn't work properly, please edit the process name in 'edit info'.", "First Run Help", MessageBoxButton.OK, MessageBoxImage.Information);
            Settings.Default.FirstRun = false;
            Settings.Default.Save();
        }

        private void editGameButton_Click(object sender, RoutedEventArgs e)
        {
            GameEditWindow gameEditWindow = new GameEditWindow(gameList.SelectedItem as Game ?? throw new Exception("No game selected."));
            gameEditWindow.ShowDialog();
            FileService.SaveMetadata(Resources["gameRepo"] as Repo ?? throw new Exception("Error getting repo."), true);
        }

        private void bangumiButton_Click(object sender, RoutedEventArgs e)
        {
            Game game = gameList.SelectedItem as Game ?? throw new Exception("No game selected.");
            if (!string.IsNullOrEmpty(game.BangumiID))
            {
                Process.Start("explorer.exe", "https://bgm.tv/subject/" + game.BangumiID);
            }
            else
            {
                Growl.Warning("This game does not have a Bangumi ID.");
                return;
            }
        }

        private void vndbButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void syncButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void repoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Multiselect = false;
            openFolderDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFolderDialog.ShowDialog() == true)
            {
                Resources["gameRepo"] = FileService.InitializeRepo(openFolderDialog.FolderName);
                EnableGlobalButtons(true);
                // Save repo path to settings
                Settings.Default.Repo = openFolderDialog.FolderName;
                Settings.Default.Save();
            }

        }
        /// <summary>
        /// Enable or disable buttons that require a repo to be loaded
        /// </summary>
        /// <param name="enable"></param>
        private void EnableGlobalButtons(bool enable)
        {
            rescanButton.IsEnabled = enable;
            refreshInfoButton.IsEnabled = enable;
        }
    }
}