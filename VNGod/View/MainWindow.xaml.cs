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
using Microsoft.Win32;
using VNGod.Data;
using VNGod.Services;

namespace VNGod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EnableGlobalButtons(false);
        }

        private void gameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void rescanButton_Click(object sender, RoutedEventArgs e)
        {
            EnableGlobalButtons(false);
            FileService.ScanGames(Resources["gameRepo"] as Repo ??
                throw new Exception("Invalid Repo"));
            EnableGlobalButtons(true);
        }

        private void refreshInfoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void openGameFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editGameButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bangumiButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void vndbButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {

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