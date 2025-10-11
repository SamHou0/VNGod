using HandyControl.Controls;
using System.Threading.Tasks;
using System.Windows;
using VNGod.Data;
using VNGod.Services;

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
            if (!await NetworkService.GetBangumiInfoAsync(GetGame(), true))
                Growl.Error(VNGod.Resource.Strings.Strings.GetBangumiInfoFail);
            EnableGetInfoButtons(true);

        }

        private async void GetVNDBInfoButton_Click(object sender, RoutedEventArgs e)
        {
            EnableGetInfoButtons(false);
            if (!await NetworkService.GetVNDBInfoAsync(GetGame(), true))
                Growl.Error(VNGod.Resource.Strings.Strings.GetVNDBInfoFail);
            EnableGetInfoButtons(true);
        }
    }
}
