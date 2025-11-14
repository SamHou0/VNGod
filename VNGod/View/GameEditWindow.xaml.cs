using HandyControl.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using VNGod.Data;
using VNGod.Utils;

namespace VNGod.View
{
    /// <summary>
    /// GameEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameEditWindow : HandyControl.Controls.Window
    {
        public GameEditWindow(Game game)
        {
            InitializeComponent();
            DataContext = game;
        }

        private Game GetGame()
        {
            return DataContext as Game ?? throw new Exception("Error getting game.");
        }
        private void EnableGetInfoButtons(bool isEnabled)
        {
            getBangumiInfoButton.IsEnabled = isEnabled;
            getVNDBInfoButton.IsEnabled = isEnabled;
        }
        private async void GetBangumiInfoButton_Click(object sender, RoutedEventArgs e)
        {
            EnableGetInfoButtons(false);
            if (!await NetworkHelper.GetBangumiInfoAsync(GetGame(), true))
                Growl.Error(VNGod.Resource.Strings.Strings.GetBangumiInfoFail);
            EnableGetInfoButtons(true);

        }

        private async void GetVNDBInfoButton_Click(object sender, RoutedEventArgs e)
        {
            EnableGetInfoButtons(false);
            if (!await NetworkHelper.GetVNDBInfoAsync(GetGame(), true))
                Growl.Error(VNGod.Resource.Strings.Strings.GetVNDBInfoFail);
            EnableGetInfoButtons(true);
        }

        private void BrowserSeachButton_Click(object sender, RoutedEventArgs e)
        {
            // Add " to arguments to avoid broken url
            Process.Start("explorer.exe", "\"https://bgm.tv/subject_search/"+GetGame().DirectoryName.Replace(" ","+")+ "?cat=4\"");
            Process.Start("explorer.exe", "\"https://vndb.org/v?sq=" + GetGame().DirectoryName.Replace(" ", "+")+"\"");
        }
    }
}
